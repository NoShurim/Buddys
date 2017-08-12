using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using System;
using System.Linq;
using static Thresh_Beta_Fixed.Menus;
using static Thresh_Beta_Fixed.Spells;

namespace Thresh_Beta_Fixed
{
    class MyHero
    {
        private static AIHeroClient Thresh => Player.Instance;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += On_Complete;
        }

        private static void On_Complete(EventArgs args)
        {
            if (Thresh.Hero != Champion.Thresh) return;

            Menus.Execute();
            Spells.Execute();

            Gapcloser.OnGapcloser += AntiGapCloser;
            Interrupter.OnInterruptableSpell += Interupt;
            Game.OnTick += Game_Update;
        }

        private static void Game_Update(EventArgs args)
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
            Shild();

        }

        private static void Shild()
        {
            if (W.IsReady())
            {
                foreach (var ally in EntityManager.Heroes.Allies.Where(x => x.IsValidTarget(W.Range) && !x.IsMe && !x.IsDead))
                {
                    W.Cast(ally);
                }
            }
        }

        private static void ByCombo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var targete = TargetSelector.GetTarget(E.Range, DamageType.Magical);

            if (Qc() && Q.IsReady() && target.IsValidTarget(Q.Range))
            {
                CastQ(target);
            }
            if (Ec() && E.IsReady() && target.IsValidTarget(E.Range))
            {
                CastE(targete);
            }
            if (Rc() && R.IsReady() && target.IsInRange(Thresh, R.Range))
            {
                R.Cast();
            }
        }
                            
        private static void ByLane()
        {
            throw new NotImplementedException();
        }

        private static void ByJungle()
        {
            throw new NotImplementedException();
        }

        private static void AutoHara()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (Hq() && Q.IsReady() && target.IsValidTarget(Q.Range) && target.Health < 25)
            {
                var prediction = Q.GetPrediction(target);
                if (prediction.HitChance >= HitChance.High)
                {
                    Q.Cast(target);
                }
            }
        }
        

        private static void AntiGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (Gap() && sender.IsEnemy && sender.IsValidTarget(E.Range) && e.End.Distance(Thresh) <= 250)
            {
                E.Cast(e.End);
            }
            if (Gap() && sender.IsEnemy && sender.IsValidTarget(Q.Range) && e.End.Distance(Thresh) <= 250)
            {
                Q.Cast(e.End);
            }
        }

        private static void Interupt(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (!sender.IsEnemy) return;

            if (e.DangerLevel == DangerLevel.High && Int() && Q.IsReady())
            {
                Q.Cast(sender.Position);
            }
        }
    }
}
