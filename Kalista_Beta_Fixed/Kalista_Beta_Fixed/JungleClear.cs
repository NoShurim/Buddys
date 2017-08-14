using System.Linq;
using EloBuddy.SDK;
using Settings = Kalista_Beta_Fixed.Menus.Modes.JungleClear;

namespace Kalista_Beta_Fixed
{
    public class JungleClear : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear);
        }

        public override void Execute()
        {
            if (!Settings.UseE || !E.IsReady())
            {
                return;
            }

            if (EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.ServerPosition, E.Range, false).Any(m => m.IsRendKillable()))
            {
                E.Cast();
            }
        }
    }
}
