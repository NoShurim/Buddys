using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace Cassiopeia_Beta_Fixed
{
    internal class Menus
    {
        public static Menu casio, Combo, Hara, Harass, Farm, Misc, Draws, KillSteal;

        internal static void Execute()
        {
            casio = MainMenu.AddMenu("Cassiopeia", "Cassiopeia");
            //
            Combo = casio.AddSubMenu("Combo");
            Combo.Add("Qc", new CheckBox("Use [Q] Enemy"));
            Combo.Add("Wc", new CheckBox("Use [W] Enemy"));
            Combo.Add("Ec", new CheckBox("Use [E] Enemy"));
            Combo.Add("Eca", new CheckBox("Use [E] even if not poisoned", false));
            Combo.Add("DisAA", new CheckBox("Smart disableAA in Combo", true));
            Combo.Add("QAA", new CheckBox("Use [Q] + AA"));
            Combo.Add("EAA", new CheckBox("Use [E] + AA"));
            Combo.AddSeparator();
            Combo.AddLabel("Settings [W]");
            Combo.Add("minWw", new Slider("Min enemies to use [W]", 1, 1, 5));
            Combo.AddSeparator();
            Combo.AddLabel("Settings [R]");
            Combo.Add("Rc", new CheckBox("Use [R] In Combo"));
            Combo.AddSeparator();
            Combo.AddLabel("Percent Enemys [R]");
            Combo.Add("Re", new Slider("Min enemies to ult", 2, 1, 5));
            Combo.Add("Rb", new CheckBox("Only ult if main target is stunnable"));
            //
            Harass = casio.AddSubMenu("Harass");
            Harass.Add("Qh", new CheckBox("[Q]"));
            Harass.Add("Eh", new CheckBox("[E]"));
            Harass.AddSeparator();
            Harass.AddLabel("Mana Percent");
            Harass.Add("mana", new Slider("Mana Percent > %", 30, 0));
            //
            Hara = casio.AddSubMenu("AutoHarass");
            Hara.Add("AutoQ", new CheckBox("Auto [Q]"));
            Hara.AddSeparator();
            Hara.AddLabel("Mana Percent");
            Hara.Add("mana", new Slider("Mana Percent > %", 65, 0));
            //
            KillSteal = casio.AddSubMenu("KillSteal");
            KillSteal.Add("KsQ", new CheckBox("KillSteal [Q]"));
            KillSteal.Add("KsW", new CheckBox("KillSteal [W]", false));
            KillSteal.Add("KsE", new CheckBox("KillSteal [E]"));
            KillSteal.Add("KsR", new CheckBox("KillSteal [R]", false));
            //
            Farm = casio.AddSubMenu("Farming");
            Farm.AddGroupLabel("LaneClear");
            Farm.AddSeparator();
            Farm.Add("Qf", new CheckBox("Use [Q]"));
            Farm.Add("Wf", new CheckBox("Use [W]"));
            Farm.Add("Ef", new CheckBox("Use [E]"));
            Farm.Add("Buff", new CheckBox("Position [Buff]", false));
            Farm.Add("Elast", new CheckBox("LastHit [E]"));
            Farm.AddSeparator();
            Farm.AddLabel("Settings [Q/W]");
            Farm.Add("Qq", new Slider("Percent Minion [Q] > %", 2, 1, 6));
            Farm.Add("Ww", new Slider("Percent Minion [W] > %", 3, 1, 6));
            Farm.AddSeparator();
            Farm.AddLabel("Mana Percent");
            Farm.Add("Manal", new Slider("Mana Percent > %", 25, 0));
            Farm.AddSeparator();
            Farm.AddGroupLabel("JungleClear");
            Farm.AddSeparator();
            Farm.Add("Qj", new CheckBox("Use [Q]"));
            Farm.Add("Wj", new CheckBox("Use [W]"));
            Farm.Add("Ej", new CheckBox("Use [E]"));
            Farm.Add("AAw", new CheckBox("AA Weaving"));
            Farm.AddSeparator();
            Farm.AddLabel("Mana Percent");
            Farm.Add("Manaj", new Slider("Mana Percent > %", 25, 0));
            //
            Misc = casio.AddSubMenu("Misc");
            Misc.Add("Gap", new CheckBox("GapClose"));
            Misc.Add("Int", new CheckBox("Interrupt"));
            Misc.Add("AAoff", new CheckBox("Disable AA if can E (For URF)", false));
            //
            Draws = casio.AddSubMenu("Draws");
            Draws.Add("DQ", new CheckBox("Draws [Q]"));
            Draws.Add("DW", new CheckBox("Draws [W]"));
            Draws.Add("DE", new CheckBox("Draws [E]"));
            Draws.Add("DR", new CheckBox("Draws [R]"));


        }
    }
}