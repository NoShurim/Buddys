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
            Chat.Print("[Addon] [Champion] [Kayn]", System.Drawing.Color.LightBlue);
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
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (Combo["E"].Cast<CheckBox>().CurrentValue)
            {
                if (target.Distance(ObjectManager.Player) <= E.Range && E.IsReady())
                {
                    E.Cast(target);
                }

                target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
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
        }

        private static void ByLane()
        {
            var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Kayn.Position, W.Range).ToArray();

            if (minions != null)
            {

                var wpred = EntityManager.MinionsAndMonsters.GetLineFarmLocation(minions, W.Width, (int)W.Range);

                if (Lane["Qlane"].Cast<CheckBox>().CurrentValue && Q.IsLearned && Q.IsReady())
                {
                    foreach (var minion in minions.Where(x => x.IsValid() && !x.IsDead && x.Health > 15))
                    {
                        if (Lane["Qmode"].Cast<ComboBox>().CurrentValue == 0 &&
                            Prediction.Position.PredictUnitPosition(minion, Q.CastDelay).Distance(Kayn.Position) <= (Q.Range - 50))
                        {
                            Q.Cast(minion.Position);
                        }

                        else { Q.Cast(minion.Position); }

                    }
                }
                if (Lane["WLane"].Cast<CheckBox>().CurrentValue && W.IsLearned && W.IsReady())
                {
                    if (Lane["Win"].Cast<Slider>().CurrentValue == 1)
                    {
                        switch (Lane["Wmode"].Cast<ComboBox>().CurrentValue)
                        {
                            case 0:
                                if (wpred.HitNumber == Lane["WP"].Cast<Slider>().CurrentValue) { W.Cast(wpred.CastPosition); }
                                break;
                            case 1:
                                W.Cast(minions.Where(x => x.Distance(Kayn.Position) < W.Range &&
                                                               !x.IsDead && x.Health > 25 && x.IsValid()).OrderBy(x => x.Distance(Kayn.Position))
                                                                                                         .FirstOrDefault().Position);
                                break;
                        }
                    }
                    else
                    {
                        if (wpred.HitNumber >= Lane["WP"].Cast<Slider>().CurrentValue)
                        {
                            W.Cast(wpred.CastPosition);
                        }
                    }
                }
            }
        }
        private static void ByJungle()
        {
            var Monsters = EntityManager.MinionsAndMonsters.GetJungleMonsters(Kayn.Position, 1800f);

            var Wp = EntityManager.MinionsAndMonsters.GetLineFarmLocation(Monsters, W.Width, (int) W.Range);

            if (Jungle["Qjungle"].Cast<CheckBox>().CurrentValue && Q.IsLearned && Q.IsReady())
            {
                foreach (var monster in Monsters.Where(x => !x.IsDead && x.IsValidTarget(Q.Range) && x.Health > 100))
                {
                   Q.Cast(monster.Position);
                }
            }

            if (Jungle["Wjungle"].Cast<CheckBox>().CurrentValue && W.IsLearned && W.IsReady())
            {
                if (Wp.HitNumber >= Jungle["J"].Cast<Slider>().CurrentValue) W.Cast(Wp.CastPosition);
            }
        }
        private static void InitializeSpells()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 550, EloBuddy.SDK.Enumerations.SkillShotType.Circular);
            W = new Spell.Skillshot(SpellSlot.W, 700, EloBuddy.SDK.Enumerations.SkillShotType.Linear);
            E = new Spell.Active(SpellSlot.E, 3000);
            R = new Spell.Targeted(SpellSlot.R, 550);
            R2 = new Spell.Skillshot(SpellSlot.R, 150, EloBuddy.SDK.Enumerations.SkillShotType.Linear);
        }
    }
}
