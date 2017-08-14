using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using System;

namespace Fiddle_Beta_Fixed
{
    internal class SpellsManager
    {
        public static Spell.Targeted Q;
        public static Spell.Targeted W;
        public static Spell.Targeted E;
        public static Spell.Skillshot R;

        internal static void Execute()
        {
            Q = new Spell.Targeted(SpellSlot.Q, 575);
            W = new Spell.Targeted(SpellSlot.W, 575);
            E = new Spell.Targeted(SpellSlot.E, 750);
            R = new Spell.Skillshot(SpellSlot.R, 800, SkillShotType.Circular);
        }
    }
}