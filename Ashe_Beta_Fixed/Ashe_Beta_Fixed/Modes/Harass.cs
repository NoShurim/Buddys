using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

using Settings = Ashe_Beta_Fixed.MenusSetting.Modes.Harass;

namespace Ashe_Beta_Fixed.Modes
{
    public sealed class Harass : ModeBase
    {
        public override bool ExecuteOnComands()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
        }

        public override void Execute()
        {
            if (Settings.Mana >= Player.Instance.ManaPercent)
                return;

            if (Settings.UseQ && Q.IsReady())
            {
                if (Player.Instance.CountEnemiesInRange(610) > 0)
                {
                    foreach (var b in Player.Instance.Buffs)
                        if (b.Name == "asheqcastready")
                        {
                            Q.Cast();
                        }
                }
            }
            if (Settings.UseW && W.IsReady())
            {
                var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                if (target != null && E.GetPrediction(target).HitChance >= HitChance.High)
                {
                    W.Cast(W.GetPrediction(target).CastPosition);
                }
            }
        }
    }
}
