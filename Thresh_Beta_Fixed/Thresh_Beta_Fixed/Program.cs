using static Thresh_Beta_Fixed.Menus;
using static Thresh_Beta_Fixed.Spells;
using static Thresh_Beta_Fixed.MyChampion;
using EloBuddy;
using EloBuddy.SDK.Events;
using System;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;
using System.Linq;
using System.Collections.Generic;
using SharpDX;

namespace Thresh_Beta_Fixed
{
    class Program
    {
        public static List<AIHeroClient> Enemies = new List<AIHeroClient>(), Allies = new List<AIHeroClient>();
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Thresh_Loading;
        }

        private static void Thresh_Loading(EventArgs args)
        {
            if (MyChampion.MyThresh.Hero != Champion.Thresh) return;
            Chat.Print("[Addon] [Champion] [Thresh]", System.Drawing.Color.ForestGreen);
            Spells.Execute();
            Menus.Execute();
            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += On_Draws;
            Interrupter.OnInterruptableSpell += OnInterrupter;
            Gapcloser.OnGapcloser += OnGapcloser;
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.Equals(Orbwalker.ActiveModes.Combo))
            {
                byCombo();
            }
            AutoHarass();
            AutosHild();
        }

        private static void AutosHild()
        {
            throw new NotImplementedException();
        }

        private static void byCombo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (!target.IsValidTarget(Q.Range) || target == null)
            {
                return;
            }
            if (Combo["Qc"].Cast<CheckBox>().CurrentValue)
            {
                if (Q.IsReady() && Q.CanCast(target))
                {
                    var prediction = Q.GetPrediction(target);
                    if (target.IsValidTarget(Q.Range) && prediction.HitChancePercent >= Pre["pre"].Cast<Slider>().CurrentValue)
                    {
                        Q.Cast(prediction.CastPosition);
                    }
                }
            }
            if (Combo["Wc"].Cast<CheckBox>().CurrentValue)
            {
                if (W.IsReady())
                {
                    foreach (var ally in Allies.Where(ally => ally.IsValid && !ally.IsDead && _Player.Distance(ally.ServerPosition) < W.Range + 500))
                    {
                        if (Enemys.Distance(ally.ServerPosition) > 800 && _Player.Distance(ally.ServerPosition) > 600)
                        {
                            W.Cast(ally);
                        }
                    }
                }
                if (Combo["Ec"].Cast<CheckBox>().CurrentValue)
                {
                    if (E.IsReady())
                    {
                        foreach (var ally in Allies.Where(ally => ally.IsValid && !ally.IsDead && _Player.Distance(ally.ServerPosition) < E.Range + 500))
                        {
                            if (Enemys.Distance(ally.ServerPosition) > 800 && _Player.Distance(ally.ServerPosition) > 600)
                            {
                                E.Cast(E.GetPrediction(ally).CastPosition);
                            }
                        }
                    }

                    //ee
                    if (Menus.Combo["Wc"].Cast<CheckBox>().CurrentValue && Spells.W.IsReady() &&
                            (!Menus.Combo["Ec"].Cast<CheckBox>().CurrentValue || !Spells.E.IsReady()))
                    {
                        var targetby = TargetSelector.GetTarget(Spells.E.Range, DamageType.Mixed);
                        if (target == null)
                            Casts.PushE.CastE(target);
                    }
                }
            }
        }
        private static void AutoHarass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (Auto["Qhara"].Cast<CheckBox>().CurrentValue)
            {
                var prediction = Q.GetPrediction(target);
                if (target.IsValidTarget(Q.Range) && prediction.HitChancePercent >= Pre["preQ"].Cast<Slider>().CurrentValue)
                {
                    if (Player.Instance.ManaPercent > Auto["mana"].Cast<Slider>().CurrentValue)
                    {
                        Q.Cast(target);
                    }
                }
            }
        }
        private static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!sender.IsValidTarget()) return;
            if (Misc["Gap"].Cast<CheckBox>().CurrentValue)
            {
                Q.Cast(sender);
            }
            if (Misc["Gap"].Cast<CheckBox>().CurrentValue)
            {
                E.Cast(sender);
            }
        }
        private static void OnInterrupter(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (!sender.IsValidTarget()) return;
            if (Misc["Int"].Cast<CheckBox>().CurrentValue)
            {
                E.Cast(sender);
            }
        }
        public static AIHeroClient W_Ally(AIHeroClient target)
        {
            return target != null ? EntityManager.Heroes.Allies.Where(h => h.IsValidTarget() && !h.IsMe && !h.IsInAutoAttackRange(target) && target.Distance(MyChampion.MyThresh, true) < target.Distance(h, true)).OrderByDescending(h => h.GetPriority() / h.HealthPercent).FirstOrDefault() : null;
        }
        private static void On_Draws(EventArgs args)
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