using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kayn_BETA_Fixed
{
    class Program
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Active E;
        public static Spell.Targeted R;
        public static Spell.Skillshot R2;
        public static AIHeroClient _Player;
        public static Menu Kmenu, Combo, Hara, Lane, Jungle, Misc;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_On;
        }

        private static void Loading_On(EventArgs args)
        {
            if (Player.Instance.Hero == Champion.Kayn)
                Chat.Print("[Addon] [Champion] [Kayn]", System.Drawing.Color.LightBlue);
            else if (Player.Instance.Hero != Champion.Kayn)
            {
                return;
            }
            Q = new Spell.Skillshot(SpellSlot.Q, 350, EloBuddy.SDK.Enumerations.SkillShotType.Linear, 150, 75, 37);
            W = new Spell.Skillshot(SpellSlot.W, 700, EloBuddy.SDK.Enumerations.SkillShotType.Linear, 350, 175, 87);
            E = new Spell.Active(SpellSlot.E);
            R = new Spell.Targeted(SpellSlot.R, 550);
            R2 = new Spell.Skillshot(SpellSlot.R, 150, EloBuddy.SDK.Enumerations.SkillShotType.Linear, 75, 37, 18);

        }
    }
}
