using EloBuddy;
using EloBuddy.SDK;
using static Kayn.Menus;
using static Kayn.SpellsManager;
using System;
using EloBuddy.SDK.Menu.Values;

namespace Kayn.Modes
{
    internal class Harass
    {
        internal static void Execute()
        {
            var wtarget = TargetSelector.GetTarget(W.Range, DamageType.Physical);

            if (Hara["Wh"].Cast<CheckBox>().CurrentValue && W.IsReady())
            {
                var wpre = W.GetPrediction(wtarget);
                if (wpre.HitChance >= EloBuddy.SDK.Enumerations.HitChance.High)
                {
                    W.Cast(wpre.CastPosition);
                }
            }


        }
    }
}