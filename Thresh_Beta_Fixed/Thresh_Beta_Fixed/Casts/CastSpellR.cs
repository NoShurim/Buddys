using EloBuddy;
using EloBuddy.SDK;
using static Thresh_Beta_Fixed.Spells;
namespace Thresh_Beta_Fixed.Casts
{
   public static class CastSpellR
    {
        public static void CastR(AIHeroClient target)
        {
            if (SpellSlot.R.IsReady() && target.IsValidTarget(R.Range) && target.IsEnemy)
            {
                MyChampion.MyThresh.Spellbook.CastSpell(SpellSlot.R);
            }
        }
    }
}
