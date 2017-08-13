using EloBuddy;
using EloBuddy.SDK;

namespace Ashe_Beta_Fixed.Modes
{
    public sealed class LastHit : ModeBase
    {
        public override bool ExecuteOnComands()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit);
        }

        public override void Execute()
        {
            if (Q.IsReady())
            {
               var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
               if (target != null)
               {
                  Q.Cast(target);
               }
            }
        }
    }
}
