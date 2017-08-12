using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System;

namespace Thresh_Beta_Fixed
{
    public static class Menus
    {
        private static Menu TH;
        public static Menu Pre, Combo, Harass, FastLane, Misc, Draws;

        private static void LoadingMenu()
        {
           MainThemeMenu();
            ComboSettings();
            Predictions();
            Farming();
            Haras();
            Missc();
            Drawing();
        }

        private static void MainThemeMenu()
        {
            TH = MainMenu.AddMenu("Thresh", "Thresh");
        }

        private static void Predictions()
        {
            Pre = TH.AddSubMenu("Prediction");
            Pre.Add("Pq", new Slider("Prediction [Q] > %", 75, 0, 100));
            Pre.Add("Pe", new Slider("Prediction [E] > %", 50, 0, 100));
        }

        private static void ComboSettings()
        {
            Combo = TH.AddSubMenu("Combo");
            Combo.AddLabel("Settings Combo");
            Combo.AddSeparator();
            Combo.Add("Qc", new CheckBox("Use [Q] In Cast"));
            Combo.Add("Wc", new CheckBox("Use [W] In Ally"));
            Combo.Add("Ec", new CheckBox("Use [E] In Cast"));
            Combo.Add("Rc", new CheckBox("Use [R] In Cast"));
            Combo.AddSeparator();
            Combo.AddLabel("Settings [W]");
            Combo.Add("Wa", new Slider("Prediction [W] Ally", 75, 0, 100));
        }
        
        private static void Haras()
        {
            Harass = TH.AddSubMenu("Harass");
            Harass.AddLabel("Settings [Q]");
            Harass.Add("Hq", new CheckBox("Use Harass [Q]"));
            Harass.AddSeparator();
            Harass.AddLabel("Settings [W]");
            Harass.Add("Hw", new CheckBox("Use Harass [W]"));

        }

        private static void Farming()
        {
            FastLane = TH.AddSubMenu("FastLane");
            FastLane.Add("Sup", new CheckBox("Mode Farme Support", false));
        }

        private static void Missc()
        {
            Misc = TH.AddSubMenu("Misc");
            Misc.Add("Int", new CheckBox("Interrupt"));
            Misc.Add("Gap", new CheckBox("GapClose"));
        }

        private static void Drawing()
        {
            Draws = TH.AddSubMenu("Drawings");
            Draws.Add("DQ", new CheckBox("Use [Q] Drawings"));
            Draws.Add("DW", new CheckBox("Use [W] Drawings", false));
            Draws.Add("DE", new CheckBox("Use [E] Drawings", false));
            Draws.Add("DR", new CheckBox("Use [R] Drawings", false));
        }

        public static float Pq()
        {
            return Pre["Pq"].Cast<Slider>().CurrentValue;
        }

        public static float Pe()
        {
            return Pre["Pe"].Cast<Slider>().CurrentValue;
        }

        public static bool Qc()
        {
            return Combo["Qc"].Cast<CheckBox>().CurrentValue;
        }

        public static bool Wc()
        {
            return Combo["Wc"].Cast<CheckBox>().CurrentValue;
        }

        public static bool Ec()
        {
            return Combo["Ec"].Cast<CheckBox>().CurrentValue;
        }

        public static bool Rc()
        {
            return Combo["Rc"].Cast<CheckBox>().CurrentValue;
        }

        public static float Wa()
        {
            return Combo["Wa"].Cast<Slider>().CurrentValue;
        }

        public static bool Hq()
        {
            return Harass["Hq"].Cast<CheckBox>().CurrentValue;
        }

        public static bool Hw()
        {
            return Harass["Hw"].Cast<CheckBox>().CurrentValue;
        }

        public static bool Sup()
        {
            return FastLane["Sup"].Cast<CheckBox>().CurrentValue;
        }

        public static bool Int()
        {
            return Misc["Int"].Cast<CheckBox>().CurrentValue;
        }

        public static bool Gap()
        {
            return Misc["Gap"].Cast<CheckBox>().CurrentValue;
        }

        public static bool DQ()
        {
            return Draws["DQ"].Cast<CheckBox>().CurrentValue;
        }

        public static bool DW()
        {
            return Draws["DW"].Cast<CheckBox>().CurrentValue;
        }

        public static bool DE()
        {
            return Draws["DE"].Cast<CheckBox>().CurrentValue;
        }

        public static bool DR()
        {
            return Draws["DR"].Cast<CheckBox>().CurrentValue;
        }

        internal static void Execute()
        {
          
        }
    }
}