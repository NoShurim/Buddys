using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using Settings = Karthus_Beta_Fixed.SettingsMenus.Modes.Combo;
using SettingsPred = Karthus_Beta_Fixed.SettingsMenus.Modes.PredictionMenu;

namespace Karthus_Beta_Fixed.Modes
{
    public sealed class Combo : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
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

        public override void Execute()
        {
            if (Settings.UseQ && Q.IsReady())
            {
                var Target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                var Pred = Q.GetPrediction(Target);
                if (Target != null && Target.IsValid)
                {
                    if (Pred.HitChance >= PredQ())
                    {
                        Q.Cast(Pred.CastPosition);
                    }
                }
            }
            if (Settings.UseE && E.IsReady())
            {
                var Target = TargetSelector.GetTarget(E.Range+30, DamageType.Magical);
                if (Target != null && Target.IsValid)
                {
                    if (Player.Instance.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1) 
                        E.Cast();
                }
                else
                {
                    if (Player.Instance.Spellbook.GetSpell(SpellSlot.E).ToggleState == 2) 
                        if (Settings.saveE)
                        {
                            E.Cast();
                        }
                }
            }
            if (Settings.UseW && W.IsReady())
            {
                var Target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                var Pred = W.GetPrediction(Target);
                if (Target != null && Target.IsValid)
                {
                    if (Pred.HitChance >= PredW())
                    {
                        W.Cast(Pred.CastPosition);
                    }
                }
            }
        }

    }
}
