using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using System;

namespace Tristana_Beta_Fixed
{
    internal class SpellsManager
    {

        public static Spell.Active Q;
        public static Spell.Skillshot W;
        public static Spell.Targeted E;
        public static Spell.Targeted R;
        internal static void Execute()
        {
            var range = (uint)(630 + 7 * (Player.Instance.Level - 1));
            Q = new Spell.Active(SpellSlot.Q, range);
            W = new Spell.Skillshot(SpellSlot.W, 925, SkillShotType.Circular, 250, 1200, 150);
            E = new Spell.Targeted(SpellSlot.E, range);
            R = new Spell.Targeted(SpellSlot.R, range, DamageType.Magical);
        }
    }
}