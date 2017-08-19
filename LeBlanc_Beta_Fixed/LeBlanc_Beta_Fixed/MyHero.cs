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

        public static bool IsPassiveJ(Obj_AI_Base hero)
        {
            return hero.HasBuff("leblancpomonsters") && Game.Time - hero.GetBuff("leblancpomonsters").StartTime > 1;
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
            Chat.Print("[Verdion 1.v]", System.Drawing.Color.Blue);

            Menus.StartMenu();
            Lib.W.AllowedCollisionCount = int.MaxValue;
            Game.OnTick += Game_OnUpdate;
            //Evade
            Drawing.OnDraw += OnDraws;
            Gapcloser.OnGapcloser += AntiGapcloser_Execute;

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
                Combo2();
                DystonCombo();
            }
            KillSteal();
        }

        private static void DystonCombo()
        {
            var target = TargetSelector.GetTarget(925, DamageType.Magical);
            var useQ = CastCheckbox(Menus.Comb, "Q");
            var useW = CastCheckbox(Menus.Comb, "W");
            var useE = CastCheckbox(Menus.Comb, "E");
            var useR = CastCheckbox(Menus.Comb, "R");
            var UseRQ = CastCheckbox(Menus.Comb, "RQ");
            var UseRW = CastCheckbox(Menus.Comb, "RW");
            var UseRE = CastCheckbox(Menus.Comb, "RE");
            if (Menus.Comb["modes"].Cast<ComboBox>().CurrentValue == 3)
            {
                return;
            }
            switch (Menus.Comb["m2"].Cast<ComboBox>().CurrentValue)
            {
                case 0:
                    if (useQ)
                    {
                        CastQ(target);
                    }
                    if (useW)
                    {
                        CastW(W.GetPrediction(target).CastPosition);
                    }
                    if (useE)
                    {
                        CastE(target);
                    }
                    if (UseRW && IsR1())
                    {
                        CastR("RW", target);
                    }
                    break;
                case 1:
                    if (useQ)
                    {
                        CastQ(target);
                    }
                    if (UseRQ && IsPassive(target))
                    {
                        CastR("RQ", target);
                    }
                    if (useE)
                    {
                        CastE(target);
                    }
                    if (useW)
                    {
                        CastW(W.GetPrediction(target).CastPosition);
                    }
                    break;
                case 2:
                    if (useE)
                    {
                        CastE(target);
                    }
                    if (useQ && IsPassive(target))
                    {
                        CastQ(target);
                    }
                    if (useW && IsW1())
                    {
                        CastW(W.GetPrediction(target).CastPosition);
                    }
                    if (IsR1() && UseRW)
                    {
                        CastR("RW", target);
                    }
                    break;
                case 3:
                    if (useE)
                    {
                        CastE(target);
                    }
                    if (useW && IsW1())
                    {
                        CastW(W.GetPrediction(target).CastPosition);
                    }
                    if (useQ && IsPassive(target))
                    {
                        CastQ(target);
                    }
                    if (UseRQ)
                    {
                        CastR("RQ", target);
                    }
                    break;
                case 4:
                    if (useW && IsW1())
                    {
                        CastW(W.GetPrediction(target).CastPosition);
                    }
                    if (UseRW)
                    {
                        CastR("RW", target);
                    }
                    if (useQ && IsPassive(target))
                    {
                        CastQ(target);
                    }
                    if (useE)
                    {
                        CastE(target);
                    }
                    break;
                case 5:
                    if (useW && IsW1())
                    {
                        CastW(W.GetPrediction(target).CastPosition);
                    }
                    if (useQ && IsPassive(target))
                    {
                        CastQ(target);
                    }
                    if (UseRQ && IsR1())
                    {
                        CastR("RQ", target);
                    }
                    if (useE)
                    {
                        CastE(target);
                    }
                    break;
                case 6:
                    if (useQ)
                    {
                        CastQ(target);
                    }
                    if (UseRQ && IsR1())
                    {
                        CastR("RQ", target);
                    }
                    if (useW && IsW1())
                    {
                        CastW(W.GetPrediction(target).CastPosition);
                    }
                    if (useE)
                    {
                        CastE(target);
                    }
                    break;
            }
        }

        private static void Modes()
        {
            switch (Menus.Comb["modes"].Cast<ComboBox>().SelectedIndex)
            {

                case 0:
                    ByCombo();
                    break;

                case 1:
                    Combo2();
                    break;

                case 2:
                    DystonCombo();
                    break;
            }
        }

        private static void Combo2()
        {
            var target = TargetSelector.GetTarget(925, DamageType.Magical);
            var useQ = CastCheckbox(Menus.Comb, "Q");
            var useW = CastCheckbox(Menus.Comb, "W");
            var useE = CastCheckbox(Menus.Comb, "E");
            var useR = CastCheckbox(Menus.Comb, "R");
            var UseRQ = CastCheckbox(Menus.Comb, "RQ");
            var UseRW = CastCheckbox(Menus.Comb, "RW");
            var UseRE = CastCheckbox(Menus.Comb, "RE");

            if (LeBlanc.Distance(target) < W.Range)  //wQRE
            {
                if (useW && IsW1())
                {
                    CastW(W.GetPrediction(target).CastPosition);
                }
                if (useQ && IsPassive(target))
                {
                    CastQ(target);
                }
                if (UseRQ && IsR1())
                {
                    CastR("RQ", target);
                }
                if (useE)
                {
                    CastE(target);
                }
            }
            else if (LeBlanc.Distance(target) < E.Range)//REQEW
            {
                if (UseRE && IsR1())
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
                if (useW && IsW1())
                {
                    CastW(W.GetPrediction(target).CastPosition);
                }

            }
            else if (target.IsValidTarget(W.Range + Q.Range))//gapclose combo W-R(E)-E-Q
            {
                var wpos = Player.Instance.Position.Extend(target, Lib.W.Range).To3D();
                if (IsW1() && useW)
                {
                    CastW(wpos);
                }
                if (UseRE && IsR1())
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
                        if (W.GetPrediction(monstros).CollisionObjects.Where(may => may.IsEnemy && !may.IsDead && may.IsValid && !may.IsInvulnerable).Count() >= mini)
                        {
                            W.Cast(monstros);
                        }
                        else if (W.IsReady() && Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancwreturn")
                        {
                            WAc.Cast();
                        }
                        else if (IsPassiveJ(monstros))
                        {
                            Q.Cast(monstros);
                        }
                    }
                }
            }
        }

        private static void ByCombo()
        {
            var target = TargetSelector.GetTarget(925, DamageType.Magical);
            var useQ = CastCheckbox(Menus.Comb, "Q");
            var useW = CastCheckbox(Menus.Comb, "W");
            var useE = CastCheckbox(Menus.Comb, "E");
            var useR = CastCheckbox(Menus.Comb, "R");
            var UseRQ = CastCheckbox(Menus.Comb, "RQ");
            var UseRW = CastCheckbox(Menus.Comb, "RW");
            var UseRE = CastCheckbox(Menus.Comb, "RE");
            var wpos = Player.Instance.Position.Extend(target, Lib.W.Range).To3D();

            if (LeBlanc.Distance(target) < Q.Range)
            {
                if (useQ)
                {
                    CastQ(target);
                }
                if (useE && IsPassive(target) && !Q.IsReady())
                {
                    CastE(target);
                }
                if (useR && IsPassive(target))
                {
                    CastR("RQ", target);
                }
                if (useW && IsPassive(target) && IsW1() && !RActive.IsReady())
                {
                    CastW(target.ServerPosition);
                }
            }
            else if (LeBlanc.Distance(target) < E.Range)
            {
                if (useE)
                {
                    CastE(target);
                }
                if (IsPassive(target) && useQ && !E.IsReady())
                {
                    CastQ(target);
                }
                if (UseRQ && IsPassive(target) && !Q.IsReady())
                {
                    CastR("RQ", target);
                }
                if (useW && IsW1() && !RActive.IsReady())
                {
                    CastW(target.ServerPosition);
                }
            }
            else if (LeBlanc.Distance(target) < Q.Range)
            {
                if (useQ)
                {
                    CastQ(target);
                }
                if (useE && !Q.IsReady())
                {
                    CastE(target);
                }
                if (UseRE)
                {
                    CastR("RE", target);
                }
                if (useW && IsW1() && !RActive.IsReady())
                {
                    CastW(target.ServerPosition);
                }
            }
            else if (LeBlanc.Distance(target) < E.Range)
            {
                if (useE && IsPassive(target))
                {
                    CastE(target);
                }

                if (useQ && IsPassive(target))
                {
                    CastQ(target);
                }
                if (UseRE)
                {
                    CastR("RE", target);
                }
                if (useW && IsW1() && !RActive.IsReady())
                {
                    CastW(target.ServerPosition);
                }
                else
                {
                    if (LeBlanc.Distance(target) < W.Range)
                    {
                        if (useQ)
                        {
                            CastQ(target);
                        }
                        if (useE && !Q.IsReady())
                        {
                            CastE(target);
                        }
                        if (UseRW)
                        {
                            CastR("RW", target);
                        }
                        if (UseRW && IsW1() && !RActive.IsReady())
                        {
                            CastW(target.ServerPosition);
                        }
                    }
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