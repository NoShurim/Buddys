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
            Combo.AddLabel("Predction");
            Combo.Add("Qhit", new Slider("HitChance [Q]", 65, 1));
            Combo.Add("Whit", new Slider("HitChance [W]", 80, 1));
            Lane = Kmenu.AddSubMenu("Lane");
            Lane.Add("Qlane", new CheckBox("Use [Q]"));
            Lane.Add("WLane", new CheckBox("Use [W]"));
            Lane.Add("qmana", new Slider("Mini Mana [Q]", 65, 1));
            Lane.AddLabel("Mana W");
            Lane.Add("wmana", new Slider("Mini Mana [W]", 65, 1));
            Lane.AddLabel("Mode [Q]");
            Lane.Add("Qmode", new ComboBox("Use Prediction [Q]", 0, "On", "Off"));
            Lane.Add("Win", new Slider("Min Minions To Hit With W", 2, 1, 6));
            Lane.Add("WMode", new ComboBox("Use Prediction For W", 0, "On", "Off"));
            Lane.Add("WP", new Slider("Select % Hitchance", 80, 1, 100));
            Jungle = Kmenu.AddSubMenu("Jungle");
            Jungle.Add("Qjungle", new CheckBox("Use [Q]"));
            Jungle.Add("Wjungle", new CheckBox("Use [W]"));
            Jungle.Add("qmana", new Slider("Mini Mana [Q]", 65, 1));
            Jungle.AddLabel("Mana W");
            Jungle.Add("wmana", new Slider("Mini Mana [W]", 65, 1));
            Jungle.Add("J", new Slider("Min Monsters To Hit With W", 1, 1, 4));
            Misc = Kmenu.AddSubMenu("Misc");
            Misc.Add("KSR", new CheckBox("Use [R] Fish"));
            Misc.Add("KS", new CheckBox("KillSteal"));
            Draws = Kmenu.AddSubMenu("Drawings");
            Draws.Add("DQ", new CheckBox("Use [Q] Draw"));
            Draws.Add("DW", new CheckBox("Use [W] Draw"));
            Draws.Add("DE", new CheckBox("Use [E] Draw"));
            Draws.Add("DR", new CheckBox("Use [R]/[R2] Draw"));

        }
    }
}
