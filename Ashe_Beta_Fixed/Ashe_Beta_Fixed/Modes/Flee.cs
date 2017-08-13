using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using Settings = Ashe_Beta_Fixed.MenusSetting.Modes.Flee;

namespace Ashe_Beta_Fixed.Modes
{
    public sealed class Flee : ModeBase
    {
        public override bool ExecuteOnComands()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee);
        }

        public override void Execute()
        {
            if (Settings.UseW && W.IsReady())
            {
                var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                if (target != null && target.IsValidTarget() && target.Health > 0 && W.GetPrediction(target).HitChance >= HitChance.High)
                {
                    W.Cast(target);
                }
            }
        }
    }
}
