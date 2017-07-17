using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using System;
using System.Linq;
using static Kayn_BETA_Fixed.Menus;

namespace Kayn_BETA_Fixed
{
    class Program
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Active E;
        public static Spell.Targeted R;
        public static Spell.Skillshot R2;
        public static AIHeroClient _Player;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_On;
        }

        private static void Loading_On(EventArgs args)
        {
            if (Player.Instance.Hero == Champion.Kayn)
                Chat.Print("[Addon] [Champion] [Kayn]", System.Drawing.Color.LightBlue);
            else if (Player.Instance.Hero != Champion.Kayn)
            {
                return;
            }
            CreateMenu();
            InitializeSpells();
            Drawing.OnDraw += OnDraw;
            Game.OnTick += OnTick;
        }

        private static void OnDraw(EventArgs args)
        {
            if (Draws["DQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                Q.DrawRange(System.Drawing.Color.LightBlue);
            }
            if (Draws["DE"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                E.DrawRange(System.Drawing.Color.Crimson);
            }
            if (Draws["DR"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                R.DrawRange(System.Drawing.Color.LawnGreen);
            }
        }
        private static void OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.Equals(Orbwalker.ActiveModes.Combo))
            {
                byCombo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                ByLane();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                ByJungle();
            }
        }
        private static void byCombo()
        {
            throw new NotImplementedException();
        }

        private static void ByLane()
        {
            throw new NotImplementedException();
        }

        private static void ByJungle()
        {
            if (Jungle["Qjungle"].Cast<CheckBox>().CurrentValue && Q.IsReady()) //range missing
            {
                var jungleMonsterQ = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Q.Range);
                if (jungleMonsterQ != null)
                {
                    foreach (var junglemonsterq in jungleMonsterQ)
                    {
                        if (Player.Instance.ManaPercent >= Jungle["qmana"].Cast<Slider>().CurrentValue)
                        {
                            Q.Cast(junglemonsterq);
                        }
                    }
                }
            }

            if (Jungle["Wjungle"].Cast<CheckBox>().CurrentValue && W.IsReady()) //range missing
            {
                var jungleMonsterW = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, W.Range);
                if (jungleMonsterW != null)
                {
                    foreach (var junglemonstere in jungleMonsterW)
                    {
                        if (Player.Instance.ManaPercent >= Jungle["wmana"].Cast<Slider>().CurrentValue)
                        {
                           W.Cast(junglemonstere);
                        }
                    }
                }
            }
        }
        private static void InitializeSpells()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 350, EloBuddy.SDK.Enumerations.SkillShotType.Linear, 150, 75, 37);
            W = new Spell.Skillshot(SpellSlot.W, 700, EloBuddy.SDK.Enumerations.SkillShotType.Linear, 350, 175, 87);
            E = new Spell.Active(SpellSlot.E);
            R = new Spell.Targeted(SpellSlot.R, 550);
            R2 = new Spell.Skillshot(SpellSlot.R, 150, EloBuddy.SDK.Enumerations.SkillShotType.Linear, 75, 37, 18);
        }
    }
}
