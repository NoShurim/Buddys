using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

namespace Ashe_Beta_Fixed
{
    public static class MenusSetting
    {
        private const string MenuName = "Ashe";
        private static readonly Menu MyHeroAshe;

        static MenusSetting()
        {
            MyHeroAshe = MainMenu.AddMenu(MenuName, MenuName.ToLower());
            Modes.Initialize();
        }

        public static void Execute()
        {

        }

        public static class Misc
        {

            private static readonly Menu Menu;
            public static readonly CheckBox _drawW;
            private static readonly CheckBox _ksW;
            private static readonly CheckBox _useAutoW;
            private static readonly CheckBox _useAutoE;
            private static readonly CheckBox _useHeal;
            private static readonly CheckBox _useQSS;
            private static readonly CheckBox _autoBuyStartingItems;
            private static readonly CheckBox _autolevelskills;
            private static readonly Slider _skinId;
            public static readonly CheckBox _useSkinHack;
            private static readonly CheckBox[] _useHealOn = { new CheckBox("", false), new CheckBox("", false), new CheckBox("", false), new CheckBox("", false), new CheckBox("", false) };

            public static bool useHealOnI(int i)
            {
                return _useHealOn[i].CurrentValue;
            }
            public static bool ksW
            {
                get { return _ksW.CurrentValue; }
            }
            public static bool useAutoW
            {
                get { return _useAutoW.CurrentValue; }
            }
            public static bool useE
            {
                get { return _useAutoE.CurrentValue; }
            }
            public static int autoWMana
            {
                get { return Menu["autoWMana"].Cast<Slider>().CurrentValue; }
            }
            public static bool useHeal
            {
                get { return _useHeal.CurrentValue; }
            }
            public static bool useQSS
            {
                get { return _useQSS.CurrentValue; }
            }
            public static bool autoBuyStartingItems
            {
                get { return _autoBuyStartingItems.CurrentValue; }
            }
            public static bool autolevelskills
            {
                get { return _autolevelskills.CurrentValue; }
            }
            public static int skinId
            {
                get { return _skinId.CurrentValue; }
            }
            public static bool UseSkinHack
            {
                get { return _useSkinHack.CurrentValue; }
            }


           static Misc()
            {
                Menu = MenusSetting.MyHeroAshe.AddSubMenu("Misc");
                _drawW = Menu.Add("drawW", new CheckBox("Draw [W]"));
                Menu.AddSeparator();
                _ksW = Menu.Add("ksW", new CheckBox("Smart KS with [W]"));
                _useAutoW = Menu.Add("useAutoW", new CheckBox("use W automatically if"));
                Menu.Add("autoWMana", new Slider("Mana Percent > %", 80));
                _useAutoE = Menu.Add("useAutoE", new CheckBox("use E"));
                Menu.AddSeparator();
                _useHeal = Menu.Add("useHeal", new CheckBox("Use Heal Smart"));
                _useQSS = Menu.Add("useQSS", new CheckBox("Use QSS"));
                Menu.AddSeparator();
                for (int i = 0; i < EntityManager.Heroes.Allies.Count; i++)
                {
                    _useHealOn[i] = Menu.Add("useHeal" + i, new CheckBox("Use Heal to save " + EntityManager.Heroes.Allies[i].ChampionName));
                }
                Menu.AddSeparator();
                _autolevelskills = Menu.Add("autolevelskills", new CheckBox("Autolevelskills"));
                _autoBuyStartingItems = Menu.Add("autoBuyStartingItems", new CheckBox("Autobuy Starting Items (SR only)"));
                Menu.AddSeparator();
                _useSkinHack = Menu.Add("useSkinHack", new CheckBox("Use Skinhack"));
                _skinId = Menu.Add("skinId", new Slider("Skin ID", 7, 1, 8));
            }

            public static void Initialize()
            {
            }

        }

        public static class Modes
        {
            private static readonly Menu Menu;

            static Modes()
            {
                Menu = MenusSetting.MyHeroAshe.AddSubMenu("Modes");

                Combo.Initialize();
                Menu.AddSeparator();

                Harass.Initialize();
                LaneClear.Initialize();
                JungleClear.Initialize();
                Flee.Initialize();
            }

            public static void Initialize()
            {
            }

            public static class Combo
            {
                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useW;
                private static readonly CheckBox _useR;
                private static readonly CheckBox _useBOTRK;
                private static readonly CheckBox _useYOUMOUS;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }
                public static bool UseW
                {
                    get { return _useW.CurrentValue; }
                }
                public static bool UseR
                {
                    get { return _useR.CurrentValue; }
                }
                public static bool useBOTRK
                {
                    get { return _useBOTRK.CurrentValue; }
                }
                public static bool useYOUMOUS
                {
                    get { return _useYOUMOUS.CurrentValue; }
                }

                static Combo()
                {
                    Menu.AddGroupLabel("Combo");
                    _useQ = Menu.Add("comboUseQ", new CheckBox("Use [Q]"));
                    _useW = Menu.Add("comboUseW", new CheckBox("Use [W]"));
                    _useR = Menu.Add("comboUseR", new CheckBox("Use [R]"));
                    Menu.AddSeparator();
                    _useBOTRK = Menu.Add("useBotrk", new CheckBox("Use Blade of the Ruined King (Smart) and Cutlass"));
                    _useYOUMOUS = Menu.Add("useYoumous", new CheckBox("Use Youmous"));
                }

                public static void Initialize()
                {
                }

            }

            public static class Harass
            {
                public static bool UseQ
                {
                    get { return Menu["harassUseQ"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool useQMinionKillOnly
                {
                    get { return Menu["harassUseQKillingBlowOnly"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool UseW
                {
                    get { return Menu["harassUseW"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool UseE
                {
                    get { return Menu["harassUseE"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool UseR
                {
                    get { return Menu["harassUseR"].Cast<CheckBox>().CurrentValue; }
                }
                public static int Mana
                {
                    get { return Menu["harassMana"].Cast<Slider>().CurrentValue; }
                }

                static Harass()
                {
                    Menu.AddGroupLabel("Harass");
                    Menu.Add("harassUseQ", new CheckBox("Use [Q]"));
                    Menu.Add("harassUseW", new CheckBox("Use [W]"));
                    Menu.AddSeparator();
                    Menu.Add("harassMana", new Slider("Mana Percent > %", 40));
                }
                public static void Initialize()
                {
                }

            }

            public static class LaneClear
            {
                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useW;
                private static readonly Slider _mana;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }
                public static bool UseW
                {
                    get { return _useW.CurrentValue; }
                }
                public static int mana
                {
                    get { return _mana.CurrentValue; }
                }

                static LaneClear()
                {

                    Menu.AddGroupLabel("Lane Clear");
                    _useQ = Menu.Add("clearUseQ", new CheckBox("Use [Q]"));
                    _useW = Menu.Add("clearUseW", new CheckBox("Use [W]"));
                    _mana = Menu.Add("clearMana", new Slider("Mana Percent > %", 40));
                }

                public static void Initialize()
                {
                }
            }
            public static class JungleClear
            {
                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useW;
                private static readonly Slider _mana;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }
                public static bool UseW
                {
                    get { return _useW.CurrentValue; }
                }
                public static int mana
                {
                    get { return _mana.CurrentValue; }
                }

                static JungleClear()
                {
                    Menu.AddGroupLabel("Jungle Clear");
                    _useQ = Menu.Add("jglUseQ", new CheckBox("Use [Q]"));
                    _useW = Menu.Add("jglUseW", new CheckBox("Use [W]"));
                    _mana = Menu.Add("jglMana", new Slider("Mana Percent > %", 40));
                }

                public static void Initialize()
                {
                }
            }
            public static class Flee
            {
                private static readonly CheckBox _useW;

                public static bool UseW
                {
                    get { return _useW.CurrentValue; }
                }

                static Flee()
                {

                    Menu.AddGroupLabel("Flee");
                    _useW = Menu.Add("fleeUseW", new CheckBox("Use [W]"));
                }

                public static void Initialize()
                {
                }
            }
        }
    }
}
