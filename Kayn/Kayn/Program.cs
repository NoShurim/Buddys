using System;
using EloBuddy;
using EloBuddy.SDK.Events;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using System.Linq;
using EloBuddy.SDK.Enumerations;


namespace Kayn
{
    class Program
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Active E;
        public static Spell.Targeted R;
        public static Spell.Skillshot R2;
        public static AIHeroClient _Player;
        public static Menu Kmenu, Combo, Hara, Lane, Jungle, Misc, Evade;

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
            W = new Spell.Skillshot(SpellSlot.W, 700, EloBuddy.SDK.Enumerations.SkillShotType.Linear, 350, 175, 87);
            E = new Spell.Active(SpellSlot.E);
            R = new Spell.Targeted(SpellSlot.R, 475);
            R2 = new Spell.Skillshot(SpellSlot.R, 150, EloBuddy.SDK.Enumerations.SkillShotType.Linear, 75, 37, 18);

            Kmenu = MainMenu.AddMenu("Kayn", "Kayn");
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

            Evade = Kmenu.AddSubMenu("Evade");
            Evade.Add("ER", new CheckBox("Enabled Evade"));
            Evade.AddLabel("Evade In Faze Beta");
        
            Drawing.OnDraw += Draws_Load;
            Game.OnTick += Game_OnTick;
            Interrupter.OnInterruptableSpell += Interrupter_Spell;
            AIHeroClient.OnProcessSpellCast += EvadeBeta;
            Gapcloser.OnGapcloser += OnGapcloser;

        }

        private static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void EvadeBeta(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            throw new NotImplementedException();
        }

        private static void Interrupter_Spell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            var wPred = W.GetPrediction(target);

            if (Misc["Inter"].Cast<CheckBox>().CurrentValue && W.IsReady() && Q.GetPrediction(target).HitChance >= HitChance.High)
            {
                if (_Player.Distance(_Player.ServerPosition, true) <= W.Range && Q.GetPrediction(target).HitChance >= HitChance.High)
                {
                    W.Cast(target);
                }
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            Haraaa();
            Missc();
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

        private static void Missc()
        {
            if (Misc["KS"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var enemy in
                     EntityManager.Heroes.Enemies.Where(
                         x => x.Distance(_Player) <= R.Range && x.IsValidTarget() && !x.IsInvulnerable && !x.IsZombie))
                    if (R.IsReady() && RDamage(enemy) >= enemy.Health)
                    {
                        R.Cast();
                    }
            }
            if (Misc["End"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var enemy in
                     EntityManager.Heroes.Enemies.Where(
                         x => x.Distance(_Player) <= R.Range && x.IsValidTarget() && !x.IsInvulnerable && !x.IsZombie))
                    if (R.IsReady() && RDamage(enemy) >= enemy.Health)
                    {
                        R.Cast();
                    }
            }
        }
        private static void Haraaa()
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            var wPred = W.GetPrediction(target);
            if (wPred.HitChance >= EloBuddy.SDK.Enumerations.HitChance.High && W.IsReady() && Player.Instance.ManaPercent >= Hara["mW"].Cast<Slider>().CurrentValue)
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
                if (target.IsValidTarget(Q.Range) && qPred.HitChance >= EloBuddy.SDK.Enumerations.HitChance.High && Q.IsReady())
                {
                    Q.Cast(qPred.CastPosition);
                }
            }
                if (Combo["Wk"].Cast<CheckBox>().CurrentValue)
                {
                var traget = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                    var wPred = W.GetPrediction(traget);
                        if (traget.IsValidTarget(W.Range) && wPred.HitChance >= EloBuddy.SDK.Enumerations.HitChance.High && W.IsReady())
                    {
                        W.Cast(wPred.CastPosition);
                    }
                }
                if (Combo["Rk"].Cast<CheckBox>().CurrentValue)
                {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                    foreach (var enemy in
                     EntityManager.Heroes.Enemies.Where(
                         x => x.Distance(_Player) <= R.Range && x.IsValidTarget() && !x.IsInvulnerable && !x.IsZombie))
                        if (target.IsValidTarget(R.Range) && R.IsReady() && RDamage(enemy) >= enemy.Health)
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
                if (target.IsValidTarget(W.Range) && wPred.HitChance >= EloBuddy.SDK.Enumerations.HitChance.High && W.IsReady() && Player.Instance.ManaPercent >= Hara["mW"].Cast<Slider>().CurrentValue)
                {
                    W.Cast(wPred.CastPosition);
                }
            }
        }

        private static void LaneClear()
        {
            if (Lane["Ql"].Cast<CheckBox>().CurrentValue)
            {
                var clear = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Q.Range);

                foreach (var clearQ in clear)
                {
                    if (Player.Instance.ManaPercent >= Lane["Mi"].Cast<Slider>().CurrentValue)
                    {
                        Q.Cast(clearQ);
                    }

                    if (Lane["Wl"].Cast<CheckBox>().CurrentValue && W.IsReady()) 
                    {
                        var clear2 = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, W.Range);

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