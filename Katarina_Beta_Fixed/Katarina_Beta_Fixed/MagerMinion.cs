using EloBuddy;
using EloBuddy.SDK;
using static Katarina_Beta_Fixed.Program;

namespace Katarina_Beta_Fixed
{
   public static class MagerMinion
    {
        public static float GetMinionTarget(this Obj_AI_Base target, SpellSlot slot)
        {
            var damageType = DamageType.Magical;
            var ap = Player.Instance.TotalMagicalDamage;
            var sLevel = Player.GetSpell(slot).Level - 1;

            var dmg = 0f;

            switch (slot)
            {
                case SpellSlot.Q:
                    if (Q.IsReady())
                        dmg += new float[] { 75, 120, 165, 210, 255 }[sLevel] + 0.70f * ap;
                    break;
                case SpellSlot.W:
                    if (W.IsReady())
                        dmg += new float[] { 10, 15, 20, 25, 30 }[sLevel] + 0.10f * ap;
                    break;
                case SpellSlot.E:
                    if (E.IsReady() && !(target.HasBuffOfType(BuffType.Poison)))
                        dmg += new float[] { 64, 70, 80, 90, 110 }[sLevel] + 0.10f * ap;
                    if (E.IsReady() && target.HasBuffOfType(BuffType.Poison))
                        dmg += new float[] { 66, 100, 154, 192, 222 }[sLevel] + 0.55f * ap;
                    break;
                case SpellSlot.R:
                    if (R.IsReady())
                        dmg += new float[] { 150, 250, 350 }[sLevel] + 0.50f * ap;
                    break;
            }

            return Player.Instance.CalculateDamageOnUnit(target, damageType, dmg - 10);
        }
    }
}
