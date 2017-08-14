using System.Linq;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Fiddle_Beta_Fixed
{
    internal class MenusSettings
    {
        public static Menu Fid, Combo, Haras, Lane, Jungle, Misc, Draws;

        internal static void Execute()
        {
            Fid = MainMenu.AddMenu("FiddleStick", "FiddleStick");

            Combo = Fid.AddSubMenu("Combo");
            Combo.Add("Qc", new CheckBox("Use [Q]"));
            Combo.Add("Wc", new CheckBox("Use [W]"));
            Combo.Add("Ec", new CheckBox("Use [E]"));
            Combo.Add("Qcc", new CheckBox("Use [Q] CC"));
            Combo.AddSeparator();
            Combo.AddLabel("Settings [R]");
            Combo.Add("Rc", new CheckBox("Use [R]", false));
            Combo.Add("En", new Slider("Min Enemys", 2, 0, 5));
        
            Haras = Fid.AddSubMenu("Harass");
            Haras.Add("He", new CheckBox("Use [E] Harass"));
            Haras.AddLabel("Mana Settings");
            Haras.Add("mana", new Slider("Mana Percent", 50, 1, 100));

            Lane = Fid.AddSubMenu("LaneClear");
            Lane.Add("Ql", new CheckBox("Use [Q]", false));
            Lane.Add("Wl", new CheckBox("Use [W]"));
            Lane.Add("El", new CheckBox("Use [E]"));
            Lane.AddSeparator();
            Lane.AddLabel("Mana Settings");
            Lane.Add("manal", new Slider("Mana Percent", 50, 1, 100));

            Jungle = Fid.AddSubMenu("JungleClear");
            Jungle.Add("Qj", new CheckBox("Use [Q]", false));
            Jungle.Add("Wj", new CheckBox("Use [W]"));
            Jungle.Add("Ej", new CheckBox("Use [E]"));

            Misc = Fid.AddSubMenu("Misc");
            Misc.Add("Gap", new CheckBox("AintGapClose"));
            Misc.Add("Int", new CheckBox("Interrupt"));

            Draws = Fid.AddSubMenu("Draws");
            Draws.Add("DQ", new CheckBox("Use Draws [Q/W]"));
            Draws.Add("DE", new CheckBox("Draws [E]"));
            Draws.Add("DR", new CheckBox("Draws [R]"));
        }
    }
}