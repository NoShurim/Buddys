using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using SharpDX;

namespace Thresh_Beta_Fixed
{
    internal class Spells
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Skillshot E;
        public static Spell.Active R;

        internal static void Execute()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1070, SkillShotType.Linear, 500, 1900, 60)
            {
                AllowedCollisionCount = 0
            };
            W = new Spell.Skillshot(SpellSlot.W, 950, SkillShotType.Circular, 250, 1800, 300)
            {
                AllowedCollisionCount = int.MaxValue
            };
            E = new Spell.Skillshot(SpellSlot.E, 500, SkillShotType.Linear, 0, 2000, 110)
            {
                AllowedCollisionCount = int.MaxValue
            };
            R = new Spell.Active(SpellSlot.R, 450);
        }
    }
}