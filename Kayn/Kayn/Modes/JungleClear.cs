using EloBuddy.SDK;
using static Kayn.Menus;
using static Kayn.SpellsManager;
using System;
using System.Linq;
using EloBuddy.SDK.Menu.Values;

namespace Kayn.Modes
{
    internal class JungleClear
    {
        internal static void Execute()
        {
            var TragetMonster = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(a => a.MaxHealth).FirstOrDefault(a => a.IsValidTarget(500));

            if (TragetMonster == null || TragetMonster.IsInvulnerable || TragetMonster.MagicImmune)
            {
                return;
            }
            if (Jungle["Qj"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                Q.Cast(TragetMonster);
            }
            if (Jungle["Wj"].Cast<CheckBox>().CurrentValue && W.IsReady() && TragetMonster.IsValidTarget(W.Range))
            {
                W.Cast(TragetMonster);
            }
        }
    }
}