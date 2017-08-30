using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using static Kayn_BETA_Fixed.SpellManager;
using System;
using System.Linq;
using static Kayn_BETA_Fixed.Menus;
using Kayn_BETA_Fixed;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace Kayn_BETA_Fixed
{
    class Program
    {
        public static AIHeroClient Kayn => Player.Instance;

        public static bool CastCheckbox(Menu obj, string value)
        {
            return obj[value].Cast<CheckBox>().CurrentValue;
        }

        public static bool CastKey(Menu obj, string value)
        {
            return obj[value].Cast<KeyBind>().CurrentValue;
        }

        public static int CastSlider(Menu obj, string value)
        {
            return obj[value].Cast<Slider>().CurrentValue;
        }

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_On;
        }

        private static void Loading_On(EventArgs args)
        {
            if (Kayn.Hero != Champion.Kayn) return;

            Chat.Print("[Addon] [Champion] [Kayn]", System.Drawing.Color.LightBlue);
            Chat.Print("[Addon] [Version 7.17]", System.Drawing.Color.LightBlue);

            CreateMenu();
            SpellManager.Execute();
            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += Game_OnUpdate;
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

            if (Combo["UE"].Cast<KeyBind>().CurrentValue)
            {
                UseE();
            }
            Escape();
        }

        private static void Escape()
        {
            var target = R.GetTarget();
            if (target != null && Kayn.Health < 45)
            {
                if (!target.IsInRange(Kayn, R.Range) && R.IsReady())
                {
                    return;
                }
                {
                    R.Cast(target);
                }
            }
        }
    
        private static void ByCombo()
        {

            if (Player.Instance.IsKayn())
            {
                // Q usage
                if (Qk.IsBackRange(Orbwalker.ActiveModes.Combo) && !Kayn.IsAboutToTransform())
                {
                    var target = Qk.GetTarget();
                    if (target != null)
                    {
                        var prediction = Qk.GetPrediction(target);

                        switch (prediction.HitChance)
                        {
                            case HitChance.High:
                            case HitChance.Immobile:

                                // Regular Q cast
                                if (Qk.Cast(prediction.CastPosition))
                                {
                                    return;
                                }
                                break;

                            case HitChance.Collision:

                                // Special case for colliding enemies
                                var colliding = prediction.CollisionObjects.OrderBy(o => o.Distance(Kayn, true)).ToList();
                                if (colliding.Count > 0)
                                {
                                    // First colliding target is < 100 units away from our main target
                                    if (colliding[0].IsInRange(target, 100))
                                    {
                                        if (Qk.Cast(prediction.CastPosition))
                                        {
                                            return;
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
                if (Wk.IsBackRange(Orbwalker.ActiveModes.Combo) && !Kayn.IsAboutToTransform())
                {
                    var target = Wk.GetTarget();
                    if (target != null)
                    {
                        var prediction = Wk.GetPrediction(target);

                        switch (prediction.HitChance)
                        {
                            case HitChance.High:
                            case HitChance.Immobile:

                                // Regular W cast
                                if (Wk.Cast(prediction.CastPosition))
                                {
                                    return;
                                }
                                break;

                            case HitChance.Collision:

                                // Special case for colliding enemies
                                var colliding = prediction.CollisionObjects.OrderBy(o => o.Distance(Kayn, true)).ToList();
                                if (colliding.Count > 0)
                                {
                                    // First colliding target is < 100 units away from our main target
                                    if (colliding[0].IsInRange(target, 100))
                                    {
                                        if (Wk.Cast(prediction.CastPosition))
                                        {
                                            return;
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }

                if (R.IsBackRange(Orbwalker.ActiveModes.Combo))
                {
                    var target = R.GetTarget();
                    if (target != null && target.HealthPercent < Menus.Combo["Rs"].Cast<Slider>().CurrentValue)
                    {
                        if (!target.IsInRange(Kayn, R.Range) && R.IsReady())
                        {
                            return;
                        }
                        {
                            R.Cast(target);
                        }
                    }
                }
            }
        }

        private static void ByLane()
        {
            var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Kayn.Position, W.Range).ToArray();
            var mana = Lane["mana"].Cast<Slider>().CurrentValue;
            if (Kayn.ManaPercent < mana) return;
            if (minions != null)
            {
                var wpred = EntityManager.MinionsAndMonsters.GetLineFarmLocation(minions, Qk.Width, (int)W.Range);
                if (Lane["Ql"].Cast<CheckBox>().CurrentValue && Qk.IsLearned && Qk.IsReady())
                {
                    foreach (var minion in minions.Where(x => x.IsValid() && !x.IsDead && x.Health > 15))
                    {
                        if (Lane["Qmode"].Cast<ComboBox>().CurrentValue == 0 &&
                            Prediction.Position.PredictUnitPosition(minion, Qk.CastDelay).Distance(Kayn.Position) <= (Qk.Range - 50))
                        {
                            Qk.Cast(minion.Position);
                        }

                        else { Qk.Cast(minion.Position); }

                    }
                }
                if (Lane["Wl"].Cast<CheckBox>().CurrentValue && W.IsLearned && W.IsReady())
                {
                    if (wpred.HitNumber >= Lane["Min"].Cast<Slider>().CurrentValue) W.Cast(wpred.CastPosition);
                    {
                        foreach (var minion in minions.Where(x => x.IsValid() && !x.IsDead && x.Health > 15))
                        {
                            if (Lane["Wmode"].Cast<ComboBox>().CurrentValue == 0 &&
                                Prediction.Position.PredictUnitPosition(minion, W.CastDelay).Distance(Kayn.Position) <= (W.Range + 700))
                            {
                                W.Cast(minion.Position);
                            }

                            else { W.Cast(minion.Position); }
                        }
                    }
                }
            }
        }

        private static void ByJungle()
        {
            var Monsters = EntityManager.MinionsAndMonsters.GetJungleMonsters(Kayn.Position, 1800f);
            var mana = Jungle["jmana"].Cast<Slider>().CurrentValue;
            if (Kayn.ManaPercent < mana) return;
            var WPred = EntityManager.MinionsAndMonsters.GetLineFarmLocation(Monsters, Wk.Width, (int)W.Range);

            if (Jungle["Qj"].Cast<CheckBox>().CurrentValue && Qk.IsLearned && Qk.IsReady())
            {
                foreach (var monster in Monsters.Where(x => !x.IsDead && x.IsValidTarget(Qk.Range) && x.Health > 100))
                {
                    Qk.Cast(monster.Position);
                }
            }

            if (Jungle["Wj"].Cast<CheckBox>().CurrentValue && W.IsLearned && W.IsReady())
            {
                if (WPred.HitNumber >= Jungle["J"].Cast<Slider>().CurrentValue) W.Cast(WPred.CastPosition);
            }
        }

        private static void UseE()
        {
            if (Ek.IsReady())
            {
                Ek.Cast();
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Draws["DQ"].Cast<CheckBox>().CurrentValue && Qk.IsReady())
            {
                Qk.DrawRange(System.Drawing.Color.LightBlue);
            }
            if (Draws["DW"].Cast<CheckBox>().CurrentValue && W.IsReady())
            {
                W.DrawRange(System.Drawing.Color.Crimson);
            }
            if (Draws["DE"].Cast<CheckBox>().CurrentValue && Ek.IsReady())
            {
                Ek.DrawRange(System.Drawing.Color.Crimson);
            }
            if (Draws["DR"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                R.DrawRange(System.Drawing.Color.LawnGreen);
            }
        }
    }
}