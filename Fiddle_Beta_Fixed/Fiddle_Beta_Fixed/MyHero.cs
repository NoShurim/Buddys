using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System;
using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using static Fiddle_Beta_Fixed.SpellsManager;
using static Fiddle_Beta_Fixed.MenusSettings;   
using EloBuddy.SDK.Enumerations;


namespace Fiddle_Beta_Fixed
{
    class MyHero
    {
        private static AIHeroClient Fiddle => Player.Instance;
        public static bool Checking;
        public static bool Senderprocess;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnComplete;
        }

        private static void Loading_OnComplete(EventArgs args)
        {
            if (Fiddle.Hero != Champion.FiddleSticks) return;
            Bootstrap.Init(null);
            Chat.Print("[Addon] [Champion] [FiddleSticks]", System.Drawing.Color.AliceBlue);

            MenusSettings.Execute();
            SpellsManager.Execute();

            //Comands
            Gapcloser.OnGapcloser += Gap_Closer;
            Interrupter.OnInterruptableSpell += Interupt;
            Obj_AI_Base.OnProcessSpellCast += OnSpellCast;
            Obj_AI_Base.OnBuffLose += OnBuffLose;
            Game.OnTick += OnUpdate;
            Drawing.OnDraw += On_Draw;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                ByCombo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                ByHaras();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                ByLane();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                ByJungle();
            }
            AutoCC();
            AutoR();

        }

        private static void ByCombo()
        {
            var Enemys = EntityManager.Heroes.Enemies.OrderByDescending(a => a.HealthPercent).Where(a => !a.IsMe && a.IsValidTarget() && a.Distance(Fiddle) <= Q.Range);
            var useW = Combo["Wc"].Cast<CheckBox>().CurrentValue;
            var useR = Combo["Rc"].Cast<CheckBox>().CurrentValue;
            var En = Combo["En"].Cast<Slider>().CurrentValue;
            var target = TargetSelector.GetTarget(1400, DamageType.Physical);
            if (!target.IsValidTarget(Q.Range) || target == null)
            {
                return;
            }
            if (E.IsReady() && target.IsValidTarget(E.Range))
                foreach (var eenemies in Enemys)
                {
                    var useE = Combo["Ec"].Cast<CheckBox>().CurrentValue;
                    if (useE)
                    {
                        E.Cast(target);
                    }
                }
            if (Q.IsReady() && target.IsValidTarget(Q.Range))
                foreach (var eenemies in Enemys)
                {
                    var useQ = Combo["Qc"].Cast<CheckBox>().CurrentValue;
                    if (useQ)
                    {
                        Q.Cast(target);
                    }
                }
            if (useW)
            {
                if (W.IsReady() && Q.IsOnCooldown && E.IsOnCooldown &&
                    target.IsValidTarget(W.Range) && !target.IsInvulnerable)
                {
                    W.Cast(target);
                }
            }
            if (useR)
            {
                if (R.IsReady() && Fiddle.CountEnemiesInRange(R.Range) >= En)
                {
                    Core.DelayAction(() => R.Cast(target.Position), 250);
                }
            }
        }

        private static void ByHaras()
        {
            var enemys = EntityManager.Heroes.Enemies.OrderByDescending(a => a.HealthPercent).Where(a => !a.IsMe && a.IsValidTarget() && a.Distance(Fiddle) <= E.Range);
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (!target.IsValidTarget())
            {
                return;
            }

            if (E.IsReady() && target.IsValidTarget(E.Range))
                foreach (var eenemies in enemys)
                {
                    var useE = Haras["He"].Cast<CheckBox>().CurrentValue;
                    var mana = Haras["mana"].Cast<Slider>().CurrentValue;

                    if (useE && Player.Instance.ManaPercent > mana)
                    {
                        E.Cast(target);
                    }
                }
        }

        private static void ByLane()
        {
            var byatack = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition, Player.Instance.AttackRange, false).Count();
            var sus = EntityManager.MinionsAndMonsters.GetLaneMinions().OrderBy(a => a.MaxHealth).FirstOrDefault(a => a.IsValidTarget(Q.Range));
            var useQ = Lane["Ql"].Cast<CheckBox>().CurrentValue;
            var useW = Lane["Wl"].Cast<CheckBox>().CurrentValue;
            var useE = Lane["El"].Cast<CheckBox>().CurrentValue;
            var manal = Lane["manal"].Cast<Slider>().CurrentValue;

            if (byatack == 0) return;

            if (useE && E.IsReady() && Player.Instance.ManaPercent > manal)
            {
                E.Cast(sus);
            }
            if (useQ && Q.IsReady() && Player.Instance.ManaPercent > manal)
            {
                Q.Cast(sus);
            }
            if (useW && W.IsReady() && E.IsOnCooldown && Q.IsOnCooldown && Player.Instance.ManaPercent > manal)
            {
                W.Cast(sus);
            }
        }

        private static void ByJungle()
        {
            var m = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(a => a.MaxHealth).FirstOrDefault(a => a.IsValidTarget(900));
            var useQ = Lane["Qj"].Cast<CheckBox>().CurrentValue;
            var useE = Lane["Ej"].Cast<CheckBox>().CurrentValue;
            var useW = Lane["Wj"].Cast<CheckBox>().CurrentValue;

            if (useE && E.IsReady() && m.IsValidTarget(E.Range))
            {
                E.Cast(m);
            }
            if (useQ && Q.IsReady() && m.IsValidTarget(Q.Range))
            {
                Q.Cast(m);
            }
            if (useW && W.IsReady() && E.IsOnCooldown && Q.IsOnCooldown && m.IsValidTarget(W.Range))
            {
                W.Cast(m);
            }
        }

        private static void AutoCC()
        {
            if (!Combo["Qcc"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }
            var Target = EntityManager.Heroes.Enemies.FirstOrDefault(
                    x =>
                        x.HasBuffOfType(BuffType.Charm) || x.HasBuffOfType(BuffType.Knockup) ||
                        x.HasBuffOfType(BuffType.Stun) || x.HasBuffOfType(BuffType.Suppression) ||
                        x.HasBuffOfType(BuffType.Slow) ||
                        x.HasBuffOfType(BuffType.Snare));
            if (Target != null)
            {
                Q.Cast(Target.ServerPosition);
            }
        }

        private static void AutoR()
        {
            throw new NotImplementedException();
        }

        private static void Gap_Closer(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!sender.IsEnemy) return;

            if (sender.IsValidTarget(Q.Range) && Fiddle.Distance(e.End) < 150)
            {
                Q.Cast(sender);
            }
            if (sender.IsValidTarget(E.Range) && Fiddle.Distance(e.End) < 150)
            {
                E.Cast(sender);
            }
        }

        private static void Interupt(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (!sender.IsEnemy) return;
            if (e.DangerLevel == DangerLevel.High)
            {
                Q.Cast(sender);
            }
        }

        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "Drain")
            {
                Checking = true;
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
                Senderprocess = true;
                Checking = true;
            }
        }

        private static void OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender.IsMe && args.Buff.DisplayName == "Drain")
            {
                Checking = false;
                Orbwalker.DisableAttacking = false;
                Orbwalker.DisableMovement = false;
                Senderprocess = false;
            }
        }

        private static void On_Draw(EventArgs args)
        {
            if (Draws["DW"].Cast<CheckBox>().CurrentValue && W.IsReady())
            {
                W.DrawRange(System.Drawing.Color.Crimson);
            }
            if (Draws["DE"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                E.DrawRange(System.Drawing.Color.Crimson);
            }
            if (Draws["DR"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                R.DrawRange(System.Drawing.Color.LawnGreen);
            }
        }
    }
}