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
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalker.OnPostAttack += OnAfterAttack;
            Game.OnTick += Game_OnTick;

        }

        private static void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    if (target == null || !(target is AIHeroClient) || target.IsDead || target.IsInvulnerable ||
                        !target.IsEnemy || target.IsPhysicalImmune || target.IsZombie)
                        return;

                var enemy = target as AIHeroClient;
                if (enemy == null)
                    return;
                if (Combo["QAA"].Cast<CheckBox>().CurrentValue)
                {
                    if (Q.IsReady())
                    {
                        Q.Cast(enemy);
                    }
                    if (Combo["EAA"].Cast<CheckBox>().CurrentValue)
                    {
                        if (E.IsReady())
                        {
                            E.Cast(enemy);
                        }
                    }
                }
            }
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
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                ByLast();
            }
            AutoHara();
            AutoR();
            Killteal();

        }

        private static void ByLast()
        {
            var minion = EntityManager.MinionsAndMonsters.Get(EntityManager.MinionsAndMonsters.EntityType.Minion, EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition, E.Range).FirstOrDefault(m => E.IsReady() && m.IsValidTarget(E.Range) && Prediction.Health.GetPrediction(m, E.CastDelay) <= Logics.GetMinionTarget(m, SpellSlot.E));

            if (Farm["Elast"].Cast<CheckBox>().CurrentValue && !(Farm["Buff"].Cast<CheckBox>().CurrentValue) && E.IsReady() && minion.IsValidTarget(E.Range))
            {
                E.Cast(minion);
            }

            if (Farm["Elast"].Cast<CheckBox>().CurrentValue && Farm["Buff"].Cast<CheckBox>().CurrentValue && E.IsReady() && minion.IsValidTarget(E.Range) && minion.HasBuffOfType(BuffType.Poison))
            {
                E.Cast(minion);
            }
        }

        private static void ByCombo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if ((target == null) || target.IsInvulnerable)
                return;
         
            var PositionEnemys = EntityManager.Heroes.Enemies.Find(e => e.IsValidTarget(E.Range) && e.HasBuffOfType(BuffType.Poison));

            if (R.IsReady() && Combo["Rc"].Cast<CheckBox>().CurrentValue && !target.IsDead && target.IsValidTarget(R.Range))
            {
                R.Cast(target.Position);
            }

            if (R.IsReady() && Combo["Rc"].Cast<CheckBox>().CurrentValue &&  !target.IsDead && target.IsValidTarget(R.Range))
            { 
                R.Cast(target.Position);
            }

            if (Combo["Wc"].Cast<CheckBox>().CurrentValue && W.IsReady() && target.IsValidTarget(W.Range))
            {
                var prediction = W.GetPrediction(target);
                if (prediction.HitChance >= HitChance.High)
                {
                    W.Cast(prediction.CastPosition);
                }
            }

            if (Combo["Qc"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.IsValidTarget(Q.Range + 100))
            {
                var prediction = Q.GetPrediction(target);
                if (prediction.HitChance >= HitChance.High)
                {
                    Q.Cast(prediction.CastPosition);
                }

            }

            if (E.IsReady() && Combo["Ec"].Cast<CheckBox>().CurrentValue && PositionEnemys.IsValidTarget(E.Range) && (Q.IsOnCooldown || !target.IsInRange(Cassio, Q.Range)))
            {             
                E.Cast(PositionEnemys);
            }

            else if (E.IsReady() && Combo["Ec"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(E.Range) && (Q.IsOnCooldown || !target.IsInRange(Cassio, Q.Range)))
            {
               E.Cast(target);
            }
        }

        private static void ByLane()
        {
            var minion1 = EntityManager.MinionsAndMonsters.GetLaneMinions().Where(m => m.IsValidTarget(E.Range)).OrderBy(m => !(m.Health <= Logics.GetMinionTarget(m, SpellSlot.E))).ThenBy(m => !m.HasBuffOfType(BuffType.Poison)).ThenBy(m => m.Health).FirstOrDefault();
            var minion2 = EntityManager.MinionsAndMonsters.GetLaneMinions().FirstOrDefault(m => m.IsValidTarget(E.Range) && m.HasBuffOfType(BuffType.Poison));
            var minion3 = EntityManager.MinionsAndMonsters.GetLaneMinions().Where(m => m.IsValidTarget(W.Range)).ToArray();
            if (minion3.Length == 0) return;

            var minionlocation = Prediction.Position.PredictCircularMissileAoe(minion3, W.Range, W.Width, W.CastDelay, W.Speed).OrderByDescending(r => r.GetCollisionObjects<Obj_AI_Minion>().Length).FirstOrDefault();

            if (Farm["Qf"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                var predictedMinion = minionlocation.GetCollisionObjects<Obj_AI_Minion>();
                if (predictedMinion.Length >= Farm["Qq"].Cast<Slider>().CurrentValue)
                {
                    Q.Cast(minionlocation.CastPosition);
                }
            }

            if (Farm["Qf"].Cast<CheckBox>().CurrentValue && Q.IsReady() && _Player.ManaPercent >= Farm["Manal"].Cast<Slider>().CurrentValue)
            {
                var predictedMinion = minionlocation.GetCollisionObjects<Obj_AI_Minion>();
                if (predictedMinion.Length >= Farm["Qq"].Cast<Slider>().CurrentValue)
                {
                    Q.Cast(minionlocation.CastPosition);
                }
            }

            if (Farm["Wf"].Cast<CheckBox>().CurrentValue && W.IsReady() && _Player.ManaPercent >= Farm["Manal"].Cast<Slider>().CurrentValue)
            {
                var predictedMinion = minionlocation.GetCollisionObjects<Obj_AI_Minion>();
                if (predictedMinion.Length >= Farm["Ww"].Cast<Slider>().CurrentValue)
                {
                    W.Cast(minionlocation.CastPosition);

                }
            }

            if (Farm["Ef"].Cast<CheckBox>().CurrentValue && E.IsReady() && Cassio.ManaPercent >= Farm["Manal"].Cast<Slider>().CurrentValue && minion1.IsValidTarget(E.Range))
            {

                E.Cast(minion2);
            }
        }
               
        private static void ByJungle()
        {
            throw new NotImplementedException();
        }

        private static void AutoHara()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if ((target == null) || target.IsInvulnerable)
                return;

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