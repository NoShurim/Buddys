﻿using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using System;

namespace Nunu_Beta_Fixed
{
    public static class SpellManager
    {
        public static Spell.Targeted Q;
        public static Spell.Targeted W;
        public static Spell.Targeted E;
        public static Spell.Active R;
        public static Spell.Targeted Smite;
        public static Spell.Targeted Ignite;

        

        static SpellManager()
        {
            Q = new Spell.Targeted(SpellSlot.Q, 350);
            W = new Spell.Targeted(SpellSlot.W, 700);
            E = new Spell.Targeted(SpellSlot.E, 550);
            R = new Spell.Active(SpellSlot.R, 650);

            if (SmiteDamage.SmiteNames.ToList().Contains(Player.Instance.Spellbook.GetSpell(SpellSlot.Summoner1).Name))
            {
                Smite = new Spell.Targeted(SpellSlot.Summoner1, 570);
                return;
            }
            if (SmiteDamage.SmiteNames.ToList().Contains(Player.Instance.Spellbook.GetSpell(SpellSlot.Summoner2).Name))
            {
                Smite = new Spell.Targeted(SpellSlot.Summoner2, 570);
            }

            if (Player.Instance.Spellbook.GetSpell(SpellSlot.Summoner1).Name.Equals("summonerdot", StringComparison.CurrentCultureIgnoreCase))
            {
                Ignite = new Spell.Targeted(SpellSlot.Summoner1, 600);
            }
            if (Player.Instance.Spellbook.GetSpell(SpellSlot.Summoner2).Name.Equals("summonerdot", StringComparison.CurrentCultureIgnoreCase))
            {
                Ignite = new Spell.Targeted(SpellSlot.Summoner2, 600);
            }
        }

        public static void Execute()
        {
        }

        public static bool HasSmite()
        {
            return Smite != null && Smite.IsLearned;
        }

        public static bool HasIgnite()
        {
            return Ignite != null;
        }
    }
}
