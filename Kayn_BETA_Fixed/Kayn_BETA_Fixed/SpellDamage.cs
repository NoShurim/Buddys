using EloBuddy;
using EloBuddy.SDK;
using static Kayn_BETA_Fixed.SpellManager;

namespace Kayn_BETA_Fixed
{
    internal class SpellDamage
    {
        internal static float GetRawDamage(Obj_AI_Base target)
        {
            float damage = 0;
            if (target != null)
            {
                if (Qk.IsReady())
                {
                    damage += Player.Instance.GetSpellDamage(target, SpellSlot.Q);
                    damage += Player.Instance.GetAutoAttackDamage(target);
                }
                if (W.IsReady())
                {
                    damage += Player.Instance.GetSpellDamage(target, SpellSlot.W);
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
                (float)(new[] { 0, 55, 75, 95, 115, 135 }[Qk.Level] + 1.1f * Player.Instance.FlatPhysicalDamageMod + 0.4f * Player.Instance.FlatMagicDamageMod));
        }
        public static float Emage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 0, 0, 0 }[Qk.Level] + 1.1f * Player.Instance.FlatPhysicalDamageMod + 0.4f * Player.Instance.FlatMagicDamageMod));
        }
        public static float Wmage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 80, 125, 170, 215, 260 }[W.Level] + 0.8f * Player.Instance.FlatMagicDamageMod));
        }
        public static float Rmage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 250, 350, 450, 550 }[R.Level] + 1f * Player.Instance.FlatPhysicalDamageMod + 1f * Player.Instance.FlatMagicDamageMod));
        }
    }
}
