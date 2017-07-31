using EloBuddy;
using EloBuddy.SDK;
using static Thresh_Beta_Fixed.Spells;
using static Thresh_Beta_Fixed.Menus;
using EloBuddy.SDK.Menu.Values;

namespace Thresh_Beta_Fixed.Casts
{
    public static class PushE
    {
        public static void CastE(Obj_AI_Base target)
        {
            if (SpellSlot.E.IsReady() && target.IsValidTarget(E.Range) && target.IsEnemy)
            {
                var pred = E.GetPrediction(target);
                if (pred.HitChancePercent >= Pre["pree"].Cast<Slider>().CurrentValue)
                {
                    E.Cast(pred.CastPosition);
                }
            }
        }
    }
}