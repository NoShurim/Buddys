using EloBuddy;
using EloBuddy.SDK;
using static Cassiopeia_Beta_Fixed.SpellManager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cassiopeia_Beta_Fixed
{
    public static class Logics
    {
        private static readonly Dictionary<SpellSlot, int[]> BaseDamage = new Dictionary<SpellSlot, int[]>();
        private static readonly Dictionary<SpellSlot, float[]> BonusDamage = new Dictionary<SpellSlot, float[]>();

        internal static void Execute()
        {
            BaseDamage.Add(SpellSlot.Q, new[] { 75, 125, 165, 210, 255 });
            BaseDamage.Add(SpellSlot.W, new[] { 20, 35, 50, 65, 80 });
            BaseDamage.Add(SpellSlot.E, new[] { 10, 40, 70, 100, 130 });
            BaseDamage.Add(SpellSlot.R, new[] { 150, 250, 350 });

            BonusDamage.Add(SpellSlot.Q, new[] { 0.65f, 0.65f, 0.65f, 0.65f, 0.65f });
            BonusDamage.Add(SpellSlot.W, new[] { 0.6f, 0.6f, 0.6f, 0.6f, 0.6f });
            BonusDamage.Add(SpellSlot.E, new[] { 0.55f, 0.55f, 0.55f, 0.55f, 0.55f });
            BonusDamage.Add(SpellSlot.R, new[] { 0.5f, 0.5f, 0.5f });
        }
        public static float CalculateDamage(SpellSlot slot, Obj_AI_Base unit)
        {
            if (slot == SpellSlot.Internal)
            {
                return Player.Instance.CalculateDamageOnUnit(unit, DamageType.Magical, unit.MaxHealth * 0.08f) - unit.FlatHPRegenMod * 4;
            }

            var spellLevel = Player.GetSpell(slot).Level;
            var abilityPower = Player.Instance.TotalMagicalDamage;
            var baseDmg = BaseDamage[slot];
            var bonusDmg = BonusDamage[slot];

            if (spellLevel == 0)
            {
                return 0;
            }

            return Player.Instance.CalculateDamageOnUnit(unit, DamageType.Magical,
                baseDmg[spellLevel - 1] + bonusDmg[spellLevel - 1] * abilityPower);
        }

        public static float TotalDamage(SpellSlot slot, Obj_AI_Base unit)
        {
            return CalculateDamage(slot, unit) + CalculateDamage(SpellSlot.Internal, unit);
        }

        public static bool Killable(this Obj_AI_Base target, SpellSlot slot)
        {
            return TotalDamage(slot, target) >= target.Health;
        }

        public static float GetIgniteDamage()
        {
            return (10 + Player.Instance.Level * 4) * 5;
        }

        public static float GetBlazeRemainingDamage(Obj_AI_Base unit)
        {
            var buff = unit.GetBuff("caassiopeiaspeed");

            if (buff == null)
            {
                return 0;
            }

            return Player.Instance.CalculateDamageOnUnit(unit, DamageType.Magical,
                unit.MaxHealth * 0.02f * (float)Math.Floor(buff.EndTime - Game.Time));
        }

        public static bool WillDie(this Obj_AI_Base unit)
        {
            return GetBlazeRemainingDamage(unit) >= unit.Health;
        }
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

        public static float GetRealDamage(this Obj_AI_Base target)
        {
            var slots = new[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
            var dmg = Player.Spells.Where(s => slots.Contains(s.Slot)).Sum(s => target.GetMinionTarget(s.Slot));

            return dmg;
        }
    }
}

