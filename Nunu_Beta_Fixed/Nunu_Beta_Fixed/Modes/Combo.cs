using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using Settings = Nunu_Beta_Fixed.MenusSettings.Modes.Combo;

namespace Nunu_Beta_Fixed.Modes
{
    public sealed class Combo : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        public override void Execute()
        {

            if (ChannelingR()) 
            {
                var TargetR = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                if (Player.Instance.CountEnemiesInRange(575) < Settings.MinR) 
                {

                    Orbwalker.DisableMovement = false;
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, TargetR);
                    Orbwalker.DisableAttacking = false;
                }
            }

            if (Settings.UseR && R.IsReady())
            {
                if (EntityManager.Heroes.Enemies.Count(h => h.IsValidTarget(300)) >= Settings.MinR)
                {
                    Orbwalker.DisableAttacking = true;
                    Orbwalker.DisableMovement = true;
                    R.Cast();
                    return;
                }

            }



            if (Settings.UseE && E.IsReady() && !ChannelingR())
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                if (target.HasBuffOfType(BuffType.Invulnerability))
                {
                    return;
                }
                if (target != null)
                {
                    E.Cast(target);
                    return;
                }
            }

            if (Settings.UseW && W.IsReady() && !ChannelingR() && Player.Instance.ManaPercent >= Settings.ManaW)
            {
                var ally = EntityManager.Heroes.Allies.OrderByDescending(a => a.TotalAttackDamage).FirstOrDefault(b => b.Distance(Player.Instance) < 1000);
                if (ally != null && Player.Instance.CountEnemiesInRange(1500) > 0)
                {
                    W.Cast(ally);
                    return;
                }
                if (Settings.UseW && W.IsReady() && Player.Instance.CountEnemiesInRange(1500) > 0)
                {
                    W.Cast(Player.Instance);
                    return;
                }
            }           
        }
    }
}
