using System;
using EloBuddy;
using EloBuddy.SDK;
using Settings = Vayne_Beta_Fixed.Menus.Modes.LaneClear;

namespace Vayne_Beta_Fixed
{
    internal class byLaneClear : ModeBase
    {
        public static
            bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear);
        }

        public static
            void Execute()
        {
        }

        internal static void Spellblade(AttackableUnit target, EventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                return;
            if (Settings.mana >= Player.Instance.ManaPercent || !Settings.UseQ)
                return;
            LogicOber.CastQ(false);
        }
    }
}

