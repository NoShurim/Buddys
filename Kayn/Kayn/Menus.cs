using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Kayn
{
    internal class Menus
    {
        public static Menu Kmenu, Combo, Hara, Lane, Jungle, Misc, Evade, Pre, Draws;

        public Menus()
        {
            Kmenu = MainMenu.AddMenu("Kayn", "Kayn");

            Pre = Kmenu.AddSubMenu("Prediction");
            Pre.AddLabel("Settings Kayn");
            Pre.Add("Pq", new Slider("Prediction [Q]", 65, 1, 100));
            Pre.Add("Pw", new Slider("Prediction [W]", 70, 1, 100));
            Pre.AddLabel("Settings Shodown Asasin");
            Pre.Add("Qq", new Slider("Prediction [Q]", 65, 1, 100));
            Pre.Add("Ww", new Slider("Prediction [W]", 70, 1, 100));
            Pre.AddLabel("Settings Rhaast");
            Pre.Add("r2", new Slider("Minimum of life to use the utimate", 30, 1, 100));

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
            Misc.Add("Gap", new CheckBox("GapCloser"));
            Misc.Add("AAR", new CheckBox("Reset AA+R"));
            Misc.AddLabel("Settings GapCloser");
            Misc.Add("sG", new Slider("Mini Mana Gap", 70, 1, 100));

            Draws = Kmenu.AddSubMenu("Draws");
            Draws.Add("DQ", new CheckBox("Draw [Q]"));
            Draws.Add("DW", new CheckBox("Draw [W]"));
            Draws.Add("DR", new CheckBox("Draw [R]"));

            Evade = Kmenu.AddSubMenu("Evade");
            Evade.Add("ER", new CheckBox("Enabled Evade"));
            Evade.AddLabel("Evade In Faze Beta");
        }
    }
}