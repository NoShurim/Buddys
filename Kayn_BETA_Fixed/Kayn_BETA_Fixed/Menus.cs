using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Kayn_BETA_Fixed
{
    class Menus
    {
        public static Menu Kmenu, Combo, Lane, Jungle, Draws, Misc, AutoHara;

        internal static void CreateMenu()
        {
            Kmenu = MainMenu.AddMenu("Kayn BETA", "Kayn Beta");
            Combo = Kmenu.AddSubMenu("Combo");
            Combo.Add("Q", new CheckBox("Use [Q]"));
            Combo.Add("QE", new Slider("Use [Q] Min Enemy", 2, 1, 5));
            Combo.Add("W", new CheckBox("Use [W]"));
            Combo.Add("E", new CheckBox("Use [E]", false));
            Combo.AddSeparator();
            Combo.AddLabel("Key [E]");
            Combo.Add("UE", new KeyBind("Use [E]", false, KeyBind.BindTypes.HoldActive, 'T'));
            Combo.Add("R", new CheckBox("Use [R]", false));
            Combo.Add("Rs", new Slider("Use [R] Life Enemy", 50, 1));
            Combo.AddSeparator();
            Combo.AddLabel("Predction");
            Combo.Add("Qhit", new ComboBox("HitChance [Q]", 1,"Low", "Medium", "High"));
            Combo.Add("Whit", new Slider("HitChance [W]", 80, 1));
            Combo.AddSeparator();
            Combo.AddLabel("Settings [R]");
            Combo.Add("ultR", new CheckBox("Use [R] Evade"));
            Combo.Add("MR", new Slider("My HP Use Evade [R] <=", 15));

            AutoHara = Kmenu.AddSubMenu("AutoHarass");
            AutoHara.Add("AutoW", new CheckBox("Auto [W]"));
            AutoHara.Add("Mn", new Slider("Mana AutoHarass", 65, 1));

            Lane = Kmenu.AddSubMenu("Lane");
            Lane.Add("Ql", new CheckBox("Use [Q]"));
            Lane.Add("Wl", new CheckBox("Use [W]"));
            Lane.Add("mana", new Slider("Mana [Q]/[W]", 45, 1));
            Lane.AddSeparator();
            Lane.AddLabel("Minion");
            Lane.Add("Min", new Slider("Mini Minion > {0}", 3, 1, 5));
            Lane.AddSeparator();
            Lane.AddLabel("Mode [Q]");
            Lane.Add("Qmode", new ComboBox("Use Prediction [Q]", 0, "On", "Off"));
            Lane.Add("Win", new Slider("Min Minions To Hit With W", 2, 1, 6));
            Lane.Add("WMode", new ComboBox("Use Prediction For W", 0, "On", "Off"));
            Lane.Add("WP", new Slider("Select % Hitchance", 80, 1, 100));

            Jungle = Kmenu.AddSubMenu("Jungle");
            Jungle.Add("Qj", new CheckBox("Use [Q]"));
            Jungle.Add("Wj", new CheckBox("Use [W]"));
            Lane.AddSeparator();
            Jungle.AddLabel("Mana");
            Jungle.Add("jmana", new Slider("Mana [Q]/[W]", 45, 1));
            Jungle.Add("J", new Slider("Min Monsters To Hit With W", 1, 1, 4));

            Misc = Kmenu.AddSubMenu("Misc");
            Misc.Add("KSR", new CheckBox("Use [R] Fish"));
            Misc.Add("KS", new CheckBox("KillSteal"));
            Misc.AddLabel("Flash");
            Misc.Add("FR", new CheckBox("Use [Flash + R]", false));
            Misc.Add("FW", new CheckBox("Use [Flash + W]", false));

            Draws = Kmenu.AddSubMenu("Drawings");
            Draws.Add("DQ", new CheckBox("Use [Q] Draw"));
            Draws.Add("DW", new CheckBox("Use [W] Draw"));
            Draws.Add("DE", new CheckBox("Use [E] Draw"));
            Draws.AddSeparator();
            Draws.Add("DR", new CheckBox("Use [R]/[R2] Draw"));
            Draws.Add("DF", new CheckBox("Use [Flash] Draw"));
            Draws.Add("DW2", new CheckBox("Draws Logic W"));
            Draws.Add("D2R", new CheckBox("Use Draw Int R"));

        }
    }
}
