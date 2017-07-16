using EloBuddy.SDK;
using static Kayn.Menus;
using static Kayn.SpellsManager;
using System;
using System.Linq;
using EloBuddy.SDK.Menu.Values;
using EloBuddy;

namespace Kayn.Modes
{
    internal class JungleClear
    {
        internal static void Execute()
        {
            if (Jungle["Qj"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                var jungleQ = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Q.Range);
                if (jungleQ != null)
                {
                    foreach (var junglemonsterq in jungleQ)
                    {
                        if (Player.Instance.ManaPercent >= Lane["Mi"].Cast<Slider>().CurrentValue)
                        {
                            Q.Cast(junglemonsterq);
                        }
                    }
                }
            }
            if (Jungle["Wj"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                var jungleE = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, W.Range);
                if (jungleE != null)
                {
                    foreach (var junglemonstere in jungleE)
                    {
                        if (Player.Instance.ManaPercent >= Lane["Mi2"].Cast<Slider>().CurrentValue)
                        {
                            W.Cast(junglemonstere);
                        }
                    }
                }
            }
        }
    }
}