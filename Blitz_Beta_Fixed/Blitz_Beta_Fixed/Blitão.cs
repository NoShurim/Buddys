using System;
using EloBuddy;
using EloBuddy.SDK.Events;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using System.Linq;

namespace Blitz_Beta_Fixed
{
    class Blitão
    {
        private static AIHeroClient Blitz => Player.Instance;
        public static int Caunter;
        public static float Grabs;
        private static AIHeroClient DrawTarget { get; set; }
        private static Geometry.Polygon.Rectangle Qpredictions { get; set; }
        private static bool Getcheckboxvalue(Menu menu, string menuvalue)
        {
            return menu[menuvalue].Cast<CheckBox>().CurrentValue;
        }

        //Spells
        public static Spell.Skillshot Q;
        public static Spell.Active W;
        public static Spell.Active E;
        public static Spell.Active R;

        public static Menu Bli, Blits;

        static void Main(string[] args)

        { Loading.OnLoadingComplete += Loading_OnComplete; }


        private static void Loading_OnComplete(EventArgs args)
        {
            if (Blitz.Hero != Champion.Blitzcrank) return;
            Chat.Print("[Addon] [Champion] [Blitzcrank", System.Drawing.Color.OrangeRed);

            Bli = MainMenu.AddMenu("BlitzCrank", "BlitzCrank");
            Blits = Bli.AddSubMenu("Settings");
            Blits.AddGroupLabel("Combo");
            Blits.Add("Qc", new CheckBox("Q [Combo]"));
            Blits.Add("Wc", new CheckBox("W [Combo]"));
            Blits.Add("Ec", new CheckBox("E [Combo]"));
            Blits.Add("Rc", new CheckBox("R [Combo]"));
            Blits.AddSeparator();
            Blits.AddLabel("[Q] Settings");
            Blits.Add("Pre", new ComboBox("Prediction", 1, "To3D", "Logic [Q]"));
            Blits.AddLabel("Not use Grap");
            foreach (var hero in EntityManager.Heroes.Enemies)
            {
                Blits.Add("graps" + hero.ChampionName.ToLower(),
                    TargetSelector.GetPriority(hero) <= 2
                        ? new CheckBox(hero.ChampionName)
                        : new CheckBox(hero.ChampionName, false));
            }
            Blits.AddSeparator();
            Blits.AddLabel("[R] Percent Enemys");
            Blits.Add("rs", new Slider("Percent Enemy [R]", 2, 0, 5));
            Blits.AddSeparator();
            Blits.AddGroupLabel("Modes Basic");
            Blits.Add("misc", new CheckBox("Mode [Misc]"));
            Blits.AddSeparator();
            Blits.AddLabel("Draws [Spells]");
            Blits.Add("drawQ", new CheckBox("Use [Q] Draw"));
            Blits.Add("Qint", new CheckBox("Draw [Q] Int", false));
            Blits.Add("drawR", new CheckBox("Use [R] Draw"));

            Q = new Spell.Skillshot(SpellSlot.Q, 980, SkillShotType.Linear, (int)250f, (int)1800f, (int)70f);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Active(SpellSlot.E, 150);
            R = new Spell.Active(SpellSlot.R, 600);

            Drawing.OnDraw += Draws_OnComple;
            Game.OnTick += GameOnUpdate;
            Spellbook.OnCastSpell += CasProcesseSpell;
            Interrupter.OnInterruptableSpell += IntPorecsse;
            Gapcloser.OnGapcloser += OnGapcloser;
            Qpredictions = new Geometry.Polygon.Rectangle(Blitz.Position, Blitz.Position, Q.Width);

        }

        private static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if(sender.IsValidTarget(R.Range) && sender.IsEnemy)
            {
                R.Cast(sender);
            }
        }
        
        private static void IntPorecsse(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
           if(sender.IsValidTarget(R.Range) && sender.IsEnemy && sender.IsFacing(Blitz))
            {
                R.Cast(sender);
            }
        }

        private static void CasProcesseSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!sender.Owner.IsMe) return;
            if (args.Slot == SpellSlot.E) Orbwalker.ResetAutoAttack();
            if (args.Slot == SpellSlot.Q && (Game.Time - Grabs) > 2)
            {
                Caunter++;
                Grabs = Game.Time;
            }
        }

        private static void GameOnUpdate(EventArgs args)
        {
            DrawTarget = TargetSelector.GetTarget(R.Range, DamageType.Magical);

            if (DrawTarget.IsValidTarget())
            {
                Qpredictions.Start = Blitz.Position.To2D();
                Qpredictions.End = Q.GetPrediction(DrawTarget).CastPosition.To2D();
                Qpredictions.UpdatePolygon();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                ByCombo();
            }
            AutoLogic();
        }

        private static void AutoLogic()
        {
            if (Blits["misc"].Cast<CheckBox>().CurrentValue)
            {
                var max = EntityManager.Heroes.Enemies.Max(t => TargetSelector.GetPriority(t));
                foreach (
                    var enemy in
                        EntityManager.Heroes.Enemies.Where(
                            e =>
                                e.IsValidTarget(Q.Range) &&
                                !Blits["grabs" + e.ChampionName.ToLower()].Cast<CheckBox>().CurrentValue)
                            .Where(enemy => TargetSelector.GetPriority(enemy) == max))
                {
                    QLogic(enemy);
                }
            }
            if (Blits["misc"].Cast<CheckBox>().CurrentValue)
            {
                var enemy = EntityManager.Heroes.Enemies.Find(e => !Blits["grabs" + e.ChampionName.ToLower()].Cast<CheckBox>().CurrentValue && e.IsValidTarget(Q.Range) &&
                (Q.GetPrediction(e).HitChance == HitChance.Dashing || Q.GetPrediction(e).HitChance == HitChance.Immobile));
                if (enemy != null)
                    QLogic(enemy);
            }
            if (Blits["misc"].Cast<CheckBox>().CurrentValue)
            {
                var enemy = EntityManager.Heroes.Enemies.Find(e => e.IsValidTarget(R.Range) && Blitz.GetSpellDamage(e, SpellSlot.R) >= Prediction.Health.GetPrediction(e, R.CastDelay));
                if (enemy != null)
                    R.Cast();
            }
        }

        private static void ByCombo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target != null)
                if (Blits["Qc"].Cast<CheckBox>().CurrentValue && Blitz.Distance(target) > Blitz.GetAutoAttackRange() && Blitz.Distance(target) < Q.Range)
                    QLogic(target);

            if (Blits["Wc"].Cast<CheckBox>().CurrentValue && W.IsReady())
            {
                var enemy = EntityManager.Heroes.Enemies.FirstOrDefault(e => Blitz.Distance(e) < 400);
                if (enemy != null)
                {
                    W.Cast();
                }
            }

            if (Blits["Ec"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                var enemy = EntityManager.Heroes.Enemies.FirstOrDefault(e => Blitz.Distance(e) < 300);
                if (enemy != null)
                {
                    Orbwalker.DisableMovement = true;
                    Orbwalker.DisableAttacking = true;

                    E.Cast();
                    Player.IssueOrder(GameObjectOrder.AttackUnit, enemy);
                    Orbwalker.DisableMovement = false;
                    Orbwalker.DisableAttacking = false;
                }
            }
            if (Blits["Rc"].Cast<CheckBox>().CurrentValue &&
                EntityManager.Heroes.Enemies.Exists(e => Blitz.Distance(e) < R.Range && e.HasBuffOfType(BuffType.Knockup)) && R.IsReady())
            {
                R.Cast();
            }
            if (Blits["Rc"].Cast<CheckBox>().CurrentValue && EntityManager.Heroes.Enemies.Count(e => Blitz.Distance(e) < R.Range) >=
              Blits["rs"].Cast<Slider>().CurrentValue && R.IsReady())
            {
                R.Cast();
            }
        }

        private static void Draws_OnComple(EventArgs args)
        {
            var drawQ = Getcheckboxvalue(Blits, "drawQ");
            var drawR = Getcheckboxvalue(Blits, "drawR");
            var drawQint = Getcheckboxvalue(Blits, "Qint");

            var position = Blitz.Position;

            if (drawQ && Q.IsReady())
            {
                Circle.Draw(Color.LightBlue, Q.Range, Player.Instance.Position);
            }

            if (drawR)
            {
                Circle.Draw(Color.Red, R.Range, Player.Instance.Position);
            }

            if (drawQint && Q.IsReady() && DrawTarget.IsValidTarget())
            {
                Qpredictions.Draw(System.Drawing.Color.CornflowerBlue, 3);
            }
        }

        private static void QLogic(AIHeroClient target)
        {
            if (target == null || !target.IsValidTarget(Q.Range) || Q.GetPrediction(target).HitChance < HitChance.High) return;
            Q.Cast(Q.GetPrediction(target).CastPosition);
        }
        private static void ELogic(AIHeroClient target)
        {
            if (target == null || !target.IsValidTarget(E.Range)) return;
        }
    }
}

   
