using EloBuddy;
using EloBuddy.SDK;
using System;
using static Thresh_Beta_Fixed.Spells;

namespace Thresh_Beta_Fixed.Casts
{
  public static class CastSpellW
    {
        public static void CastW(Obj_AI_Base target)
        {
            if (SpellSlot.W.IsReady() && target.IsValidTarget(1300) && (target.IsAlly || target.IsMe))
            {
                var pred = W.GetPrediction(target);
                var castPosition = MyChampion.MyThresh.Position + (pred.CastPosition - MyChampion.MyThresh.Position).Normalized() * Math.Min(MyChampion.MyThresh.Distance(pred.CastPosition), W.Range); MyChampion.MyThresh.Spellbook.CastSpell(SpellSlot.W, castPosition);
            }
        }
    }
}
