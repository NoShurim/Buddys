using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urgot_Beta_Fixed
{
    class Champion
    {
        public static Spell.Skillshot Q;
        public static Spell.Active W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;
        public static Spell.Active R2;

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }

        }
        public static Menu Ur, pre, Combo, Auto, Lane, Jungle, UtiR, Draws, AutoShild;

        private static AIHeroClient Urgot => Player.Instance;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_urgot;
        }

        private static void Loading_urgot(EventArgs args)
        {
            if (Urgot.Hero != EloBuddy.Champion.Urgot) return;
            SpellLoad();
            MenuLoading();
            Comands();
        }

        private static void Comands()
        {
            Drawing.OnDraw += OnsDraws;
            Game.OnTick += Game_OnTick;
        }

        private static void OnsDraws(EventArgs args)
        {
            if (Draws["DQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                Q.DrawRange(System.Drawing.Color.LawnGreen);
            }
            if (Draws["DE"].Cast<CheckBox>().CurrentValue && W.IsReady())
            {
                W.DrawRange(System.Drawing.Color.LawnGreen);
            }
            if (Draws["DW"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                E.DrawRange(System.Drawing.Color.LawnGreen);
            }
            if (Draws["DR"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                R.DrawRange(System.Drawing.Color.LawnGreen);
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
            AutoHara();
            Shild();
            AutoR();
            AutoR2();
            Killteal();
        }

        private static void Shild()
        {
            if (AutoShild["Wauto"].Cast<CheckBox>().CurrentValue)
            {
                var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                if (target.IsValidTarget(AutoShild["Ran"].Cast<Slider>().CurrentValue) && W.IsReady() && Player.Instance.HealthPercent < Auto["Life"].Cast<Slider>().CurrentValue)
                {
                    W.Cast();
                }
            }
        }
        private static void AutoR2()
        {
            if (UtiR["Rmode"].Cast<ComboBox>().CurrentValue == 1)
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (UtiR["useR"].Cast<CheckBox>().CurrentValue)
                {
                    if (R.IsReady())
                    {
                        var prediction = R.GetPrediction(target);
                        if (target.IsValidTarget(R.Range) && prediction.HitChancePercent >= pre["Rp"].Cast<Slider>().CurrentValue && target.HealthPercent < 25)
                        {
                            R.Cast(prediction.CastPosition);
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
            if (Combo["Ec"].Cast<CheckBox>().CurrentValue)
            {
                if (E.IsReady() && E.CanCast(target))
                {
                    var prediction = E.GetPrediction(target);
                    if (target.IsValidTarget(E.Range) && prediction.HitChancePercent >= pre["Ep"].Cast<Slider>().CurrentValue)
                    {
                        E.Cast(prediction.CastPosition);
                    }
                }
            }
            if (Combo["Qc"].Cast<CheckBox>().CurrentValue)
            {
                if (Q.IsReady() && Q.CanCast(target))
                {
                    var prediction = Q.GetPrediction(target);
                    if (target.IsValidTarget(Q.Range) && prediction.HitChancePercent >= pre["Qp"].Cast<Slider>().CurrentValue)
                    {
                        Q.Cast(prediction.CastPosition);
                    }
                }
            }
            if (Combo["Wc"].Cast<CheckBox>().CurrentValue)
            {
                if (W.IsReady() && W.CanCast(target))
                {
                    if (target.IsValidTarget(W.Range))
                    {
                        W.Cast(target);
                        return;
                    }
                }
            }
        }
        private static void ByLane()
        {
            if (Lane["Lq"].Cast<CheckBox>().CurrentValue && _Player.ManaPercent > Lane["Mana"].Cast<Slider>().CurrentValue)
            {
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(min => min.IsEnemy && !min.IsDead && min.IsValid && !min.IsInvulnerable && min.IsInRange(_Player.Position, Q.Range));
                foreach (var mayminoon in minions)
                {
                    if (Q.GetPrediction(mayminoon).CollisionObjects.Where(may => may.IsEnemy && !may.IsDead && may.IsValid && !may.IsInvulnerable).Count() >= Lane["Mi"].Cast<Slider>().CurrentValue)
                    {
                        Q.Cast(mayminoon);
                    }
                }
            }
        }
        private static void ByJungle()
        {
            if (Jungle["Qj"].Cast<CheckBox>().CurrentValue && Q.IsReady() && Player.Instance.ManaPercent > Jungle["manaJ"].Cast<Slider>().CurrentValue)
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
                if (target.IsValidTarget(Q.Range) && prediction.HitChancePercent >= pre["Qp"].Cast<Slider>().CurrentValue)
                {
                    if (Player.Instance.ManaPercent > Auto["manaQ"].Cast<Slider>().CurrentValue)
                    {
                        Q.Cast(target);
                    }
                }
            }
        }
        private static void AutoR()
        {
            if (UtiR["Rmode"].Cast<ComboBox>().CurrentValue == 0)
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (UtiR["useR"].Cast<CheckBox>().CurrentValue)
                {
                    var prediction = R.GetPrediction(target);
                    if (prediction.HitChancePercent >= pre["Rp"].Cast<Slider>().CurrentValue && target.Health < UrDamage.Rmage(target))
                    {
                        if (target.IsValidTarget(R.Range) && R.IsReady())
                        {
                            R.Cast(prediction.CastPosition);
                        }
                    }
                }
            }
        }
        private static void Killteal()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (Q.IsReady() && Q.CanCast(target))
            {
                var prediction = Q.GetPrediction(target);
                if (target.IsValidTarget(Q.Range) && prediction.HitChancePercent >= pre["Qp"].Cast<Slider>().CurrentValue && target.Health < UrDamage.Qmage(target))
                {
                    Q.Cast(prediction.CastPosition);
                }
            }
        }
        private static void MenuLoading()
        {
            Ur = MainMenu.AddMenu("Urgot", "Urgot");
            pre = Ur.AddSubMenu("Prediction");
            pre.AddSeparator();
            pre.AddLabel("Prediction [Q]");
            pre.Add("Qp", new Slider("Prediction [Q]", 75, 1));
            pre.AddSeparator();
            pre.AddLabel("Prediction [E]");
            pre.AddSeparator();
            pre.Add("Ep", new Slider("Prediction [E]", 50, 1));
            pre.AddSeparator();
            pre.AddLabel("Prediction [R]");
            pre.Add("Rp", new Slider("Prediction [R]", 85, 1));
            //Combo
            Combo = Ur.AddSubMenu("Combo");
            Combo.Add("Qc", new CheckBox("Use [Q]"));
            Combo.Add("Wc", new CheckBox("Use [W]"));
            Combo.Add("Ec", new CheckBox("Use [E]"));
            //Auto
            Auto = Ur.AddSubMenu("AutoHarass");
            Auto.Add("AutoQ", new CheckBox("Use [Q] AutoHarass"));
            Auto.AddSeparator();
            Auto.AddLabel("Mana Percent");
            Auto.Add("manaQ", new Slider("Mana Percent", 65, 1));
            //Lane
            Lane = Ur.AddSubMenu("Lane");
            Lane.Add("Lq", new CheckBox("Use [Q] Lane"));
            Lane.Add("Lw", new CheckBox("Use [W] Lane", false));
            Lane.AddSeparator();
            Lane.AddLabel("Settings LaneClear");
            Lane.AddLabel("Minions");
            Lane.Add("Mi", new Slider("Minions Percent", 3, 1, 6));
            Lane.AddSeparator();
            Lane.AddLabel("Mana Percent");
            Lane.Add("Mana", new Slider("Mana Percent", 50, 1));
            //Jungle
            Jungle = Ur.AddSubMenu("Jungle");
            Jungle.Add("Qj", new CheckBox("Use [Q] Jungle"));
            Jungle.Add("Wj", new CheckBox("Use [W] Jungle"));
            Jungle.AddSeparator();
            Jungle.AddLabel("Mana Percent");
            Jungle.Add("manaJ", new Slider("Mana Percent", 50, 1));
            //Uti
            UtiR = Ur.AddSubMenu("Utimate [R]");
            UtiR.Add("Rmode", new ComboBox("Mode [R]", 1, "Damager [R]", "Beta [R] Life"));
            UtiR.AddSeparator();
            UtiR.AddLabel("Settings [Beta [R] Life]");
            UtiR.Add("useR", new CheckBox("Use [R]"));
            UtiR.Add("per", new Slider("Percent Life Enemy", 25, 1));
            //Draws
            Draws = Ur.AddSubMenu("Draws");
            Draws.Add("DQ", new CheckBox("Use [Q] Draw"));
            Draws.Add("DW", new CheckBox("Use [W] Draw"));
            Draws.Add("DE", new CheckBox("Use [E] Draw"));
            Draws.Add("DR", new CheckBox("Use [R] Draw"));
            //Shild
            AutoShild = Ur.AddSubMenu("AutoShild");
            AutoShild.Add("Wauto", new CheckBox("Use [W] AutoShild"));
            AutoShild.AddSeparator();
            AutoShild.AddLabel("Percent Life");
            AutoShild.Add("Life", new Slider("Percent Life", 35, 1));
            AutoShild.AddSeparator();
            AutoShild.AddLabel("Range");
            AutoShild.Add("Ran", new Slider("Percent Range", 300, 1, 500));
        }

        private static void SpellLoad()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 800, SkillShotType.Circular, 600, 2000, 70);
            W = new Spell.Active(SpellSlot.W, 500);
            E = new Spell.Skillshot(SpellSlot.E, 425, SkillShotType.Linear, 200, null, 100);
            R = new Spell.Skillshot(SpellSlot.R, 1525, SkillShotType.Linear, 700, 250, 100);
            R2 = new Spell.Active(SpellSlot.R, 1525);

        }
    }
}
