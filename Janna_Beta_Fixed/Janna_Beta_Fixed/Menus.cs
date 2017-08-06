using System.Linq;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Janna_Beta_Fixed
{
    internal class Menus
    {
        private static Menu Jana;
        public static Menu Combo, Harass, Healing, Drawing, Misc;
        private static string[] PredictionSliderValues = { "Low", "Medium", "High" };
        public static Slider SkinSelect;

        internal static void fodeu()
        {
            Jana = MainMenu.AddMenu("Janna Yuuki", "Janna Yuuki");

            Combo = Jana.AddSubMenu("Combo");
            Combo.AddGroupLabel("Combo");
            Combo.AddSeparator();
            Combo.Add("useQCombo", new CheckBox("Use > Q"));
            Combo.Add("useWCombo", new CheckBox("Use > W"));
            Combo.Add("useECombo", new CheckBox("Use > E"));
            Combo.Add("minMcombo", new Slider("Mana > %", 20));

            Harass = Jana.AddSubMenu("Harass");
            Harass.AddGroupLabel("Harass");
            Harass.AddSeparator();
            Harass.Add("useQHarass", new CheckBox("Use > Q"));
            Harass.Add("useEHarass", new CheckBox("Use > W"));
            Harass.Add("minMharass", new Slider("Mana % for Harras", 20));
            Harass.AddSeparator();
            var sliderValue = Harass.Add("predNeeded", new Slider("Prediction Hitchange: ", 0, 0, 2));
            sliderValue.OnValueChange +=
                delegate
                {
                    sliderValue.DisplayName = "Prediction Hitchange: " + PredictionSliderValues[sliderValue.CurrentValue];
                };
            sliderValue.DisplayName = "Prediction Hitchange: " + PredictionSliderValues[sliderValue.CurrentValue];

            Healing = Jana.AddSubMenu("Yuuki Moderator");
            Healing.AddGroupLabel("E Settings");
            Healing.AddSeparator();
            Healing.Add("useE", new CheckBox("Auto E"));
            Healing.Add("dontEF", new CheckBox("Dont E in Fountain"));
            Healing.AddSeparator();

            foreach (var hero in EntityManager.Heroes.Allies.Where(x => !x.IsMe))
            {
                Healing.AddSeparator();
                Healing.Add("w" + hero.ChampionName, new CheckBox("Heal " + hero.ChampionName));
                Healing.AddSeparator();
                Healing.Add("wpct" + hero.ChampionName, new Slider("Health % " + hero.ChampionName, 45));
            }
            Healing.AddSeparator();
            Healing.AddGroupLabel("R Settings");
            Healing.AddSeparator();
            Healing.Add("useR", new CheckBox("Use R"));
            Healing.Add("useRslider", new Slider("HP % to R", 20));

            // Misc Menu
            Misc = Jana.AddSubMenu("Misc");
            Misc.AddGroupLabel("Misc");
            Misc.AddSeparator();
            Misc.Add("useQGapCloser", new CheckBox("Q on GapCloser"));
            Misc.Add("useWGapCloser", new CheckBox("W on GapCloser"));
            Misc.Add("qInterrupt", new CheckBox("Use Q to Interrupt"));
            Misc.Add("AttackMinions", new CheckBox("Attack Minions"));

            // Drawing Menu
            Drawing = Jana.AddSubMenu("Drawing");
            Drawing.AddGroupLabel("Drawing");
            Drawing.AddSeparator();
            Drawing.Add("drawQ", new CheckBox("Draw Q"));
            Drawing.Add("drawE", new CheckBox("Draw W"));
            Drawing.Add("drawW", new CheckBox("Draw E"));
            Drawing.Add("drawR", new CheckBox("Draw R"));
            Drawing.AddSeparator();
            Drawing.Add("drawH", new CheckBox("Draw H on Healing Needed Heroes"));
        }
    }
}