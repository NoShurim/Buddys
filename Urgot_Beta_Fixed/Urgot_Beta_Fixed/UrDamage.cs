using EloBuddy;
using EloBuddy.SDK;
using static Urgot_Beta_Fixed.Champion;

namespace Urgot_Beta_Fixed
{
    class UrDamage
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
                if (W.IsReady())
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
        public static float Qmage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 25, 70, 115, 160, 205 }[Q.Level] + 1.1f * Player.Instance.FlatPhysicalDamageMod + 0.4f * Player.Instance.FlatMagicDamageMod));
        }
        public static float Wmage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 12, 20, 24, 28, 32, 36 }[W.Level] + 1.1f * Player.Instance.FlatPhysicalDamageMod + 0.4f * Player.Instance.FlatMagicDamageMod));
        }
        public static float Emage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 60, 100, 140, 180, 220 }[E.Level] + 1.1f * Player.Instance.FlatPhysicalDamageMod + 0.4f * Player.Instance.FlatMagicDamageMod));
        }
        public static float Rmage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 250, 300, 450 }[R.Level] + 1f * Player.Instance.FlatPhysicalDamageMod + 1f * Player.Instance.FlatMagicDamageMod));
        }
    }
}
   