using EloBuddy;
using static Brand_Beta_Fixed.SpellsManager;
using static Brand_Beta_Fixed.Utils;
using static Brand_Beta_Fixed.Logics;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System;
using EloBuddy.SDK;
using System.Linq;

namespace Brand_Beta_Fixed
{
    public static class Fire
    {
        //Efects Randius
        public const int LoigcRanges = 300;
        public const int LogicRange = 600;
        //Buffs
        public static bool ShouldCast(bool allowAutos = true)
        {
            return !Player.Instance.Spellbook.IsCastingSpell || (!allowAutos || (Player.Instance.Spellbook.IsAutoAttacking && Orbwalker.CanBeAborted));
        }

        public static bool IsBlazed(this Obj_AI_Base target)
        {
            return target.HasBuff("BrandAblaze");
        }
        public static Obj_AI_Minion RLogic;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }

        }
        public static Menu Br, Pre, Comb, Hara, Lane, Jungle, Misc, Draws, FullCombo, KillSteal;
        private static AIHeroClient Brand => Player.Instance;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += LoaderComplete_Om;
        }

        private static void LoaderComplete_Om(EventArgs args)
        {
            if (Brand.Hero != Champion.Brand) return;
            Chat.Print("[Addon] [Champion] [Brnad]", System.Drawing.Color.Red);

            Menus();
            SpellsManager.Execute();
            Logics.Execute();
            //
            Drawing.OnDraw += On_Draws;
            Interrupter.OnInterruptableSpell += OnInterrupter;
            Gapcloser.OnGapcloser += OnGapcloser;
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

            const int range = 1100;
            const float aoeratio = 0.2f;

            var enemies = EntityManager.Heroes.Enemies.Where(n => n.IsValidTarget(range));
            var selectedTarget = TargetSelector.GetTarget(range, DamageType.Magical);
            var allTargets = new[] { selectedTarget }.Concat(enemies.Where(n => n.Index != selectedTarget.Index).OrderByDescending(n => Logics.TotalDamage(SpellSlot.Q, n) / n.Health)).Where(n => n.IsValidTarget() && !n.WillDie());

            if (selectedTarget == null && !enemies.Any())
            {
                return;
            }

            var useQ = Comb["Qc"].Cast<CheckBox>().CurrentValue &&
                       Player.CanUseSpell(SpellSlot.Q) == SpellState.Ready;
            var useW = Comb["Wc"].Cast<CheckBox>().CurrentValue &&
                       Player.CanUseSpell(SpellSlot.W) == SpellState.Ready;
            var useE = Comb["Ec"].Cast<CheckBox>().CurrentValue &&
                       Player.CanUseSpell(SpellSlot.E) == SpellState.Ready;
            var useR = Comb["Rc"].Cast<CheckBox>().CurrentValue &&
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
                    var enemyE =
                        enemies.Where(n => n.IsValidTarget(E.Range) && n.IsBlazed())
                            .OrderByDescending(n => n.CountEnemiesInRange(LoigcRanges))
                            .FirstOrDefault();

                    if (enemyE != null && enemyE.CountEnemiesInRange(LoigcRanges) > 1)
                    {
                        E.Cast(enemyE);
                        return;
                    }
                }
            }

            if (useE)
            {
                var targets = allTargets.Where(n => Player.Instance.IsInRange(n, E.Range));

                foreach (var target in targets)
                {
                    E.Cast(target);
                    return;
                }
            }

            if (useW)
            {
                foreach (var target in allTargets)
                {
                    if (W.Cast(target))
                    {
                        return;
                    }
                }
            }

            if (useQ)
            {
                foreach (var target in allTargets.Where(n => n.IsBlazed()))
                {
                    if (Q.Cast(target))
                    {
                        return;
                    }
                }
            }

            if (useR)
            {
                var bestTarget =
                    allTargets.Where(n => Player.Instance.IsInRange(n, R.Range))
                        .OrderByDescending(n => n.CountEnemiesInRange(LogicRange))
                        .FirstOrDefault();

                if (bestTarget != null &&
                    bestTarget.CountEnemiesInRange(LogicRange) >=
                    Comb["En"].Cast<Slider>().CurrentValue)
                {
                    if ((!useW && !useE || !bestTarget.IsBlazed()))
                    {
                        R.Cast(bestTarget);
                    }
                }
            }
        }

        private static void ByLane()
        {
            var useW = Lane["Wl"].Cast<CheckBox>().CurrentValue && Player.Instance.ManaPercent >= Lane["manal"].Cast<Slider>().CurrentValue && Player.CanUseSpell(SpellSlot.W) == SpellState.Ready;
            var useE = Lane["El"].Cast<CheckBox>().CurrentValue && Player.Instance.ManaPercent >= Lane["manal"].Cast<Slider>().CurrentValue && Player.CanUseSpell(SpellSlot.E) == SpellState.Ready;

            if (useW)
            {
                var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, 1500, false);
                var predict = Prediction.Position.PredictCircularMissileAoe(minions.Cast<Obj_AI_Base>().ToArray(), W.Range, W.Radius, W.CastDelay, W.Speed).OrderByDescending(r => r.GetCollisionObjects<Obj_AI_Minion>().Length).FirstOrDefault();

                if (predict != null && predict.CollisionObjects.Length >= Lane["Wmin"].Cast<Slider>().CurrentValue)
                {
                    W.Cast(predict.CastPosition);
                    return;
                }
            }

            if (useE)
            {
                var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, E.Range + 20, false).Where(m => m.IsBlazed());

                var group = GroupObjects(minions, LoigcRanges).FirstOrDefault();

                if (group != null && group.Item1.Length >= Lane["Wmin"].Cast<Slider>().CurrentValue)
                {
                    var minion = group.Item1.OrderByDescending(m => m.Distance(group.Item2, true)).Last();

                    E.Cast(minion);
                }
            }
        }

        private static void ByJungle()
        {
            if (Jungle["Wj"].Cast<CheckBox>().CurrentValue && W.IsReady() && Player.Instance.ManaPercent > Jungle["manaj"].Cast<Slider>().CurrentValue)
            {
                var miniQ = EntityManager.MinionsAndMonsters.GetJungleMonsters(_Player.Position, W.Range).Where(my => !my.IsDead && my.IsValid && !my.IsInvulnerable);
                if (miniQ.Count() > 0)
                {
                    W.Cast(miniQ.First());
                }
            }
            if (Jungle["Ej"].Cast<CheckBox>().CurrentValue && E.IsReady() && Player.Instance.ManaPercent > Jungle["manaj"].Cast<Slider>().CurrentValue)
            {
                var miniE = EntityManager.MinionsAndMonsters.GetJungleMonsters(_Player.Position, E.Range).Where(my => !my.IsDead && my.IsValid && !my.IsInvulnerable);
                if (miniE.Count() > 0)
                {
                    W.Cast(miniE.First());
                }
            }
        }

        private static void AutoHara()
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            if (Hara["AutoW"].Cast<CheckBox>().CurrentValue)
            {
                var prediction = W.GetPrediction(target);
                if (target.IsValidTarget(W.Range) && prediction.HitChancePercent >= Pre["Wp"].Cast<Slider>().CurrentValue)
                {
                    if (Player.Instance.ManaPercent > Hara["Mana"].Cast<Slider>().CurrentValue)
                    {
                        W.Cast(target);
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
            const int range = 1100;
            const float aoeratio = 0.2f;

            var enemies = EntityManager.Heroes.Enemies.Where(n => n.IsValidTarget(range));
            var selectedTarget = TargetSelector.GetTarget(range, DamageType.Magical);
            var allTargets = new[] { selectedTarget }.Concat(enemies.Where(n => n.Index != selectedTarget.Index).OrderByDescending(n => Logics.TotalDamage(SpellSlot.Q, n) / n.Health)).Where(n => n.IsValidTarget() && !n.WillDie());
            

            var ksQ = KillSteal["ksQ"].Cast<CheckBox>().CurrentValue &&
                      Player.CanUseSpell(SpellSlot.Q) == SpellState.Ready;
            var ksW = KillSteal["ksW"].Cast<CheckBox>().CurrentValue &&
                      Player.CanUseSpell(SpellSlot.W) == SpellState.Ready;
            var ksE = KillSteal["ksE"].Cast<CheckBox>().CurrentValue &&
                      Player.CanUseSpell(SpellSlot.E) == SpellState.Ready;
            var ksR = KillSteal["ksR"].Cast<CheckBox>().CurrentValue &&
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

                    if (prediction.HitChancePercent >= Pre["Wp"].Cast<Slider>().CurrentValue)
                    {
                        W.Cast(prediction.CastPosition);
                        return;
                    }
                }

                if (ksQ && Player.Instance.IsInRange(enemy, Q.Range) && enemy.Killable(SpellSlot.Q))
                {
                    var prediction = Q.GetPrediction(enemy);

                    if (prediction.HitChancePercent >= Pre["Qp"].Cast<Slider>().CurrentValue)
                    {
                        Q.Cast(prediction.CastPosition);
                        return;
                    }
                }

                if (ksR && Player.Instance.IsInRange(enemy, R.Range) && enemy.Killable(SpellSlot.R))
                {
                    R.Cast(enemy);
                    return;
                }
            }
        }

            private static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!sender.IsValidTarget()) return;
            if (Misc["Gap"].Cast<CheckBox>().CurrentValue)
            {
                E.Cast(sender);
            }
            if (Misc["Gap"].Cast<CheckBox>().CurrentValue)
            {
                Q.Cast(sender);
            }
        }
        private static void OnInterrupter(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (!sender.IsValidTarget()) return;
            if (Misc["Int"].Cast<CheckBox>().CurrentValue)
            {
                E.Cast(sender);
            }
            if (Misc["Int"].Cast<CheckBox>().CurrentValue)
            {
                Q.Cast(sender);
            }
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

        private static void Menus()
        {
            Br = MainMenu.AddMenu("Brand", "Brand");
            //
            Pre = Br.AddSubMenu("Prediction");
            Pre.AddLabel("Predictions");
            Pre.Add("Qp", new Slider("Prediction [Q]", 75, 1));
            Pre.Add("Wp", new Slider("Prediction [W]", 50, 1));
            //
            Comb = Br.AddSubMenu("Combo");
            Comb.Add("Qc", new CheckBox("Use [Q]"));
            Comb.Add("Wc", new CheckBox("Use [W]"));
            Comb.Add("Ec", new CheckBox("Use [E]"));
            Comb.AddSeparator();
            Comb.AddLabel("Settings [R] Combo");
            Comb.Add("Rc", new CheckBox("Use [R]"));
            Comb.Add("En", new Slider("Max Range Radiun Enemys > %", 2, 1, 5));
            Comb.Add("stack", new CheckBox("Use Stack Passive", false));
            //
            Hara = Br.AddSubMenu("AutoHarass");
            Hara.Add("AutoW", new CheckBox("Use Auto[W]"));
            Hara.AddSeparator();
            Hara.AddLabel("Mana Percent");
            Hara.Add("Mana", new Slider("Mana Percent Auto [W] > %", 50, 1));
            //
            FullCombo = Br.AddSubMenu("FullCombo");
            FullCombo.Add("Eb", new CheckBox("Use FullCombo"));
            //
            Lane = Br.AddSubMenu("LaneClear");
            Lane.Add("Ql", new CheckBox("Use [Q] Lane", false));
            Lane.Add("Wl", new CheckBox("Use [W] Lane"));
            Lane.Add("El", new CheckBox("Use [E] Lane"));
            Lane.AddSeparator();
            Lane.AddLabel("Mana Percent");
            Lane.Add("manal", new Slider("Mana Percent > %", 50, 1));
            Lane.AddSeparator();
            Lane.AddLabel("Minion Percent");
            Lane.Add("Wmin", new Slider("Minion Percent > %", 3, 1, 6));
            //
            Jungle = Br.AddSubMenu("JungleClear");
            Jungle.Add("Qj", new CheckBox("Use [Q]"));
            Jungle.Add("Wj", new CheckBox("Use [W]"));
            Jungle.Add("Ej", new CheckBox("Use [E]"));
            Jungle.AddSeparator();
            Jungle.AddLabel("Mana Percent");
            Jungle.Add("manaj", new Slider("Mana Percent > %", 50, 1));
            //
            Misc = Br.AddSubMenu("Misc");
            Misc.Add("In", new CheckBox("Use Interrupt"));
            Misc.Add("Gap", new CheckBox("Use GapClose"));
            //
            KillSteal = Br.AddSubMenu("KillSteal");
            KillSteal.Add("KsQ", new CheckBox("Use [Q] KS"));
            KillSteal.Add("KsW", new CheckBox("Use [W] KS"));
            KillSteal.Add("KsE", new CheckBox("Use [E] KS"));
            KillSteal.Add("KsR", new CheckBox("Use [R] KS"));
            //
            Draws = Br.AddSubMenu("Draws");
            Draws.Add("DQ", new CheckBox("Use Draw [Q]"));
            Draws.Add("DW", new CheckBox("Use Draw [W]"));
            Draws.Add("DE", new CheckBox("Use Draw [E]"));
            Draws.Add("DR", new CheckBox("Use Draw [R]"));
        }
    }
}