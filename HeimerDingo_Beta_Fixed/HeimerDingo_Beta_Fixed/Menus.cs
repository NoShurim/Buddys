using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System;

namespace HeimerDingo_Beta_Fixed
{
    internal class Menus
    {
        public static Menu hei, pre, comb, Auto, Lane, Jungle, Misc, Draws;
        internal static void InMenu()
        {
            hei = MainMenu.AddMenu("Heimer", "Heimer");
            pre = hei.AddSubMenu("Prediction");
            pre.Add("Wp", new Slider("Prediction [W] > {0}", 50,1));
            pre.Add("Ep", new Slider("Prediction [E] > {0}", 65, 1));
            comb = hei.AddSubMenu("Combo");
            comb.Add("Cq", new CheckBox("Use [Q] Combo"));
            comb.Add("Cw", new CheckBox("Use [W] Combo"));
            comb.Add("Ce", new CheckBox("Use [E] Combo"));
            comb.Add("Cr", new CheckBox("Use [R] Combo"));
            comb.AddSeparator();
            comb.AddLabel("Settings [R]");
            comb.Add("Rq", new CheckBox("Use [R] + [Q]", false));
            comb.Add("Rw", new CheckBox("Use [R] + [W]"));
            comb.Add("Re", new CheckBox("Use [R] + [E]", false));
            Auto = hei.AddSubMenu("Auto Harass");
            Auto.Add("AutoW", new CheckBox("Use [W]"));
            Auto.Add("AutoE", new CheckBox("Use [E]"));
            Auto.AddSeparator();
            Auto.AddLabel("Mana Percent");
            Auto.Add("Mana", new Slider("Mana Percent [W] And [E] > {0}", 65,1));
            Lane = hei.AddSubMenu("Lane [Clear]");
            Lane.Add("Ql", new CheckBox("Use [Q] Lane"));
            Lane.Add("Wl", new CheckBox("Use [W] Lane"));
            Lane.Add("El", new CheckBox("Use [E] Lane"));
            Lane.AddSeparator();
            Lane.AddLabel("Mana Percent");
            Lane.Add("ManaL", new Slider("Mana Percent > {0}", 50, 1));
            Lane.AddSeparator();
            Lane.AddLabel("Settings [Q]");
            Lane.Add("MiniQ", new Slider("Minimum [Q]", 2, 1, 3));
            Jungle = hei.AddSubMenu("Jungle [Clear]");
            Jungle.AddLabel("Not Add");
            Misc = hei.AddSubMenu("Misc");
            Misc.Add("inter", new CheckBox("Use [E] Inter"));
            Misc.Add("Gap", new CheckBox("Aint-Gap"));
            Draws = hei.AddSubMenu("Draws");
            Draws.Add("DQ", new CheckBox("Use [Q] Draws"));
            Draws.Add("DW", new CheckBox("Use [W] Draws"));
            Draws.Add("DE", new CheckBox("Use [E] Draws"));

        }
    }
}