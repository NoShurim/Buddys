using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace Gragas_Beta_Fixed
{
    class MyHero
    {
        private static AIHeroClient Gragas => Player.Instance;

        private static float HealthPercent()
        {
            return (Gragas.Health / Gragas.MaxHealth) * 100;
        }

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        public static Spell.Skillshot Q;
        public static Spell.Active W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;
        public static Spell.Targeted Ignite;
        public static Menu Menu, Menu1, Menu2, Draws, Misc;
        public static bool CastedQ;
        public static Vector3 insecpos, eqpos, movingawaypos;
        public static Vector3 teste;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnComplete;
        }

        private static void Loading_OnComplete(EventArgs args)
        {
            if (Gragas.Hero != Champion.Gragas) return;
            Bootstrap.Init(null);
            Chat.Print("[Addon] [Champion] [Gragas]");

            Q = new Spell.Skillshot(SpellSlot.Q, 775, SkillShotType.Circular, 1, 1000, 110);
            Q.AllowedCollisionCount = int.MaxValue;
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Skillshot(SpellSlot.E, 675, SkillShotType.Linear, 0, 1000, 50);
            R = new Spell.Skillshot(SpellSlot.R, 1100, SkillShotType.Circular, 1, 1000, 700);
            R.AllowedCollisionCount = int.MaxValue;

            Menu = MainMenu.AddMenu("Gragas", "Gragas");

            var Enemies = EntityManager.Heroes.Enemies.Where(a => !a.IsMe).OrderBy(a => a.BaseSkinName);
            Menu1 = Menu.AddSubMenu("Combo");
            Menu1.AddSeparator();
            Menu1.AddLabel("Combo Configs");
            Menu1.Add("ComboQ", new CheckBox("Use [Q] on Combo"));
            Menu1.Add("ComboW", new CheckBox("Use [W] on Combo"));
            Menu1.Add("ComboE", new CheckBox("Use [E] on Combo"));
            Menu1.Add("ComboR", new CheckBox("Use [R] on Combo", false));
            Menu1.AddLabel("Use [R] only on:");
            foreach (var a in Enemies)
            {
                Menu1.Add("Ult_" + a.BaseSkinName, new CheckBox(a.BaseSkinName));
            }
            Menu1.Add("MinR", new Slider("Use R if min Champs on [R] range:", 2, 1, 5));
            Menu1.AddSeparator();

            Menu1.AddLabel("Harass");
            Menu1.Add("ManaH", new Slider("Mana Percent  > %", 40));
            Menu1.Add("HarassQ", new CheckBox("Use [Q] on Harass"));
            Menu1.Add("HarassW", new CheckBox("Use [W] on Harass"));
            Menu1.Add("HarassE", new CheckBox("Use [E] on Harass"));
            Menu1.AddSeparator();
            Menu1.AddLabel("KillSteal");
            Menu1.Add("KQ", new CheckBox("Use Q on KillSteal"));
            Menu1.Add("KE", new CheckBox("Use E to KillSteal"));
            Menu1.Add("KR", new CheckBox("Use R to KillSteal"));

            Menu2 = Menu.AddSubMenu("Farming");
            Menu2.AddLabel("LastHit Configs");
            Menu2.Add("ManaL", new Slider("Dont use Skills if Mana <= ", 40));
            Menu2.Add("LastQ", new CheckBox("Use Q on LastHit"));
            Menu2.Add("LastW", new CheckBox("Use W on LastHit"));
            Menu2.Add("LastE", new CheckBox("Use E on LastHit"));
            Menu2.AddLabel("Lane Clear Config");
            Menu2.Add("ManaF", new Slider("Dont use Skills if Mana <=", 40));
            Menu2.Add("FarmQ", new CheckBox("Use Q on LaneClear"));
            Menu2.Add("FarmW", new CheckBox("Use W on LaneClear"));
            Menu2.Add("FarmE", new CheckBox("Use E on LaneClear"));
            Menu2.Add("MinionQ", new Slider("Use Q when count minions more than :", 3, 1, 5));
            Menu2.AddLabel("Jungle Clear Config");
            Menu2.Add("ManaJ", new Slider("Dont use Skills if Mana <=", 40));
            Menu2.Add("JungQ", new CheckBox("Use Q on Jungle"));
            Menu2.Add("JungW", new CheckBox("Use W on Jungle"));
            Menu2.Add("JungE", new CheckBox("Use E on Jungle"));

            Draws = Menu.AddSubMenu("Draws");
            Draws.Add("drawQ", new CheckBox(" Draw do Q"));
            Draws.Add("drawE", new CheckBox(" Draw do E"));
            Draws.Add("drawR", new CheckBox(" Draw do R"));

            Misc = Menu.AddSubMenu("Misc");
            Misc.Add("useEGapCloser", new CheckBox("E on GapCloser"));
            Misc.Add("useRGapCloser", new CheckBox("R on GapCloser"));
            Misc.Add("useEInterrupter", new CheckBox("use E to Interrupt"));
            Misc.Add("useRInterrupter", new CheckBox("use R to Interrupt"));
            Misc.Add("Key", new KeyBind("Key to insec", false, KeyBind.BindTypes.HoldActive, (uint)'A'));

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Game_OnDraw;
            GameObject.OnCreate += Game_ObjectCreate;
            //GameObject.OnDelete += Game_OnDelete;
            //Orbwalker.OnPostAttack += Reset;
            Game.OnTick += Game_OnTick;
            Interrupter.OnInterruptableSpell += game_Interrupter;
            Gapcloser.OnGapcloser += Gap_Closer;

        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Misc["Key"].Cast<KeyBind>().CurrentValue)
            {
                Insec();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                ByCombo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                ByHarass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {

                ByLaneClear();

            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {

                ByJungleClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                ByLastHit();

            }
        }

        private static void Insec()
        {
            var Player = Gragas;
            var ins = insecpos;
            var mov = movingawaypos;
            var pos = eqpos;
            var alvo = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            Orbwalker.MoveTo(Game.CursorPos);

            pos = Player.Position.Extend(alvo, R.Range + 300).To3D();
            ins = Player.Position.Extend(alvo.Position, Player.Distance(alvo) + 200).To3D();
            mov = Player.Position.Extend(alvo.Position, Player.Distance(alvo) + 300).To3D();

            if (R.IsReady() && !(alvo == null))
            {
                if (alvo.IsFacing(Player) == false && alvo.IsMoving & (R.IsInRange(ins) && alvo.Distance(ins) < 300))
                    R.Cast(mov);

                if (R.IsInRange(ins) && alvo.Distance(ins) < 300 && alvo.IsFacing(Player) && alvo.IsMoving)
                    R.Cast(pos);

                else if (R.IsInRange(ins) && alvo.Distance(ins) < 300)
                    R.Cast(ins);

                if (Q.IsReady() && alvo.IsValidTarget(Q.Range))
                {
                    Q.Cast(alvo);
                    CastedQ = true;
                }

                if (E.IsReady() && alvo.IsValidTarget(E.Range))
                {
                    E.Cast(alvo);


                }
            }
        }

        private static void ByCombo()
        {

            var alvo = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            var predPosQ = Prediction.Position.PredictLinearMissile(alvo, Q.Range, Q.Width, Q.CastDelay, Q.Speed, int.MaxValue, null, false);
            var predPos = Prediction.Position.PredictLinearMissile(alvo, R.Range, R.Width, R.CastDelay, R.Speed, int.MaxValue, null, false);
            var predPosE = Prediction.Position.PredictLinearMissile(alvo, E.Radius, E.Width, E.CastDelay, E.Speed, 0, null, true);
            if (!alvo.IsValid()) return;

            if (Q.IsReady() && alvo.IsValidTarget(Q.Range) && Menu1["ComboQ"].Cast<CheckBox>().CurrentValue)
            {
                Q.Cast(alvo);
                CastedQ = true;
            }

            if (W.IsReady() && alvo.IsValidTarget(E.Range) && Menu1["ComboW"].Cast<CheckBox>().CurrentValue)
            {
                W.Cast();

            }
            if (E.IsReady() && alvo.IsValidTarget(E.Range) && Menu1["ComboE"].Cast<CheckBox>().CurrentValue)
            {
                E.Cast(alvo);


            }
            if (R.IsReady() && alvo.IsValidTarget(R.Range) && (Menu1["ComboR"].Cast<CheckBox>().CurrentValue) && Menu1["Ult_" + alvo.BaseSkinName].Cast<CheckBox>().CurrentValue)//&& !(Q.IsInRange(alvo)))
            {
                R.Cast(predPos.CastPosition + 100);

            }
            if (E.IsReady() && alvo.IsValidTarget(R.Range) && !(Q.IsInRange(alvo)))
            {
                E.Cast(predPosE.CastPosition);

            }
        }

        private static void ByHarass()
        {
            throw new NotImplementedException();
        }

        private static void ByLaneClear()
        {
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(Q.Range));
            var minion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, Q.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
            if (minions == null) return;
            if ((_Player.ManaPercent <= Menu2["ManaF"].Cast<Slider>().CurrentValue))
            {
                return;
            }

            if (Q.IsReady() && Q.IsInRange(minions) && Menu2["FarmQ"].Cast<CheckBox>().CurrentValue && (minion >= Menu2["MinionQ"].Cast<Slider>().CurrentValue))
            {

                Q.Cast(Q.GetPrediction(minions).CastPosition);

                CastedQ = true;

            }

            if (W.IsReady() && E.IsInRange(minions) && Menu2["FarmW"].Cast<CheckBox>().CurrentValue)
            {
                W.Cast();

            }
            if (E.IsReady() && E.IsInRange(minions) && Menu2["FarmE"].Cast<CheckBox>().CurrentValue)
            {
                E.Cast(minions);

            }
        }

        private static void ByJungleClear()
        {
            var jungleMonsters = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(Q.Range));
            var minioon = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, E.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
            if (jungleMonsters == null) return;
            if ((_Player.ManaPercent <= Menu2["ManaJ"].Cast<Slider>().CurrentValue))
            {
                return;
            }
            if (Q.IsReady() && Q.IsInRange(jungleMonsters) && Menu2["JungQ"].Cast<CheckBox>().CurrentValue)

                Q.Cast(Q.GetPrediction(jungleMonsters).CastPosition);

            CastedQ = true;

            if (W.IsReady() && E.IsInRange(jungleMonsters) && Menu2["JungW"].Cast<CheckBox>().CurrentValue)
            {
                W.Cast();

            }
            if (E.IsReady() && E.IsInRange(jungleMonsters) && Menu2["JungE"].Cast<CheckBox>().CurrentValue)
            {
                E.Cast(jungleMonsters);

            }
        }

        private static void ByLastHit()
        {
            var qminions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(Q.Range) && (DamageLib.QCalc(m) > m.Health));
            var eminions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(E.Range) && (DamageLib.ECalc(m) > m.Health));
            var minion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, E.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
            if (qminions == null) return;
            var prediction = Q.GetPrediction(qminions);
            if (Q.IsReady() && Q.IsInRange(qminions) && Menu2["LastQ"].Cast<CheckBox>().CurrentValue && qminions.Health < DamageLib.QCalc(qminions))

                Q.Cast(Q.GetPrediction(qminions).CastPosition);

            CastedQ = true;

            if (E.IsReady() && E.IsInRange(eminions) && Menu2["LastE"].Cast<CheckBox>().CurrentValue && eminions.Health < DamageLib.ECalc(eminions))
            {
                E.Cast(eminions);
            }
        }

        private static void Game_OnDraw(EventArgs args)
        {

            if (Draws["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Color.LightBlue, Q.Range, Player.Instance.Position);
            }
            if (Draws["drawE"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Color.LightBlue, E.Range, Player.Instance.Position);
            }
            if (Draws["drawR"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Color.LightBlue, R.Range, Player.Instance.Position);
            }
        }

        private static void Game_ObjectCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == ("Gragas_Base_Q_Ally.Troy"))
            {
                if (Player.Instance.Spellbook.GetSpell(SpellSlot.Q).ToggleState == 2 || Player.Instance.Spellbook.GetSpell(SpellSlot.Q).ToggleState == 1)
                {
                    Q.Cast(Player.Instance);
                    CastedQ = false;
                }
                else
                {
                    CastedQ = false;
                }
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            throw new NotImplementedException();
        }

        private static void game_Interrupter(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (e.DangerLevel == DangerLevel.High && sender.IsEnemy && sender is AIHeroClient && sender.Distance(_Player) < E.Range && E.IsReady() && Misc["useEInterrupter"].Cast<CheckBox>().CurrentValue)
            {
                E.Cast(sender);
            }
            if (e.DangerLevel == DangerLevel.High && sender.IsEnemy && sender is AIHeroClient && sender.Distance(_Player) < R.Range && R.IsReady() && Misc["useRInterrupter"].Cast<CheckBox>().CurrentValue)
            {
                R.Cast(sender);
            }
        }
        private static void Gap_Closer(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender.IsEnemy && sender is AIHeroClient && sender.Distance(_Player) < E.Range && E.IsReady() && Misc["useEGapCloser"].Cast<CheckBox>().CurrentValue)
            {
                E.Cast(sender);
            }
            if (sender.IsEnemy && sender is AIHeroClient && sender.Distance(_Player) < R.Range && R.IsReady() && Misc["useRGapCloser"].Cast<CheckBox>().CurrentValue)
            {
                R.Cast(sender);

            }
        }
    }
}