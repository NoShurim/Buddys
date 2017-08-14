using System.Linq;
using EloBuddy.SDK;
using Settings = Kalista_Beta_Fixed.Menus.Modes.LaneClear;

namespace Kalista_Beta_Fixed
{
    public class LaneClear : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear);
        }

        public override void Execute()
        {
            if (Player.ManaPercent < Settings.MinMana)
            {
                return;
            }

            if (!(Settings.UseQ && Q.IsReady()) && !(Settings.UseE && E.IsReady()))
            {
                return;
            }
            var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.ServerPosition, Q.Range, false);

            #region E usage

            if (Settings.UseE && E.IsReady())
            {
                var minionsInRange = minions.Where(m => E.IsInRange(m)).ToArray();


                if (minionsInRange.Length >= Settings.MinNumberE)
                {
                    var killableNum = 0;
                    foreach (var minion in minionsInRange.Where(minion => minion.IsRendKillable()))
                    {

                        killableNum++;

                        if (killableNum >= Settings.MinNumberE)
                        {
                            E.Cast();
                            break;
                        }
                    }
                }
            }

            #endregion
        }
    }
}
