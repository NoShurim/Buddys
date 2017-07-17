using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Kayn_BETA_Fixed
{
    class Menus
    {
        public static Menu Kmenu, Combo, Lane, Jungle, Draws, Misc;

        internal static void CreateMenu()
        {
            Kmenu = MainMenu.AddMenu("Kayn BETA", "Kayn Beta");
            Combo = Kmenu.AddSubMenu("Combo");
            Combo.Add("Q", new CheckBox("Use [Q]"));
            Combo.Add("W", new CheckBox("Use [W]"));
            Combo.Add("E", new CheckBox("Use [E]"));
            Combo.Add("R", new CheckBox("Use [R]"));
            Lane = Kmenu.AddSubMenu("Lane");
            Lane.Add("Qlane", new CheckBox("Use [Q]"));
            Lane.Add("WLane", new CheckBox("Use [W]"));
            Lane.Add("qmana", new Slider("Mini Mana [Q]", 65, 1));
            Lane.AddLabel("Mana W");
            Lane.Add("wmana", new Slider("Mini Mana [W]", 65, 1));
            Jungle = Kmenu.AddSubMenu("Jungle");
            Jungle.Add("Qjungle", new CheckBox("Use [Q]"));
            Jungle.Add("Wjungle", new CheckBox("Use [W]"));
            Jungle.Add("qmana", new Slider("Mini Mana [Q]", 65, 1));
            Jungle.AddLabel("Mana W");
            Jungle.Add("wmana", new Slider("Mini Mana [W]", 65, 1));
            Misc = Kmenu.AddSubMenu("Misc");
            Misc.Add("KS", new CheckBox("Use [R] KillSteal"));
            Draws = Kmenu.AddSubMenu("Drawings");
            Draws.Add("DQ", new CheckBox("Use [Q] Draw"));
            Draws.Add("DW", new CheckBox("Use [W] Draw"));
            Draws.Add("DR", new CheckBox("Use [R] Draw"));

        }
    }
}
