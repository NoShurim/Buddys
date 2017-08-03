using EloBuddy;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using System;
using static Tristana_Beta_Fixed.Menus;
using static Tristana_Beta_Fixed.SpellsManager;
using static Tristana_Beta_Fixed.DamageManege;
using System.Linq;
using EloBuddy.SDK;

namespace Tristana_Beta_Fixed
{
    class Program
    {
        private static AIHeroClient Tristana => Player.Instance;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }

        }
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_On_Complete;
        }
        public static int GetStacks(Obj_AI_Base target)
        {
            var totalstacks = 0;
            var stacks1 = target.GetBuffCount("TristanaECharge");
            var stacks2 = target.GetBuffCount("TristanaEChargeSound");

            if (stacks1 == -1 && stacks2 == -1)
            {
                totalstacks = 0;
            }
            if (stacks1 == -1 && stacks2 == 1)
            {
                totalstacks = 1;
            }
            if (stacks1 == 1 && stacks2 == 1)
            {
                totalstacks = 2;
            }
            if (stacks1 == 2 && stacks2 == 1)
            {
                totalstacks = 3;
            }
            if (stacks1 == 3 && stacks2 == 1)
            {
                totalstacks = 4;
            }

            return totalstacks;
        }
        private static void Loading_On_Complete(EventArgs args)
        {
            if (Tristana.Hero != Champion.Tristana) return;
            Chat.Print("[Addon] [Champion] [Tristana]", System.Drawing.Color.AliceBlue);
            Menus.Execute();
            SpellsManager.Execute();
            //
            Drawing.OnDraw += Draws_On;
            Interrupter.OnInterruptableSpell += OnInterrupter;
            Gapcloser.OnGapcloser += OnGapcloser;
            Game.OnTick += Game_Ontick;
        }

        private static void Game_Ontick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                ByCombo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                ByLane();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                ByJungle();
            }
            AutoHara();
            AutoR();
            Killteal();

        }

        private static void ByCombo()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var wpred = W.GetPrediction(target);
            if (Combo["Qc"].Cast<CheckBox>().CurrentValue)
            {
                if (target.IsValidTarget(630 + 7) && Q.IsReady())
                {
                    Q.Cast(target);
                }
            }
            if (Combo["Ec"].Cast<CheckBox>().CurrentValue)
            {
                if (target.IsValid() && E.IsReady())
                {
                    E.Cast(target);
                }
            }
            if (St["Wc"].Cast<CheckBox>().CurrentValue)
            {
                if (GetStacks(target) > St["stack"].Cast<Slider>().CurrentValue)
                {
                    if (target.IsValidTarget(925) && W.IsReady() && wpred.HitChancePercent >= St["pw"].Cast<Slider>().CurrentValue)
                    {
                        W.Cast(wpred.CastPosition);
                    }
                }
                if (Combo["Rc"].Cast<CheckBox>().CurrentValue)
                {
                    if (target.IsValidTarget(R.Range) && R.IsReady() && target.Health < Rmage(target))
                    {
                        R.Cast(target);
                    }
                }
            }
        }
        private static void ByLane()
        {
            var LaneCount = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,   Player.Instance.ServerPosition,
                Player.Instance.AttackRange, false).Count();
            var LaneClear = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition, Player.Instance.AttackRange).OrderByDescending(a => a.MaxHealth).FirstOrDefault();
            var LaneClearE = EntityManager.MinionsAndMonsters.GetLaneMinions().FirstOrDefault(m => m.IsValidTarget(Player.Instance.AttackRange) && m.HasBuff("TristanaECharge"));

            if (Lane["forE"].Cast<CheckBox>().CurrentValue)
            {
                if (LaneClearE != null)
                {
                    Orbwalker.ForcedTarget = LaneClearE;
                }
                else
                {
                    Orbwalker.ForcedTarget = null;
                }
            }
            if (LaneCount == 0) return;

            if (Lane["El"].Cast<CheckBox>().CurrentValue && Player.Instance.ManaPercent >= Lane["mana"].Cast<Slider>().CurrentValue && E.IsReady())
            {
                E.Cast(LaneClear);
            }

            if (Lane["Ql"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                Q.Cast();
            }

        }
        private static void ByJungle()
        {
        }
        private static void AutoHara()
        {
            throw new NotImplementedException();
        }

        private static void Killteal()
        {
            if (W.IsReady() && E.IsReady() &&
                R.IsReady())
            {
                var target = TargetSelector.GetTarget(W.Range, DamageType.Physical) ?? TargetSelector.GetTarget(E.Range, DamageType.Magical);
                if (target.IsValidTarget(925) && target.Health < (Wmage(target) + Emage(target) + Rmage(target)))
                {
                    var pred = W.GetPrediction(target);
                    W.Cast(pred.CastPosition);
                    E.Cast(target);
                }
            }
        }
        private static void AutoR()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (Combo["Rc"].Cast<CheckBox>().CurrentValue)
            {
                if (target.IsValidTarget(R.Range) && R.IsReady() && target.Health < Rmage(target))
                {
                    R.Cast(target);
                }
            }
        }
        private static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!sender.IsValidTarget()) return;
            if (Misc["Gap"].Cast<CheckBox>().CurrentValue)
            {
                R.Cast(sender);
            }
        }
        private static void OnInterrupter(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (!sender.IsValidTarget()) return;
            if (Misc["Int"].Cast<CheckBox>().CurrentValue)
            {
                R.Cast(sender);
            }
        }
        private static void Draws_On(EventArgs args)
        {
            if (Draws["DW"].Cast<CheckBox>().CurrentValue && W.IsReady())
            {
                W.DrawRange(System.Drawing.Color.Crimson);
            }
            if (Draws["DR"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                R.DrawRange(System.Drawing.Color.LawnGreen);
            }
        }
    }
}