using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System;

namespace Caitlyn__Beta_Fixed
{
    internal class Menus
    {
        public static
           Menu
           Caiy,
           Comb,
           pre,
           Auto,
           Lane,
           Jungle,
           Misc,
           Draws;

        internal static void InMenu()
        {
            Caiy = MainMenu.AddMenu("Caitlyn", "Caitlyn");
            pre = Caiy.AddSubMenu("Prediction");
            pre.Add("HitQ", new ComboBox("HitChance [Q]", 1, "Low", "Medium", "High"));
            pre.Add("HitW", new ComboBox("HitChance [W]", 2, "Low", "Medium", "High"));
            pre.Add("HitE", new ComboBox("HitChance [E]", 1, "Low", "Medium", "High"));
            Comb = Caiy.AddSubMenu("Combo");
            Comb.Add("Qc", new CheckBox("Use [Q]"));
            Comb.Add("Wc", new CheckBox("Use [W]"));
            Comb.Add("Ec", new CheckBox("Use [E]"));
            Comb.AddSeparator();
            Comb.AddLabel("Settings [R]");
            Comb.Add("Rf", new CheckBox("Use [R] Fish Enemy"));
            Auto = Caiy.AddSubMenu("Auto Harass");
            Auto.Add("AutoQ", new CheckBox("AutoHarass [Q]"));
            Auto.AddSeparator();
            Auto.AddLabel("Mana Percent");
            Auto.Add("ManaQ", new Slider("Mana Percent [Q] > {0}", 65, 1));
            Lane = Caiy.AddSubMenu("Lane [Clear]");
            Lane.Add("Ql", new CheckBox("Use [Q] Lane"));
            Lane.AddSeparator();
            Lane.AddLabel("Mana Percent");
            Lane.Add("ManaL", new Slider("Mana Percent > {0}", 50, 1,100));
            Lane.AddSeparator();
            Lane.AddLabel("Minions");
            Lane.Add("Qmi", new Slider("Minion Percent > {0}", 3, 1, 6));
            Jungle = Caiy.AddSubMenu("Jungle [Clear]");
            Jungle.Add("Qj", new CheckBox("Use [Q] Jungle"));
            Jungle.AddSeparator();
            Jungle.AddLabel("Mana Percent");
            Jungle.Add("Q/J", new Slider("Mana Percent [Q/E]", 65, 1));
            Misc = Caiy.AddSubMenu("Misc");
            Misc.Add("Ks", new CheckBox("KillSteal"));
            Draws = Caiy.AddSubMenu("Draws [Spells]");
            Draws.Add("DQ", new CheckBox("[Q] Draws"));
            Draws.Add("DW", new CheckBox("[W] Draws"));
            Draws.Add("DE", new CheckBox("[E] Draws"));
            Draws.Add("DR", new CheckBox("[R] Draws"));


        }
    }
}