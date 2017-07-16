using EloBuddy;
using EloBuddy.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kayn.Modes
{
    class Active
    {
        public static void InitializeModes()
        {
            Game.OnTick += Game_OnTick;
        }

        private static void Game_OnTick(EventArgs args)
        {

            KillSeteal.Execute();
            Draws.Execute();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combos.Execute();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass.Execute();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear.Execute();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear.Execute();
            }
        }
    }
}