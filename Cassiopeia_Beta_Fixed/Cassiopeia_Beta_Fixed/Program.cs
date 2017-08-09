using System;
using EloBuddy;
using EloBuddy.SDK.Events;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using static Cassiopeia_Beta_Fixed.SpellManager;
using static Cassiopeia_Beta_Fixed.Utils;
using static Cassiopeia_Beta_Fixed.Menus;
using System.Linq;
using EloBuddy.SDK.Enumerations;
using SharpDX;

namespace Cassiopeia_Beta_Fixed
{
    public static class Program
    {
        private static AIHeroClient Cassio => Player.Instance;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Cassioreia;
        }

        private static void Cassioreia(EventArgs args)
        {
            if (Cassio.Hero != Champion.Cassiopeia) return;
            Chat.Print("[Addon] [Champion] [Cassiopeia]", System.Drawing.Color.DimGray);

            Menus.Execute();
            SpellManager.Execute();
            Logics.Execute();
            //Comands
            Interrupter.OnInterruptableSpell += OnInterrupter;
            Gapcloser.OnGapcloser += OnGapcloser;
            Orbwalker.OnPreAttack += DisableAAreset;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnTick += Game_OnTick;

        }

        private static void Game_OnTick(EventArgs args)
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

        }

        private static void ByCombo()
        {
            const int range = 850;
            const float aoeratio = 0.2f;

            var enemies = EntityManager.Heroes.Enemies.Where(n => n.IsValidTarget(range));
            var selectedTarget = TargetSelector.GetTarget(range, DamageType.Magical);
            var allTargets = new[] { selectedTarget }.Concat(enemies.Where(n => n.Index != selectedTarget.Index).OrderByDescending(n => Logics.TotalDamage(SpellSlot.Q, n) / n.Health)).Where(n => n.IsValidTarget() && !n.WillDie());

            if (selectedTarget == null && !enemies.Any())
            {
                return;
            }

            var useQ = Combo["Qc"].Cast<CheckBox>().CurrentValue &&
                       Player.CanUseSpell(SpellSlot.Q) == SpellState.Ready;
            var useW = Combo["Wc"].Cast<CheckBox>().CurrentValue &&
                       Player.CanUseSpell(SpellSlot.W) == SpellState.Ready;
            var useE = Combo["Ec"].Cast<CheckBox>().CurrentValue &&
                       Player.CanUseSpell(SpellSlot.E) == SpellState.Ready;
            var useR = Combo["Rc"].Cast<CheckBox>().CurrentValue &&
                       Player.CanUseSpell(SpellSlot.R) == SpellState.Ready;

            if (enemies.Count() > 1)
            {
                if (useW)
                {
                    var aoePrediction = Prediction.Position.PredictCircularMissileAoe(enemies.Cast<Obj_AI_Base>().ToArray(), W.Range, W.Radius, W.CastDelay, W.Speed).OrderByDescending(r => r.GetCollisionObjects<Obj_AI_Minion>().Length).FirstOrDefault();
                    if (aoePrediction != null)
                    {
                        var predictedHeroes = aoePrediction.GetCollisionObjects<AIHeroClient>();

                        if (predictedHeroes.Length > 1 && (float)predictedHeroes.Length / enemies.Count() >= aoeratio)
                        {
                            W.Cast(aoePrediction.CastPosition);
                            return;
                        }
                    }
                }

                if (useE)
                {
                    foreach (var target in allTargets)
                    {
                        if (!target.IsInRange(_Player, E.Range) && !E.IsReady())
                            return;
                        {
                            if (E.IsReady() && useE)
                            {

                                E.Cast(target);
                                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

                            }
                            if (E.IsReady() && !useE)
                            {
                                E.Cast(target);
                            }
                        }
                    }
                }
            }
            if (useW)
            {
                foreach (var target in allTargets)
                {
                    if (!W.IsReady() && _Player.Distance(target) >= 700) return;
                    {

                        var Wpred = W.GetPrediction(target);
                        if (Wpred.HitChance >= HitChance.High && target.IsValidTarget(W.Range))
                        {
                            if (useW)
                            {
                                var Enemys = EntityManager.Heroes.Enemies.Where(x => x.IsInRange(_Player.Position, W.Range));
                                if (Enemys != null)
                                {
                                    if (Enemys.Count() >= Combo["minWw"].Cast<Slider>().CurrentValue)
                                    {
                                        W.Cast(target.ServerPosition);
                                    }
                                    else if (Enemys.Count() >= Combo["minWw"].Cast<Slider>().CurrentValue)
                                    {
                                        W.Cast(target);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (useQ)
            {
                foreach (var target in allTargets)
                {
                    if (Q.IsReady())
                    {
                        var canHitMoreThanOneTarget =
                          EntityManager.Heroes.Enemies.OrderByDescending(x => x.CountEnemyChampionsInRange(Q.Width))
                          .FirstOrDefault(x => x.IsValidTarget(Q.Range) && x.CountEnemyChampionsInRange(Q.Width) >= 2);
                        if (canHitMoreThanOneTarget != null)
                        {
                            var getAllTargets = EntityManager.Heroes.Enemies.FindAll(x => x.IsValidTarget() && x.IsInRange(canHitMoreThanOneTarget, Q.Width));
                            var center = getAllTargets.Aggregate(Vector3.Zero, (current, x) => current + x.ServerPosition) / getAllTargets.Count;
                            if (!center.IsZero)
                            {
                                var Qpred = Q.GetPrediction(target);
                                if (Qpred.HitChance >= HitChance.High && target.IsValidTarget(Q.Range))
                                {
                                    Q.Cast(target);
                                }
                            }
                        }
                    }
                }

                if (useQ)
                {
                    foreach (var target in allTargets)
                    {
                        var canHitMoreThanOneTarget =
                          EntityManager.Heroes.Enemies.OrderByDescending(x => x.CountEnemyChampionsInRange(Q.Width))
                          .FirstOrDefault(x => x.IsValidTarget(Q.Range) && x.CountEnemyChampionsInRange(Q.Width) >= 1);
                        if (canHitMoreThanOneTarget != null)
                        {
                            var getAllTargets = EntityManager.Heroes.Enemies.FindAll(x => x.IsValidTarget() && x.IsInRange(canHitMoreThanOneTarget, Q.Width));
                            var center = getAllTargets.Aggregate(Vector3.Zero, (current, x) => current + x.ServerPosition) / getAllTargets.Count;
                            if (!center.IsZero)
                            {
                                var Qpred = Q.GetPrediction(target);
                                if (Qpred.HitChance >= HitChance.High && target.IsValidTarget(Q.Range))
                                {
                                    Q.Cast(target);
                                }
                            }
                        }
                    }
                }

                if (useQ)
                {
                    foreach (var target in allTargets)
                    {
                        var Qpred = Q.GetPrediction(target);
                        if (Qpred.HitChance >= HitChance.High && target.IsValidTarget(Q.Range))
                        {
                            Core.DelayAction(() => Q.Cast(target), 100);
                        }
                    }
                }
            }

            if (useR)
            {
                foreach (var target in allTargets)
                {
                    var Enemys = EntityManager.Heroes.Enemies.Where(x => x.IsInRange(_Player.Position, R.Range - 25));
                    if (Enemys != null)
                    {
                        if (Enemys.Count() >= Combo["Re"].Cast<Slider>().CurrentValue && target.IsFacing(_Player))
                        {
                            Player.IssueOrder(GameObjectOrder.MoveTo, target);
                            R.Cast(target);
                        }
                        if (Enemys.Count() >= Combo["Re"].Cast<Slider>().CurrentValue)
                        {
                            Player.IssueOrder(GameObjectOrder.MoveTo, target);
                            R.Cast(target);
                        }

                        if (target.IsFacing(_Player) && target.IsInRange(_Player, R.Range))
                        {
                            Player.IssueOrder(GameObjectOrder.MoveTo, target);
                            R.Cast(target.ServerPosition);
                        }
                    }
                    if (target.IsInRange(_Player, R.Range))
                    {
                        Player.IssueOrder(GameObjectOrder.MoveTo, target);
                        R.Cast(target.ServerPosition);
                    }
                }
            }
        }

        private static void ByLane()
        {
            var useQ = Farm["Qf"].Cast<CheckBox>().CurrentValue && Player.Instance.ManaPercent >= Farm["Manal"].Cast<Slider>().CurrentValue && Player.CanUseSpell(SpellSlot.Q) == SpellState.Ready;
            var useW = Farm["Wf"].Cast<CheckBox>().CurrentValue && Player.Instance.ManaPercent >= Farm["Manal"].Cast<Slider>().CurrentValue && Player.CanUseSpell(SpellSlot.W) == SpellState.Ready;
            var useE = Farm["Ef"].Cast<CheckBox>().CurrentValue && Player.Instance.ManaPercent >= Farm["Manal"].Cast<Slider>().CurrentValue && Player.CanUseSpell(SpellSlot.E) == SpellState.Ready;

            if (useQ)
            {
                var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, 850, false);
                var predict = Prediction.Position.PredictCircularMissileAoe(minions.Cast<Obj_AI_Base>().ToArray(), Q.Range, Q.Radius, Q.CastDelay, Q.Speed).OrderByDescending(r => r.GetCollisionObjects<Obj_AI_Minion>().Length).FirstOrDefault();

                if (predict != null && predict.CollisionObjects.Length >= Farm["Qq"].Cast<Slider>().CurrentValue)
                {
                    Q.Cast(predict.CastPosition);
                    return;
                }
            }
            if (useW)
            {
                var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, 800, false);
                var predict = Prediction.Position.PredictCircularMissileAoe(minions.Cast<Obj_AI_Base>().ToArray(), W.Range, W.Radius, W.CastDelay, W.Speed).OrderByDescending(r => r.GetCollisionObjects<Obj_AI_Minion>().Length).FirstOrDefault();

                if (predict != null && predict.CollisionObjects.Length >= Farm["Ww"].Cast<Slider>().CurrentValue)
                {
                    W.Cast(predict.CastPosition);
                    return;
                }
            }

            if (useE)
            {
                var minione = EntityManager.MinionsAndMonsters.GetLaneMinions().FirstOrDefault(m => m.IsValidTarget(E.Range) && m.HasBuffOfType(BuffType.Poison));
                {
                    E.Cast(minione);
                }
            }
        }

        private static void ByJungle()
        {
            throw new NotImplementedException();
        }

        private static void AutoHara()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (Hara["AutoQ"].Cast<CheckBox>().CurrentValue)
            {
                var prediction = Q.GetPrediction(target);
                if (target.IsValidTarget(Q.Range) && prediction.HitChance >= HitChance.High)
                {
                    if (Player.Instance.ManaPercent > Hara["mana"].Cast<Slider>().CurrentValue)
                    {
                        Q.Cast(target);
                    }
                }
            }
        }

        private static void AutoR()
        {
            throw new NotImplementedException();
        }

        private static void Killteal()
        {
            const int range = 850;
            const float aoeratio = 0.2f;

            var enemies = EntityManager.Heroes.Enemies.Where(n => n.IsValidTarget(range));
            var selectedTarget = TargetSelector.GetTarget(range, DamageType.Magical);
            var allTargets = new[] { selectedTarget }.Concat(enemies.Where(n => n.Index != selectedTarget.Index).OrderByDescending(n => Logics.TotalDamage(SpellSlot.Q, n) / n.Health)).Where(n => n.IsValidTarget() && !n.WillDie());


            var ksQ = KillSteal["KsQ"].Cast<CheckBox>().CurrentValue &&
                      Player.CanUseSpell(SpellSlot.Q) == SpellState.Ready;
            var ksW = KillSteal["KsW"].Cast<CheckBox>().CurrentValue &&
                      Player.CanUseSpell(SpellSlot.W) == SpellState.Ready;
            var ksE = KillSteal["KsE"].Cast<CheckBox>().CurrentValue &&
                      Player.CanUseSpell(SpellSlot.E) == SpellState.Ready;
            var ksR = KillSteal["KsR"].Cast<CheckBox>().CurrentValue &&
                      Player.CanUseSpell(SpellSlot.R) == SpellState.Ready;

            foreach (var enemy in allTargets)
            {
                if (ksE && Player.Instance.IsInRange(enemy, E.Range) && enemy.Killable(SpellSlot.E))
                {
                    E.Cast(enemy);
                    return;
                }

                if (ksW && Player.Instance.IsInRange(enemy, W.Range + W.Radius) && enemy.Killable(SpellSlot.W))
                {
                    var prediction = W.GetPrediction(enemy);

                    if (prediction.HitChance >= HitChance.High)
                    {
                        W.Cast(prediction.CastPosition);
                        return;
                    }
                }

                if (ksQ && Player.Instance.IsInRange(enemy, Q.Range) && enemy.Killable(SpellSlot.Q))
                {
                    var prediction = Q.GetPrediction(enemy);

                    if (prediction.HitChance >= HitChance.High)
                    {
                        Q.Cast(prediction.CastPosition);
                        return;
                    }
                }

                if (ksR && Player.Instance.IsInRange(enemy, R.Range) && enemy.Killable(SpellSlot.R))
                {
                    var prediction = R.GetPrediction(enemy);

                    if (prediction.HitChance >= HitChance.High)
                    {
                        R.Cast(prediction.CastPosition);
                        return;
                    }
                }
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
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

        public static bool TargetBuffCasioo(this Obj_AI_Base target, float time)
        {
            var buff = target.Buffs.OrderByDescending(x => x.EndTime).FirstOrDefault(x => x.Type == BuffType.Poison && x.IsActive && x.IsValid);
            return buff == null || time > (buff.EndTime - Game.Time) * 1000f;
        }
        private static bool Immobile(Obj_AI_Base unit)
        {
            return unit.HasBuffOfType(BuffType.Charm) || unit.HasBuffOfType(BuffType.Stun) ||
                   unit.HasBuffOfType(BuffType.Knockup) || unit.HasBuffOfType(BuffType.Snare) ||
                   unit.HasBuffOfType(BuffType.Taunt) || unit.HasBuffOfType(BuffType.Suppression) ||
                   unit.HasBuffOfType(BuffType.Polymorph);
        }
        private static void DisableAAreset(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (
                !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                target.Type != GameObjectType.AIHeroClient || !Combo["DisAA"].Cast<CheckBox>().CurrentValue)
                return;

            if (Prediction.Position.Collision.LinearMissileCollision(
                target as Obj_AI_Base, _Player.ServerPosition.To2D(), target.Position.To2D(), int.MaxValue,
                (int)target.BoundingRadius, 0))
            {
                Orbwalker.DisableAttacking = true;
            }
        }

        private static void OnInterrupter(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (!sender.IsValidTarget()) return;
            if (Misc["Int"].Cast<CheckBox>().CurrentValue)
            {
                R.Cast(sender);
            }
        }
        private static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!sender.IsValidTarget()) return;
            if (Misc["Gap"].Cast<CheckBox>().CurrentValue)
            {
                W.Cast(sender);
            }
        }
    }
}