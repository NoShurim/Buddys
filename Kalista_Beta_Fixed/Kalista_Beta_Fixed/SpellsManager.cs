using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using SharpDX;

namespace Kalista_Beta_Fixed
{
    internal class SpellsManager
    {
        public static Spell.Skillshot Q;
        public static Spell.Targeted W;
        public static Spell.Active E;
        public static Spell.Active R;

        internal static void Execute()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1150, SkillShotType.Linear, 250, 2100, 40);
            W = new Spell.Targeted(SpellSlot.W, 5000);
            E = new Spell.Active(SpellSlot.E, 1000);
            R = new Spell.Active(SpellSlot.R, 1100);
        }
    }
}