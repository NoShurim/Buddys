using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System.Reflection;

namespace LeBlanc_Beta_Fixed
{
    class Menus
    {
        public static Menu Menu, Comb, Lane, KillStea, AntiGapcloser, Haras, Draws, Flwe, Misc;
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
            Draws = Menu.AddSubMenu("Drawings", "draw");
            Draws.Add("Q", new CheckBox("Draw Q range"));
            Draws.Add("W", new CheckBox("Draw W range", false));
            Draws.Add("WPos", new CheckBox("Draw W Position", false));
            Draws.Add("E", new CheckBox("Draw E range", false));


        }
        private static void ComboMenu()
        {
            Comb = Menu.AddSubMenu("Combo", "combo");
            Comb.Add("Q", new CheckBox("Use Q"));
            Comb.Add("W", new CheckBox("Use W"));
            Comb.Add("extW", new CheckBox("Extended W (W to gapclose)", false));
            Comb.Add("E", new CheckBox("Use E"));
            Comb.Add("R", new CheckBox("Use R"));
            Comb.Add("RQ", new CheckBox("Use R (Q)"));
            Comb.Add("RW", new CheckBox("Use R (W)", false));
            Comb.Add("RE", new CheckBox("Use R (E)"));

        }
        private static void LaneClearMenu()
        {
            Lane = Menu.AddSubMenu("Laneclear", "laneclear");
            Lane.Add("Q", new CheckBox("Use Q", false));
            Lane.Add("QMana", new Slider("Min % mana to Q", 20, 0, 100));
            Lane.Add("W", new CheckBox("Use W"));
            Lane.Add("WMana", new Slider("Min % mana to W", 20, 0, 100));
            Lane.Add("W2", new CheckBox("Auto W2"));
            Lane.Add("WMin", new Slider("Min minions to W", 4, 1, 10));


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

            HarassMenu();
        }
        private static void HarassMenu()
        {
            Haras = Menu.AddSubMenu("Harass");

            Haras.Add("Q", new CheckBox("Use Q"));
            Haras.Add("QMana", new Slider("Min mana % to Q", 40, 0, 100));
            Haras.AddSeparator();
            Haras.Add("W", new CheckBox("Use W"));
            Haras.Add("extW", new CheckBox("Extended W (W to gapclose)", false));
            Haras.Add("AutoW", new CheckBox("Auto W2 after harass"));
            Haras.Add("WMana", new Slider("Min mana % to W", 40, 0, 100));
            Haras.AddSeparator();
            Haras.Add("E", new CheckBox("Use E", false));
            Haras.Add("EMana", new Slider("Min mana % to E", 40, 0, 100));
            Haras.AddSeparator();
            Haras.Add("Auto", new KeyBind("Auto harass", false, KeyBind.BindTypes.PressToggle, 'N'));

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