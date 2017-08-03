using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System;

namespace Tristana_Beta_Fixed
{
    internal class Menus
    {
        public static Menu ty, St, Combo, Hara, Lane, Jungle, Misc, Draws;
        internal static void Execute()
        {
            ty = MainMenu.AddMenu("Tristana", "Tristana");
            St = ty.AddSubMenu("Config Tristana");
            St.AddLabel("Prediction [W]");
            St.Add("pw", new Slider("Prediction [W] > {0}", 75,0,100));
            St.AddSeparator();
            St.AddLabel("Settings Stacks [W]");
            St.Add("stack", new Slider("Use [W] Stacks > {0}", 3, 0, 5));
            St.AddSeparator();
            St.AddLabel("[W] Settings");
            St.Add("Wc", new CheckBox("Use [W] Jump"));
            St.AddLabel("Settings [W] (Confing Tristana)");
            //
            Combo = ty.AddSubMenu("Combo");
            Combo.Add("Qc", new CheckBox("Use [Q] Combo"));
            Combo.Add("Ec", new CheckBox("Use [E] Combo"));
            Combo.Add("Rc", new CheckBox("Use [R] Combo"));
            //
            Hara = ty.AddSubMenu("AutoHarass");
            Hara.Add("active", new CheckBox("Enabled"));
            //
            Lane = ty.AddSubMenu("Lane");
            Lane.Add("Ql", new CheckBox("Use [Q] LaneClear"));
            Lane.Add("El", new CheckBox("Use [E] LaneClear"));
            Lane.Add("forE", new CheckBox("Use [E] For target minion"));
            Lane.AddSeparator();
            Lane.AddLabel("Mana Percent");
            Lane.Add("mana", new Slider("Mana Percent > {0}", 50, 0, 100));
            Jungle = ty.AddSubMenu("Jungle");
            Jungle.AddGroupLabel("Development");
            Misc = ty.AddSubMenu("Misc");
            Misc.Add("Gap", new CheckBox("Use [Aint-GapClose]"));
            Misc.Add("Int", new CheckBox("Use [Interrumpt]"));
            Draws = ty.AddSubMenu("Drawings");
            Draws.Add("DW", new CheckBox("[W] Draws"));
            Draws.Add("DR", new CheckBox("[R/Q/E] Draws"));
        }
    }
}