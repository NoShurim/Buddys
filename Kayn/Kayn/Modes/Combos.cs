using EloBuddy.SDK.Menu.Values;
using System;
using EloBuddy;
using static Kayn.Menus;
using static Kayn.SpellsManager;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace Kayn.Modes
{
    internal class Combos
    {
        internal static void Execute()
        {
            if (Combo["Qk"].Cast<CheckBox>().CurrentValue)
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                var qpre = Q.GetPrediction(target);
                if (target.IsValidTarget(Q.Range) && Q.IsReady() && qpre.HitChance >= HitChance.High)
                {
                    Q.Cast(target);
                }
            }
            if (Combo["Wk"].Cast<CheckBox>().CurrentValue)
            {
                var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                var wpre = W.GetPrediction(target);
                if (target.IsValidTarget(W.Range) && W.IsReady() && wpre.HitChance >= HitChance.High)
                {
                    W.Cast(target);
                }
            }
            if (Combo["Rk"].Cast<CheckBox>().CurrentValue)
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                var predHealth = Prediction.Health.GetPrediction(target, R.CastDelay);
                if (target.IsValidTarget(R.Range) && R.IsReady())
                {
                    R.Cast(target);
                }
            }
        }
    }
}