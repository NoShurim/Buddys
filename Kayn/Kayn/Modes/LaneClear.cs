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
            var LaneW = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition, W.Range, true).Count();
            if (LaneW == 0) return;
            var startvoid = EntityManager.MinionsAndMonsters.GetLaneMinions().OrderBy(a => a.MaxHealth).FirstOrDefault(a => a.IsValidTarget(Q.Range));
            if (startvoid == null || startvoid.IsInvulnerable || startvoid.MagicImmune)
            {
                return;
            }

            if (Lane["W1"].Cast<CheckBox>().CurrentValue && W.IsReady() && Player.Instance.ManaPercent >= Lane["Mi"].Cast<Slider>().CurrentValue)
            {
                W.Cast(startvoid);
            }
            if (Lane["Q1"].Cast<CheckBox>().CurrentValue && Q.IsReady() && Player.Instance.ManaPercent >= Lane["Mi"].Cast<Slider>().CurrentValue)
            {
                Q.Cast(startvoid);
            }
        }
    }
}