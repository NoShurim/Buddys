using System;
using EloBuddy;
using EloBuddy.SDK.Events;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using System.Linq;
using EloBuddy.SDK.Enumerations;
using Microsoft.SqlServer.Server;

namespace Kayn
{
    class Program
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot Q2;
        public static Spell.Skillshot W;
        public static Spell.Skillshot W2;
        public static Spell.Active E;
        public static Spell.Targeted R;
        public static Spell.Skillshot R2;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }

        }
        public static Menu Kmenu, Combo, Hara, Lane, Jungle, Misc, Evade, Pre;

        public static float QDamage(Obj_AI_Base target)
        {
            if (!Player.GetSpell(SpellSlot.Q).IsLearned) return 0;
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)new double[] { 55, 75, 95, 115, 135 }[Q.Level - 1] + 1 * Player.Instance.FlatMagicDamageMod);
        }
        public static float WDamage(Obj_AI_Base target)
        {
            if (!Player.GetSpell(SpellSlot.W).IsLearned) return 0;
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)new double[] { 170, 215, 260 }[W.Level - 1] + 1 * Player.Instance.FlatMagicDamageMod);
        }
        public static float RDamage(Obj_AI_Base target)
        {
            if (!Player.GetSpell(SpellSlot.R).IsLearned) return 0;
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)new double[] { 150, 250, 350 }[R.Level - 1] + 1 * Player.Instance.FlatMagicDamageMod);
        }

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_On;
        }

        private static void Loading_On(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Kayn") { return; }

            Chat.Print("[Addon]", System.Drawing.Color.LightBlue);
            Chat.Print("[Champion]", System.Drawing.Color.Red, "[Kayn]", System.Drawing.Color.Blue);

            Q = new Spell.Skillshot(SpellSlot.Q, 250, EloBuddy.SDK.Enumerations.SkillShotType.Linear, 150, 75, 37);
            Q2 = new Spell.Skillshot(SpellSlot.Q, 375, SkillShotType.Linear, 150, 75, 50);
            W = new Spell.Skillshot(SpellSlot.W, 700, EloBuddy.SDK.Enumerations.SkillShotType.Linear, 350, 175, 87);
            W2 = new Spell.Skillshot(SpellSlot.W, 800, SkillShotType.Linear, 400, 200, 100);
            E = new Spell.Active(SpellSlot.E);
            R = new Spell.Targeted(SpellSlot.R, 475);
            R2 = new Spell.Skillshot(SpellSlot.R, 150, EloBuddy.SDK.Enumerations.SkillShotType.Linear, 75, 37, 18);

            Kmenu = MainMenu.AddMenu("Kayn", "Kayn");

            Pre = Kmenu.AddSubMenu("Prediction");
            Pre.AddLabel("Settings Kayn");
            Pre.Add("Pq", new Slider("Prediction [Q]", 65, 1, 100));
            Pre.Add("Pw", new Slider("Prediction [W]", 70, 1, 100));
            Pre.AddLabel("Settings Shodown Asasin");
            Pre.Add("Qq", new Slider("Prediction [Q]", 65, 1, 100));
            Pre.Add("Ww", new Slider("Prediction [W]", 70, 1, 100));
            Pre.AddLabel("Settings Rhaast");
            Pre.Add("r2", new Slider("Minimum of life to use the utimate", 30, 1, 100)); 

            Combo = Kmenu.AddSubMenu("Combo");
            Combo.Add("Qk", new CheckBox("[Use Q]"));
            Combo.Add("Wk", new CheckBox("[Use W]"));
            Combo.Add("Ek", new CheckBox("[Use E]", false));
            Combo.AddLabel("The Ability E Still in test is not working yet");
            Combo.Add("Rk", new CheckBox("[Use R]"));

            Hara = Kmenu.AddSubMenu("Harass");
            Hara.AddLabel("Harass Automatic");
            Hara.Add("Wh", new CheckBox("[Use W]"));
            Hara.Add("mW", new Slider("Mana W", 50, 1, 100));

            Lane = Kmenu.AddSubMenu("Lane");
            Lane.Add("Ql", new CheckBox("[Use Q]"));
            Lane.Add("Wl", new CheckBox("[Use W]"));
            Lane.AddLabel("Setting Lane");
            Lane.Add("Mi", new Slider("Mana", 50, 1, 100));

            Jungle = Kmenu.AddSubMenu("Jungle");
            Jungle.Add("Qj", new CheckBox("[Use Q]"));
            Jungle.Add("Wj", new CheckBox("[Use W]"));
            Jungle.AddLabel("Setting Lane");
            Jungle.Add("Ma", new Slider("Mana", 50, 1, 100));

            Misc = Kmenu.AddSubMenu("Misc");
            Misc.Add("KS", new CheckBox("Use R KillSteal"));
            Misc.Add("End", new CheckBox("Always use R to finish"));
            Misc.Add("Inter", new CheckBox("Interrupter"));
            Misc.Add("Gap", new CheckBox("GapCloser"));
            Misc.Add("AAR", new CheckBox("Reset AA+R"));
            Misc.AddLabel("Settings GapCloser");
            Misc.Add("sG", new Slider("Mini Mana Gap", 70, 1, 100));

            Evade = Kmenu.AddSubMenu("Evade");
            Evade.Add("ER", new CheckBox("Enabled Evade"));
            Evade.AddLabel("Evade In Faze Beta");
        
            Drawing.OnDraw += Draws_Load;
            Game.OnTick += Game_OnTick;
            Interrupter.OnInterruptableSpell += Interrupter_Spell;
            AIHeroClient.OnProcessSpellCast += EvadeBeta;
            Gapcloser.OnGapcloser += OnGapcloser;
            Orbwalker.OnAttack += Atack;
            Orbwalker.OnPreMove += PreMovieR;

        }

        private static void PreMovieR(EventArgs args)
        {
            var rTarget = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (rTarget != null && rTarget.Health < ObjectManager.Player.GetAutoAttackDamage(rTarget, true)
                  * Misc["AAR"].Cast<Slider>().CurrentValue) return;
            if (rTarget.IsValidTarget(R.Range) && !ObjectManager.Player.HasBuff("KaynDash"))
            {
                R.Cast();
            }
        }

        private static void Atack(AttackableUnit target, EventArgs args)
        {
            var rTarget = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (rTarget != null && rTarget.Health < ObjectManager.Player.GetAutoAttackDamage(rTarget, true)
                * Misc["AAR"].Cast<Slider>().CurrentValue) return;
            if (rTarget.IsValidTarget(R.Range) && !ObjectManager.Player.HasBuff("KaynDash"))
            { 
              R.Cast();
            }
        }

        private static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if(_Player.IsDead || sender.IsMe || sender.IsAlly) return;

            if (Misc["Gap"].Cast<CheckBox>().CurrentValue && !R.IsReady() && !sender.IsValidTarget(R.Range) && Player.Instance.ManaPercent >= Hara["sG"].Cast<Slider>().CurrentValue) return;
            {
                R.Cast(sender);
            }
            if (Misc["Gap"].Cast<CheckBox>().CurrentValue && !E.IsReady() && Player.Instance.ManaPercent >= Hara["sG"].Cast<Slider>().CurrentValue) return;
            {
                E.Cast(sender);
            }

        }

        private static void EvadeBeta(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (_Player.IsDead || !(sender is AIHeroClient)) return;

            var EOMenu = Misc["ER"].Cast<CheckBox>().CurrentValue && Q.IsReady() && R.IsReady();
            {

                if (sender.IsValidTarget() && sender.IsEnemy && Q.IsReady() && R.IsReady() && _Player.Distance(sender) <= args.SData.CastRange)
                {
                    if (Q.IsReady() && R.IsReady())
                    {
                        var Target = TargetSelector.GetTarget(250, DamageType.Physical);
                        Core.DelayAction(delegate
                    {
                        if (Target != null && Target.IsValidTarget(Q.Range)) Q.Cast(Target);
                    }, (int)args.SData.SpellCastTime - Game.Ping - 100);

                        Core.DelayAction(delegate
                        {
                            if (sender.IsValidTarget(Q.Range)) Q.Cast(sender);
                        }, (int)args.SData.SpellCastTime - Game.Ping - 50);

                        return;
                    }

                    else if (R.IsReady() && _Player.IsFacing(sender) && ((args.Target != null && args.Target.IsMe) || _Player.Position.To2D().Distance(args.Start.To2D(), args.End.To2D(), true, true) < args.SData.LineWidth * args.SData.LineWidth || args.End.Distance(_Player) < args.SData.CastRadius))
                    {
                        var Target = TargetSelector.GetTarget(700, DamageType.Physical);
                        int delay = (int)(_Player.Distance(sender) / ((args.SData.MissileMaxSpeed + args.SData.MissileMinSpeed) / 2) * 1000) - 150 + (int)args.SData.SpellCastTime;

                        if (args.SData.Name != "ZedR" && args.SData.Name != "NocturneUnpeakableHorror")
                        {
                            Core.DelayAction(() => W.Cast(), delay);
                            if (Target != null) Core.DelayAction(() => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, Target), delay + 100);
                        }
                        return;
                    }
                }
            }
        }

    private static void Interrupter_Spell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            var wPred = W.GetPrediction(target);

            if (Misc["Inter"].Cast<CheckBox>().CurrentValue && W.IsReady() && W.GetPrediction(target).HitChance >= HitChance.High)
            {
                if (_Player.Distance(_Player.ServerPosition, true) <= W.Range && W.GetPrediction(target).HitChance >= HitChance.High)
                {
                    W.Cast(target);
                }
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            Haraaa();
            Missc();
            Rhaast();
            Assassin();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combos();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
            }
        }

        private static void Assassin()
        {
            if (Combo["Qk"].Cast<CheckBox>().CurrentValue)
            {
                var Target = TargetSelector.GetTarget(Q2.Range, DamageType.Physical);
                var Q2Pred = W2.GetPrediction(Target);
                if (Target.IsValidTarget(Q2.Range) && Q2.IsReady() && Q2Pred.HitChancePercent >= Pre["Qq"].Cast<Slider>().CurrentValue && W.IsReady())
                {
                    Q2.Cast(Q2Pred.CastPosition);
                }
            }
            if (Combo["Qk"].Cast<CheckBox>().CurrentValue)
            {
                var Target = TargetSelector.GetTarget(W2.Range, DamageType.Physical);
                var w2Pred = W2.GetPrediction(Target);
                if (Target.IsValidTarget(W.Range) && W.IsReady() && w2Pred.HitChancePercent >= Pre["Ww"].Cast<Slider>().CurrentValue && W.IsReady())
                {
                    W2.Cast(w2Pred.CastPosition);
                }
            }
        }

        private static void Rhaast()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (target.IsValidTarget(R.Range) && R.IsReady() && Player.Instance.Health >= 30)
            {
                R.Cast();
            }
        }

        private static void Missc()
        {
            if (Misc["KS"].Cast<CheckBox>().CurrentValue)
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                foreach (var enemy in
                     EntityManager.Heroes.Enemies.Where(
                         x => x.Distance(_Player) <= R.Range && x.IsValidTarget() && !x.IsInvulnerable && !x.IsZombie))
                    if (target.IsValidTarget(R.Range) && R.IsReady() && RDamage(enemy) >= enemy.Health)
                    {
                        R.Cast();
                    }
            }
            if (Misc["End"].Cast<CheckBox>().CurrentValue)
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                foreach (var enemy in
                     EntityManager.Heroes.Enemies.Where(
                         x => x.Distance(_Player) <= R.Range && x.IsValidTarget() && !x.IsInvulnerable && !x.IsZombie))
                    if (target.IsValidTarget(R.Range) && R.IsReady() && RDamage(enemy) >= enemy.Health)
                    {
                        R.Cast();
                    }
            }
        }
        private static void Haraaa()
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            var wPred = W.GetPrediction(target);
            if (target.IsValidTarget(W.Range) && W.IsReady() && wPred.HitChancePercent >= Pre["Pw"].Cast<Slider>().CurrentValue && W.IsReady() && Player.Instance.ManaPercent >= Hara["mW"].Cast<Slider>().CurrentValue)
            {
                W.Cast(wPred.CastPosition);
            }
        }

    private static void Combos()
        {
            if (Combo["Qk"].Cast<CheckBox>().CurrentValue)
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                var qPred = Q.GetPrediction(target);
                if (target.IsValidTarget(Q.Range) && Q.IsReady() && qPred.HitChancePercent >= Pre["Pq"].Cast<Slider>().CurrentValue)
                {
                    Q.Cast(qPred.CastPosition);
                }
            }
                if (Combo["Wk"].Cast<CheckBox>().CurrentValue)
                {
                var traget = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                    var wPred = W.GetPrediction(traget);
                        if (traget.IsValidTarget(W.Range) && W.IsReady() && wPred.HitChancePercent >= Pre["Pw"].Cast<Slider>().CurrentValue)
                {
                        W.Cast(wPred.CastPosition);
                    }
                }
                if (Combo["Rk"].Cast<CheckBox>().CurrentValue)
                {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                        if (target.IsValidTarget(R.Range) && R.IsReady())
                        {
                            R.Cast();
                        }
                        else if (R2.IsReady())
                        {
                            R2.Cast();
                        }
                }
            }
                 
        private static void Harass()
        {
            if (Hara["Wh"].Cast<CheckBox>().CurrentValue)
            {
                var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                var wPred = W.GetPrediction(target);
                if (target.IsValidTarget(W.Range) && W.IsReady() && wPred.HitChancePercent >= Pre["Pw"].Cast<Slider>().CurrentValue && Player.Instance.ManaPercent >= Hara["mW"].Cast<Slider>().CurrentValue)
                {
                    W.Cast(wPred.CastPosition);
                }
            }
        }

        private static void LaneClear()
        {
            if (Lane["Ql"].Cast<CheckBox>().CurrentValue)
            {
                var clear = EntityManager.MinionsAndMonsters.GetLaneMonsters(Player.Instance.Position, Q.Range);

                foreach (var clearQ in clear)
                {
                    if (Player.Instance.ManaPercent >= Lane["Mi"].Cast<Slider>().CurrentValue)
                    {
                        Q.Cast(clearQ);
                    }

                    if (Lane["Wl"].Cast<CheckBox>().CurrentValue && W.IsReady()) 
                    {
                        var clear2 = EntityManager.MinionsAndMonsters.GetLaneMonsters(Player.Instance.Position, W.Range);

                        foreach (var clearW in clear2)
                        {
                            if (Player.Instance.ManaPercent >= Lane["Mi"].Cast<Slider>().CurrentValue)
                            {
                                W.Cast(clearW);
                            }
                        }
                    }
                }
            }
        }

        private static void JungleClear()
        {
            if (Lane["QJ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                var jungle = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Q.Range);

                foreach (var jungleQ in jungle)
                {
                    if (Player.Instance.ManaPercent >= Lane["Ma"].Cast<Slider>().CurrentValue)
                    {
                        Q.Cast(jungleQ);
                    }

                    if (Lane["Wj"].Cast<CheckBox>().CurrentValue && W.IsReady())
                    {
                        var jungle2 = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, W.Range);

                        foreach (var jungleW in jungle2)
                        {
                            if (Player.Instance.ManaPercent >= Jungle["Ma"].Cast<Slider>().CurrentValue)
                            {
                                W.Cast(jungleW);
                            }
                        }
                    }
                }
            }
        }

        private static void Draws_Load(EventArgs args)
        {
            new Circle
            {
                Color = System.Drawing.Color.LightCyan,
                Radius = W.Range,BorderWidth = 1f}.Draw
                    (Player.Instance.Position);
        }
    }
}
