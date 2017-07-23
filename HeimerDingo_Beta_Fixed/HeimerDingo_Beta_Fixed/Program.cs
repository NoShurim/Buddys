using System;
using EloBuddy.SDK.Events;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu.Values;
using static HeimerDingo_Beta_Fixed.Menus;
using System.Linq;

namespace HeimerDingo_Beta_Fixed
{
    class Program
    {
        public static Spell.Skillshot Q, W, E;
        public static Spell.Active R;
        public static Spell.Skillshot Q2, W2, E2;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }

        }
        private static AIHeroClient Heimerdinger => Player.Instance;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnComplete;
        }

        private static void Loading_OnComplete(EventArgs args)
        {
            if (Heimerdinger.Hero != Champion.Heimerdinger) return;

            Chat.Print("[Addon] [Champion] [HeimerDingo]", System.Drawing.Color.Gray);
            Menus.InMenu();
            InSpells();
            Game.OnTick += Game_On;
            Drawing.OnDraw += OnDraw;
            Gapcloser.OnGapcloser += Gap_Closer;
            Interrupter.OnInterruptableSpell += Inte_On;

        }

        private static void Inte_On(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void Gap_Closer(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            throw new NotImplementedException();
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
        }
        private static void Game_On(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                ByCombo();
                ByQr();
                ByWr();
                ByEr();
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
            Killteal();
        }

        private static void ByEr()
        {
            var targete = TargetSelector.GetTarget(E2.Range, DamageType.Magical);
            if (comb["Re"].Cast<CheckBox>().CurrentValue)
            {
                if (E.IsReady() && R.IsReady())
                {
                    if (targete.IsValidTarget(E2.Range))
                    {
                        R.Cast();
                        E2.Cast(targete);
                    }
                }
            }
        }
        private static void ByWr()
        {
            var targetw = TargetSelector.GetTarget(W2.Range, DamageType.Magical);
            if (comb["Rw"].Cast<CheckBox>().CurrentValue)
            {
                if (W2.IsReady() && R.IsReady())
                {
                    if (targetw.IsValidTarget(W2.Range))
                    {
                        R.Cast();
                        W2.Cast(targetw);
                    }
                }
            }
        }
        private static void ByQr()
        {
            var targetq = TargetSelector.GetTarget(Q2.Range, DamageType.Magical);
            if (comb["Cr"].Cast<CheckBox>().CurrentValue)
            {
                if (comb["Rq"].Cast<CheckBox>().CurrentValue)
                {
                    if (Q2.IsReady() && R.IsReady())
                    {
                        if (targetq.IsValidTarget(Q2.Range))
                        {
                            R.Cast();
                            Q2.Cast(targetq);
                        }
                    }
                }
            }
        }
        private static void ByCombo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (comb["Cq"].Cast<CheckBox>().CurrentValue)
            {
                if (Q.IsReady())
                {
                    if (target.IsValidTarget(Q.Range))
                    {
                        Q.Cast(target);
                    }
                }
                target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                if (comb["Cw"].Cast<CheckBox>().CurrentValue)
                {
                    if (W.IsReady())
                    {
                        var predw = W.GetPrediction(target);
                        if (target.IsValidTarget(W.Range) && predw.HitChancePercent >= pre["Wp"].Cast<Slider>().CurrentValue)
                        {
                            W.Cast(target);
                        }
                    }
                    target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                    if (comb["Ce"].Cast<CheckBox>().CurrentValue)
                    {
                        if (E.IsReady())
                        {
                            var prediction = E.GetPrediction(target);
                            if (target.IsValidTarget(E.Range) && prediction.HitChancePercent >= pre["Ep"].Cast<Slider>().CurrentValue)
                            {
                                E.Cast(prediction.CastPosition);
                            }
                        }
                    }
                }
            }
        }
        private static void ByLane()
        {
            if (Lane["Ql"].Cast<CheckBox>().CurrentValue && _Player.ManaPercent > Lane["ManaL"].Cast<Slider>().CurrentValue)
            {
                var minionsq = EntityManager.MinionsAndMonsters.EnemyMinions.Where(min => min.IsEnemy && !min.IsDead && min.IsValid && !min.IsInvulnerable && min.IsInRange(_Player.Position, Q.Range));
                foreach (var mayminoon in minionsq)
                {
                    if (Q.GetPrediction(mayminoon).CollisionObjects.Where(may => may.IsEnemy && !may.IsDead && may.IsValid && !may.IsInvulnerable).Count() >= 0)
                    {
                        Q.Cast(mayminoon);
                    }
                }
                if (Lane["Wl"].Cast<CheckBox>().CurrentValue && _Player.ManaPercent > Lane["ManaL"].Cast<Slider>().CurrentValue)
                {
                    var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(min => min.IsEnemy && !min.IsDead && min.IsValid && !min.IsInvulnerable && min.IsInRange(_Player.Position, W.Range));
                    foreach (var mayminoon in minions)
                    {
                        if (W.GetPrediction(mayminoon).CollisionObjects.Where(may => may.IsEnemy && !may.IsDead && may.IsValid && !may.IsInvulnerable).Count() >= 0)
                        {
                            W.Cast(mayminoon);
                        }
                    }
                    if (Lane["El"].Cast<CheckBox>().CurrentValue && _Player.ManaPercent > Lane["ManaL"].Cast<Slider>().CurrentValue)
                    {
                        var minionsE = EntityManager.MinionsAndMonsters.EnemyMinions.Where(min => min.IsEnemy && !min.IsDead && min.IsValid && !min.IsInvulnerable && min.IsInRange(_Player.Position, E.Range));
                        foreach (var mayminoon in minionsE)
                        {
                            if (E.GetPrediction(mayminoon).CollisionObjects.Where(may => may.IsEnemy && !may.IsDead && may.IsValid && !may.IsInvulnerable).Count() >= 0)
                            {
                                E.Cast(mayminoon);
                            }
                        }
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
            var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            if (Auto["AutoW"].Cast<CheckBox>().CurrentValue)
            {
                var prediction = W.GetPrediction(target);
                if (target.IsValidTarget(W.Range) && prediction.HitChancePercent >= pre["Wp"].Cast<Slider>().CurrentValue)
                {
                    if (Player.Instance.ManaPercent > Auto["Mana"].Cast<Slider>().CurrentValue)
                    {
                        W.Cast(target);
                    }
                }
                target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                if (Auto["AutoE"].Cast<CheckBox>().CurrentValue)
                {
                    var predictione = E.GetPrediction(target);
                    if (target.IsValidTarget(E.Range) && predictione.HitChancePercent >= pre["Ep"].Cast<Slider>().CurrentValue)
                    {
                        if (Player.Instance.ManaPercent > Auto["Mana"].Cast<Slider>().CurrentValue)
                        {
                            E.Cast(target);
                        }
                    }
                }
            }
        }
        private static void Killteal()
        {
            throw new NotImplementedException();
        }

        private static void InSpells()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 700, SkillShotType.Linear, 1000,500,150);
            W = new Spell.Skillshot(SpellSlot.W, 1325, SkillShotType.Linear, 625, 2200, 90);
            E = new Spell.Skillshot(SpellSlot.E, 970, SkillShotType.Linear, 500, 210, 20);
            R = new Spell.Active(SpellSlot.R);
            //Uti
            Q2 = new Spell.Skillshot(SpellSlot.Q, 450, SkillShotType.Linear);
            W2 = new Spell.Skillshot(SpellSlot.W, 1100, SkillShotType.Linear);
            E2 = new Spell.Skillshot(SpellSlot.E, 925, SkillShotType.Linear);

        }
    }
}
