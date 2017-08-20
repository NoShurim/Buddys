using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Rendering;
using SharpDX;
using EloBuddy.SDK.Menu.Values;
using System.Collections.Generic;
using System.Linq;

namespace Akali_Beta_Fixed
{
    class MyChampion
    {
        private static AIHeroClient Akali => Player.Instance;
        public static Menu Aka, Combo, Harass, LaneClear, JungleClear, LastHit, Misc, Draws;
        static void Main(string[] args) { Loading.OnLoadingComplete += Loading_OnComplete; }
        //logic
        public static bool LogicTarget(AIHeroClient target, float range, SpellSlot key)
        {
            var x = target;
            if (x.IsValidTarget(range) && !x.HasBuffOfType(BuffType.Invulnerability) &&
                x.TotalShieldHealth() + 5 <= Player.Instance.GetSpellDamage(x, key))
            {
                return true;
            }

            return false;
        }

        public static List<Obj_AI_Minion> Minions(EntityManager.UnitTeam team, float range, Vector3 from = default(Vector3))
        {
            return EntityManager.MinionsAndMonsters.GetLaneMinions(team, from, range).ToList();
        }

        //LogicMonster
        public static List<Obj_AI_Minion> Monsters(float range, Vector3 from = default(Vector3))
        {
            return EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(x => x.IsInRange(from, range)).ToList();
        }

        public static bool LogicMinion(Obj_AI_Minion target, float range, SpellSlot key)
        {
            var x = target;
            if (x.IsValidTarget(range) && x.TotalShieldHealth() <= Player.Instance.GetSpellDamage(x, key))
            {
                return true;
            }

            return false;
        }

        public static List<AIHeroClient> Enemies(float range = 1500, Vector3 from = default(Vector3))
        {
            return EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(range, false, from)).ToList();
        }

        public static List<AIHeroClient> Allies(float range = 1500)
        {
            return EntityManager.Heroes.Allies.Where(a => a.IsValidTarget(range) && !a.IsMe).ToList();
        }

        //items
        public static Item Bilge = new Item(ItemId.Bilgewater_Cutlass, 550);
        public static Item Botrk = new Item(ItemId.Blade_of_the_Ruined_King, 550);
        public static Item Hextech = new Item(ItemId.Hextech_Gunblade, 700);
        public static Item Tiamat = new Item(ItemId.Tiamat_Melee_Only, 400);
        public static Item Hydra = new Item(ItemId.Ravenous_Hydra_Melee_Only, 400);
        public static Item Titanic = new Item(ItemId.Titanic_Hydra, Player.Instance.AttackRange);
        public static Item Youmuus = new Item(ItemId.Youmuus_Ghostblade, Player.Instance.AttackRange + Player.Instance.BoundingRadius + 300);

        public static List<Item> ItemList = new List<Item>
        {
            Tiamat,
            Hydra,
            Bilge,
            Botrk,
            Hextech,
            Titanic,
            Youmuus
        };

        public static Spell.Targeted Q;
        public static Spell.Active W;
        public static Spell.Active E;
        public static Spell.Targeted R;

        public static bool CastCheckbox(Menu obj, string value)
        {
            return obj[value].Cast<CheckBox>().CurrentValue;
        }

        public static int CastSlider(Menu obj, string value)
        {
            return obj[value].Cast<Slider>().CurrentValue;
        }

        private static void Loading_OnComplete(EventArgs args)
        {
            if (Akali.Hero != Champion.Akali) return;
            Chat.Print("[Addon] [Champion] [Akali]", System.Drawing.Color.AliceBlue);

            Q = new Spell.Targeted(SpellSlot.Q, 600);
            W = new Spell.Active(SpellSlot.W, 275);
            E = new Spell.Active(SpellSlot.E, 300);
            R = new Spell.Targeted(SpellSlot.R, 700);
            //Comands

            //Events
            Orbwalker.OnPostAttack += OnPostAttack;
            Gapcloser.OnGapcloser += OnGapcloser;
            Drawing.OnDraw += OnDrawings;
            Game.OnTick += Game_OnTick;

            Aka = MainMenu.AddMenu("Akali", "Akali");
            //Combo
            Combo = Aka.AddSubMenu("Combo");
            Combo.Add("Qc", new CheckBox("Use [Q] Combo"));
            Combo.Add("Wc", new CheckBox("Use [W] Combo"));
            Combo.Add("Ec", new CheckBox("Use [E] Combo"));
            Combo.Add("Rc", new CheckBox("Use [R] Combo"));
            //Harass
            Harass = Aka.AddSubMenu("Harass");
            Harass.Add("Hq", new CheckBox("Use [Q] Harass"));
            //LaneClear
            LaneClear = Aka.AddSubMenu("LaneClear");
            LaneClear.Add("Ql", new CheckBox("Use [Q] LaneClear"));
            LaneClear.Add("El", new CheckBox("Use [E] LaneClear"));
            LaneClear.AddSeparator();
            LaneClear.AddLabel("[Percent Mana]");
            LaneClear.Add("mana", new Slider("Percent Mana > %", 25, 0, 100));
            //LastHit
            LastHit = Aka.AddSubMenu("LastHit");
            LastHit.Add("Qlast", new CheckBox("Use [Q] LastHit"));
            //JungleClear
            JungleClear = Aka.AddSubMenu("JungleClear");
            JungleClear.Add("Qj", new CheckBox("Use [Q] Jungle"));
            JungleClear.Add("Ej", new CheckBox("Use [E] Jungle"));
            JungleClear.AddSeparator();
            JungleClear.AddLabel("[Percent Mana]");
            JungleClear.Add("manaj", new Slider("Percent Mana > % ", 25, 0, 100));
            //Misc
            Misc = Aka.AddSubMenu("Misc");
            Misc.Add("Rgap", new CheckBox("Use [R] GapClose"));
            Misc.Add("Fw", new CheckBox("Use Flee [W]"));
            Misc.Add("It", new CheckBox("Use Items"));
            //Draws
            Draws = Aka.AddSubMenu("Drawings");
            Draws.Add("Dq", new CheckBox("[Q] Draw"));
            Draws.Add("Dw", new CheckBox("[W] Draw", false));
            Draws.Add("De", new CheckBox("[E] Draw", false));
            Draws.Add("Dr", new CheckBox("[R] Draw"));

        }

        private static void ActiveItems(AIHeroClient target)
        {
            foreach (var item in ItemList.Where(i => i.IsReady() && target.IsValidTarget(i.Range)))
            {
                item.Cast(target);
            }
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
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                ByLast();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                ByHarass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                ByFlee();
            }
        }

        private static void ByCombo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var useQ = CastCheckbox(Combo, "Qc");
            var useW = CastCheckbox(Combo, "Wc");
            var useE = CastCheckbox(Combo, "Ec");
            var useR = CastCheckbox(Combo, "Rc");
            var useIn = CastCheckbox(Misc, "It");

            if (target == null || !target.IsValidTarget()) return;

            if (useIn)
            {
                ActiveItems(target);
            }

            if (useQ && Q.IsReady())
            {
                Q.Cast(target);
            }

            if (useR && R.IsReady() && !Player.Instance.IsInRange(target, E.Range))
            {
                R.Cast(target);
            }

            if (useW && W.IsReady() && Player.Instance.CountEnemiesInRange(Q.Range) >= 2)
            {
                W.Cast();
            }
        }

        private static void ByLane()
        {
            var useQ = CastCheckbox(LaneClear, "Ql");
            var useE = CastCheckbox(LaneClear, "El");
            var mana = CastSlider(LaneClear, "mana");

            if (Player.Instance.ManaPercent < mana) return;

            var minion = Minions(EntityManager.UnitTeam.Enemy, Q.Range, Player.Instance.ServerPosition).FirstOrDefault();

            if (minion == null || !minion.IsValidTarget(Q.Range)) return;

            if (useQ && Q.IsReady())
            {
                Q.Cast(minion);
            }

            if (useQ && E.IsReady() && minion.IsValidTarget(E.Range))
            {
                E.Cast();
            }
        }

        private static void ByJungle()
        {
            var useQ = CastCheckbox(JungleClear, "Qj");
            var useE = CastCheckbox(JungleClear, "Ej");
            var mana = CastSlider(JungleClear, "manaj");

            if (Player.Instance.ManaPercent < mana) return;
            var monster = Monsters(Q.Range, Player.Instance.ServerPosition).FirstOrDefault();

            if (monster == null || !monster.IsValidTarget()) return;

            if (useQ && Q.IsReady())
            {
                Q.Cast(monster);
            }
        }

        private static void ByLast()
        {
            var qminion = Minions(EntityManager.UnitTeam.Enemy, Q.Range, Player.Instance.ServerPosition).FirstOrDefault();
            var useQlast = CastCheckbox(LastHit, "Qlast");
            var qiskillable = LogicMinion(qminion, Q.Range, SpellSlot.Q);

            if (qminion != null && !Player.Instance.IsInAutoAttackRange(qminion))
            {
                if (useQlast && Q.IsReady() && qiskillable)
                {
                    Q.Cast(qminion);
                }
            }
        }

        private static void ByHarass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var useQh = CastCheckbox(Harass, "Hq");
            if (target == null || !target.IsValidTarget(Q.Range)) return;

            if (useQh && Q.IsReady())
            {
                Q.Cast(target);
            }
        }

        private static void ByFlee()
        {
            var useWfl = CastCheckbox(Misc, "Fw");

            if (useWfl && W.IsReady())
            {
                W.Cast();
            }
        }

        private static void OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (target == null || !(target is AIHeroClient) || target.IsDead || target.IsInvulnerable ||
                    !target.IsEnemy || target.IsPhysicalImmune || target.IsZombie || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                return;

            var enemy = target as AIHeroClient;
            if (enemy == null)
                return;

            if (E.IsReady())
            {
                E.Cast(enemy);
            }
        }

        private static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            var RGap = CastCheckbox(Misc, "Rgap");

            if (!sender.IsEnemy || sender.IsUnderEnemyturret()) return;

            if (RGap && R.IsReady() && sender.IsValidTarget(R.Range))
            {
                R.Cast(sender);
            }
        }

        private static void OnDrawings(EventArgs args)
        {
            if (CastCheckbox(Draws, "Dq"))
            {
                Circle.Draw(Color.Aqua, Q.Range, Player.Instance.Position);
            }
            if (CastCheckbox(Draws, "Dw"))
            {
                Circle.Draw(Color.Aqua, W.Range, Player.Instance.Position);
            }
            if (CastCheckbox(Draws, "De"))
            {
                Circle.Draw(Color.Aqua, E.Range, Player.Instance.Position);
            }
            if (CastCheckbox(Draws, "Dr"))
            {
                Circle.Draw(Color.Aqua, R.Range, Player.Instance.Position);
            }
        }
    }
}