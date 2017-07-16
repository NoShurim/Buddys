using EloBuddy;
using EloBuddy.SDK.Events;
using EloBuddy.SDK;
using System;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System.Linq;
using SharpDX;

namespace Kayn_Official
{
    class Champi
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Active E;
        public static Spell.Targeted R;
        public static Menu kay, comb, hara, lane, jungle, Misc;
        private static AIHeroClient Kayn => Player.Instance;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }

        }
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += LoadingKayn;
        }

        private static void LoadingKayn(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Kayn) return;
            Chat.Print("Kayn");

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += DamageIndicator.Indicator;
            Gapcloser.OnGapcloser += Modes.OnGapcloser;
            Interrupter.OnInterruptableSpell += Modes.InteruptSpells;

            Q = new Spell.Skillshot(SpellSlot.Q, 350, EloBuddy.SDK.Enumerations.SkillShotType.Linear);
            W = new Spell.Skillshot(SpellSlot.W, 700, EloBuddy.SDK.Enumerations.SkillShotType.Linear);
            E = new Spell.Active(SpellSlot.E);
            R = new Spell.Targeted(SpellSlot.R, 550);

            kay = MainMenu.AddMenu("Kayn", "Kayn");
            comb = kay.AddSubMenu("Combo");
            comb.Add("Qc", new CheckBox("Use [Q] in Combo"));
            comb.Add("Wc", new CheckBox("Use [W] in Combo"));
            comb.Add("Ec", new CheckBox("Use [E] in Combo", false));
            comb.Add("Rc", new CheckBox("Use [R] in Combo"));
            comb.AddLabel("[R] Settings");
            comb.Add("RKS", new CheckBox("Use [R] Always ends"));
            comb.Add("Rkss", new Slider("Ute percent enemy", 1, 0, 5));
            hara = kay.AddSubMenu("Harass");
            hara.Add("Wh", new CheckBox("Use [W] in Harass"));
            hara.AddLabel("Harass Mana Manager");
            hara.Add("Mana", new Slider("Mana [W]", 50, 1, 100));
            lane = kay.AddSubMenu("Lane");
            lane.Add("Ql", new CheckBox("Use [Q] in Lane"));
            lane.Add("Wl", new CheckBox("Use [W] in Lane"));
            lane.Add("Manl", new Slider("Mana [LaneClear]", 50, 1, 100));
            jungle = kay.AddSubMenu("Jungle");
            jungle.Add("Qj", new CheckBox("Use [Q] in Jungle"));
            jungle.Add("Wj", new CheckBox("Use [W] in Jungle"));
            Misc = kay.AddSubMenu("Misc");
            Misc.Add("Inter", new CheckBox("Interompt"));
            Misc.Add("Gap", new CheckBox("Aint-Gap"));
            Misc.Add("Ks", new CheckBox("Use KS"));
            Misc.Add("HP", new Slider("Health Percent", 15, 0, 100));
            //

        }
        public static readonly AIHeroClient KaynShadown = ObjectManager.Player;

        private static void OnDraw(EventArgs args)
        {
            throw new NotImplementedException();
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            Modes.Load();
        }

        private static void Kayn_Tick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            { Modes.KCombo(); }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            { Modes.KHarass(); }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            { Modes.KLane(); }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            { Modes.KJungle(); }

        }
    }
}