using EloBuddy;
using EloBuddy.SDK.Events;
using System;
using static Rengar_Beta_Fixed.Menus;
using static Rengar_Beta_Fixed.SpellManager;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;
using System.Collections.Generic;
using EloBuddy.SDK.Menu;

namespace Rengar_Beta_Fixed
{
    class Kitten
    {
        public static AIHeroClient Gatinho => Player.Instance;
        public static readonly List<string> SpecialChampions = new List<string> { "Annie", "Jhin" };

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
            Loading.OnLoadingComplete += Loadin_OnComple;
        }

        private static void Loadin_OnComple(EventArgs args)
        {
            if (Gatinho.Hero != Champion.Rengar) return;

            Menus.Execute();
            SpellManager.Execute();

            Drawing.OnDraw += Draws_DrawsOn;
            Game.OnTick += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Gatinho.IsDead)
            {
                return;
            }

            if (Combo["autow"].Cast<CheckBox>().CurrentValue)
            {
                if (Player.HasBuffOfType(BuffType.Charm) || Player.HasBuffOfType(BuffType.Stun) || Player.HasBuffOfType(BuffType.Fear) || Player.HasBuffOfType(BuffType.Snare) || Player.HasBuffOfType(BuffType.Taunt) || Player.HasBuffOfType(BuffType.Knockback) || Player.HasBuffOfType(BuffType.Suppression))
                {

                    if (Gatinho.Mana == 4)
                    {
                        W.Cast();
                    }

                }
            }
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
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                ByHarass();
            }
            if (Combo["KeyR"].Cast<KeyBind>().CurrentValue)
            {
                if (R.IsReady())
                {
                    R.Cast();
                }
            }
        }

        private static void ByCombo()
        {
            var useQ = CastCheckbox(Combo, "Qc");
            var useW = CastCheckbox(Combo, "Wc");
            var useE = CastCheckbox(Combo, "Ec");
            var eq = CastCheckbox(Combo, "eq");
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);

            if (!target.IsValidTarget() || Player.HasBuff("RengarR"))
            {
                return;
            }

            if (Gatinho.Mana == 4)
            {
                if (eq)
                {
                    if (target.Distance(Gatinho) > Q.Range)
                    {
                        if (target.IsValidTarget(E.Range) && target != null)
                        {
                            E.Cast(target);
                        }
                    }
                }
                switch (Combo["ps"].Cast<ComboBox>().SelectedIndex)
                {
                    case 0:
                        if (target.IsValidTarget(Q.Range) && target != null)
                        {
                            Q.Cast(target);
                        }

                        break;
                    case 1:

                        if (target.IsValidTarget(W.Range - 100) && target != null)
                        {
                            W.Cast();
                        }

                        break;
                    case 2:


                        if (target.IsValidTarget(E.Range - 100) && target != null)
                        {
                            E.Cast(target);
                        }

                        break;
                }
            }
            if (Gatinho.Mana < 4)
            {
                if (E.IsReady() && useE && target.IsValidTarget(E.Range))
                {
                    if (target != null)
                    {
                        E.Cast(target);
                    }
                }
                if (W.IsReady() && useW && target.IsValidTarget(W.Range - 100))

                {
                    if (target != null)
                    {
                        W.Cast();
                    }
                }
                if (Q.IsReady() && useQ && target.IsValidTarget(Q.Range))
                {
                    if (target != null)
                    {
                        Q.Cast(target);
                    }
                }
            }
        }

        private static void ByLane()
        {
            var stacks = CastCheckbox(Lane, "stack");
            var useQ = CastCheckbox(Lane, "Ql");
            var useW = CastCheckbox(Lane, "Wl");
            var useE = CastCheckbox(Lane, "El");
            var hitsw = CastSlider(Lane, "Wm");
            var minios = EntityManager.MinionsAndMonsters.Get(EntityManager.MinionsAndMonsters.EntityType.Minion, EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition, Q.Range);
            var miniosW = EntityManager.MinionsAndMonsters.Get(EntityManager.MinionsAndMonsters.EntityType.Minion, EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition, W.Range);
            var miniosE = EntityManager.MinionsAndMonsters.Get(EntityManager.MinionsAndMonsters.EntityType.Minion, EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition, E.Range);

            if (stacks)
            {
                if (Gatinho.Mana == 4)
                {
                    return;
                }
            }
            if (Gatinho.Mana == 4)
            {
                switch (Lane["ps"].Cast<ComboBox>().SelectedIndex)
                {
                    case 0:
                        foreach (var minion in minios)
                        {


                            if (minion.IsValidTarget(Q.Range) && minion != null)
                            {
                                Q.Cast(minion);
                            }
                        }
                        break;
                    case 1:
                        foreach (var minion in miniosW)
                        {

                            if (minion.IsValidTarget(W.Range - 100) && minion != null)
                            {
                                W.Cast();
                            }
                        }
                        break;
                    case 2:
                        foreach (var minion in miniosE)
                        {


                            if (minion.IsValidTarget(E.Range - 100) && minion != null)
                            {
                                E.Cast();
                            }
                        }
                        break;
                }
            }
            if (Gatinho.Mana < 4)
            {
                if (useE)
                {
                    foreach (var minion in miniosE)
                    {


                        if (minion.IsValidTarget(E.Range) && minion != null)
                        {
                            E.Cast(minion);
                        }
                    }
                }
                if (useW)
                {
                    foreach (var minion in miniosW)
                    {


                        if (minion.IsValidTarget(W.Range - 100) && minion != null)
                        {
                            W.Cast();
                        }
                    }
                }
                if (useQ)
                {
                    foreach (var minion in minios)
                    {


                        if (minion.IsValidTarget(Q.Range) && minion != null)
                        {
                            Q.Cast(minion);
                        }
                    }
                }
            }
        }

        private static void ByJungle()
        {
            var stacks = CastCheckbox(Jungle, "stack");
            var useQ = CastCheckbox(Jungle, "Qj");
            var useW = CastCheckbox(Jungle, "Wj");
            var useE = CastCheckbox(Jungle, "Ej");
            var mobs = EntityManager.MinionsAndMonsters.Get(EntityManager.MinionsAndMonsters.EntityType.Monster, EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition, Q.Range);
            foreach (var monsters in mobs)
            {
                if (!monsters.IsValidTarget())
                {
                    return;
                }
              
                if (stacks)
                {
                    if (Gatinho.Mana == 4)
                    {
                        return;
                    }
                }


                if (Gatinho.Mana == 4)
                {
                    switch (Jungle["ps"].Cast<ComboBox>().SelectedIndex)
                    {
                        case 0:
                            if (monsters.IsValidTarget(Q.Range) && monsters != null)
                            {
                                Q.Cast(monsters);
                            }

                            break;
                        case 1:

                            if (monsters.IsValidTarget(W.Range - 100) && monsters != null)
                            {
                                W.Cast();
                            }

                            break;
                        case 2:


                            if (monsters.IsValidTarget(E.Range - 100) && monsters != null)
                            {
                                E.Cast(monsters);
                            }

                            break;
                    }
                }
                if (Gatinho.Mana < 4)
                {
                    if (useE)
                    {

                        if (monsters.IsValidTarget(E.Range) && monsters != null)
                        {
                            E.Cast(monsters);
                        }
                    }

                    if (useW)
                    {

                        if (monsters.IsValidTarget(W.Range - 100) && monsters != null)
                        {
                            W.Cast();
                        }
                    }

                    if (useQ)
                    {
                        if (monsters.IsValidTarget(Q.Range) && monsters != null)
                        {
                            Q.Cast(monsters);
                        }
                    }
                }
            }
        }

        private static void ByHarass()
        {
            var useQ = CastCheckbox(Harass, "Hq");
            var useW = CastCheckbox(Harass, "Hw");
            var useE = CastCheckbox(Harass, "He");
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);

            if (!target.IsValidTarget())
            {
                return;
            }
            if (Gatinho.Mana == 4)
            {
                switch (Harass["ps"].Cast<ComboBox>().SelectedIndex)
                {
                    case 0:


                        if (target.IsValidTarget(Q.Range) && target != null)
                        {
                            Q.Cast(target);
                        }

                        break;
                    case 1:

                        if (target.IsValidTarget(W.Range - 100) && target != null)
                        {
                            W.Cast();
                        }

                        break;
                    case 2:


                        if (target.IsValidTarget(E.Range - 100) && target != null)
                        {
                            E.Cast(target);
                        }

                        break;
                }
            }
            if (Gatinho.Mana < 4)
            {
                if (E.IsReady() && useE && target.IsValidTarget(E.Range))
                {
                    if (target != null)
                    {
                        E.Cast(target);
                    }
                }
                if (W.IsReady() && useW && target.IsValidTarget(W.Range - 100))

                {
                    if (target != null)
                    {
                        W.Cast();
                    }
                }
                if (Q.IsReady() && useQ && target.IsValidTarget(Q.Range))
                {

                    if (target != null)
                    {
                        Q.Cast(target);
                    }

                }
            }
        }

        private static void Draws_DrawsOn(EventArgs args)
        {
            if (Draws["DQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                Q.DrawRange(System.Drawing.Color.LightBlue);
            }
            if (Draws["DR"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                R.DrawRange(System.Drawing.Color.LightBlue);
            }
        }
    }
}