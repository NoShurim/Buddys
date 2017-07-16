using EloBuddy.SDK;
using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Events;
using static Kayn_Official.Champi;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Enumerations;

namespace Kayn_Official
{
    internal class Modes
    {
        internal static void Load()
        {
            foreach (var target in EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(550) && !x.IsZombie))
            {
                if (Misc["Ks"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(R.Range) && R.IsReady())
                {
                    if (target != null && target.Health < DamageIndicator.Rdama(target) && !target.HasUndyingBuff())
                    {
                        R.Cast();
                    }
                }
            }
        }

        internal static void InteruptSpells(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            var Interupt = Misc["Inter"].Cast<CheckBox>().CurrentValue;
            if (!sender.IsEnemy || !(sender is AIHeroClient) || Player.Instance.IsRecalling())
            {
                return;
            }

            if (Interupt && R.IsReady() && e.DangerLevel == DangerLevel.High && R.IsInRange(sender))
            {
                R.Cast();
            }
        }
        internal static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (Misc["Rag"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                if (Player.Instance.HealthPercent < Misc["HP"].Cast<Slider>().CurrentValue)
                {
                    if (sender != null
                            && sender.IsEnemy
                                && sender.IsValid
                                    && (sender.IsAttackingPlayer || Player.Instance.Distance(e.End) < R.Range || e.End.IsInRange(Player.Instance, R.Range)))
                    {
                        R.Cast(e.End);
                    }
                }
            }
        }

        internal static void KCombo()
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Mixed);
            var wpred = W.GetPrediction(target);
            if (comb["Wc"].Cast<CheckBox>().CurrentValue && W.IsReady() && target.IsValidTarget(700) && wpred.HitChance >= HitChance.High && !target.IsDead && !target.IsZombie)
            {
                W.Cast(wpred.CastPosition);
            }
            var targetq = TargetSelector.GetTarget(Q.Range, DamageType.Mixed);
            var qpred = Q.GetPrediction(target);
            if (comb["Qc"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.IsValidTarget(350) && qpred.HitChance >= HitChance.High && !target.IsDead && !target.IsZombie)
            {
                Q.Cast(wpred.CastPosition);
            }

            if (comb["Rc"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                var Count = KaynShadown.CountEnemyChampionsInRange(550);
                if (Count >= comb["Rkss"].Cast<Slider>().CurrentValue)
                {
                    R.Cast();
                }
            }
        }

        internal static void KHarass()
        {
            var joosha = TargetSelector.GetTarget(E.Range, DamageType.Mixed);
            if (hara["Wh"].Cast<CheckBox>().CurrentValue && W.IsReady() && joosha.IsValidTarget(700) && !joosha.IsDead && !joosha.IsZombie)
            {
                W.Cast();
            }
        }

        internal static void KLane()
        {
            var minions = ObjectManager.Get<Obj_AI_Base>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            if (lane["Manl"].Cast<Slider>().CurrentValue <= Player.Instance.ManaPercent)
                foreach (var m in minions)
                {
                    if (lane["Wl"].Cast<CheckBox>().CurrentValue && W.IsReady() && m.IsValidTarget(700))
                    {
                        W.Cast();
                    }
                    if (lane["Ql"].Cast<CheckBox>().CurrentValue && Q.IsReady() && m.IsValidTarget(300))
                    {
                        Q.Cast(m);
                    }
                }
        }

        internal static void KJungle()
        {
            var jungleMonsters = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(250));
            if (lane["Manl"].Cast<Slider>().CurrentValue <= Player.Instance.ManaPercent)
            {
                if (jungle["Wj"].Cast<CheckBox>().CurrentValue && W.IsReady() && jungleMonsters.IsValidTarget(700))
                {
                    W.Cast();
                }
                if (jungle["Qj"].Cast<CheckBox>().CurrentValue && Q.IsReady() && jungleMonsters.IsValidTarget(300))
                {
                    Q.Cast(jungleMonsters);
                }
            }
        }
    }
}