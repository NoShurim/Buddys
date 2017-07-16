using EloBuddy.SDK;
using static Kayn.SpellsManager;
using static Kayn.Menus;
using System;
using EloBuddy;
using System.Linq;
using EloBuddy.SDK.Menu.Values;

namespace Kayn.Modes
{
    internal class LaneClear
    {
        internal static void Execute()
        {
            var laneQ = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, Q.Range);
            var laneE = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, E.Range);

            if (Lane["Q1"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                foreach (var minionQ in laneQ)
                {
                    if (Player.Instance.ManaPercent >= Lane["Mi"].Cast<Slider>().CurrentValue)
                    {
                        Q.Cast(minionQ);
                    }
                }
            }
            if (Lane["W1"].Cast<CheckBox>().CurrentValue && W.IsReady()) 
            {
                foreach (var minionE in laneE)
                {
                    if (Player.Instance.ManaPercent >= Lane["Mi2"].Cast<Slider>().CurrentValue)
                    {
                        W.Cast(minionE);
                    }
                }
            }
        }
    }
}