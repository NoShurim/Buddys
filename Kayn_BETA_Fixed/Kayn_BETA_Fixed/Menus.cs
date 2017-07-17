using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Kayn_BETA_Fixed
{
    class Menus
    {
        public static Menu Kmenu, Combo, Lane, Jungle, Misc;

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
            Jungle = Kmenu.AddSubMenu("Jungle");
            Jungle.Add("Qjungle", new CheckBox("Use [Q]"));
            Jungle.Add("Wjungle", new CheckBox("Use [W]"));
            Misc = Kmenu.AddSubMenu("Misc");
            Misc.Add("KS", new CheckBox("Use [R] KillSteal"));

        }
    }
}
