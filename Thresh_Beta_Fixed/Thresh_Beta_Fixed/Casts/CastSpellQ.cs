using System;
using EloBuddy;
using EloBuddy.SDK;
using static Thresh_Beta_Fixed.Logics.LogicQ;
using static Thresh_Beta_Fixed.Spells;
using static Thresh_Beta_Fixed.Menus;
using EloBuddy.SDK.Menu.Values;

namespace Thresh_Beta_Fixed.Casts
{
    public static class CastSpellQ
    {
        public static void CastQ(Obj_AI_Base target)
        {
            if (SpellSlot.Q.IsReady() && target.IsValidTarget() && target.IsEnemy)
            {
                if (Q1)
                {
                    CastQ1(target);
                }
                else
                {
                    CastQ2(target);
                }
            }
        }
        private static void CastQ2(Obj_AI_Base target)
        {
            if (SpellSlot.Q.IsReady() && !Q1 && target.IsValidTarget() && target.IsEnemy && QT.Distance(MyChampion.MyThresh, true) > QT.Distance(target, true) && Game.Time - TimeQlogic >= 1.1f)
            {
                MyChampion.MyThresh.Spellbook.CastSpell(SpellSlot.Q);
            }
        }
        private static void CastQ1(Obj_AI_Base target)
        {
            if (SpellSlot.Q.IsReady() && Q1 && target.IsValidTarget(Q.Range) && target.IsEnemy)
            {
                Q.Width = 70;
                var pred = Q.GetPrediction(target);
                if (pred.HitChancePercent >= Pre["pre"].Cast<Slider>().CurrentValue)
                {
                    Q.Cast(pred.CastPosition);
                }
            }
        }
        public static bool IsReady(this SpellSlot slot)
        {
            return slot.GetSpellDataInst().IsReady;
        }
        public static SpellDataInst GetSpellDataInst(this SpellSlot slot)
        {
            return MyChampion.MyThresh.Spellbook.GetSpell(slot);
        }
    }
}