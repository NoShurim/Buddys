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
            Chat.Print("[Addon] [Champion] [Kayn]", System.Drawing.Color.LightBlue);
            Chat.Print("[Version] [1.2.1.6]", System.Drawing.Color.Red);
            Chat.Print("[Combo]", System.Drawing.Color.LightBlue);
            Chat.Print("[Haras]", System.Drawing.Color.LightBlue);
            Chat.Print("[LaneClear]", System.Drawing.Color.LightBlue);
            Chat.Print("[JungleClear]", System.Drawing.Color.LightBlue);
            Chat.Print("[Mics]", System.Drawing.Color.LightBlue);

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
            Bacck();
        }
        private static void Bacck()
        {
            var useR = Combo["ultR"].Cast<CheckBox>().CurrentValue;
            var evadeR = Combo["MR"].Cast<Slider>().CurrentValue;
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if(target.Distance(ObjectManager.Player) <= R.Range)
            {
                if (useR && !target.IsInRange(_Player, R.Range) && R.IsReady() && Kayn.HealthPercent <= evadeR)
                {
                    R.Cast(target);
                }
            }
        }
                              
        private static void Kill()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical); //By BestSNA
            if (Misc["KSR"].Cast<CheckBox>().CurrentValue)
            {
                if (target != null && target.Health < SpellDamage.Rmage(target))
                {
                    if (!target.IsInRange(_Player, R.Range) && R.IsReady())
                    {
                        return;
                    }
                    {
                        R.Cast(target);
                    }
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
            var mana = Lane["mana"].Cast<Slider>().CurrentValue;
            if (_Player.ManaPercent < mana) return;
            if (minions != null)
            {
                var wpred = EntityManager.MinionsAndMonsters.GetLineFarmLocation(minions, W.Width, (int)W.Range);
                var minW = Lane["Min"].Cast<Slider>().CurrentValue;

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
                    foreach (var minion in minions.Where(x => x.IsValid() && !x.IsDead && x.Health > 15))
                    {
                        if (Lane["Wmode"].Cast<ComboBox>().CurrentValue == 0 && wpred.HitNumber >= minW &&
                            Prediction.Position.PredictUnitPosition(minion, W.CastDelay).Distance(Kayn.Position) <= (W.Range + 700))
                        {
                            W.Cast(minion.Position);
                        }

                        else { W.Cast(minion.Position); }

                    }
                }
            }
        }
        private static void ByJungle()
        {
            var Monsters = EntityManager.MinionsAndMonsters.GetJungleMonsters(Kayn.Position, 1800f);
            var mana = Jungle["jmana"].Cast<Slider>().CurrentValue;
            if (_Player.ManaPercent < mana) return;
            var WPred = EntityManager.MinionsAndMonsters.GetLineFarmLocation(Monsters, W.Width, (int) W.Range);

            if (Jungle["Qjungle"].Cast<CheckBox>().CurrentValue && Q.IsLearned && Q.IsReady())
            {
                foreach (var monster in Monsters.Where(x => !x.IsDead && x.IsValidTarget(Q.Range) && x.Health > 100))
                {
                   Q.Cast(monster.Position);
                }
            }

            if (Jungle["Wjungle"].Cast<CheckBox>().CurrentValue && W.IsLearned && W.IsReady())
            {
                if (WPred.HitNumber >= Jungle["J"].Cast<Slider>().CurrentValue) W.Cast(WPred.CastPosition);
            }
        }
        private static void InitializeSpells()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 550, EloBuddy.SDK.Enumerations.SkillShotType.Circular);
            W = new Spell.Skillshot(SpellSlot.W, 700, EloBuddy.SDK.Enumerations.SkillShotType.Linear);
            E = new Spell.Active(SpellSlot.E, 2000);
            R = new Spell.Targeted(SpellSlot.R, 550);

        }
    }
}
