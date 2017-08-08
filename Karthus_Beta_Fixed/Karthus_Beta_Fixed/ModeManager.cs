using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using Karthus_Beta_Fixed.Modes;
using Settings = Karthus_Beta_Fixed.SettingsMenus.Modes.Combo;
using SettingsPred = Karthus_Beta_Fixed.SettingsMenus.Modes.PredictionMenu;
using SharpDX;


namespace Karthus_Beta_Fixed
{
    public static class ModeManager
    {
        private static List<ModeBase> Modes { get; set; }

        //private static SpellSlot ignite;
        public static readonly Spell.Targeted Ignite =
        new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerdot"), 600);

        static ModeManager()
        {
            Modes = new List<ModeBase>();

            Modes.AddRange(new ModeBase[]
            {
                new PermaActive(),
                new Combo(),
                new Harass(),
                new LaneClear(),
                new JungleClear(),
                new LastHit(),
                new Flee()
            });

            // Listen to events we need
            Game.OnTick += OnTick;
        }

        internal static void Execute()
        {
        }

        public static float GetTargetHealth(AIHeroClient playerInfo)
        {
            if (playerInfo.IsVisible)
                return playerInfo.Health;

            var predictedhealth = playerInfo.Health + playerInfo.HPRegenRate * (SpellManager.R.CastDelay / 1000);

            return predictedhealth > playerInfo.MaxHealth ? playerInfo.MaxHealth : predictedhealth;
        }

        public static float RDamage(Obj_AI_Base target)
        {
            var DMG = 0f;

            if (SpellManager.R.Level == 1)
            {
                DMG = 250f + (0.60f * Player.Instance.FlatMagicDamageMod);
                DMG = Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, DMG);
            }
            if (SpellManager.R.Level == 2)
            {
                DMG = 400f + (0.60f * Player.Instance.FlatMagicDamageMod);
                DMG = Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, DMG);
            }
            if (SpellManager.R.Level == 3)
            {
                DMG = 550f + (0.60f * Player.Instance.FlatMagicDamageMod);
                DMG = Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, DMG);
            }

            return DMG;
        }

        public static void CheckIgnite_Enemy()
        {
            if (Settings.useIgnite)
            {
                var Target = TargetSelector.GetTarget(SpellManager.Q.Range, DamageType.Magical);

                if (Ignite.IsInRange(Target)
                    && Target.Health < 50 + 20 * Player.Instance.Level - (Target.HPRegenRate / 5 * 3))
                {
                    Ignite.Cast(Target);
                }
            }
        }

        public static void AutoCast()
        {
            if (Settings.useAC)
            {
                if (Player.Instance.IsDead || Player.Instance.IsZombie)
                {
                    if (Settings.UseQ && SpellManager.Q.IsReady())
                    {
                        var Target = TargetSelector.GetTarget(SpellManager.Q.Range, DamageType.Magical);
                        var Pred = SpellManager.Q.GetPrediction(Target);
                        if (Target != null && Target.IsValid)
                        {
                            if (Pred.HitChance >= PredQ())
                            {
                                SpellManager.Q.Cast(Pred.CastPosition);
                            }
                        }
                    }
                    if (Settings.UseW && SpellManager.W.IsReady())
                    {
                        var Target = TargetSelector.GetTarget(SpellManager.W.Range, DamageType.Magical);
                        var Pred = SpellManager.W.GetPrediction(Target);
                        if (Target != null && Target.IsValid)
                        {
                            if (Pred.HitChance >= PredW())
                            {
                                SpellManager.W.Cast(Pred.CastPosition);
                            }
                        }
                    }
                }
            }

            if (Settings.useUltKS)
            {
                var EnemiesTxt = "";
                var EnemiesRange = false;

                //Show Enemies
                var enemies = EntityManager.Heroes.Enemies.Where(a => a.IsEnemy && a.IsValid);
                Vector2 WTS = Drawing.WorldToScreen(Player.Instance.Position);

                ///////////////////////////////////////////////////
                foreach (var enemy in enemies)
                {
                    if ((GetTargetHealth(enemy) - RDamage(enemy)) <= 0)
                    {
                        if (!enemy.IsDead)
                        {
                            // CHECK SEC ULT

                            if (Settings.ultSecure)
                            {
                                if (Player.Instance.IsDead || Player.Instance.IsZombie)
                                {
                                    EnemiesTxt = enemy.BaseSkinName + " | ";
                                }
                                else
                                {
                                    EnemiesTxt = enemy.BaseSkinName + " | ";
                                    if (enemy.Distance(Player.Instance) < 2000)
                                    {
                                        EnemiesRange = true;
                                    }
                                }

                            }
                            else
                            {
                                EnemiesTxt = enemy.BaseSkinName + " | ";
                            }

                            //
                        }
                    }
                }

                if (EnemiesTxt != "")
                {
                    if (Settings.ultSecure && !EnemiesRange)
                    {
                        if (SpellManager.R.IsLearned && SpellManager.R.IsReady())
                        {
                            var Target = TargetSelector.GetTarget(SpellManager.R.Range, DamageType.Magical);
                            var Pred = SpellManager.R.GetPrediction(Target);
                            if (Target != null && Target.IsValid)
                            {
                                SpellManager.R.Cast(Pred.CastPosition);
                            }
                        }
                    }

                    if (!Settings.ultSecure)
                    {
                        if (SpellManager.R.IsLearned && SpellManager.R.IsReady())
                        {
                            var Target = TargetSelector.GetTarget(SpellManager.R.Range, DamageType.Magical);
                            var Pred = SpellManager.R.GetPrediction(Target);
                            if (Target != null && Target.IsValid)
                            {
                                SpellManager.R.Cast(Pred.CastPosition);
                            }
                        }
                    }
                }

                //////////////////////////////////////////////////////

            }
        }

        private static HitChance PredQ()
        {
            var mode = SettingsPred.QPrediction;
            switch (mode)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
            }
            return HitChance.Medium;
        }

        private static HitChance PredW()
        {
            var mode = SettingsPred.WPrediction;
            switch (mode)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
            }
            return HitChance.Medium;
        }

        public static void SafeMana()
        {
            if (Settings.saveE)
            {
                if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                {
                    if (!SpellManager.E.IsReady() || Player.Instance.Spellbook.GetSpell(SpellSlot.E).ToggleState != 2) // 1 = off , 2 = on
                        return;

                    var Range = SpellManager.E.Range + 100;
                    var Etarget = TargetSelector.GetTarget(Range, DamageType.Magical);

                    if (SpellManager.E.IsReady() && Etarget == null)
                    {
                        SpellManager.E.Cast();
                    }
                }
            }
        }

        private static void OnTick(EventArgs args)
        {
            try

            {
                CheckIgnite_Enemy();
                AutoCast();
                // Execute all modes
                Modes.ForEach(mode =>
                {
                    CheckIgnite_Enemy();
                    AutoCast();
                    // Precheck if the mode should be executed
                    if (mode.ShouldBeExecuted())
                    {
                        // Execute the mode
                        ModeManager.CheckIgnite_Enemy();
                        ModeManager.AutoCast();
                        mode.Execute();
                        //Program.UltKS();
                    }
                    else SafeMana();

                });

            }
            catch (Exception e)
            {

            }
        }
    }
}