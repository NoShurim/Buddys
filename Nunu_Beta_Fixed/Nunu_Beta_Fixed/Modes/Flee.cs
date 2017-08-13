using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Settings = Nunu_Beta_Fixed.MenusSettings.Modes.Flee;

namespace Nunu_Beta_Fixed.Modes
{
    public sealed class Flee : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee);
        }

        public override void Execute()
        {

            if (Settings.UseW && W.IsReady() && !ChannelingR())
            {
                var ally = EntityManager.Heroes.Allies.OrderByDescending(a => a.TotalAttackDamage).FirstOrDefault(b => b.Distance(Player.Instance) < 700);
                if (ally != null && Player.Instance.CountEnemiesInRange(1500) > 0)
                {
                    W.Cast(ally);
                    return;
                }
                else
                {
                    W.Cast(Player.Instance);
                    return;
                }
            }

            if (Settings.UseE && E.IsReady() && !ChannelingR())
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                if (target != null)
                {
                    E.Cast(target);
                    return;
                }
            }
        }
    }
}
