using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using System;

namespace Cassiopeia_Beta_Fixed
{
    internal class SpellManager
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Targeted E;
        public static Spell.Skillshot R;
        public static Spell.Skillshot Flash;

        internal static void Execute()
        {
            Q = new Spell.Skillshot(spellSlot: SpellSlot.Q, spellRange: 850, skillShotType: SkillShotType.Circular, castDelay: 500, spellSpeed: 1550, spellWidth: 60);

            /*{
               AllowedCollisionCount = 0
            };*/

            W = new Spell.Skillshot(spellSlot: SpellSlot.W, spellRange: 800, skillShotType: SkillShotType.Circular, castDelay: 500, spellSpeed: 1550, spellWidth: 60);
           
            E = new Spell.Targeted(SpellSlot.E, 700);

            R = new Spell.Skillshot(spellSlot: SpellSlot.R, spellRange: 825, skillShotType: SkillShotType.Cone, castDelay: 500, spellSpeed: 1550, spellWidth: 60);
        }
    }
}