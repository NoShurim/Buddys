using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace Janna_Beta_Fixed
{
    internal class Spells
    {
        public static Spell.Skillshot Q;
        public static Spell.Chargeable Q2;
        public static Spell.Targeted W;
        public static Spell.Targeted E;
        public static Spell.Active R;

        internal static void Execute()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 875, SkillShotType.Linear, 0, 2000, 125);
            Q2 = new Spell.Chargeable(SpellSlot.Q, 875, 1700, 200);
            W = new Spell.Targeted(SpellSlot.W, 600);
            E = new Spell.Targeted(SpellSlot.E, 800);
            R = new Spell.Active(SpellSlot.R, 725);
        }
    }
}