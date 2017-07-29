using System;
using EloBuddy;
using EloBuddy.SDK;
using Settings = Vayne_Beta_Fixed.Menus.Modes.Harass;

namespace Vayne_Beta_Fixed
{
    internal class byHarass : ModeBase
    {
        public static bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
        }

        public void Execute()
        {
            if (Settings.Mana >= Player.Instance.ManaPercent)
                return;

            if (Settings.UseE && E.IsReady())
            {
                LogicOber.CastE();
            }
        }

        internal static void Spellblade(AttackableUnit target, EventArgs args)
        {
            if (Settings.Mana >= Player.Instance.ManaPercent || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) || !Settings.UseQ || Player.Instance.CountEnemiesInRange(850) == 0)
                return;
            LogicOber.CastQ(true);
        }
    }
}
