using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using SharpDX;

namespace Vayne_Beta_Fixed
{
    internal class SpellndCast
    {
        public static Spell.Active Q { get; private set; }
        public static Spell.Targeted E { get; private set; }
        public static Spell.Skillshot E2 { get; private set; }
        public static Spell.Active R { get; private set; }

        internal static void Execute()
        {
            Q = new Spell.Active(SpellSlot.Q, 300);
            E = new Spell.Targeted(SpellSlot.E, 550);
            E2 = new Spell.Skillshot(SpellSlot.E, 600, SkillShotType.Linear, 250, 1250);
            R = new Spell.Active(SpellSlot.R);
        }
    }
}