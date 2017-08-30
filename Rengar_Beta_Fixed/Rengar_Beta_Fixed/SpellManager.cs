using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using System;
using System.Linq;
using static Rengar_Beta_Fixed.Kitten;

namespace Rengar_Beta_Fixed
{
    internal class SpellManager
    {
        public static Spell.Active W, R;
        public static Spell.Skillshot E, Q;
        public static Spell.Targeted Smite;

        internal static void Execute()
        {
            Q = new Spell.Skillshot(spellSlot: SpellSlot.Q, spellRange: 325, skillShotType: SkillShotType.Linear, castDelay: 250, spellSpeed: 1550, spellWidth: 60)
            {
                AllowedCollisionCount = 0
            };
            W = new Spell.Active(spellSlot: SpellSlot.W, spellRange: 450);
            E = new Spell.Skillshot(spellSlot: SpellSlot.E, spellRange: 1000, skillShotType: SkillShotType.Linear, castDelay: 250, spellSpeed: 1550, spellWidth: 60)
             {
                AllowedCollisionCount = int.MaxValue
            };
            R = new Spell.Active(SpellSlot.R, 2000);

            var smiteSlot = Gatinho.Spellbook.Spells.FirstOrDefault(x => x.Name.ToLower().Contains("smite"));
            if (smiteSlot != null)
            {

                Smite = new Spell.Targeted(smiteSlot.Slot, 700);

            }
        }
    }
}