using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System;
using System.Collections.Generic;
using static Katarina_Beta_Fixed.DaggersLogics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace Katarina_Beta_Fixed
{
    class Program
    {
        public static AIHeroClient Katarina => Player.Instance;

        public static Spell.Targeted Q;
        public static Spell.Active W;
        public static Spell.Skillshot E;
        public static Spell.Active R;
        public static Spell.Targeted Ignite;
        public static Item Hextech = new Item(ItemId.Hextech_Gunblade, 700);

        public static List<Item> ItemList = new List<Item>
        {
            Hextech
        };

        public static void CastItems(AIHeroClient target)
        {
            foreach (var item in ItemList.Where(i => i.IsReady() && target.IsValidTarget(i.Range)))
            {
                item.Cast(target);
            }
        }

        public static Menu Kat, Combo, Harass, Farming, Draws;

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
            Loading.OnLoadingComplete += Loading_OnComple;
        }

        private static void Loading_OnComple(EventArgs args)
        {
            if (Katarina.Hero != Champion.Katarina) return;
            Chat.Print("[Addon] [Champion] [Katarina]", System.Drawing.Color.AliceBlue);

            Q = new Spell.Targeted(SpellSlot.Q, 600, DamageType.Magical);
            W = new Spell.Active(SpellSlot.W, 150, DamageType.Magical);
            E = new Spell.Skillshot(SpellSlot.E, 700, SkillShotType.Circular, 7, null, 150, DamageType.Magical);
            R = new Spell.Active(SpellSlot.R, 550, DamageType.Magical);
            Ignite = new Spell.Targeted(SpellSlot.Summoner1, 600);

            Kat = MainMenu.AddMenu("Katarina", "Katarina");
            //Combo
            Combo = Kat.AddSubMenu("Combo");
            Combo.Add("modec", new ComboBox("Combo Mode", 1, "Q + E", "E + Q"));
            Combo.Add("Qc", new CheckBox("Use [Q]"));
            Combo.Add("Wc", new CheckBox("Use [W]"));
            Combo.Add("Ec", new CheckBox("Use [E]"));
            Combo.Add("Rc", new CheckBox("Use [R]"));
            Combo.Add("savee", new CheckBox("Save E if no Daggers", false));
            Combo.Add("modee", new ComboBox("Mode [E]", 1, "Infront", "Behind", "Logic"));
            Combo.AddSeparator();
            Combo.AddLabel("Settings [R]");
            Combo.Add("moder", new ComboBox("[R] Mode", 1, "Always", "Only if Killable"));
            Combo.Add("dagger", new Slider("Use X R Daggers for damge chack", 8, 1, 16));
            Combo.Add("rit", new Slider("Enemys Use [R]", 1, 1, 5));
            Combo.Add("cancel", new CheckBox("Cancel [R] is no Enemys (Check Damage)"));
            Combo.Add("cancelKs", new CheckBox("Cancel [R] is no Enemy KillSteal"));
            Combo.AddSeparator();
            Combo.AddLabel("Percent Life [R]");
            Combo.Add("rlife", new Slider("Don't waste R if Enemy HP lower than", 100, 0, 500));
            //Harass
            Harass = Kat.AddSubMenu("Harass");
            Harass.Add("modeh", new ComboBox("Harass Mode", 0, "Q > E", "E > Q"));
            Harass.Add("Hq", new CheckBox("Use [Q] Harass"));
            Harass.Add("Hw", new CheckBox("Use [W] Harass"));
            Harass.Add("He", new CheckBox("Use [E] Harass"));
            //Farm
            Farming = Kat.AddSubMenu("Farming");
            Farming.Add("Qf", new CheckBox("Use [Q]"));
            Farming.Add("lastQ", new CheckBox("Use [Q] LastHit"));
            Farming.Add("lastaa", new CheckBox("Don't Last Hit in AA Range"));
            Farming.Add("Wf", new CheckBox("Use [W]"));
            Farming.Add("whit", new Slider("Percent Mininos [W]", 3, 1, 6));
            Farming.Add("Ef", new CheckBox("Use [E]"));
            Farming.Add("hite", new Slider("if Daggers hits", 3, 1, 6));
            Farming.Add("turrent", new CheckBox("Don't E Under the Turret"));
            //Draws
            Draws = Kat.AddSubMenu("Drawings");
            Draws.Add("DQ", new CheckBox("Draws [Q]"));
            Draws.Add("DE", new CheckBox("Draws [E]"));
            Draws.AddSeparator();
            Draws.AddLabel("[Draws] Logic [Dagger]");
            Draws.Add("drawdag", new CheckBox("Draws Dagger"));


            Drawing.OnDraw += Drawwings;
            Game.OnUpdate += Game_OnUpdate;
            Game.OnTick += Game_UpdateGame;
            Obj_AI_Base.OnProcessSpellCast += OnCastSpell;
            Obj_AI_Base.OnBuffGain += OnBuffGain;
            Obj_AI_Base.OnBuffLose += OnBuffLose;
        }

        private static void Game_UpdateGame(EventArgs args)
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (meowmeowRthing != true)
            {
                CastItems(target);
            }
        }

        private static void OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (args.Buff.Name.ToLower() == "katarinarsound" || args.Buff.Name.ToLower() == "katarinar")
            {
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
                meowmeowRthing = true;
            }
        }

        private static void OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (args.Buff.Name.ToLower() == "katarinarsound" || args.Buff.Name.ToLower() == "katarinar")
            {
                Orbwalker.DisableAttacking = false;
                Orbwalker.DisableMovement = false;
                meowmeowRthing = false;
            }
        }

        private static void Drawwings(EventArgs args)
        {
            var dagger = GetDaggers();

            if (CastCheckbox(Draws, "DQ"))
            {
                Circle.Draw(Color.Aqua, Q.Range, Player.Instance.Position);
            }
            if (CastCheckbox(Draws, "DE"))
            {
                Circle.Draw(Color.Aqua, E.Range, Player.Instance.Position);
            }
            if (CastCheckbox(Draws, "drawdag"))
            {
                foreach (var daggers in dagger)
                {
                    if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid)
                    {

                        if (daggers.CountEnemyChampionsInRange(450) != 0)
                        {
                            Circle.Draw(Color.LawnGreen, 450, daggers.Position);
                            Circle.Draw(Color.LawnGreen, 150, daggers.Position);
                        }
                        if (daggers.CountEnemyChampionsInRange(450) == 0)
                        {
                            Circle.Draw(Color.Red, 450, daggers.Position);
                            Circle.Draw(Color.Red, 150, daggers.Position);
                        }
                    }
                }
            }
        }

        private static void OnCastSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.Slot == SpellSlot.W)
            {
                timeW = Game.Time + 1200;
            }

            if (sender.IsMe)
            {
                if ((args.Slot == SpellSlot.Q || args.Slot == SpellSlot.W || args.Slot == SpellSlot.E ||
                     args.Slot == SpellSlot.Item1 || args.Slot == SpellSlot.Item2 || args.Slot == SpellSlot.Item3 ||
                     args.Slot == SpellSlot.Item4 || args.Slot == SpellSlot.Item5 || args.Slot == SpellSlot.Item6) &&
                    Katarina.HasBuff("katarinarsound"))

                {
                    args.Process = false;

                }
            }
        }
        static double GetR(Obj_AI_Base target)
        {
            double meow = 0;
            if (Katarina.Spellbook.GetSpell(SpellSlot.R).Level == 1)
            {
                meow = 25;
            }
            if (Katarina.Spellbook.GetSpell(SpellSlot.R).Level == 2)
            {
                meow = 37.5;
            }
            if (Katarina.Spellbook.GetSpell(SpellSlot.R).Level == 3)
            {
                meow = 50;
            }
            double ap = Katarina.BaseAbilityDamage * 0.19;
            double ad = (Katarina.TotalAttackDamage - Katarina.BaseAttackDamage) * 0.22;
            double damage = Katarina.CalculateDamageOnUnit(target, DamageType.Magical, 300);
            return damage;

        }
        public static double Passive(Obj_AI_Base target)
        {
            double dmg = 0;
            double yay = 0;
            var d = GetClosestDagger();
            var dd = GetDaggers();

            foreach (var dagger in dd)
            {

                if (dagger != null)
                {
                    if (dagger.Distance(target) < 450)
                    {
                        if (Katarina.Level >= 1 && Katarina.Level < 6)
                        {
                            dmg = 0.55;
                        }
                        if (Katarina.Level >= 6 && Katarina.Level < 11)
                        {
                            dmg = 0.7;
                        }
                        if (Katarina.Level >= 11 && Katarina.Level < 16)
                        {
                            dmg = 0.85;
                        }
                        if (Katarina.Level >= 16)
                        {
                            dmg = 1;
                        }
                        double psv = 35 + (Katarina.Level * 12);
                        double psvdmg = Katarina.TotalMagicalDamage * dmg;
                        double full = psvdmg + psv + (Katarina.TotalAttackDamage - Katarina.BaseAttackDamage);
                        double damage = Katarina.CalculateDamageOnUnit(target, DamageType.Magical, 200);
                        yay = damage;
                    }

                }
            }
            return yay;
        }

        public static readonly List<string> SpecialChampions = new List<string> { "Annie", "Jhin" };

        public static int SxOffset(Obj_AI_Base target)
        {
            return SpecialChampions.Contains(target.BaseSkinName) ? 1 : 10;
        }

        public static int SyOffset(Obj_AI_Base target)
        {
            return SpecialChampions.Contains(target.BaseSkinName) ? 3 : 20;
        }

        private static float timeW;
        private static bool meowmeowRthing;
        private static float meowwwwwwwwwww;


        private static void Game_OnUpdate(EventArgs args)
        {           
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                OnCombo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Clearing();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                OnHarass();
            }
            KillSteal();
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
                    var dmgI = (50 + ((Katarina.Level) * 20));
                    if (Katarina.Distance(hptarget) < Q.Range && Health < dmgI)

                    {
                        Ignite.Cast(hptarget);
                    }
                }
            }
        }

        private static void OnCombo()
        {
            var useQ = CastCheckbox(Combo, "Qc");
            var useW = CastCheckbox(Combo, "Wc");
            var SaveE = CastCheckbox(Combo, "savee");
            var useE = CastCheckbox(Combo, "Ec");
            var useR = CastCheckbox(Combo, "Rc");
            var cancel = CastCheckbox(Combo, "cancel");
            var ksR = CastCheckbox(Combo, "cancelKs");
            var hitR = CastSlider(Combo, "rit");
            var dagggggggers = CastSlider(Combo, "dagger");
            var meow = CastSlider(Combo, "rlife");
            var dagger = GetDaggers();
            var d = GetClosestDagger();
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (cancel)
            {
                if (Katarina.HasBuff("katarinarsound"))
                {
                    if (Katarina.CountEnemyChampionsInRange(R.Range) == 0)
                    {
                        //Player.(MoveTo, Game.CursorPos);
                    }
                }
            }
            if (ksR)
            {
                if (target != null)
                {
                    if (Player.HasBuff("katarinarsound"))
                    {
                        if (target.Distance(Katarina) >= R.Range - 100 && E.IsReady())
                        {
                            //Player.IssueOrder(OrderType.MoveTo, Game.CursorPos);
                            foreach (var daggers in dagger)

                            {
                                if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid)
                                {
                                    if (target.Distance(daggers) < 450 &&
                                        target.IsValidTarget(E.Range) && E.IsReady())

                                    {

                                        E.Cast(LogicDistacne(d, target));//200


                                    }

                                    if (daggers.Distance(Katarina) > E.Range)
                                    {
                                        E.Cast(LogicInstance(d, target));//-50
                                    }
                                    if (daggers.Distance(target) > 450)
                                    {

                                        E.Cast(LogicInstance(d, target));//-50
                                    }
                                    break;

                                }
                                if (dagger.Count() == 0)
                                {

                                    E.Cast(LogicInstance(d, target));//-50
                                }
                            }
                        }
                        if (Katarina.GetSpellDamage(target, SpellSlot.Q) + Katarina.GetSpellDamage(target, SpellSlot.E) >= target.Health)
                        {
                            foreach (var daggers in dagger)
                            {
                                if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid && E.IsReady())
                                {
                                    //Player.IssueOrder(OrderType.MoveTo, Game.CursorPos);
                                    if (target.Distance(daggers) < 450 &&
                                        target.IsValidTarget(E.Range) && E.IsReady())

                                    {

                                        E.Cast(LogicDistacne(d, target));//200


                                    }
                                    if (daggers.Distance(Katarina) > E.Range)
                                    {
                                        E.Cast(LogicInstance(d, target));
                                    }
                                    if (daggers.Distance(target) > 450)
                                    {

                                        E.Cast(LogicInstance(d, target));
                                    }
                                }
                                if (dagger.Count() == 0 && E.IsReady())
                                {
                                    //Player.IssueOrder(OrderType.MoveTo, Game.CursorPos);
                                    E.Cast(LogicInstance(d, target));
                                }

                                if (target.IsValidTarget(Q.Range) && Q.IsReady())
                                {
                                    //Player.IssueOrder(OrderType.MoveTo, Game.CursorPos);
                                    Q.Cast(target);
                                }

                            }

                        }
                    }
                }
            }
            /*if (!target.IsValidTarget())
            {
                return;
            }
            if (target == null)
            {
                return;
            }
            if (!Player.HasBuff("katarinarsound"))
            {
                var items = new[] { ItemId.Hextech_Gunblade, ItemId.Bilgewater_Cutlass };
                if (Katarina.HasItem(ItemId.Hextech_Gunblade) || Katarina.HasItem(ItemId.Bilgewater_Cutlass))
                {
                    var slot = Katarina.InventoryItems.Slots.First(s => items.Contains(s.ItemId));
                    if (slot != null)
                    {

                        var spellslot = slot.SpellSlot;
                        if (spellslot != SpellSlot.Unknown &&
                            Katarina.Spellbook.GetSpell(spellslot).State == SpellState.Ready)
                        {
                            Katarina.Spellbook.CastSpell(spellslot, target);


                        }
                    }
                }
            }*/

            switch (Combo["modec"].Cast<ComboBox>().SelectedIndex)
            {
                case 0:
                    if (!target.IsValidTarget())
                    {
                        return;
                    }
                    if (Q.IsReady() && useQ && target.IsValidTarget(Q.Range))
                    {
                        if (target != null)
                        {
                            Q.Cast(target);
                        }
                    }

                    if (E.IsReady() && useE && target.IsValidTarget(E.Range) && !Q.IsReady())
                    {
                        if (target != null)
                        {
                            foreach (var daggers in dagger)

                            {
                                if (!SaveE)
                                {
                                    if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid)
                                    {
                                        if (target.Distance(daggers) < 450 &&
                                            target.IsValidTarget(E.Range))
                                        {

                                            E.Cast(LogicDistacne(d, target));


                                        }
                                        switch (Combo["modee"].Cast<ComboBox>().SelectedIndex)
                                        {
                                            case 0:
                                                if (daggers.Distance(Katarina) > E.Range)
                                                {
                                                    E.Cast(ELogic(d, target));
                                                }
                                                if (daggers.Distance(target) > 450)
                                                {

                                                    E.Cast(ELogic(d, target));
                                                }
                                                break;
                                            case 1:
                                                if (daggers.Distance(Katarina) > E.Range)
                                                {
                                                    E.Cast(LogicInstance(d, target));
                                                }
                                                if (daggers.Distance(target) > 450)
                                                {

                                                    E.Cast(LogicInstance(d, target));
                                                }
                                                break;
                                            case 2:
                                                if (!R.IsReady() || Player.GetSpell(SpellSlot.R).Level == 0)
                                                {
                                                    if (daggers.Distance(Katarina) > E.Range)
                                                    {
                                                        E.Cast(ELogic(d, target));
                                                    }
                                                    if (daggers.Distance(target) > 450)
                                                    {

                                                        E.Cast(ELogic(d, target));
                                                    }
                                                }
                                                if (R.IsReady())
                                                {
                                                    if (daggers.Distance(Katarina) > E.Range)
                                                    {
                                                        E.Cast(LogicInstance(d, target));
                                                    }
                                                    if (daggers.Distance(target) > 450)
                                                    {

                                                        E.Cast(LogicInstance(d, target)); ;
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                    switch (Combo["modee"].Cast<ComboBox>().SelectedIndex)
                                    {
                                        case 0:
                                            if (dagger.Count() == 0)
                                            {

                                                E.Cast(ELogic(d, target));
                                            }
                                            break;
                                        case 1:
                                            if (dagger.Count() == 0)
                                            {

                                                E.Cast(LogicInstance(d, target));
                                            }
                                            break;
                                        case 2:
                                            if (dagger.Count() == 0)
                                            {

                                                if (!R.IsReady() || Player.GetSpell(SpellSlot.R).Level == 0)
                                                {


                                                    E.Cast(ELogic(d, target));

                                                }
                                                if (R.IsReady())
                                                {

                                                    E.Cast(LogicInstance(d, target));

                                                }
                                            }
                                            break;
                                    }

                                }
                                if (SaveE)
                                {
                                    if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid)
                                    {
                                        if (target.Distance(daggers) < 450 &&
                                            target.IsValidTarget(E.Range))
                                        {

                                            E.Cast(LogicDistacne(d, target));


                                        }
                                    }
                                }
                            }

                        }
                    }
                    if (W.IsReady() && useW)
                    {
                        if (Katarina.CountEnemyChampionsInRange(W.Range) > 0)
                        {
                            W.Cast();
                        }

                    }
                    if (useR)
                    {
                        switch (Combo["moder"].Cast<ComboBox>().SelectedIndex)
                        {
                            case 0:
                                if (R.IsReady() && target.IsValidTarget(R.Range - 50))
                                {
                                    if (target != null && Katarina.CountEnemyChampionsInRange(R.Range - 150) >= hitR)
                                    {
                                        if (target.Health > meow && !Q.IsReady())
                                        {
                                            R.Cast();
                                        }
                                    }
                                }
                                break;
                            case 1:
                                if (R.IsReady() && target.IsValidTarget(R.Range - 150))
                                {
                                    if (target != null && target.Health <=
                                        Katarina.GetSpellDamage(target, SpellSlot.Q) +
                                        Katarina.GetSpellDamage(target, SpellSlot.E) + Passive(target) +
                                        GetR(target) * dagggggggers)
                                    {
                                        if (target.Health > meow && !Q.IsReady())
                                        {
                                            R.Cast();
                                        }
                                    }
                                }
                                break;
                        }

                    }

                    break;
                case 1:
                    if (!target.IsValidTarget())
                    {
                        return;
                    }
                    if (E.IsReady() && useE && target.IsValidTarget(E.Range))
                    {
                        if (target != null)
                        {
                            foreach (var daggers in dagger)

                            {
                                if (!SaveE)
                                {
                                    if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid)
                                    {
                                        if (target.Distance(daggers) < 450 &&
                                            target.IsValidTarget(E.Range))
                                        {

                                            E.Cast(LogicDistacne(d, target));


                                        }
                                        switch (Combo["modee"].Cast<ComboBox>().SelectedIndex)
                                        {
                                            case 0:
                                                if (daggers.Distance(Katarina) > E.Range)
                                                {
                                                    E.Cast(ELogic(d, target));
                                                }
                                                if (daggers.Distance(target) > 450)
                                                {

                                                    E.Cast(ELogic(d, target));
                                                }
                                                break;
                                            case 1:
                                                if (daggers.Distance(Katarina) > E.Range)
                                                {
                                                    E.Cast(LogicInstance(d, target));
                                                }
                                                if (daggers.Distance(target) > 450)
                                                {

                                                    E.Cast(LogicInstance(d, target));
                                                }
                                                break;
                                            case 2:
                                                if (!R.IsReady() || Player.GetSpell(SpellSlot.R).Level == 0)
                                                {
                                                    if (daggers.Distance(Katarina) > E.Range)
                                                    {
                                                        E.Cast(ELogic(d, target));
                                                    }
                                                    if (daggers.Distance(target) > 450)
                                                    {

                                                        E.Cast(ELogic(d, target));
                                                    }
                                                }
                                                if (R.IsReady())
                                                {
                                                    if (daggers.Distance(Katarina) > E.Range)
                                                    {
                                                        E.Cast(LogicInstance(d, target));
                                                    }
                                                    if (daggers.Distance(target) > 450)
                                                    {

                                                        E.Cast(LogicInstance(d, target));
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                    switch (Combo["modee"].Cast<ComboBox>().SelectedIndex)
                                    {
                                        case 0:
                                            if (dagger.Count() == 0)
                                            {

                                                E.Cast(ELogic(d, target));
                                            }
                                            break;
                                        case 1:
                                            if (dagger.Count() == 0)
                                            {

                                                E.Cast(LogicInstance(d, target));
                                            }
                                            break;
                                        case 2:
                                            if (dagger.Count() == 0)
                                            {

                                                if (!R.IsReady() || Player.GetSpell(SpellSlot.R).Level == 0)
                                                {


                                                    E.Cast(ELogic(d, target));

                                                }
                                                if (R.IsReady())
                                                {

                                                    E.Cast(LogicInstance(d, target));

                                                }
                                            }
                                            break;
                                    }

                                }
                                if (SaveE)
                                {
                                    if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid)
                                    {
                                        if (target.Distance(daggers) < 450 &&
                                            target.IsValidTarget(E.Range))
                                        {

                                            E.Cast(LogicDistacne(d, target));


                                        }
                                    }
                                }

                            }
                        }
                    }
                    if (W.IsReady() && useW)
                    {
                        if (Katarina.CountEnemyChampionsInRange(W.Range) > 0)
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
                    if (useR)
                    {
                        switch (Combo["moder"].Cast<ComboBox>().SelectedIndex)
                        {
                            case 0:
                                if (R.IsReady() && target.IsValidTarget(R.Range - 150))
                                {
                                    if (target != null && Katarina.CountEnemyChampionsInRange(R.Range - 50) >= hitR)
                                    {
                                        if (target.Health > meow && !Q.IsReady())
                                        {
                                            R.Cast();
                                        }
                                    }
                                }
                                break;
                            case 1:
                                if (R.IsReady() && target.IsValidTarget(R.Range - 150))
                                {
                                    if (target != null && target.Health <=
                                        Katarina.GetSpellDamage(target, SpellSlot.Q) +
                                        Katarina.GetSpellDamage(target, SpellSlot.E) + Passive(target) +
                                        GetR(target) * dagggggggers)
                                    {
                                        if (target.Health > meow && !Q.IsReady())
                                        {
                                            R.Cast();
                                        }
                                    }
                                }
                                break;
                        }

                    }
                    break;
                case 2:

                    if (!target.IsValidTarget())
                    {
                        return;
                    }
                    if (Q.IsReady() && useQ && target.IsValidTarget(Q.Range) && !R.IsReady() && meowwwwwwwwwww < Game.Time)
                    {
                        if (target != null)
                        {
                            Q.Cast(target);
                        }
                    }
                    if (E.IsReady() && useE && target.IsValidTarget(E.Range))
                    {
                        if (target != null)
                        {
                            foreach (var daggers in dagger)

                            {
                                if (!SaveE)
                                {
                                    if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid)
                                    {
                                        if (target.Distance(daggers) < 450 &&
                                            target.IsValidTarget(E.Range))
                                        {

                                            E.Cast(LogicDistacne(d, target));


                                        }
                                        switch (Combo["modee"].Cast<ComboBox>().SelectedIndex)
                                        {
                                            case 0:
                                                if (daggers.Distance(Katarina) > E.Range)
                                                {
                                                    E.Cast(ELogic(d, target));
                                                }
                                                if (daggers.Distance(target) > 450)
                                                {

                                                    E.Cast(ELogic(d, target));
                                                }
                                                break;
                                            case 1:
                                                if (daggers.Distance(Katarina) > E.Range)
                                                {
                                                    E.Cast(LogicInstance(d, target));
                                                }
                                                if (daggers.Distance(target) > 450)
                                                {

                                                    E.Cast(LogicInstance(d, target)); 
                                                }
                                                break;
                                            case 2:
                                                if (!R.IsReady() || Player.GetSpell(SpellSlot.R).Level == 0)
                                                {
                                                    if (daggers.Distance(Katarina) > E.Range)
                                                    {
                                                        E.Cast(ELogic(d, target));
                                                    }
                                                    if (daggers.Distance(target) > 450)
                                                    {

                                                        E.Cast(ELogic(d, target));
                                                    }
                                                }
                                                if (R.IsReady())
                                                {
                                                    if (daggers.Distance(Katarina) > E.Range)
                                                    {
                                                        E.Cast(LogicInstance(d, target));
                                                    }
                                                    if (daggers.Distance(target) > 450)
                                                    {

                                                        E.Cast(LogicInstance(d, target));
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                    switch (Combo["modee"].Cast<ComboBox>().SelectedIndex)
                                    {
                                        case 0:
                                            if (dagger.Count() == 0)
                                            {

                                                E.Cast(ELogic(d, target));
                                            }
                                            break;
                                        case 1:
                                            if (dagger.Count() == 0)
                                            {

                                                E.Cast(LogicInstance(d, target));
                                            }
                                            break;
                                        case 2:
                                            if (dagger.Count() == 0)
                                            {

                                                if (!R.IsReady() || Player.GetSpell(SpellSlot.R).Level == 0)
                                                {


                                                    E.Cast(ELogic(d, target));

                                                }
                                                if (R.IsReady())
                                                {

                                                    E.Cast(LogicInstance(d, target));

                                                }
                                            }
                                            break;
                                    }

                                }
                                if (SaveE)
                                {
                                    if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid)
                                    {
                                        if (target.Distance(daggers) < 450 &&
                                            target.IsValidTarget(E.Range))
                                        {

                                            E.Cast(LogicDistacne(d, target));


                                        }
                                    }
                                }
                            }

                        }
                    }
                    if (W.IsReady() && useW)
                    {
                        if (Katarina.CountEnemyChampionsInRange(W.Range) > 0)
                        {
                            W.Cast();
                        }

                    }

                    if (useR)
                    {

                        if (R.IsReady() && target.IsValidTarget(R.Range - 150) && !W.IsReady())
                        {

                            if (R.Cast())
                            {

                                meowwwwwwwwwww = Game.Time + 1000;
                            }

                        }

                    }
                    break;

            }

        }

        private static void OnHarass()
        {
            var useQ = CastCheckbox(Harass, "Hq");
            var useW = CastCheckbox(Harass, "Hw");
            var useE = CastCheckbox(Harass, "He");
            var dagger = GetDaggers();
            var d = GetClosestDagger();
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            switch (Harass["modeh"].Cast<ComboBox>().SelectedIndex)
            {
                case 0:
                    if (!target.IsValidTarget())
                    {
                        return;
                    }
                    if (target == null)
                    {
                        return;
                    }
                    if (Q.IsReady() && useQ && target.IsValidTarget(Q.Range))
                    {
                        if (target != null)
                        {
                            Q.Cast(target);
                        }
                    }

                    if (E.IsReady() && useE && target.IsValidTarget(E.Range) && !Q.IsReady())
                    {
                        if (target != null)
                        {
                            foreach (var daggers in dagger)

                            {
                                if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid)
                                {
                                    if (target.Distance(daggers) < 450 &&
                                        target.IsValidTarget(E.Range))
                                    {

                                        E.Cast(GetBestDaggerPoint(d, target));


                                    }
                                    if (daggers.Distance(Katarina) > E.Range)
                                    {
                                        E.Cast(GetBestDaggerPoint(d, target));
                                    }
                                    if (daggers.Distance(target) > 450)
                                    {

                                        E.Cast(GetBestDaggerPoint(d, target));
                                    }
                                }
                                if (dagger.Count() == 0)
                                {
                                    E.Cast(GetBestDaggerPoint(d, target));
                                }

                            }

                        }
                    }
                    if (W.IsReady() && useW)
                    {
                        if (Katarina.CountEnemyChampionsInRange(W.Range) > 0)
                        {
                            W.Cast();
                        }

                    }

                    break;
                case 1:
                    if (!target.IsValidTarget())
                    {
                        return;
                    }
                    if (target == null)
                    {
                        return;
                    }
                    if (E.IsReady() && useE && target.IsValidTarget(E.Range))
                    {
                        if (target != null)
                        {
                            foreach (var daggers in dagger)

                            {
                                if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid)
                                {
                                    if (target.Distance(daggers) < 450 &&
                                        target.IsValidTarget(E.Range))
                                    {

                                        E.Cast(GetBestDaggerPoint(d, target));


                                    }
                                    if (daggers.Distance(Katarina) > E.Range)
                                    {
                                        E.Cast(GetBestDaggerPoint(d, target));
                                    }
                                    if (daggers.Distance(target) > 450)
                                    {

                                        E.Cast(GetBestDaggerPoint(d, target));
                                    }
                                }
                                if (dagger.Count() == 0)
                                {

                                    E.Cast(GetBestDaggerPoint(d, target));
                                }

                            }

                        }
                    }

                    if (W.IsReady())
                    {
                        if (Katarina.CountEnemyChampionsInRange(W.Range) > 0)
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
                    break;
            }
        }

        private static void Lasthit()
        {
            var minion = EntityManager.MinionsAndMonsters.Get(EntityManager.MinionsAndMonsters.EntityType.Minion, EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition, E.Range).FirstOrDefault(m => E.IsReady() && m.IsValidTarget(E.Range) && Prediction.Health.GetPrediction(m, E.CastDelay) < MagerMinion.GetMinionTarget(m, SpellSlot.E));

            if (Farming["lastQ"].Cast<CheckBox>().CurrentValue && Q.IsReady() && minion.IsValidTarget(Q.Range))
            {
                E.Cast(minion);
            }
        }

        private static void Clearing()
        {
            var useQ = CastCheckbox(Farming, "Qf");
            var useW = CastCheckbox(Farming, "Wf");
            var useE = CastCheckbox(Farming, "Ef");
            var hitW = CastSlider(Farming, "whit");
            var hitE = CastSlider(Farming, "hite");
            var dagger = GetDaggers();
            var d = GetClosestDagger();
            var minion = EntityManager.MinionsAndMonsters.Get(EntityManager.MinionsAndMonsters.EntityType.Minion, EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition, E.Range);
            if (useQ)
            {
                foreach (var minionn in minion)
                {
                    if (minionn.IsValidTarget(Q.Range) && minionn != null)
                    {
                        Q.Cast(minionn);
                    }
                }
            }
            if (useE)
            {
                foreach (var minionn in minion)
                {

                    if (minionn.IsValidTarget(E.Range) && minionn != null)
                    {
                        {
                            foreach (var daggers in dagger)
                            {
                                if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid)
                                {

                                    if (timeW < Game.Time)
                                    {
                                        if (Farming["turrent"].Cast<CheckBox>().CurrentValue)
                                        {
                                            if (!daggers.IsUnderEnemyturret())
                                            {
                                                E.Cast(GetBestDaggerPoint(d, minionn));

                                            }
                                        }
                                        if (!Farming["turrent"].Cast<CheckBox>().CurrentValue)
                                        {

                                            E.Cast(GetBestDaggerPoint(d, minionn));

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (useW)
                {

                    foreach (var minionn in minion)
                    {

                        if (minionn.IsValidTarget(W.Range) && minionn != null)
                        {

                            W.Cast();

                        }
                    }
                }
            }
        }
    }
}