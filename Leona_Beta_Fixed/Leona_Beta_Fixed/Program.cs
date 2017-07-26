using System;
using EloBuddy.SDK.Events;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System.Linq;

namespace Leona_Beta_Fixed
{
    class Program
    {
        private static AIHeroClient Leona => Player.Instance;
        //Spells
        public static Spell.Active Q;
        public static Spell.Active W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;
        //Menu
        public static Menu Leo, Combo, Auto, Lane, Jun, Misc, Draws;
        //Aihero
        private static AIHeroClient _Enemys
        {
            get { return ObjectManager.Player; }
        }

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loadin;
        }

        private static void Loadin(EventArgs args)
        {
            if (Leona.Hero != Champion.Leona) return;
            Chat.Print("[Addon] [Champion] [Leona]", System.Drawing.Color.LightYellow);
            SpellLeona();
            MenuLeona();
            Modos();
        }

        private static void Modos()
        {
            Drawing.OnDraw += Draw;
            Game.OnTick += Ontick;
            Gapcloser.OnGapcloser += Gap_Closer;
            Interrupter.OnInterruptableSpell += Inte_On;
        }

        private static void Inte_On(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
          // 
        }
        private static void Gap_Closer(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (Misc["Gap"].Cast<CheckBox>().CurrentValue)
            {
                if (sender.IsEnemy && sender.IsValidTarget(E.Range) && e.End.Distance(_Enemys) <= 350)
                {
                    E.Cast(e.End);
                }
            }
        }
        private static void Ontick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                ByCombo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                ByLane();
            }
            AutoHara();
            AutoR();
            Killteal();
        }

        private static void ByCombo()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (target == null)
                return;

            if (Combo["Qc"].Cast<CheckBox>().CurrentValue)
            {
                if (target.IsInRange(_Enemys, Q.Range) && Q.IsReady() && _Enemys.Distance(target) > 125)

                {
                    Q.Cast();
                }
            }
            if (Combo["Wc"].Cast<CheckBox>().CurrentValue)
            {
                if (target.IsInRange(_Enemys, W.Range) && W.IsReady())
                {
                    W.Cast();
                }
            }
            if (Combo["Ec"].Cast<CheckBox>().CurrentValue)
            {
                if (target.IsInRange(_Enemys, E.Range) && E.IsReady())
                    if (E.GetPrediction(target).HitChance >= HitChance.Impossible)
                    {
                        E.Cast(target);
                    }
            }
            if (Combo["Rc"].Cast<CheckBox>().CurrentValue)
            {
                if (E.GetPrediction(target).HitChance >= HitChance.High)
                    if (target.IsInRange(_Enemys, R.Range))
                    {
                        R.Cast(target);
                    }
            }
        }
        private static void ByLane()
        {
            var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,Player.Instance.Position, Q.Range).ToArray();
            if (Lane["Ql"].Cast<CheckBox>().CurrentValue && Player.Instance.ManaPercent > Lane["mana"].Cast<Slider>().CurrentValue)
            {
                int count = minions.Where(x => x.IsValid() && !x.IsDead && x.Distance(Player.Instance.Position) <= 170).Count();
                { 
                    Q.Cast();
                }

            }
        }
            private static void AutoHara()
        {
            if (Auto["LiW"].Cast<CheckBox>().CurrentValue)
            {
                var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                if (target.IsValidTarget(Auto["Tar"].Cast<Slider>().CurrentValue) && W.IsReady() && Player.Instance.HealthPercent < Auto["Vida"].Cast<Slider>().CurrentValue)
                {
                    W.Cast();
                }
            }
        }
        private static void AutoR()
        {
            throw new NotImplementedException();
        }

        private static void Killteal()
        {
            throw new NotImplementedException();
        }

        private static void Draw(EventArgs args)
        {
            if (Draws["DE"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                E.DrawRange(System.Drawing.Color.ForestGreen);
            }
            if (Draws["DR"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                R.DrawRange(System.Drawing.Color.Teal);
            }
        }
        private static void MenuLeona()
        {
            Leo = MainMenu.AddMenu("[Leona]", "[Leona]");
            Combo = Leo.AddSubMenu("[Combo]");
            Combo.Add("Qc", new CheckBox("[Q] = Combo"));
            Combo.Add("Wc", new CheckBox("[W] = Combo"));
            Combo.Add("Ec", new CheckBox("[E] = Combo"));
            Combo.Add("Rc", new CheckBox("[R] = Combo"));
            Auto = Leo.AddSubMenu("[AutoHarass]");
            Auto.Add("LiW", new CheckBox("Use [W] [Hit Life]"));
            Auto.AddSeparator();
            Auto.AddLabel("Life");
            Auto.Add("Vida", new Slider("Life [W] Uti", 35, 1));
            Auto.AddLabel("Target Valid");
            Auto.Add("Tar", new Slider("Target [W] Auto", 300, 0, 500));
            Lane = Leo.AddSubMenu("[LaneClear]");
            Lane.Add("Ql", new CheckBox("Use [Q] Lane"));
            Lane.AddSeparator();
            Lane.AddLabel("Mana Percent");
            Lane.Add("mana", new Slider("Mana Percent [Q]", 50, 1));
            Misc = Leo.AddSubMenu("[Misc]");
            Misc.Add("Inter", new CheckBox("Interupt"));
            Misc.Add("Gap", new CheckBox("GapClose"));
            Draws = Leo.AddSubMenu("[Draws]");
            Draws.AddLabel("Draws are only activated near enemies");
            Draws.AddSeparator();
            Draws.Add("DE", new CheckBox("Use [E] Draws"));
            Draws.Add("DR", new CheckBox("Use [R] Draws"));

        }
        private static void SpellLeona()
        {
            //Q
            Q = new Spell.Active(SpellSlot.Q, 125);
            //W
            W = new Spell.Active(SpellSlot.W);
            //E
            E = new Spell.Skillshot(SpellSlot.E, 875, SkillShotType.Linear, 250, 2000, 70);
            E.AllowedCollisionCount = int.MaxValue;
            //R
            R = new Spell.Skillshot(SpellSlot.R, 1200, SkillShotType.Circular, castDelay: 250, spellWidth: 200);
            R.AllowedCollisionCount = int.MaxValue;
        }
    }
}
