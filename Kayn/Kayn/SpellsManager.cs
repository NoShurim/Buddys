using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace Kayn
{
    internal class SpellsManager
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot Q2;
        public static Spell.Skillshot W;
        public static Spell.Skillshot W2;
        public static Spell.Active E;
        public static Spell.Targeted R;
        public static Spell.Skillshot R2;

        private static AIHeroClient myHero
        {
            get { return Player.Instance; }
        }

        public SpellsManager()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 250, EloBuddy.SDK.Enumerations.SkillShotType.Linear, 150, 75, 37);
            Q2 = new Spell.Skillshot(SpellSlot.Q, 375, SkillShotType.Linear, 150, 75, 50);
            W = new Spell.Skillshot(SpellSlot.W, 700, EloBuddy.SDK.Enumerations.SkillShotType.Linear, 350, 175, 87);
            W2 = new Spell.Skillshot(SpellSlot.W, 800, SkillShotType.Linear, 400, 200, 100);
            E = new Spell.Active(SpellSlot.E);
            R = new Spell.Targeted(SpellSlot.R, 475);
            R2 = new Spell.Skillshot(SpellSlot.R, 150, EloBuddy.SDK.Enumerations.SkillShotType.Linear, 75, 37, 18);
        }
        public static float QDamage(Obj_AI_Base target)
        {
            if (!Player.GetSpell(SpellSlot.Q).IsLearned) return 0;
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)new double[] { 55, 75, 95, 115, 135 }[Q.Level - 1] + 1 * Player.Instance.FlatMagicDamageMod);
        }
        public static float WDamage(Obj_AI_Base target)
        {
            if (!Player.GetSpell(SpellSlot.W).IsLearned) return 0;
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)new double[] { 170, 215, 260 }[W.Level - 1] + 1 * Player.Instance.FlatMagicDamageMod);
        }
        public static float RDamage(Obj_AI_Base target)
        {
            if (!Player.GetSpell(SpellSlot.R).IsLearned) return 0;
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)new double[] { 150, 250, 350 }[R.Level - 1] + 1 * Player.Instance.FlatMagicDamageMod);
        }
    }
}