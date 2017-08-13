using EloBuddy;
using EloBuddy.SDK;
using System;
using System.Collections.Generic;
using static Brand_Beta_Fixed.SpellsManager;

namespace Brand_Beta_Fixed
{
    public static class Logics
    {
        private static readonly Dictionary<SpellSlot, int[]> BaseDamage = new Dictionary<SpellSlot, int[]>();
        private static readonly Dictionary<SpellSlot, float[]> BonusDamage = new Dictionary<SpellSlot, float[]>();

      public static float DamageBySlot(Obj_AI_Base enemy, SpellSlot slot)
        {
            var Damage = 0f;
            if (slot == SpellSlot.Q)
            {
                if (Q.IsReady())
                    Damage += new float[] { 80, 110, 140, 170, 200 }[Player.GetSpell(slot).Level - 1] +
                              0.55f * Player.Instance.FlatMagicDamageMod;
            }
            else if (slot == SpellSlot.W)
            {
                if (W.IsReady())
                    Damage += new float[] { 75, 120, 165, 210, 255 }[Player.GetSpell(slot).Level - 1] +
                              0.6f * Player.Instance.FlatMagicDamageMod;
            }
            else if (slot == SpellSlot.E)
            {
                if (E.IsReady())
                    Damage += new float[] { 70, 90, 110, 130, 150 }[Player.GetSpell(slot).Level - 1] +
                              0.35f * Player.Instance.FlatMagicDamageMod;
            }
            else if (slot == SpellSlot.R)
            {
                if (R.IsReady())
                    Damage += new float[] { 100, 200, 300 }[Player.GetSpell(slot).Level - 1] +
                              0.25f * Player.Instance.FlatMagicDamageMod;
            }
            return Player.Instance.CalculateDamageOnUnit(enemy, DamageType.Magical, Damage);
        }
        public static float HPrediction(Obj_AI_Base e, int d) => Prediction.Health.GetPrediction(e, d);

        internal static void Execute()
        {
            BaseDamage.Add(SpellSlot.Q, new[] { 80, 110, 140, 170, 200 });
            BaseDamage.Add(SpellSlot.W, new[] { 75, 120, 165, 210, 255 });
            BaseDamage.Add(SpellSlot.E, new[] { 70, 90, 140, 175, 210 });
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
            var buff = unit.GetBuff("BrandAblaze");

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
    }
}