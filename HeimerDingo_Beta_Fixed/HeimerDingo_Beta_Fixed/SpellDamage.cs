using System;
using EloBuddy;
using EloBuddy.SDK;
using static HeimerDingo_Beta_Fixed.Program;

namespace HeimerDingo_Beta_Fixed
{
    class SpellDamage
    {
        internal static float GetRawDamage(Obj_AI_Base target)
        {
            float damage = 0;
            if (target != null)
            {
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
        public static float Wmage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 60, 90, 120, 150, 180 }[Q.Level] + 1.1f * Player.Instance.FlatPhysicalDamageMod + 0.4f * Player.Instance.FlatMagicDamageMod));
        }
        public static float Emage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 60, 100, 140, 180, 220 }[E.Level] + 1.1f * Player.Instance.FlatPhysicalDamageMod + 0.4f * Player.Instance.FlatMagicDamageMod));
        }
    }
}