using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Kayn
{
    internal class Menus
    {
        public static Menu Kmenu, Combo, Hara, Lane, Jungle, Misc;

        public Menus()
        {
            Kmenu = MainMenu.AddMenu("Kayn", "Kayn");
            Combo = Kmenu.AddSubMenu("Combo");
            Combo.Add("Qk", new CheckBox("[Use Q]"));
            Combo.Add("Wk", new CheckBox("[Use W]"));
            Combo.Add("Ek", new CheckBox("[Use E]", false));
            Combo.AddLabel("The Ability E Still in test is not working yet");
            Combo.Add("Rk", new CheckBox("[Use R]"));

            Hara = Kmenu.AddSubMenu("Harass");
            Hara.AddLabel("Harass Automatic");
            Hara.Add("Wh", new CheckBox("[Use W]"));
            Hara.Add("mW", new Slider("Mana W", 50, 1, 100));

            Lane = Kmenu.AddSubMenu("Lane");
            Lane.Add("Q1", new CheckBox("[Use Q]"));
            Lane.Add("W1", new CheckBox("[Use W]"));
            Lane.AddLabel("Setting Lane");
            Lane.Add("Mi", new Slider("Mana", 50, 1, 100));

            Jungle = Kmenu.AddSubMenu("Jungle");
            Jungle.Add("Qj", new CheckBox("[Use Q]"));
            Jungle.Add("Wj", new CheckBox("[Use W]"));
            Jungle.AddLabel("Setting Lane");
            Jungle.Add("Ma", new Slider("Mana", 50, 1, 100));

            Misc = Kmenu.AddSubMenu("Misc");
            Misc.Add("KS1", new CheckBox("Use Q KillSteal"));
            Misc.Add("KS2", new CheckBox("Use W KillSteal"));
            Misc.Add("KS3", new CheckBox("Use R KillSteal"));
            Misc.Add("End", new CheckBox("Always use R to finish"));
            Misc.Add("Inter", new CheckBox("Interrupter"));
        }
    }
}