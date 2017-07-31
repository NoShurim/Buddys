using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System;

namespace Thresh_Beta_Fixed
{
    internal class Menus
    {
        public static Menu Ty, Pre, Combo, Auto, Misc, Draws;

        internal static void Execute()
        {
            Ty = MainMenu.AddMenu("Thresh", "Thresh");
            Pre = Ty.AddSubMenu("Prediction");
            Pre.AddLabel("Prediction [Q] Combo");
            Pre.Add("pre", new Slider("Prediction [Q]", 85, 1));
            Pre.AddSeparator();
            Pre.AddLabel("Prediction [E] Combo");
            Pre.Add("pree", new Slider("Prediction [E]", 50, 1));
            Pre.AddSeparator();
            Pre.AddLabel("Prediction [Q] AutoHarass");
            Pre.Add("preQ", new Slider("Prediction [Q]", 85));
            
            Combo = Ty.AddSubMenu("Combo");
            Combo.AddLabel("Combo [Q]");
            Combo.Add("Qc", new CheckBox("Use [Q] Combo"));
            Combo.AddSeparator();
            Combo.AddLabel("Combo [W]");
            Combo.Add("Wc", new CheckBox("Use [W] Combo"));
            Combo.AddSeparator();
            Combo.AddLabel("Combo [E]");
            Combo.Add("Ec", new CheckBox("Use [E] Combo"));
            Combo.AddSeparator();
            Combo.AddLabel("Combo [R]");
            Combo.Add("Rc", new CheckBox("Use [R] Combo"));

            Auto = Ty.AddSubMenu("AutoHarass");
            Auto.Add("Qhara", new CheckBox("Use Auto [Q]"));
            Auto.AddSeparator();
            Auto.AddLabel("Prediction Life");
            Auto.Add("prelife", new Slider("Life Enemys", 45, 1));
            Auto.AddSeparator();
            Auto.AddLabel("Mana Percent");
            Auto.Add("mana", new Slider("Mana Percent", 45, 1));


            Misc = Ty.AddSubMenu("Misc");
            Misc.Add("Inter", new CheckBox("Use Aint-Rengar"));
            Misc.Add("Inter2", new CheckBox("Use Aint-Kha´Zix"));
            Misc.AddSeparator();
            Misc.Add("Gap", new CheckBox("Use GapCloser"));
            Misc.Add("Int", new CheckBox("Use Aint-Inter"));

            Draws = Ty.AddSubMenu("Draws");
            Draws.AddGroupLabel("Settings Drawings");
            Draws.AddSeparator();
            Draws.AddLabel("Drawing [Q]");
            Draws.Add("DQ", new CheckBox("Use Draw [Q]"));
            Draws.AddSeparator();
            Draws.AddLabel("Drawing [W]");
            Draws.Add("DW", new CheckBox("Use Draw [W]"));
            Draws.AddSeparator();
            Draws.AddLabel("Drawing [E]");
            Draws.Add("DE", new CheckBox("Use Draw [E]"));
            Draws.AddSeparator();
            Draws.AddLabel("Drawing [R]");
            Draws.Add("DR", new CheckBox("Use Draw [R]"));
        }
    }
}