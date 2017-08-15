using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using System;
using SharpDX;
using EloBuddy.SDK.Enumerations;
using System.Linq;

namespace LeBlanc_Beta_Fixed
{
    class MyHero
    {
        public static AIHeroClient LeBlanc => Player.Instance;
        public static bool Ult => Player.Instance.HasBuff("LeblancR");
        public static bool CastCheckbox(Menu obj, string value)
        {
            return obj[value].Cast<CheckBox>().CurrentValue;
        }
        public static int CastSlider(Menu obj, string value)
        {
            return obj[value].Cast<Slider>().CurrentValue;
        }


        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnComplete;
        }

        private static void Loading_OnComplete(EventArgs args)
        {
            if (LeBlanc.Hero != Champion.Leblanc) return;
            Chat.Print("[Addon] [Champion] [LeBlanc]", System.Drawing.Color.AliceBlue);

            Menus.StartMenu();
            Lib.W.AllowedCollisionCount = int.MaxValue;
            DamageIndicator.Initialize(Extension.GetComboDamage);
            Game.OnTick += Game_OnUpdate;
            Drawing.OnDraw += OnDraws;
            Gapcloser.OnGapcloser += Modes.AntiGapcloser.Execute;
            GameObject.OnCreate += ObjectOnCreate;
            GameObject.OnDelete += ObjectOnDelete;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                ByLane();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                ByJungle();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                ByCombo();
            }
        }

        private static void ByLane()
        {
            var _Q = CastCheckbox(Menus.Lane, "Q") && Lib.Q.IsReady() && CastSlider(Menus.Lane, "QMana") < Player.Instance.ManaPercent;
            var _W = CastCheckbox(Menus.Lane, "W") && Lib.W.IsReady() && CastSlider(Menus.Lane, "WMana") < Player.Instance.ManaPercent;
            var _R = CastCheckbox(Menus.Lane, "R") && Lib.RActive.IsReady() && Lib.RActive.Name == "LeblancSlideM";

            if (_W)
            {
                var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition, Lib.W.Range);
                if (minions != null)
                {
                    var Wminions = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minions, Lib.W.Width, (int)Lib.W.Range);
                    if (CastSlider(Menus.Lane, "WMin") <= Wminions.HitNumber)
                    {
                        Lib.CastW(Wminions.CastPosition);
                    }
                }
            }
            if (_R)
            {
                var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, LeBlanc.ServerPosition, Lib.W.Range);
                if (minions != null)
                {
                    var Rminions = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minions, Lib.W.Width, (int)Lib.W.Range);
                    if (CastSlider(Menus.Lane, "RMin") <= Rminions.HitNumber)
                    {
                        Lib.CastR(Rminions.CastPosition);
                    }
                }
            }
            if (_Q)
            {
                var Qminion = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(minion => minion.Health < LeBlanc.GetSpellDamage(minion, SpellSlot.Q)
                && LeBlanc.Distance(minion) <= Lib.Q.Range
                && minion.IsEnemy);
                if (Qminion != null)
                {
                    Lib.Q.Cast(Qminion);
                }
            }
            if (CastCheckbox(Menus.Lane, "W2"))
            {
                if (Lib.W.Name == "leblancslidereturn")
                {
                    LeBlanc.Spellbook.CastSpell(SpellSlot.W);
                }
            }
        }


    
        private static void ByJungle()
        {
            throw new NotImplementedException();
        }

        private static void ByCombo()
        {
            var target = TargetSelector.GetTarget(Lib.W.Range * 2, DamageType.Magical);
            if (target != null)

            {
                var ComboM = Menus.Comb;
                var WReady = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name != "leblancslidereturn" && Lib.W.IsReady();


                if (CastCheckbox(ComboM, "Q"))
                {
                    if (Lib.Q.IsLearned && Lib.W.IsLearned && Lib.E.IsLearned)
                    {

                        if (!WReady || Lib.E.IsReady() || Lib.Q.IsReady() || !Ult || Lib.QlasTick > Environment.TickCount || Player.Instance.Level == 1 ||
                            !Ult || (Lib.W.GetCooldown() > 0 && Lib.W.GetCooldown() < 4) || (Lib.E.GetCooldown() > 0 && Lib.E.GetCooldown() < 4))
                        {
                            if (target.IsValidTarget(Lib.Q.Range))
                            {
                                Lib.Q.Cast(target);
                            }
                        }
                    }
                }
               
                if (CastCheckbox(ComboM, "W"))
                {
                    if (CastCheckbox(ComboM, "extW") && Player.Instance.Distance(target) > Lib.Q.Range)
                    {
                        if (WReady)
                        {
                            var wpos = Player.Instance.Position.Extend(target, Lib.W.Range).To3D();
                            if (Lib.Q.IsReady() && CastCheckbox(ComboM, "Q"))
                            {
                                if (Lib.Q.Range + Lib.W.Range > LeBlanc.Distance(target))
                                {
                                    Lib.CastW(wpos);
                                }
                            }
                            else if (Lib.E.IsReady() && CastCheckbox(ComboM, "E"))
                            {
                                if (Lib.E.Range + Lib.W.Range > Player.Instance.Distance(target))
                                {
                                    Lib.CastW(wpos);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (WReady)
                        {
                            if (Lib.W.IsInRange(target))
                            {
                                if (target.HasBuff("LeblancChaosOrb") || target.HasBuff("LeblancSoulShackle") || Player.Instance.Level == 1 || target.HasBuff("LeblancChaosOrbM") || target.HasBuff("LeblancSoulShackleM") || Lib.QlasTick > Environment.TickCount)
                                {
                                    Lib.CastW(target);
                                }
                            }
                        }
                    }
                }
                if (CastCheckbox(ComboM, "E"))
                {
                    if (Lib.E.IsReady() && (!WReady || Player.Instance.Level == 1))
                    {
                        var epred = Lib.E.GetPrediction(target);

                        if (epred.HitChance >= HitChance.Medium)
                        {
                            Lib.E.Cast(epred.CastPosition);
                        }
                    }
                }
                if (CastCheckbox(ComboM, "R"))
                {
                    if (CastCheckbox(ComboM, "RQ"))
                    {
                        if (Lib.RActive.IsReady() && Lib.RTargeted.IsReady())
                        {
                            if (Ult && Lib.QUtimat.IsReady() && Lib.QUtimat.IsInRange(target))
                            {
                                Lib.QUtimat.Cast(target);
                            }
                        }

                        if (CastCheckbox(ComboM, "RW"))
                        {
                            if (Lib.RActive.Name == "LeblancSlideM") // W
                            {
                                if (target.CountEnemiesInRange(Lib.W.Width) > 1)
                                {
                                    Lib.CastR(target);
                                }
                                else if (!Lib.Q.IsReady() && !Lib.E.IsReady())
                                {
                                    Lib.CastR(target);
                                }
                            }
                        }
                        if (CastCheckbox(ComboM, "RE"))
                        {
                            if (Lib.RActive.Name == "LeblancSoulShackleM") // E
                            {
                                if (Lib.Q.IsReady() || Lib.E.IsReady())
                                {
                                    Lib.CastR(target);
                                }
                            }
                        }
                    }
                }
            }
        }


        private static void ObjectOnCreate(GameObject sender, EventArgs args)
        {

        }

        private static void ObjectOnDelete(GameObject sender, EventArgs args)
        {

        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Slot == SpellSlot.Q)
                    Lib.QlasTick = Environment.TickCount + 500;
                else if (args.Slot == SpellSlot.R && Lib.RActive.Name == "LeBlancR")
                    Lib.QlasTick = Environment.TickCount + 500;
            }
        }

        private static void OnDraws(EventArgs args)
        {
            if (CastCheckbox(Menus.Draws, "Q"))
            {
                Circle.Draw(Color.Aqua, Lib.Q.Range, Player.Instance.Position);
            }
            if (CastCheckbox(Menus.Draws, "W"))
            {
                Circle.Draw(Color.Aqua, Lib.W.Range, Player.Instance.Position);
            }
            if (CastCheckbox(Menus.Draws, "E"))
            {
                Circle.Draw(Color.Aqua, Lib.E.Range, Player.Instance.Position);
            }
        }
    }
}