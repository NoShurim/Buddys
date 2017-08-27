using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using System;
using SharpDX;
using EloBuddy.SDK.Enumerations;
using static LeBlanc_Beta_Fixed.Lib;
using System.Linq;

namespace LeBlanc_Beta_Fixed
{
    class MyHero
    {
        public static AIHeroClient LeBlanc => Player.Instance;
        public static Vector3 WPosition { get; set; }
        public static Vector3 WEnd { get; set; }
        public static GameObject ObjetBaseLeblanc { get; set; }
        public static Vector3 Update { get; set; }

        public static bool IsW1()
        {
            return LeBlanc.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancw";
        }

        public static bool IsW2()
        {
            return LeBlanc.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() != "leblancw";
        }

        public static bool IsR1()
        {
            return LeBlanc.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancrtoggle";
        }

        public static bool IsR2()
        {
            return LeBlanc.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() != "leblancrtoggle";
        }

        public static bool IsPassive(Obj_AI_Base hero)
        {
            return hero.HasBuff("LeblancPMark") && Game.Time - hero.GetBuff("LeblancPMark").StartTime > 1;
        }

        public static bool IsPassiveM(Obj_AI_Base hero)
        {
            return hero.HasBuff("leblancpminion") && Game.Time - hero.GetBuff("leblancpminion").StartTime > 1;
        }

        public static string GetComboName()
        {
            var target = TargetSelector.GetTarget(1600, DamageType.Magical);

            if (target == null || !target.IsValid)
            {
                return "DF";
            }
            if (LeBlanc.Distance(target) < W.Range)
            {
                return "W";
            }
            else if (LeBlanc.Distance(target) < E.Range)
            {
                return "RE";
            }
            else if (target.IsValidTarget(W.Range + Q.Range))
            {
                return "Gap";
            }
            return "DF";
        }


        public static bool CastCheckbox(Menu obj, string value)
        {
            return obj[value].Cast<CheckBox>().CurrentValue;
        }

        public static int CastSlider(Menu obj, string value)
        {
            return obj[value].Cast<Slider>().CurrentValue;
        }

        public static int CastBox(Menu obj, string value)
        {
            return obj[value].Cast<ComboBox>().CurrentValue;
        }

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnComplete;
        }

        private static void Loading_OnComplete(EventArgs args)
        {
            if (LeBlanc.Hero != Champion.Leblanc) return;
            Chat.Print("[Addon] [Champion] [LeBlanc]", System.Drawing.Color.AliceBlue);
            Chat.Print("[Version 1.1v]", System.Drawing.Color.OrangeRed);

            Menus.StartMenu();
            Lib.W.AllowedCollisionCount = int.MaxValue;
            Game.OnTick += Game_OnUpdate;
            //Evade
            Drawing.OnDraw += OnDraws;
            Gapcloser.OnGapcloser += AntiGapcloser_Execute;
            GameObject.OnCreate += Object_OnCreate;
            GameObject.OnDelete += Object_OnDelete;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Base.OnNewPath += OnNewPath;
        }

        private static void Object_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == Player.Instance.Name)
            {
                ObjetBaseLeblanc = sender;
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;

            if (args.SData.Name.ToLower() == "leblancw")
            {
                {
                    WPosition = args.Start;
                    WEnd = args.End;
                }
            }
        }

        private static void OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (sender.IsMe)
            {
                var path = Player.Instance.Position.Extend(args.Path[0], 1000);
                var extendedPath = new Vector3(path, NavMesh.GetHeightForPosition(path.X, path.Y));
                Update = extendedPath;
            }
        }

        private static void Object_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name == Player.Instance.Name)
            {
                ObjetBaseLeblanc = null;
            }
        }

        private static void AntiGapcloser_Execute(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender.IsAlly || sender.IsDead || sender.IsMe) return;

            if (CastCheckbox(Menus.AntiGapcloser, "E"))
            {
                if (E.IsReady())
                {
                    var epred = Lib.E.GetPrediction(sender);
                    if (epred.HitChance >= HitChance.Medium)
                    {
                        E.Cast(epred.CastPosition);
                    }
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                ByLane();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                ByJungle();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                ByCombo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                ByFlee();
            }
            if (Menus.Comb["Key1"].Cast<KeyBind>().CurrentValue)
            {
                Combo2();
            }
            KillSteal();
        }

        private static void ByFlee()
        {
            var target = TargetSelector.GetTarget(Lib.E.Range, DamageType.Magical);
            if (target != null)
            {
                if (CastCheckbox(Menus.Flwe, "E"))
                {
                    var epred = Lib.E.GetPrediction(target);
                    if (epred.HitChance >= HitChance.Medium)
                    {
                        Lib.E.Cast(epred.CastPosition);
                    }
                }
            }
            if (CastCheckbox(Menus.Flwe, "W"))
            {
                var wpos = LeBlanc.Position.Extend(Game.CursorPos, Lib.W.Range).To3D();
                if (Lib.W.IsReady())
                {
                    Lib.CastW(wpos);
                }
            }
        }

        private static void Combo2()
        {
            var target = TargetSelector.GetTarget(1600, DamageType.Magical);
            var useQ = CastCheckbox(Menus.Comb, "Q");
            var useW = CastCheckbox(Menus.Comb, "W");
            var useE = CastCheckbox(Menus.Comb, "E");
            var useR = CastCheckbox(Menus.Comb, "R");
            var useRQ = CastCheckbox(Menus.Comb, "RQ");
            var useRW = CastCheckbox(Menus.Comb, "RW");
            var useRE = CastCheckbox(Menus.Comb, "RE");
            var wpos = Player.Instance.Position.Extend(target, Lib.W.Range).To3D();

            if (LeBlanc.Distance(target) < E.Range)
            {
                if (useE)
                {
                    CastE(target);
                }

                if (useQ && IsPassive(target))
                {
                    CastQ(target);
                }
                if (useR)
                {
                    CastR("RE", target);
                }
                if (useW && IsW1() && !RActive.IsReady())
                {
                    CastW(target.ServerPosition);
                }
            }

            else if (LeBlanc.Distance(target) < W.Range)
            {
                if (useQ)
                {
                    CastQ(target);
                }
                if (useE && !Q.IsReady())
                {
                    CastE(target);
                }
                if (useR)
                {
                    CastR("RW", target);
                }
                if (useW && IsW1() && !RActive.IsReady())
                {
                    CastW(target.ServerPosition);
                }
            }
        }

        private static void KillSteal()
        {
            foreach (var hptarget in EntityManager.Enemies.Where(a => a.IsValidTarget(1200) && !a.IsDead))
            {
                if (!hptarget.IsValid || hptarget.IsDead || hptarget == null)
                {
                    return;
                }
                var Health = hptarget.Health;
                if (Ignite.IsReady())
                {
                    var dmgI = (50 + ((LeBlanc.Level) * 20));
                    if (LeBlanc.Distance(hptarget) < Q.Range && Health < dmgI)

                    {
                        Ignite.Cast(hptarget);
                    }
                }
            }
        }

        private static void ByLane()
        {
            var useLQ = CastCheckbox(Menus.Lane, "Q");
            var useLW = CastCheckbox(Menus.Lane, "W");
            var Mana = CastSlider(Menus.Lane, "Mana");
            var mini = CastSlider(Menus.Lane, "WMin");

            var minion = EntityManager.MinionsAndMonsters.Get(EntityManager.MinionsAndMonsters.EntityType.Minion, EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition, Q.Range);
            if (useLQ && useLW && LeBlanc.ManaPercent > Mana)
            {
                foreach (var mayminoon in minion)
                {
                    if (minion != null)
                    {
                        if (W.GetPrediction(mayminoon).CollisionObjects.Where(may => may.IsEnemy && !may.IsDead && may.IsValid && !may.IsInvulnerable).Count() >= mini)
                        {
                            W.Cast(mayminoon);
                        }
                        else if (W.IsReady() && Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancwreturn")
                        {
                            WAc.Cast();
                        }
                        else if (IsPassiveM(mayminoon))
                        {
                            Q.Cast(mayminoon);
                        }
                    }
                }
            }
        }

        private static void ByJungle()
        {
            var useLQ = CastCheckbox(Menus.Lane, "Q");
            var useLW = CastCheckbox(Menus.Lane, "W");
            var Mana = CastSlider(Menus.Lane, "Mana");
            var mini = CastSlider(Menus.Lane, "WMin");

            var monters = EntityManager.MinionsAndMonsters.GetJungleMonsters(LeBlanc.Position, Q.Range).Where(my => !my.IsDead && my.IsValid && !my.IsInvulnerable);
            if (useLQ && useLW && LeBlanc.ManaPercent > Mana)
            {
                foreach (var monstros in monters)
                {
                    if (monters != null)
                    {
                        if (W.GetPrediction(monstros).CollisionObjects.Where(may => may.IsEnemy && !may.IsDead && may.IsValid && !may.IsInvulnerable).Count() > 1)
                        {
                            W.Cast(monstros);
                        }
                        else if (W.IsReady() && Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancwreturn")
                        {
                            WAc.Cast();
                        }
                        else if (IsPassiveM(monstros))
                        {
                            Q.Cast(monstros);
                        }
                    }
                }
            }
        }

        private static void ByCombo()
        {
            var target = TargetSelector.GetTarget(1600, DamageType.Magical);
            var useQ = CastCheckbox(Menus.Comb, "Q");
            var useW = CastCheckbox(Menus.Comb, "W");
            var useE = CastCheckbox(Menus.Comb, "E");
            var useR = CastCheckbox(Menus.Comb, "R");
            var useRQ = CastCheckbox(Menus.Comb, "RQ");
            var useRW = CastCheckbox(Menus.Comb, "RW");
            var useRE = CastCheckbox(Menus.Comb, "RE");
            var wpos = Player.Instance.Position.Extend(target, Lib.W.Range).To3D();

            if (LeBlanc.Distance(target) < W.Range)  //wQRE
            {
                if (useW && IsW1() && IsPassive(target))
                {
                    CastW(W.GetPrediction(target).CastPosition);
                }
                if (useQ && IsPassive(target))
                {
                    CastQ(target);
                }
                if (useRQ && IsR1() && IsPassive(target))
                {
                    CastR("RQ", target);
                }
                if (useE && IsPassive(target))
                {
                    CastE(target);
                }
                if (useW && IsW1() && IsPassive(target))
                {
                    CastW(W.GetPrediction(target).CastPosition);
                }
                if (useW && IsW1())
                {
                    CastW(W.GetPrediction(target).CastPosition);
                }
                if (useE)
                {
                    CastE(target);
                }
                if (useQ)
                {
                    CastQ(target);
                }
            }
            else if (LeBlanc.Distance(target) < E.Range)//REQEW
            {
                if (useRE && IsR1() && IsPassive(target))
                {
                    CastR("RE", target);
                }
                if (useQ && IsPassive(target))
                {
                    CastQ(target);
                }
                if (useE && IsPassive(target))
                {
                    CastE(target);
                }
                if (useW && IsW1() && IsPassive(target))
                {
                    CastW(W.GetPrediction(target).CastPosition);
                }

            }
            else if (target.IsValidTarget(W.Range + Q.Range))//gapclose combo W-R(E)-E-Q
            {
                var pos = LeBlanc.ServerPosition.Extend(target.ServerPosition, W.Range);
                if (IsW1() && useW)
                {
                    CastW(wpos);
                }
                if (useRE && IsR1())
                {
                    CastR("RE", target);
                }
                if (useQ && IsPassive(target))
                {
                    CastQ(target);
                }
                if (useE)
                {
                    CastE(target);
                }
            }

        }

        private static void OnDraws(EventArgs args)
        {
            if (CastCheckbox(Menus.Draws, "Q"))
            {
                Circle.Draw(Color.Aqua, Lib.Q.Range, Player.Instance.Position);
            }
            if (CastCheckbox(Menus.Draws, "W"))
            {
                Circle.Draw(Color.Aqua, Lib.W.Range, Player.Instance.Position);
            }
            if (CastCheckbox(Menus.Draws, "E"))
            {
                Circle.Draw(Color.Aqua, Lib.E.Range, Player.Instance.Position);
            }
        }
    }
}