using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu.Values;
using System;
using static Caitlyn__Beta_Fixed.Menus;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Events;
using System.Linq;

namespace Caitlyn__Beta_Fixed
{
    class Program
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Skillshot E;
        public static Spell.Targeted R;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }

        }
        protected static bool IsTarget(AIHeroClient unit) => unit.Buffs.Any(thulio => thulio.Name == "caitlynShot");
        public static bool TiroForte => Caitlyn.Buffs.Any(x => x.Name == "HeadShot");

        public static HitChance HitQ()
        {
            switch (pre["HitQ"].Cast<ComboBox>().CurrentValue)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                default:
                    return HitChance.Unknown;
            }
        }
        public static HitChance HitE()
        {
            switch (pre["HitE"].Cast<ComboBox>().CurrentValue)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                default:
                    return HitChance.Unknown;
            }
        }
        private static AIHeroClient Caitlyn => Player.Instance;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_On;
        }

        private static void Loading_On(EventArgs args)
        {
            if (Caitlyn.Hero != Champion.Caitlyn) return;
            Chat.Print("[Addon] [Caitlyn] [Caitlyn]", System.Drawing.Color.Gray);
            Menus.InMenu();
            InSpells();
            Game.OnTick += Game_On;
            Drawing.OnDraw += OnDraw;
            Gapcloser.OnGapcloser += Gap_Closer;
            Interrupter.OnInterruptableSpell += Inte_On;
        }

        private static void Gap_Closer(AIHeroClient sender, Gapcloser.GapcloserEventArgs t)
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (sender.IsEnemy && sender.IsValidTarget(E.Range) && t.End.Distance(_Player) <= 350)
            {
                E.Cast(t.End);
            }
        }

        private static void Inte_On(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            //Não sei como faz isso ainda 
        }

        private static void Game_On(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                ByCombo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                ByLane();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                ByJungle();
            }
            AutoHara();
            AutoR();
            Killteal();
            ti();
            Back();
        }

        private static void ti()
        {
            if (Comb["ModoR"].Cast<ComboBox>().CurrentValue == 1)
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (Comb["Rf"].Cast<CheckBox>().CurrentValue)
                {
                    if (target != null && target.HealthPercent < Comb["LR"].Cast<Slider>().CurrentValue)
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
        }
        private static void Back()
        {
            if (Caiy["AutoAtack"].Cast<CheckBox>().CurrentValue)
            {
                TiroForte.Equals(EntityManager.Heroes.Enemies.Any(x => x.IsValidTarget(Caitlyn.GetAutoAttackRange())));
            }
        }
        private static void Killteal()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (Misc["Ks"].Cast<CheckBox>().CurrentValue)
            {
                if (Q.IsReady() && Q.CanCast(target))
                {
                    var prediction = W.GetPrediction(target);
                    if (target.IsValidTarget(Q.Range) && prediction.HitChance >= HitQ() && target.Health < SpellDamage.Qmage(target))
                    {
                        Q.Cast(prediction.CastPosition);
                    }
                }
            }
        }
        private static void AutoR()
        {
            if (Comb["ModoR"].Cast<ComboBox>().CurrentValue == 0)
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (Comb["Rf"].Cast<CheckBox>().CurrentValue)
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
        }
        private static void ByCombo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (!target.IsValidTarget(Q.Range) || target == null)
            {
                return;
            }
            if (Comb["Ec"].Cast<CheckBox>().CurrentValue)
            {
                if (E.IsReady() && E.CanCast(target) && !TiroForte)
                {
                    var prediction = E.GetPrediction(target);
                    if (target.IsValidTarget(E.Range) && prediction.HitChance >= HitE())
                    {
                        E.Cast(prediction.CastPosition);
                    }
                }
            }
            if (Comb["Qc"].Cast<CheckBox>().CurrentValue)
            {
                if (Q.IsReady() && Q.CanCast(target))
                {
                    var prediction = W.GetPrediction(target);
                    if (target.IsValidTarget(Q.Range) && prediction.HitChance >= HitQ())
                    {
                        Q.Cast(prediction.CastPosition);
                    }
                }
            }
            if (Comb["Wc"].Cast<CheckBox>().CurrentValue)
            {
                if (W.IsReady() && !TiroForte)
                {
                    var predictionPos = Prediction.Position.PredictUnitPosition(target, 500).To3D();

                    if (W.IsInRange(predictionPos))
                    {
                        W.Cast(predictionPos);
                    }
                }
            }
        }
        private static void ByLane()
        {
            if (Lane["Ql"].Cast<CheckBox>().CurrentValue && _Player.ManaPercent > Lane["ManaL"].Cast<Slider>().CurrentValue)
            {
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(min => min.IsEnemy && !min.IsDead && min.IsValid && !min.IsInvulnerable && min.IsInRange(_Player.Position, Q.Range));
                foreach (var mayminoon in minions)
                {
                    if (Q.GetPrediction(mayminoon).CollisionObjects.Where(may => may.IsEnemy && !may.IsDead && may.IsValid && !may.IsInvulnerable).Count() >= Lane["Qmi"].Cast<Slider>().CurrentValue)
                    {
                        Q.Cast(mayminoon);
                    }
                }
            }
        }
        private static void ByJungle()
        {
            if (Jungle["Qj"].Cast<CheckBox>().CurrentValue && Q.IsReady() && Player.Instance.ManaPercent > Jungle["Q/J"].Cast<Slider>().CurrentValue)
            {
                var miniQ = EntityManager.MinionsAndMonsters.GetJungleMonsters(_Player.Position, Q.Range).Where(my => !my.IsDead && my.IsValid && !my.IsInvulnerable);
                if (miniQ.Count() > 0)
                {
                    Q.Cast(miniQ.First());
                }
            }
        }
        private static void AutoHara()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (Auto["AutoQ"].Cast<CheckBox>().CurrentValue)
            {
                var prediction = Q.GetPrediction(target);
                if (target.IsValidTarget(Q.Range) && prediction.HitChance >= HitQ())
                {
                    if (Player.Instance.ManaPercent > Auto["ManaQ"].Cast<Slider>().CurrentValue)
                    {
                        Q.Cast(target);
                    }
                }
            }
        }
        private static void InSpells()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1225, SkillShotType.Linear, 625, 2200, 90)
            {
                AllowedCollisionCount = -1
            };
            W = new Spell.Skillshot(SpellSlot.W, 800, SkillShotType.Circular, 500, int.MaxValue, 20);
            E = new Spell.Skillshot(SpellSlot.E, 850, SkillShotType.Linear, 150, 1600, 80)
            {
                AllowedCollisionCount = 0
            };
            R = new Spell.Targeted(SpellSlot.R, 2000);
        }
        public static void Ultimate(EventArgs args)
        {
            if (R.Level == 2)
                R = new Spell.Targeted(SpellSlot.R, 2500);
            if (R.Level == 3)
                R = new Spell.Targeted(SpellSlot.R, 3000);
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
    }
}