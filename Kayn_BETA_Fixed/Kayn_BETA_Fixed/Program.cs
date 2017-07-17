using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
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
        private static AIHeroClient Kayn => Player.Instance;

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }

        }

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_On;
        }

        private static void Loading_On(EventArgs args)
        {
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
            Kill();
        }

        private static void Kill()
        {
            var Rks = Misc["KS"].Cast<CheckBox>().CurrentValue;
            var Wks = Misc["KS"].Cast<CheckBox>().CurrentValue;
            var Qks = Misc["KS"].Cast<CheckBox>().CurrentValue;
            foreach (var enemy in EntityManager.Heroes.Enemies.Where(x => x.Distance(_Player) <= W.Range && x.IsValidTarget() && !x.IsInvulnerable && !x.IsZombie
                             && !x.HasBuff("JudicatorIntervention")
                             && !x.HasBuff("sionpassivezombie")
                             && !x.HasBuff("KarthusDeathDefiedBuff")
                             && !x.HasBuff("kogmawicathiansurprise")))
            {
                if (Wks && W.IsReady() &&
                    SpellDamage.Wmage(enemy) >= enemy.Health && enemy.Distance(_Player) >= 700)

                {
                    var prediction = W.GetPrediction(enemy);
                    if (prediction.HitChance >= HitChance.High)
                    {
                        W.Cast(prediction.CastPosition);
                    }
                }
                if (Qks && Q.IsReady() &&
                    SpellDamage.Qmage(enemy) >= enemy.Health && enemy.Distance(_Player) >= 350)

                {
                    var prediction = Q.GetPrediction(enemy);
                    if (prediction.HitChance >= HitChance.High)
                    {
                        Q.Cast(prediction.CastPosition);
                    }
                }
                if (Rks && R.IsReady() &&
                    SpellDamage.Rmage(enemy) >= enemy.Health && enemy.Distance(_Player) >= 550)
                {
                    R.Cast();
                }
            }
        }
        private static void byCombo()
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (Combo["W"].Cast<CheckBox>().CurrentValue)
            {
                if (target.Distance(ObjectManager.Player) <= W.Range && W.IsReady())
                {
                    W.Cast(W.GetPrediction(target).CastPosition);
                }

                target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (Combo["Q"].Cast<CheckBox>().CurrentValue)
                {
                    if (target.Distance(ObjectManager.Player) <= Q.Range && Q.IsReady())
                    {
                        Q.Cast(Q.GetPrediction(target).CastPosition);
                    }

                    target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                    if (Combo["R"].Cast<CheckBox>().CurrentValue)
                    {
                        if (target.Distance(ObjectManager.Player) <= R.Range && R.IsReady())
                        {
                            R.Cast(target);
                        }
                    }
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
            Q = new Spell.Skillshot(SpellSlot.Q, 350, EloBuddy.SDK.Enumerations.SkillShotType.Circular);
            W = new Spell.Skillshot(SpellSlot.W, 700, EloBuddy.SDK.Enumerations.SkillShotType.Linear);
            E = new Spell.Active(SpellSlot.E);
            R = new Spell.Targeted(SpellSlot.R, 550);
            R2 = new Spell.Skillshot(SpellSlot.R, 150, EloBuddy.SDK.Enumerations.SkillShotType.Linear);
        }
    }
}
