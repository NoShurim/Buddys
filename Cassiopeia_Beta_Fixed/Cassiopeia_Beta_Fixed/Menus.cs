using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace Cassiopeia_Beta_Fixed
{
    internal class Menus
    {
        public static Menu casio, Combo, Hara, Farm, Misc, Draws, KillSteal;

        internal static void Execute()
        {
            casio = MainMenu.AddMenu("Cassiopeia", "Cassiopeia");
            //
            Combo = casio.AddSubMenu("Combo");
            Combo.Add("Qc", new CheckBox("Use [Q] Enemy"));
            Combo.Add("Wc", new CheckBox("Use [W] Enemy"));
            Combo.Add("Ec", new CheckBox("Use [E] Enemy"));
            Combo.Add("DisAA", new CheckBox("DisableAA in Combo", false));
            Combo.Add("QAA", new CheckBox("Use [Q] + AA"));
            Combo.Add("EAA", new CheckBox("Use [E] + AA"));
            Combo.AddSeparator();
            Combo.AddLabel("Settings [W]");
            Combo.Add("minWw", new Slider("Use [W] Target is Enemy > %", 1, 0, 5));
            Combo.AddSeparator();
            Combo.AddLabel("Settings [R]");
            Combo.Add("Rc", new CheckBox("Use [R] In Combo"));
            Combo.AddSeparator();
            Combo.AddLabel("Percent Enemys [R]");
            Combo.Add("Re", new Slider("Enemys Percent > %", 2, 0, 5));
            //
            Hara = casio.AddSubMenu("AutoHarass");
            Hara.Add("AutoQ", new CheckBox("Auto [Q]"));
            Hara.AddSeparator();
            Hara.AddLabel("Mana Percent");
            Hara.Add("mana", new Slider("Mana Percent > %", 65, 1));
            //
            KillSteal = casio.AddSubMenu("KillSteal");
            KillSteal.Add("KsQ", new CheckBox("KillSteal [Q]"));
            KillSteal.Add("KsW", new CheckBox("KillSteal [W]"));
            KillSteal.Add("KsE", new CheckBox("KillSteal [E]"));
            KillSteal.Add("KsR", new CheckBox("KillSteal [R]"));
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
            Farm.Add("Qq", new Slider("Percent Minion [Q] > %", 2, 0, 6));
            Farm.Add("Ww", new Slider("Percent Minion [W] > %", 3, 0, 6));
            Farm.AddSeparator();
            Farm.AddLabel("Mana Percent");
            Farm.Add("Manal", new Slider("Mana Percent > %", 50, 1));
            Farm.AddSeparator();
            Farm.AddGroupLabel("JungleClear");
            Farm.AddSeparator();
            Farm.Add("Qj", new CheckBox("Use [Q]"));
            Farm.Add("Wj", new CheckBox("Use [W]"));
            Farm.Add("Ej", new CheckBox("Use [E]"));
            Farm.AddSeparator();
            Farm.AddLabel("Mana Percent");
            Farm.Add("Manaj", new Slider("Mana Percent > %", 50, 1));
            //
            Misc = casio.AddSubMenu("Misc");
            Misc.Add("Gap", new CheckBox("GapClose"));
            Misc.Add("Int", new CheckBox("Interrupt"));
            //
            Draws = casio.AddSubMenu("Draws");
            Draws.Add("DQ", new CheckBox("Draws [Q]"));
            Draws.Add("DW", new CheckBox("Draws [W]"));
            Draws.Add("DE", new CheckBox("Draws [E]"));
            Draws.Add("DR", new CheckBox("Draws [R]"));


        }
    }
}