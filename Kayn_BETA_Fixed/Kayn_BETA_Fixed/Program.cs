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
            if (Draws["DW"].Cast<CheckBox>().CurrentValue && W.IsReady())
            {
                W.DrawRange(System.Drawing.Color.Crimson);
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
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if ((target == null) || target.IsInvulnerable) return;

            if (Combo["Q"].Cast<CheckBox>().CurrentValue)
            {
                if (target.IsValidTarget(Q.Range) && Q.IsReady())
                {
                    var qhitchance = Q.GetPrediction(target);
                    if (qhitchance.HitChancePercent >= Combo["Qhit"].Cast<Slider>().CurrentValue)
                    {
                        Q.Cast(qhitchance.CastPosition);
                    }
                }
            }
            if (Combo["W"].Cast<CheckBox>().CurrentValue)
            {
                if (target.IsValidTarget(W.Range) && W.IsReady())
                {
                    var whitchance = W.GetPrediction(target);
                    if (whitchance.HitChancePercent >= Combo["Whit"].Cast<Slider>().CurrentValue)
                    {
                        W.Cast(whitchance.CastPosition);
                    }
                }
            }
            if (Combo["E"].Cast<CheckBox>().CurrentValue)
            {
                if (E.IsReady() && target.IsValidTarget(1000) && !target.IsDead && !target.IsZombie)
                {
                    E.Cast();
                }
            }
            if (Combo["R"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                var enemy = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (enemy.IsValidTarget(R.Range) &&
                    Player.Instance.GetSpellDamage(enemy, SpellSlot.R) > enemy.Health + enemy.AttackShield)
                {
                    R.Cast(enemy);
                }
                else if (R.IsReady())
                {
                    R2.Cast();
                }
            }
        }
        private static void ByLane()
        {
            var laneQ = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, Q.Range);
            var laneW = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, W.Range);

            if (Lane["Qlane"].Cast<CheckBox>().CurrentValue && Q.IsReady()) 
            {
                foreach (var minionQ in laneQ)
                {
                    if (Player.Instance.ManaPercent >= Lane["qmana"].Cast<Slider>().CurrentValue)
                    {
                        Q.Cast(minionQ);
                    }
                }
            }
            if (Lane["WLane"].Cast<CheckBox>().CurrentValue && W.IsReady())
            {
                foreach (var minionE in laneW)
                {
                    if (Player.Instance.ManaPercent >= Lane["wmana"].Cast<Slider>().CurrentValue)
                    {
                        W.Cast(minionE);
                    }
                }
            }
        }
        private static void ByJungle()
        {
            if (Jungle["Qjungle"].Cast<CheckBox>().CurrentValue && Q.IsReady()) 
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

            if (Jungle["Wjungle"].Cast<CheckBox>().CurrentValue && W.IsReady()) 
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
