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

        public static bool IsW1 => LeBlanc.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancw";
        public static bool IsW2 => LeBlanc.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() != "leblancw";
        public static bool IsR1 => LeBlanc.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancrtoggle";
        public static bool IsR2 => LeBlanc.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() != "leblancrtoggle";
        public static bool IsPassive(Obj_AI_Base hero)
        {
            return hero.HasBuff("LeblancPMark") && Game.Time - hero.GetBuff("LeblancPMark").StartTime > 1;
        }
        public static bool IsPassiveM(Obj_AI_Base hero)
        {
            return hero.HasBuff("leblancpminion") && Game.Time - hero.GetBuff("leblancpminion").StartTime > 1;
        }

        public static bool CastCheckbox(Menu obj, string value)
        {
            return obj[value].Cast<CheckBox>().CurrentValue;
        }
        public static int CastSlider(Menu obj, string value)
        {
            return obj[value].Cast<Slider>().CurrentValue;
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
            Drawing.OnDraw += OnDraws;
            Gapcloser.OnGapcloser += AntiGapcloser_Execute;
            GameObject.OnCreate += ObjectOnCreate;
            GameObject.OnDelete += ObjectOnDelete;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private static void AntiGapcloser_Execute(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender.IsAlly || sender.IsDead || sender.IsMe) return;

            if (CastCheckbox(Menus.AntiGapcloser, "E"))
            {
                if (Lib.E.IsReady())
                {
                    var epred = Lib.E.GetPrediction(sender);
                    if (epred.HitChance >= HitChance.Medium)
                    {
                        Lib.E.Cast(epred.CastPosition);
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
        }

        private static void ByLane()
        {
            var useLQ = CastCheckbox(Menus.Lane, "Q");
            var useLW = CastCheckbox(Menus.Lane, "W");
            var Mana = CastSlider(Menus.Lane, "Mana");

            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(min => min.IsEnemy && !min.IsDead && min.IsValid && !min.IsInvulnerable && min.IsInRange(LeBlanc.Position, Q.Range));
            var minion = EntityManager.MinionsAndMonsters.Get(EntityManager.MinionsAndMonsters.EntityType.Minion, EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition, Q.Range);
            if (useLQ && useLW && LeBlanc.ManaPercent > Mana)
            {
                foreach (var mayminoon in minions)
                {
                    if (W.GetPrediction(mayminoon).CollisionObjects.Where(may => may.IsEnemy && !may.IsDead && may.IsValid && !may.IsInvulnerable).Count() >= 3)
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

        private static void ByJungle()
        {
            throw new NotImplementedException();
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
                if (useW && IsPassive(target) && IsW1 && !RActive.IsReady())
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
                if (useQ && !E.IsReady())
                {
                    CastQ(target);
                }
                if (UseRQ && !Q.IsReady())
                {
                    CastR("RQ", target);
                }
                if (useW && IsW1 && !RActive.IsReady())
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
                if (useW && IsW1 && !RActive.IsReady())
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
                if (useW && IsW1 && !RActive.IsReady())
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
                        if (useW && IsW1 && !RActive.IsReady())
                        {
                            CastW(target.ServerPosition);
                        }
                    }
                }
            }
        }
        private static void ObjectOnCreate(GameObject sender, EventArgs args)
        {

        }

        private static void ObjectOnDelete(GameObject sender, EventArgs args)
        {

        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {

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