using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System;

namespace Rengar_Beta_Fixed
{
    internal class Menus
    {
        public static Menu cat, Combo, Harass, Lane, Jungle, Draws, Misc;
        internal static void Execute()
        {
            cat = MainMenu.AddMenu("Rengar", "Rengar");

            Combo = cat.AddSubMenu("Combo");
            Combo.Add("ps", new ComboBox("Priority", 0, "Q + Stack", "W + Stack", "E + Stack"));
            Combo.Add("Qc", new CheckBox("Use [Q] In Combo"));
            Combo.Add("Wc", new CheckBox("Use [W] In Combo"));
            Combo.Add("Ec", new CheckBox("Use [E] In Combo"));
            Combo.AddSeparator();
            Combo.AddLabel("Key [R]");
            Combo.Add("KeyR", new KeyBind("Key [R]", false, KeyBind.BindTypes.HoldActive, 'A'));
            Combo.AddSeparator();
            Combo.AddLabel("Settings [Spells]");
            Combo.Add("autow", new CheckBox("Use AutoW [CC]"));
            Combo.Add("eq", new CheckBox("Use [E] + Passive + [Q] In Logic"));

            Harass = cat.AddSubMenu("Harass");
            Harass.Add("ps", new ComboBox("Priority", 0, "Q + Stack", "W + Stack", "E + Stack"));
            Harass.Add("Hq", new CheckBox("Use [Q] Harass"));
            Harass.Add("Hw", new CheckBox("Use [W] Harass"));
            Harass.Add("He", new CheckBox("Use [E] Harass"));

            Lane = cat.AddSubMenu("LaneClear");
            Lane.Add("stack", new CheckBox("Stack Lane"));
            Lane.Add("ps", new ComboBox("Priority", 0, "Q + Stack", "W + Stack", "E + Stack"));
            Lane.Add("Ql", new CheckBox("Use [Q] Lane"));
            Lane.Add("Wl", new CheckBox("Use [W] Lane"));
            Lane.Add("Wm", new Slider("Percent Minions > %", 3, 1, 6));
            Lane.Add("El", new CheckBox("Use [E] Lane"));

            Jungle = cat.AddSubMenu("JungleClear");
            Jungle.Add("stack", new CheckBox("Stack Jungle"));
            Jungle.Add("ps", new ComboBox("Priority", 0, "Q + Stack", "W + Stack", "E + Stack"));
            Jungle.Add("Qj", new CheckBox("Use [Q] Jungle"));
            Jungle.Add("Wj", new CheckBox("Use [W] Jungle"));
            Jungle.Add("Ej", new CheckBox("Use [E] Jungle"));

            Misc = cat.AddSubMenu("Misc");
            Misc.Add("Gap", new CheckBox("Use [E] GapClose"));

            Draws = cat.AddSubMenu("Draws");
            Draws.Add("DQ", new CheckBox("Draws [Q]"));
            Draws.Add("DR", new CheckBox("Draws [R]"));           
        }
    }
}