using EloBuddy.SDK.Menu.Values;
using System;
using EloBuddy;
using static Kayn.Menus;
using static Kayn.SpellsManager;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using System.Linq;

namespace Kayn.Modes
{
    internal class Combos
    {
        internal static void Execute()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if ((target == null) || target.IsInvulnerable) return;

            if (Combo["Qk"].Cast<CheckBox>().CurrentValue)
            {
                if (target.IsValidTarget(Q.Range) && Q.IsReady())
                {
                    var qhitchance = Q.GetPrediction(target);
                    if (qhitchance.HitChance >= HitChance.High) 
                    {
                        Q.Cast(qhitchance.CastPosition);
                    }
                }
            }
            if (Combo["Ek"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                var active = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                if (active.IsValidTarget(700))
                {
                    E.Cast(active);
                }
            }
            if (Combo["Wk"].Cast<CheckBox>().CurrentValue && W.IsReady())
            {
                var targetw = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                var wpre = W.GetPrediction(targetw);
                if (targetw.IsValidTarget(W.Range) && wpre.HitChance >= HitChance.High)
                {
                    W.Cast(targetw);
                }
            }
            if (Combo["Rk"].Cast<CheckBox>().CurrentValue)
            {
                var targetr = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (targetr.IsValidTarget(R.Range) && R.IsReady())
                {
                    R.Cast(targetr);
                }
                else if (R2.IsReady())
                {
                    R2.Cast();
                }
            }
        }
    }
}