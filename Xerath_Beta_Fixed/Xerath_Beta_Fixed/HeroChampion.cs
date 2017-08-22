using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using static Xerath_Beta_Fixed.SpellManager;
using EloBuddy.SDK.Rendering;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System.Collections.Generic;
using System.Linq;
using SharpDX;

namespace Xerath_Beta_Fixed
{
    class HeroChampion
    {
        private static AIHeroClient Xerath => Player.Instance;

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

        public static float lastR;

        public static Menu Xe, Combo, Harass, Lane, Jungle, Utimate, Misc, Draws;
        public static readonly Dictionary<int, float> Timers = new Dictionary<int, float>();
        static float BigQ;
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnComplete;
        }

        private static void OnComplete(EventArgs args)
        {
            if (Xerath.Hero != Champion.Xerath) return;
            Chat.Print("[Addon] [Champion] [Xerath]", System.Drawing.Color.Blue);

            SpellManager.Initialize();
            Drawing.OnDraw += DrawsOnDraws;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Obj_AI_Base.OnNewPath += Obj_AI_Base_OnNewPath;
            Game.OnTick += Game_OnUpdate;

            Xe = MainMenu.AddMenu("Xerath", "Xerath");
            Combo = Xe.AddSubMenu("Combo");
            Combo.Add("Qc", new CheckBox("Use [Q]"));
            Combo.Add("Qcc", new CheckBox("Use [Q] IsCharnell", false));
            Combo.Add("Wc", new CheckBox("Use [W]"));
            Combo.Add("Wcc", new CheckBox("Use [W] IsCharnell", false));
            Combo.Add("Ec", new CheckBox("Use [E]"));
            Combo.Add("Ecc", new CheckBox("Use [E] IsCharnell", false));
            Combo.Add("Rc", new CheckBox("Use [R] Combo", false));
            Combo.AddSeparator();
            Combo.AddLabel("Settings [Range]");
            Combo.Add("rangeQ", new Slider("Max Range > %", 300, 0, 300));
            Combo.Add("rangeQ2", new Slider("Min Range > %", 200, 0, 200));
            //Harass
            Harass = Xe.AddSubMenu("Harass");
            Harass.Add("Hq", new CheckBox("Use [Q]"));
            Harass.Add("Hw", new CheckBox("Use [W]"));
            Harass.Add("He", new CheckBox("Use [E]"));
            Harass.AddSeparator();
            Harass.AddLabel("Percent Mana");
            Harass.Add("mana", new Slider("Mana Percent > %", 75, 1));
            //Lane
            Lane = Xe.AddSubMenu("LaneClear");
            Lane.Add("Ql", new CheckBox("Use [Q]"));
            Lane.Add("Wl", new CheckBox("Use [W]"));
            Lane.AddSeparator();
            Lane.AddLabel("Mana Percent");
            Lane.Add("manal", new Slider("Mana Percent > %", 50, 1));
            Lane.AddSeparator();
            Lane.AddLabel("Minions Percent");
            Lane.Add("Min", new Slider("Minion Percent [Q] > %", 3, 1, 6));
            Lane.Add("MinW", new Slider("Minion Percent [W] > %", 3, 1, 6));
            //Jungle
            Jungle = Xe.AddSubMenu("JungleClear");
            Jungle.Add("Qj", new CheckBox("Use [Q]"));
            Jungle.Add("Wj", new CheckBox("Use [W]"));
            Jungle.AddSeparator();
            Jungle.AddLabel("Mana Percent");
            Jungle.Add("manaj", new Slider("Mana Percent > %", 50, 1));
            //Utimate
            Utimate = Xe.AddSubMenu("Utimate");
            Utimate.AddLabel("Key [T]");
            Utimate.Add("Key", new KeyBind("Shoot charge on press", false, KeyBind.BindTypes.HoldActive, 'T'));
            //Misc
            Misc = Xe.AddSubMenu("Misc");
            Misc.Add("gape", new CheckBox("Use [E] GapClose"));
            Misc.Add("inte", new CheckBox("Use [E] Interrupt"));
            //Draws
            Draws = Xe.AddSubMenu("Drawings");
            Draws.Add("Dq", new CheckBox("Use [Q] Draw"));
            Draws.Add("Dw", new CheckBox("Use [W] Draw"));
            Draws.Add("De", new CheckBox("Use [E] Draw"));
            Draws.Add("Dr", new CheckBox("Use [R] Draw"));

        }

        private static void Game_OnUpdate(EventArgs args)
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
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                ByHarass();
            }
            if (Utimate["Key"].Cast<KeyBind>().CurrentValue)
            {
                CastR();
            }
        }

        private static void ByCombo()
        {
            var rangeq = CastSlider(Combo, "rangeQ");
            var useR = CastKey(Utimate, "Key");
            if (!Q.IsCharging)
            {
                if (W.IsBackRange(Orbwalker.ActiveModes.Combo))
                {
                    if (W.CastOnBestTarget())
                    {
                        return;
                    }
                }

                if (E.IsBackRange(Orbwalker.ActiveModes.Combo))
                {
                    var target = E.GetTarget();
                    if (target != null && (target.GetStunDuration() == 0 || target.GetStunDuration() < (Xerath.ServerPosition.Distance(target.ServerPosition) / E.Speed + E.CastDelay / 1000f) * 1000))
                    {
                        if (E.Cast(target))
                        {
                            return;
                        }
                    }
                }
            }

            if (Q.IsBackRange(Orbwalker.ActiveModes.Combo))
            {
                var target = TargetSelector.GetTarget(Q.MaximumRange, DamageType.Magical);
                if (target != null)
                {
                    var prediction = Q.GetPrediction(target);
                    if (prediction.HitChance >= Q.MinimumHitChance)
                    {
                        if (!Q.IsCharging)
                        {
                            Q.StartCharging();
                            return;
                        }
                        if (Q.Range == Q.MaximumRange)
                        {
                            if (Q.Cast(target))
                            {
                                return;
                            }
                        }
                        else
                        {
                            if (Xerath.IsInRange(prediction.UnitPosition + rangeq * (prediction.UnitPosition - Xerath.ServerPosition).Normalized(), Q.Range))
                            {
                                if (Q.Cast(prediction.CastPosition))
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            if (Q.IsCharging)
            {
                return;
            }
        }

        private static void ByLane()
        {
            var mana = CastSlider(Lane, "manal");
            var minios = CastSlider(Lane, "Min");
            var miniosW = CastSlider(Lane, "MinW");

            var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Xerath.ServerPosition, Q.MaximumRange, false).ToArray();
            if (minions.Length == 0)
            {
                return;
            }

            // Q is charging, ignore mana check
            if (Q.IsBackRange(Orbwalker.ActiveModes.LaneClear) && Q.IsCharging)
            {
                // Check if we are on max range with the minions
                if (minions.Max(m => m.Distance(Xerath, true)) < Q.RangeSquared)
                {
                    if (Q.Cast(Q.GetBestLinearCastPosition(minions).CastPosition))
                    {
                        return;
                    }
                }
            }

            // Validate that Q is not charging
            if (Q.IsCharging)
            {
                return;
            }

            // Check mana
            if (mana > Xerath.ManaPercent)
            {
                return;
            }

            if (Q.IsBackRange(Orbwalker.ActiveModes.LaneClear))
            {
                if (minions.Length >= minios)
                {
                    // Check if we would hit enough minions
                    if (Q.GetBestLinearCastPosition(minions).HitNumber >= minios)
                    {
                        // Start charging
                        Q.StartCharging();
                        return;
                    }
                }
            }

            if (W.IsBackRange(Orbwalker.ActiveModes.LaneClear))
            {
                if (minions.Length >= miniosW)
                {
                    var farmLocation = W.GetCircularFarmLocation(minions);
                    if (farmLocation.HitNumber >= miniosW)
                    {
                        if (W.Cast(farmLocation.CastPosition))
                        {
                            return;
                        }
                    }
                }
            }
        }

        private static void ByJungle()
        {
            if (!Q.IsBackRange(Orbwalker.ActiveModes.JungleClear) && !W.IsBackRange(Orbwalker.ActiveModes.JungleClear) && !E.IsBackRange(Orbwalker.ActiveModes.JungleClear))
            {
                return;
            }

            var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters(Xerath.ServerPosition, W.Range, false).ToArray();
            if (minions.Length == 0)
            {
                return;
            }

            if (Q.IsBackRange(Orbwalker.ActiveModes.JungleClear))
            {
                var farmLocation = EntityManager.MinionsAndMonsters.GetLineFarmLocation(minions, Q.Width, (int)(Q.IsCharging ? Q.Range : Q.MaximumRange));
                if (farmLocation.HitNumber > 0)
                {
                    if (!Q.IsCharging)
                    {
                        Q.StartCharging();
                        return;
                    }
                    if (Q.Cast(farmLocation.CastPosition))
                    {
                        return;
                    }
                }
            }

            if (Q.IsCharging)
            {
                return;
            }

            if (W.IsBackRange(Orbwalker.ActiveModes.JungleClear))
            {
                var farmLocation = W.GetCircularFarmLocation(minions);
                if (farmLocation.HitNumber > 0)
                {
                    if (W.Cast(farmLocation.CastPosition))
                    {
                        return;
                    }
                }
            }

            if (E.IsBackRange(Orbwalker.ActiveModes.JungleClear))
            {
                E.Cast(minions[0]);
            }
        }

        private static void ByHarass()
        {
            var rangeq = CastSlider(Combo, "rangeQ");
            var mana = CastSlider(Harass, "mana");

            if (Q.IsBackRange(Orbwalker.ActiveModes.Harass) && Q.IsCharging)
            {
                var target = TargetSelector.GetTarget(Q.MaximumRange, DamageType.Magical);
                if (target != null)
                {
                    var prediction = Q.GetPrediction(target);
                    if (prediction.HitChance >= Q.MinimumHitChance)
                    {
                        if (Q.IsFullyCharged)
                        {
                            if (Q.Cast(target))
                            {
                                return;
                            }
                        }
                        else
                        {
                            if (Xerath.IsInRange(prediction.UnitPosition + rangeq * (prediction.UnitPosition - Xerath.ServerPosition).Normalized(), Q.Range))
                            {
                                if (Q.Cast(prediction.CastPosition))
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            // Validate that Q is not charging
            if (Q.IsCharging)
            {
                return;
            }

            // Check mana
            if (mana > Xerath.ManaPercent)
            {
                return;
            }

            if (W.IsBackRange(Orbwalker.ActiveModes.Harass))
            {
                if (W.CastOnBestTarget())
                {
                    return;
                }
            }

            if (E.IsBackRange(Orbwalker.ActiveModes.Harass))
            {
                if (E.CastOnBestTarget())
                {
                    return;
                }
            }

            // Q chargeup
            if (Q.IsBackRange(Orbwalker.ActiveModes.Harass) && !Q.IsCharging)
            {
                var target = TargetSelector.GetTarget(Q.MaximumRange, DamageType.Magical);
                if (target != null)
                {
                    var prediction = Q.GetPrediction(target);
                    if (prediction.HitChance >= Q.MinimumHitChance)
                    {
                        Q.StartCharging();
                    }
                }
            }
        }

        private static void DrawsOnDraws(EventArgs args)
        {
            if (Draws["Dq"].Cast<CheckBox>().CurrentValue && SpellManager.Q.IsReady())
            {
                Drawing.DrawCircle(Player.Instance.Position, SpellManager.Q.MinimumRange, System.Drawing.Color.Teal);
            }
            if (Draws["Dq"].Cast<CheckBox>().CurrentValue && SpellManager.Q.IsReady())
            {
                Drawing.DrawCircle(Player.Instance.Position, SpellManager.Q.MaximumRange, System.Drawing.Color.Teal);
            }

            if (Draws["Dw"].Cast<CheckBox>().CurrentValue && SpellManager.W.IsReady())
            {
                SpellManager.W.DrawRange(System.Drawing.Color.Crimson);
            }
            if (Draws["De"].Cast<CheckBox>().CurrentValue && SpellManager.E.IsReady())
            {
                SpellManager.E.DrawRange(System.Drawing.Color.Crimson);
            }
            if (Draws["Dr"].Cast<CheckBox>().CurrentValue && SpellManager.R.IsReady())
            {
                SpellManager.R.DrawRange(System.Drawing.Color.LawnGreen);
            }
        }

        private static void Obj_AI_Base_OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            int id = sender.NetworkId;
            if (!Timers.ContainsKey(id))
                return;
            Timers[id] = Game.Time;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.Slot == SpellSlot.Q && Game.Time > BigQ + 0.2f)
            {
                BigQ = Game.Time;
            }
        }
    }
}