using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using System;

namespace Brand_Beta_Fixed
{
    internal class SpellsManager
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Targeted E;
        public static Spell.Targeted R;

        internal static void Execute()
        {
            Q = new Spell.Skillshot(spellSlot: SpellSlot.Q, spellRange: 1050, skillShotType: SkillShotType.Linear, castDelay: 250, spellSpeed: 1550, spellWidth: 60)
            {
                AllowedCollisionCount = 0
            };
            W = new Spell.Skillshot(spellSlot: SpellSlot.W, spellRange: 900, skillShotType: SkillShotType.Circular, castDelay: 250, spellSpeed: 1550, spellWidth: 60)
            {
                AllowedCollisionCount = int.MaxValue
            };
            E = new Spell.Targeted(SpellSlot.E, 625);
            R = new Spell.Targeted(SpellSlot.R, 750);

        }
    }
}