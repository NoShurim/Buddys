using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Settings = Kalista_Beta_Fixed.Menus.Modes.Combo;

namespace Kalista_Beta_Fixed
{
    public class Combo : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        public override void Execute()
        {
            var target = TargetSelector.GetTarget((Settings.UseQ && Q.IsReady()) ? Q.Range : (E.Range * 1.2f), DamageType.Physical);
            if (target != null)
            {
                if (Q.IsReady() && Settings.UseQ && (!Settings.UseQAA || (Player.GetSpellDamage(target, SpellSlot.Q) > target.TotalShieldHealth() && !target.HasBuffOfType(BuffType.SpellShield))) &&
                    Player.ManaPercent >= Settings.ManaQ && Q.Cast(target))
                {
                    return;
                }

                var buff = target.GetRendBuff();
                if (Settings.UseE && (E.IsLearned && !E.IsOnCooldown) && buff != null && E.IsInRange(target))
                {
                    if (!Menus.Misc.UseKillsteal && target.IsRendKillable() && E.Cast())
                    {
                        return;
                    }

                    if (buff.Count >= Settings.MinNumberE)
                    {
                        if ((target.Distance(Player, true) > (E.Range * 0.8).Pow() ||
                             buff.EndTime - Game.Time < 0.3) && E.Cast())
                        {
                            return;
                        }
                    }

                    if (!Menus.Misc.UseHarassPlus && Settings.UseESlow && MyKalist.Minions.Any(o => E.IsInRange(o) && o.IsRendKillable()) && E.Cast())
                    {
                        return;
                    }
                }

                if (Settings.UseAA && Orbwalker.CanAutoAttack && !Player.IsInAutoAttackRange(target) &&
                    !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) &&
                    !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    Orbwalker.ForcedTarget = EntityManager.MinionsAndMonsters.CombinedAttackable.FirstOrDefault(o => Player.IsInAutoAttackRange(o));
                }
            }
        }
    }
}
