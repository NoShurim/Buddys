using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System.Reflection;
using System;

namespace LeBlanc_Beta_Fixed
{
    class Menus
    {
        public static Menu Menu, Comb, Lane, KillStea, AntiGapcloser, Draws, Flwe;
        public static void StartMenu()
        {
            Menu = MainMenu.AddMenu("LeBlanc", "LeBlanc");


            DrawingsMenu();
            ComboMenu();
            LaneClearMenu();
            KillstealMenu();
            AntiGapMenu();
            Flee();
        }

        private static void DrawingsMenu()
        {
            Draws = Menu.AddSubMenu("Drawings");
            Draws.Add("Q", new CheckBox("Draw Q range"));
            Draws.Add("W", new CheckBox("Draw W range", false));
            Draws.Add("E", new CheckBox("Draw E range", false));


        }
        private static void ComboMenu()
        {
            Comb = Menu.AddSubMenu("Combo");
            Comb.Add("Q", new CheckBox("Use Q"));
            Comb.Add("W", new CheckBox("Use W"));
            Comb.Add("E", new CheckBox("Use E"));
            Comb.Add("R", new CheckBox("Use R"));
            Comb.AddSeparator();
            Comb.AddLabel("Settings [R]");
            Comb.Add("RQ", new CheckBox("Use R (Q)"));
            Comb.Add("RW", new CheckBox("Use R (W)", false));
            Comb.Add("RE", new CheckBox("Use R (E)"));

        }
        private static void LaneClearMenu()
        {
            Lane = Menu.AddSubMenu("Laneclear");
            Lane.Add("Q", new CheckBox("Use Q"));
            Lane.Add("W", new CheckBox("Use W"));
            Lane.AddSeparator();
            Lane.AddLabel("Percent Minions");
            Lane.Add("WMin", new Slider("Min minions to W", 3, 1, 10));
            Lane.AddSeparator();
            Lane.AddLabel("Mana Percent");
            Lane.Add("Mana", new Slider("Mana > %", 40, 0, 100));


        }
        private static void KillstealMenu()
        {
            KillStea = Menu.AddSubMenu("Killsteal");
            KillStea.Add("Q", new CheckBox("Use Q KS"));
            KillStea.Add("W", new CheckBox("Use W KS"));
            KillStea.Add("extW", new CheckBox("Use extended W (or R) to KS (W + Q or E)"));
            KillStea.Add("wr", new CheckBox("Use W+R + Q/E to KS"));
            KillStea.Add("E", new CheckBox("Use E KS"));
            KillStea.Add("R", new CheckBox("Use R KS"));

        }
        private static void AntiGapMenu()
        {
            AntiGapcloser = Menu.AddSubMenu("Gapcloser");
            AntiGapcloser.Add("E", new CheckBox("E anti-gapclose"));
            AntiGapcloser.Add("RE", new CheckBox("R (E) anti-gapclose", false));

        }
        private static void Flee()
        {
            Flwe = Menu.AddSubMenu("Flee");

            Flwe.Add("E", new CheckBox("Use E"));
            Flwe.Add("W", new CheckBox("Use W to cursor pos"));
            Flwe.Add("R", new CheckBox("Use R to cursor pos", false));
        }
    }
}