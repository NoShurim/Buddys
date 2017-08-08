using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System.Linq;
using EloBuddy.SDK;

namespace Karthus_Beta_Fixed
{
    public static class SettingsMenus
    {
        private const string MenuName = "Karthus";

        private static readonly Menu Menu;

        static SettingsMenus()
        {
            Menu = MainMenu.AddMenu(MenuName, MenuName.ToLower());
            Modes.Initialize();
        }

        public static void Initialize()
        {
        }

        public static class Modes
        {
            private static readonly Menu Menu;

            static Modes()
            {
                Menu = SettingsMenus.Menu.AddSubMenu("Combo");
                Combo.Initialize();

                Menu = SettingsMenus.Menu.AddSubMenu("Harass");
                Harass.Initialize();

                Menu = SettingsMenus.Menu.AddSubMenu("LaneClear");
                LaneClear.Initialize();

                Menu = SettingsMenus.Menu.AddSubMenu("Drawing");
                DrawingMenu.Initialize();

                Menu = SettingsMenus.Menu.AddSubMenu("GapClose");
                GapCloseMenu.Initialize();

                Menu = SettingsMenus.Menu.AddSubMenu("Prediction");
                PredictionMenu.Initialize();

                Menu = SettingsMenus.Menu.AddSubMenu("Misc");
                SaveMeMenu.Initialize();

            }

            public static void Initialize()
            {
            }

            public static class Combo
            {
                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useW;
                private static readonly CheckBox _useE;
                private static readonly CheckBox _useAA;
                private static readonly CheckBox _useAC;
                private static readonly CheckBox _useUltKS;
                private static readonly CheckBox _ultSecure;
                private static readonly CheckBox _saveE;
                private static readonly CheckBox _useIgnite;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }
                public static bool UseW
                {
                    get { return _useW.CurrentValue; }
                }
                public static bool UseE
                {
                    get { return _useE.CurrentValue; }
                }
                public static bool useAA
                {
                    get { return _useAA.CurrentValue; }
                }
                public static bool useAC
                {
                    get { return _useAC.CurrentValue; }
                }
                public static bool useUltKS
                {
                    get { return _useUltKS.CurrentValue; }
                }
                public static bool ultSecure
                {
                    get { return _ultSecure.CurrentValue; }
                }
                public static bool saveE
                {
                    get { return _saveE.CurrentValue; }
                }
                public static bool useIgnite
                {
                    get { return _useIgnite.CurrentValue; }
                }

                static Combo()
                {
                    // Initialize the menu values
                    Menu.AddGroupLabel("Combo Options");
                    _useQ = Menu.Add("comboUseQ", new CheckBox("Use Q"));
                    _useW = Menu.Add("comboUseW", new CheckBox("Use W"));
                    _useE = Menu.Add("comboUseE", new CheckBox("Use E"));
                    Menu.AddSeparator();

                    Menu.AddGroupLabel("AA");
                    _useAA = Menu.Add("useAA", new CheckBox("Use AA on Combo", false));

                    Menu.AddSeparator();
                    Menu.AddGroupLabel("AutoCombo");
                    _useAC = Menu.Add("useAC", new CheckBox("Death AutoCombo", true));

                    Menu.AddSeparator();
                    Menu.AddGroupLabel("Misc");
                    //_autoUltOnDeath = Menu.Add("autoUltDeath", new CheckBox("Use Ultimate automaticaly", true));
                    _useIgnite = Menu.Add("useIgnite", new CheckBox("Use Ignite If Killable", true));
                    _useUltKS = Menu.Add("useUltKS", new CheckBox("Use Ultimate KS", true));
                    _ultSecure = Menu.Add("ultSecure", new CheckBox("Ult - Secure mode only! (Ult only on passive or if don't have enemy on minimum security range[2000])", true));
                    _saveE = Menu.Add("ESave", new CheckBox("Auto switch E to save MP (Turn off if you have problems with E use.)", true));
                }

                public static void Initialize()
                {
                }
            }

            public static class Harass
            {
                private static readonly CheckBox _useQlh;
                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useW;
                private static readonly CheckBox _useE;
                private static readonly Slider _harassManaQ;
                private static readonly Slider _harassManaW;
                private static readonly Slider _harassManaE;


                public static bool UseQlh
               {
                    get { return _useQlh.CurrentValue; }
                }
                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }
                public static bool UseW
                {
                    get { return _useW.CurrentValue; }
                }
                public static bool UseE
                {
                    get { return _useE.CurrentValue; }
                }
                public static int QMana
                {
                    get { return _harassManaQ.CurrentValue; }
                }
                public static int WMana
                {
                    get { return _harassManaW.CurrentValue; }
                }
                public static int EMana
                {
                    get { return _harassManaE.CurrentValue; }
                }

                static Harass()
                {
                    // Here is another option on how to use the menu, but I prefer the
                    // way that I used in the combo class
                    Menu.AddGroupLabel("Harass Options");
                    _useQ = Menu.Add("harassUseQ", new CheckBox("Use Q"));
                    _useW = Menu.Add("harassUseW", new CheckBox("Use W", false));
                    _useE = Menu.Add("harassUseE", new CheckBox("Use E"));
                    Menu.AddSeparator();

                    Menu.AddGroupLabel("Farm Options");
                    Menu.AddLabel("Q will prioritize the enemy champion.");
                    _useQlh = Menu.Add("harassUseQLastHit", new CheckBox("Use Q to Lasthit minions on Harass"));
                    Menu.AddSeparator();

                    Menu.AddGroupLabel("Mana");
                    // Adding a slider, we have a little more options with them, using {0} {1} and {2}
                    // in the display name will replace it with 0=current 1=min and 2=max value
                    _harassManaQ = Menu.Add("harassManaQ", new Slider("Q Maximum mana usage in percent ({0}%)", 60));
                    _harassManaW = Menu.Add("harassManaW", new Slider("W Maximum mana usage in percent ({0}%)", 70));
                    _harassManaE = Menu.Add("harassManaE", new Slider("E Maximum mana usage in percent ({0}%)", 70));
                }

                public static void Initialize()
                {
                }
            }

            public static class LaneClear
            {
                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useQOnlyK;
                private static readonly CheckBox _useE;
                private static readonly Slider _laneclearManaQ;
                private static readonly Slider _laneclearManaE;
                private static readonly Slider _laneclearEMinMinions;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }
                public static bool UseQOnlyK
                {
                    get { return _useQOnlyK.CurrentValue; }
                }
                public static bool UseE
                {
                    get { return _useE.CurrentValue; }
                }
                public static int QMana
                {
                    get { return _laneclearManaQ.CurrentValue; }
                }
                public static int EMana
                {
                    get { return _laneclearManaE.CurrentValue; }
                }
                public static int EMinMinions
                {
                    get { return _laneclearEMinMinions.CurrentValue; }
                }

                static LaneClear()
                {
                    // Here is another option on how to use the menu, but I prefer the
                    // way that I used in the combo class
                    Menu.AddGroupLabel("LaneClear Options");
                    _useQ = Menu.Add("laneclearUseQ", new CheckBox("Use Q"));
                    _useQOnlyK = Menu.Add("laneclearUseQOnlyK", new CheckBox("Try use Q only LastHit"));
                    _useE = Menu.Add("laneclearUseE", new CheckBox("Use E"));
                    Menu.AddSeparator();

                    Menu.AddGroupLabel("Mana");
                    // Adding a slider, we have a little more options with them, using {0} {1} and {2}
                    // in the display name will replace it with 0=current 1=min and 2=max value
                    _laneclearManaQ = Menu.Add("laneclearManaQ", new Slider("Q Maximum mana usage in percent ({0}%)", 60));
                    _laneclearManaE = Menu.Add("laneclearManaE", new Slider("E Maximum mana usage in percent ({0}%)", 60));
                    Menu.AddSeparator();

                    Menu.AddGroupLabel("Minions");
                    _laneclearEMinMinions = Menu.Add("EMinMinions", new Slider("Minimum count of minions ({0}) to use E", 3, 1, 10));
                }

                public static void Initialize()
                {
                }
            }

            public static class DrawingMenu
            {
                private static readonly CheckBox _DrawQ;
                private static readonly CheckBox _DrawW;
                private static readonly CheckBox _DrawE;
                private static readonly CheckBox _DrawMyRange;
                private static readonly CheckBox _DrawEnemyRange;
                private static readonly CheckBox _DrawRalert;
                private static readonly CheckBox _DrawLH;
                private static readonly CheckBox _ChampInfo;
                private static readonly Slider _posX;
                private static readonly Slider _posY;

                public static bool DrawQ
                {
                    get { return _DrawQ.CurrentValue; }
                }
                public static bool DrawW
                {
                    get { return _DrawW.CurrentValue; }
                }
                public static bool DrawE
                {
                    get { return _DrawE.CurrentValue; }
                }
                public static bool DrawMyRange
                {
                    get { return _DrawMyRange.CurrentValue; }
                }
                public static bool DrawEnemyRange
                {
                    get { return _DrawEnemyRange.CurrentValue; }
                }
                public static bool DrawRalert
                {
                    get { return _DrawRalert.CurrentValue; }
                }
                public static bool DrawLH
                {
                    get { return _DrawLH.CurrentValue; }
                }
                public static bool ChampInfo
                {
                    get { return _ChampInfo.CurrentValue; }
                }
                public static float PosX
                {
                    get { return _posX.CurrentValue; }
                }
                public static float PosY
                {
                    get { return _posY.CurrentValue; }
                }

                static DrawingMenu()
                {
                    // Here is another option on how to use the menu, but I prefer the
                    // way that I used in the combo class
                    Menu.AddGroupLabel("Drawing Options");
                    _DrawQ = Menu.Add("DrawQ", new CheckBox("Draw Q Range"));
                    _DrawW = Menu.Add("DrawW", new CheckBox("Draw W Range"));
                    _DrawE = Menu.Add("DrawE", new CheckBox("Draw E Range", false));
                    Menu.AddSeparator();

                    Menu.AddGroupLabel("Ultimate R Notification");
                    _DrawRalert = Menu.Add("DrawRalert", new CheckBox("Draw R Notification when Ult can kill a Enemy"));
                    Menu.AddSeparator();

                    Menu.AddGroupLabel("Misc");
                    _DrawLH = Menu.Add("drawLastHit", new CheckBox("Draw LastHit Circle"));
                    _DrawMyRange = Menu.Add("DrawMyRange", new CheckBox("Draw My Range"));
                    _DrawEnemyRange = Menu.Add("DrawEnemyRange", new CheckBox("Draw Enemy Range"));
                    _ChampInfo = Menu.Add("ChampInfo", new CheckBox("Show Champion Info."));
                    _posX = Menu.Add("posX", new Slider("Champion Info - Pos X",10,0,100));
                    _posY = Menu.Add("posY", new Slider("Show Champion - Pos Y",10,0,100));


                }

                public static void Initialize()
                {
                }
            }

            public static class GapCloseMenu
            {
                private static readonly CheckBox _UseWonGap;

                public static bool UseWonGap
                {
                    get { return _UseWonGap.CurrentValue; }
                }

                static GapCloseMenu()
                {
                    // Here is another option on how to use the menu, but I prefer the
                    // way that I used in the combo class
                    Menu.AddGroupLabel("GapClose Options");
                    _UseWonGap = Menu.Add("UseWonGap", new CheckBox("Use W on GapClose"));
                }

                public static void Initialize()
                {
                }
            }

            public static class PredictionMenu
            {
                private static readonly Slider _QPrediction;
                private static readonly Slider _WPrediction;

                public static int QPrediction
                {
                    get { return _QPrediction.CurrentValue; }
                }
                public static int WPrediction
                {
                    get { return _WPrediction.CurrentValue; }
                }

                static PredictionMenu()
                {
                    // Here is another option on how to use the menu, but I prefer the
                    // way that I used in the combo class
                    Menu.AddGroupLabel("Prediction Options");
                    Menu.AddSeparator();
                    Menu.AddGroupLabel("Q HitChante :");
                    _QPrediction = Menu.Add("QPrediction", new Slider("Q HitChance : ({0})", 2, 0, 2));
                    var qMode = new[] { "Low (Fast Casting)", "Medium", "High (Slow Casting)" };
                    _QPrediction.DisplayName = qMode[_QPrediction.CurrentValue];

                    _QPrediction.OnValueChange +=
                        delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
                        {
                            sender.DisplayName = qMode[changeArgs.NewValue];
                        };


                    Menu.AddSeparator();
                    Menu.AddGroupLabel("W HitChante :");
                    _WPrediction = Menu.Add("WPrediction", new Slider("W HitChance : ({0})", 2, 0, 2));
                    var wMode = new[] { "Low (Fast Casting)", "Medium", "High (Slow Casting)" };
                    _WPrediction.DisplayName = wMode[_WPrediction.CurrentValue];

                    _WPrediction.OnValueChange +=
                        delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
                        {
                            sender.DisplayName = wMode[changeArgs.NewValue];
                        };
                }

                public static void Initialize()
                {
                }
            }

            public static class SaveMeMenu
            {
                private static readonly CheckBox _useSeraphsDmg;
                private static readonly CheckBox _useSeraphsCC;
                private static readonly CheckBox _useZhonyasDmg;
                private static readonly CheckBox _useZhonyasCC;
                private static readonly CheckBox _useSpellshields;

                public static bool useSeraphsDmg
                {
                    get { return _useSeraphsDmg.CurrentValue; }
                }
                public static bool useSeraphsCC
                {
                    get { return _useSeraphsCC.CurrentValue; }

                }
                public static bool useZhonyasDmg
                {
                    get { return _useZhonyasDmg.CurrentValue; }
                }
                public static bool useZhonyasCC
                {
                    get { return _useZhonyasCC.CurrentValue; }
                }
                public static bool useSpellshields
                {
                    get { return _useSpellshields.CurrentValue; }
                }

                public static readonly CheckBox[] _skills = new CheckBox[EntityManager.Heroes.Enemies.Count() * 4];

                static SaveMeMenu()
                {
                    // Here is another option on how to use the menu, but I prefer the
                    // way that I used in the combo class
                    Menu.AddGroupLabel("SaveMe Options");
                    Menu.AddSeparator();
                    _useSeraphsDmg = Menu.Add("useSeraphsDmg", new CheckBox("Use Seraphs on incoming damage"));
                    _useSeraphsCC = Menu.Add("useSeraphsCC", new CheckBox("Use Seraphs on incoming dangerous spells"));
                    _useZhonyasDmg = Menu.Add("useZhonyasDmg", new CheckBox("Use Zhonyas on incoming damage"));
                    _useZhonyasCC = Menu.Add("useZhonyasCC", new CheckBox("Use Zhonyas on incoming dangerous spells"));
                    Menu.AddSeparator();
                    _useSpellshields = Menu.Add("useSpellshields", new CheckBox("Use protections on incoming dangerous spells :"));
                    Menu.AddSeparator();

                    var enemies = EntityManager.Heroes.Enemies;
                    for (int j = 0; j < enemies.Count(); j++)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            string skill = "";
                            switch (i)
                            {
                                case 0:
                                    skill = "Q";
                                    break;
                                case 1:
                                    skill = "W";
                                    break;
                                case 2:
                                    skill = "E";
                                    break;
                                case 3:
                                    skill = "R";
                                    break;
                            }
                            _skills[j * 4 + i] = Menu.Add("champ" + j + "" + i, new CheckBox("Block " + enemies[j].ChampionName + " " + skill, false));
                        }
                        Menu.AddSeparator();
                    }
                }

                public static void Initialize()
                {
                }
            }

        }
    }
}