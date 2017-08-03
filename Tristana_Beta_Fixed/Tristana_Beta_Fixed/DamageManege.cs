using EloBuddy;
using EloBuddy.SDK;
using static Tristana_Beta_Fixed.SpellsManager;

namespace Tristana_Beta_Fixed
{
    public static class DamageManege
    {
        internal static float GetRawDamage(Obj_AI_Base target)
        {
            float damage = 0;
            if (target != null)
            {
                if (Q.IsReady())
                {
                    damage += Player.Instance.GetSpellDamage(target, SpellSlot.Q);
                    damage += Player.Instance.GetAutoAttackDamage(target);
                }
                if (E.IsReady())
                {
                    damage += Player.Instance.GetSpellDamage(target, SpellSlot.E);
                    damage += Player.Instance.GetAutoAttackDamage(target);
                }
                if (R.IsReady())
                {
                    damage += Player.Instance.GetSpellDamage(target, SpellSlot.R);
                    damage += Player.Instance.GetAutoAttackDamage(target);
                }
                if (ObjectManager.Player.CanAttack)
                    damage += ObjectManager.Player.GetAutoAttackDamage(target);
            }
            return damage;
        }
        public static float Wmage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 70, 115, 160, 205, 250 }[W.Level] + 1.1f * Player.Instance.FlatPhysicalDamageMod + 0.4f * Player.Instance.FlatMagicDamageMod));
        }
        public static float Emage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 50, 75, 100, 125, 150 }[E.Level] + 1.1f * Player.Instance.FlatPhysicalDamageMod + 0.4f * Player.Instance.FlatMagicDamageMod));
        }
        public static float Rmage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 300, 400, 500 }[R.Level] + 1f * Player.Instance.FlatPhysicalDamageMod + 1f * Player.Instance.FlatMagicDamageMod));
        }
    }
}