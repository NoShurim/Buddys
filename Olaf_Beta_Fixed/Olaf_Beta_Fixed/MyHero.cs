using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;


namespace Olaf_Beta_Fixed
{
    class MyHero
    {
        private static AIHeroClient Olaf => Player.Instance;

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }

        }

        internal class OlafAxe
        {
            public GameObject Object { get; set; }
            public float NetworkId { get; set; }
            public Vector3 AxePos { get; set; }
            public double ExpireTime { get; set; }
        }

        public static Spell.Skillshot Q;
        public static Spell.Skillshot Q2;
        public static Spell.Active W;
        public static Spell.Targeted E;
        public static Spell.Active R;
        public static Menu Menu, SkillMenu, FarmingMenu, MiscMenu, DrawMenu, HarassMenu, ComboMenu, SmiteMenu, UpdateMenu;
        static Spell.Targeted Smite = null;
        private static readonly OlafAxe olafAxe = new OlafAxe();
        public static SpellSlot IgniteSlot = SpellSlot.Unknown;


        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += On_Complete;
        }

        private static void On_Complete(EventArgs args)
        {
            if (Olaf.Hero != Champion.Olaf) return;
            Bootstrap.Init(null);
            Chat.Print("[Addon] [Champion [Olaf]", System.Drawing.Color.AliceBlue);

            uint level = (uint)Player.Instance.Level;
            Q = new Spell.Skillshot(SpellSlot.Q, 1000, SkillShotType.Linear, 250, 1550, 75)
            {
                AllowedCollisionCount = int.MaxValue,
                MinimumHitChance = HitChance.High
            };
            Q2 = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear, 250, 1550, 75)
            {
                AllowedCollisionCount = int.MaxValue,
                MinimumHitChance = HitChance.High
            };
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Targeted(SpellSlot.E, 325);
            R = new Spell.Active(SpellSlot.R);

            Menu = MainMenu.AddMenu("Olaf", "Olaf");

            ComboMenu = Menu.AddSubMenu("Combo");
            ComboMenu.AddLabel("Combo Settings");
            ComboMenu.Add("QCombo", new CheckBox("Use Q"));
            ComboMenu.Add("WCombo", new CheckBox("Use W"));
            ComboMenu.Add("ECombo", new CheckBox("Use E"));
            ComboMenu.Add("RCombo", new CheckBox("Use R"));
            ComboMenu.Add("useTiamat", new CheckBox("Use Items"));

            HarassMenu = Menu.AddSubMenu("Harass");
            HarassMenu.AddLabel("Harass Settings");
            HarassMenu.Add("QHarass", new CheckBox("Use Q"));
            HarassMenu.Add("WHarass", new CheckBox("Use W"));
            HarassMenu.Add("EHarass", new CheckBox("Use E"));

            FarmingMenu = Menu.AddSubMenu("LaneClear");

            FarmingMenu.AddLabel("Lane Clear");
            FarmingMenu.Add("QLaneClear", new CheckBox("Use Q LaneClear"));
            FarmingMenu.Add("QlaneclearMana", new Slider("Mana < %", 45, 0, 100));
            FarmingMenu.Add("WLaneClear", new CheckBox("Use W LaneClear"));
            FarmingMenu.Add("WlaneclearMana", new Slider("Mana < %", 45, 0, 100));
            FarmingMenu.Add("ELaneClear", new CheckBox("Use E LaneClear"));
            FarmingMenu.Add("ElaneclearHP", new Slider("HP < %", 10, 0, 100));

            FarmingMenu.AddLabel("JungleClear");
            FarmingMenu.Add("Qjungle", new CheckBox("Use Q in Jungle"));
            FarmingMenu.Add("QjungleMana", new Slider("Mana < %", 45, 0, 100));
            FarmingMenu.Add("Wjungle", new CheckBox("Use W in Jungle"));
            FarmingMenu.Add("WjungleMana", new Slider("Mana < %", 45, 0, 100));
            FarmingMenu.Add("Ejungle", new CheckBox("Use E in Jungle"));
            FarmingMenu.Add("EjungleHP", new Slider("HP < %", 25, 0, 100));

            FarmingMenu.AddLabel("Last");
            FarmingMenu.Add("Qlasthit", new CheckBox("Use Q LastHit"));
            FarmingMenu.Add("Elasthit", new CheckBox("Use E LastHit"));
            FarmingMenu.Add("QlasthitMana", new Slider("Mana < %", 45, 0, 100));

            MiscMenu = Menu.AddSubMenu("Misc");

            MiscMenu.AddLabel("Auto");
            MiscMenu.Add("Auto Ignite", new CheckBox("Auto Ignite"));
            MiscMenu.Add("autoQ", new CheckBox("Use Auto Q to Flee/Escape"));
            MiscMenu.Add("autoR", new CheckBox("Use Auto R in Dangerous Spell", false));
            MiscMenu.Add("autoEenemyHP", new Slider("Enemy HP < %", 45, 0, 100));
            MiscMenu.AddSeparator();
            MiscMenu.AddLabel("Items");
            MiscMenu.AddLabel("BOTRK,Bilgewater Cutlass Settings");
            MiscMenu.Add("botrkHP", new Slider("My HP < %", 60, 0, 100));
            MiscMenu.Add("botrkenemyHP", new Slider("Enemy HP < %", 60, 0, 100));

            MiscMenu.AddLabel("KillSteal");
            MiscMenu.Add("Qkill", new CheckBox("Use Q KillSteal"));
            MiscMenu.Add("Ekill", new CheckBox("Use E KillSteal"));

            MiscMenu.AddLabel("Activator");
            MiscMenu.Add("useHP", new CheckBox("Use Health Potion"));
            MiscMenu.Add("useHPV", new Slider("HP < %", 45, 0, 100));
            MiscMenu.Add("useMana", new CheckBox("Use Mana Potion"));
            MiscMenu.Add("useManaV", new Slider("Mana < %", 45, 0, 100));
            MiscMenu.Add("useCrystal", new CheckBox("Use Refillable Potions"));
            MiscMenu.Add("useCrystalHPV", new Slider("HP < %", 45, 0, 100));
            MiscMenu.Add("useCrystalManaV", new Slider("Mana < %", 45, 0, 100));

            DrawMenu = Menu.AddSubMenu("Draw Settings", "Drawings");
            DrawMenu.Add("drawAA", new CheckBox("Draw AA Range"));
            DrawMenu.Add("drawQ", new CheckBox("Draw Q"));
            DrawMenu.Add("drawQpos", new CheckBox("Draw Q Position"));
            DrawMenu.Add("drawE", new CheckBox("Draw E"));

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;

        }
        private static void GameObject_OnCreate(GameObject obj, EventArgs args)
        {
            if (obj.Name == "olaf_axe_totem_team_id_green.troy")
            {
                olafAxe.Object = obj;
                olafAxe.ExpireTime = Game.Time + 8;
                olafAxe.NetworkId = obj.NetworkId;
                olafAxe.AxePos = obj.Position;
            }
        }
        private static void GameObject_OnDelete(GameObject obj, EventArgs args)
        {
            if (obj.Name == "olaf_axe_totem_team_id_green.troy")
            {
                olafAxe.Object = null;
            }
        }
   
        private static void Game_OnTick(EventArgs args)
        {

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
            }
            KillSteal();
            autoR();
        }
        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var useQ = ComboMenu["QCombo"].Cast<CheckBox>().CurrentValue;
            var useW = ComboMenu["WCombo"].Cast<CheckBox>().CurrentValue;
            var useE = ComboMenu["ECombo"].Cast<CheckBox>().CurrentValue;
            var useR = ComboMenu["RCombo"].Cast<CheckBox>().CurrentValue;
            var useItem = ComboMenu["useTiamat"].Cast<CheckBox>().CurrentValue;

            if (useQ && Q.IsReady() && Q.GetPrediction(target).HitChance >= HitChance.Medium && !target.IsDead && !target.IsZombie && target.IsFacing(_Player))
            {
                Q.Cast(target);
            }
            else if (useQ && Q.IsReady() && Q.GetPrediction(target).HitChance >= HitChance.Medium && !target.IsDead && !target.IsZombie && !target.IsFacing(_Player))
            {
                Q.Cast(target);
            }
            if (W.IsReady() && useW && target.IsValidTarget(300) && !target.IsDead && !target.IsZombie)
            {
                W.Cast();
            }
            if (E.IsReady() && useE && target.IsValidTarget(E.Range) && !target.IsDead && !target.IsZombie)
            {
                E.Cast(target);
            }
            if (useItem && !target.IsDead && !target.IsZombie)
            {
                HandleItems();
            }
            if (useR && R.IsReady() && target.IsValidTarget(800) && !target.IsDead && !target.IsZombie)
            {
                R.Cast();
            }


        }
        private static void KillSteal()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var useQ = MiscMenu["Qkill"].Cast<CheckBox>().CurrentValue;
            var useE = MiscMenu["Ekill"].Cast<CheckBox>().CurrentValue;

            if (Q.IsReady() && useQ && target.IsValidTarget(Q.Range) && !target.IsZombie && target.Health <= _Player.GetSpellDamage(target, SpellSlot.Q))
            {
                Q.Cast(target);
            }
            if (E.IsReady() && useE && target.IsValidTarget(E.Range) && !target.IsZombie && target.Health <= _Player.GetSpellDamage(target, SpellSlot.E))
            {
                E.Cast(target);
            }
        }

        internal static void HandleItems()
        {
            var botrktarget = TargetSelector.GetTarget(550, DamageType.Physical);
            var youmutarget = TargetSelector.GetTarget(800, DamageType.Physical);
            var target = TargetSelector.GetTarget(600, DamageType.Physical);
            var useItem = ComboMenu["useTiamat"].Cast<CheckBox>().CurrentValue;
            var useBotrkHP = MiscMenu["botrkHP"].Cast<Slider>().CurrentValue;
            var useBotrkEnemyHP = MiscMenu["botrkenemyHP"].Cast<Slider>().CurrentValue;
            //HYDRA
            if (useItem && Item.HasItem(3077) && Item.CanUseItem(3077) && target.IsValidTarget(400))
                Item.UseItem(3077);

            //TİAMAT
            if (useItem && Item.HasItem(3074) && Item.CanUseItem(3074) && target.IsValidTarget(400))
                Item.UseItem(3074);

            //NEW ITEM
            if (useItem && Item.HasItem(3748) && Item.CanUseItem(3748) && target.IsValidTarget(_Player.AttackRange))
                Item.UseItem(3748);

            //BİLGEWATER CUTLASS
            if (useItem && Item.HasItem(3144) && Item.CanUseItem(3144) && botrktarget.HealthPercent <= useBotrkEnemyHP && _Player.HealthPercent <= useBotrkHP && botrktarget.IsValidTarget(550))
                Item.UseItem(3144, botrktarget);

            //BOTRK
            if (useItem && Item.HasItem(3153) && Item.CanUseItem(3153) && botrktarget.HealthPercent <= useBotrkEnemyHP && _Player.HealthPercent <= useBotrkHP && botrktarget.IsValidTarget(550))
                Item.UseItem(3153, botrktarget);

            //YOUMU
            if (useItem && Item.HasItem(3142) && Item.CanUseItem(3142) && youmutarget.IsValidTarget(800))
                Item.UseItem(3142);
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var useQ = HarassMenu["QHarass"].Cast<CheckBox>().CurrentValue;
            var useW = HarassMenu["WHarass"].Cast<CheckBox>().CurrentValue;
            var useE = HarassMenu["EHarass"].Cast<CheckBox>().CurrentValue;

            if (Q.IsReady() && useQ && target.IsValidTarget(Q.Range) && !target.IsDead && !target.IsZombie)
            {
                Q.Cast(target);
            }
            if (W.IsReady() && useW && target.IsValidTarget(_Player.AttackRange) && !target.IsDead && !target.IsZombie)
            {
                W.Cast();
            }
            if (E.IsReady() && useE && target.IsValidTarget(E.Range) && !target.IsDead && !target.IsZombie)
            {
                E.Cast(target);
            }

        }
        private static void LaneClear()
        {
            var useQ = FarmingMenu["QLaneClear"].Cast<CheckBox>().CurrentValue;
            var useW = FarmingMenu["WLaneClear"].Cast<CheckBox>().CurrentValue;
            var useE = FarmingMenu["ELaneClear"].Cast<CheckBox>().CurrentValue;
            var Qmana = FarmingMenu["QlaneclearMana"].Cast<Slider>().CurrentValue;
            var Wmana = FarmingMenu["WlaneclearMana"].Cast<Slider>().CurrentValue;
            var EHP = FarmingMenu["ElaneclearHP"].Cast<Slider>().CurrentValue;
            var minions = ObjectManager.Get<Obj_AI_Base>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            foreach (var minion in minions)
            {
                if (useQ && Q.IsReady() && !minion.IsValidTarget(_Player.AttackRange) && minion.IsValidTarget(Q.Range) && Player.Instance.ManaPercent > Qmana && minion.Health <= _Player.GetSpellDamage(minion, SpellSlot.Q) && minions.Count() > 1)
                {
                    Q.Cast(minion);
                }
                if (useW && W.IsReady() && Player.Instance.ManaPercent > Wmana && minion.IsValidTarget(_Player.AttackRange) && Player.Instance.HealthPercent < 35)
                {
                    W.Cast();
                }
                if (useE && E.IsReady() && Player.Instance.HealthPercent > EHP && minion.Health <= _Player.GetSpellDamage(minion, SpellSlot.E) && !minion.IsValidTarget(_Player.AttackRange))
                {
                    E.Cast(minion);
                }
            }
        }
        private static void JungleClear()
        {
            var useQ = FarmingMenu["Qjungle"].Cast<CheckBox>().CurrentValue;
            var useQMana = FarmingMenu["QjungleMana"].Cast<Slider>().CurrentValue;
            var useW = FarmingMenu["Wjungle"].Cast<CheckBox>().CurrentValue;
            var useWMana = FarmingMenu["WjungleMana"].Cast<Slider>().CurrentValue;
            var useE = FarmingMenu["Ejungle"].Cast<CheckBox>().CurrentValue;
            var useEHP = FarmingMenu["EjungleHP"].Cast<Slider>().CurrentValue;
            foreach (var monster in EntityManager.MinionsAndMonsters.Monsters)
            {
                if (useQ && Q.IsReady() && Player.Instance.ManaPercent > useQMana)
                {
                    Q.Cast(monster);
                }
                if (useW && W.IsReady() && Player.Instance.ManaPercent > useWMana)
                {
                    W.Cast();
                }
                if (useE && E.IsReady() && Player.Instance.HealthPercent > useEHP)
                {
                    E.Cast(monster);
                }

                HandleItems();
            }
        }
        private static void LastHit()
        {
            var useQ = FarmingMenu["Qlasthit"].Cast<CheckBox>().CurrentValue;
            var useE = FarmingMenu["Elasthit"].Cast<CheckBox>().CurrentValue;
            var mana = FarmingMenu["QlasthitMana"].Cast<Slider>().CurrentValue;
            var minions = ObjectManager.Get<Obj_AI_Base>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            foreach (var minion in minions)
            {
                if (useQ && Q.IsReady() && !minion.IsValidTarget(E.Range) && minion.IsValidTarget(Q.Range) && Player.Instance.ManaPercent > mana && minion.Health <= _Player.GetSpellDamage(minion, SpellSlot.Q))
                {
                    Q.Cast(minion);
                }
                if (useE && E.IsReady() && minion.Health <= _Player.GetSpellDamage(minion, SpellSlot.E))
                {
                    E.Cast(minion);
                }
            }
        }
        private static void autoR()
        {
            var autoR = MiscMenu["autoR"].Cast<CheckBox>().CurrentValue;

            if (autoR && R.IsReady() && _Player.HasBuffOfType(BuffType.Stun)
            || _Player.HasBuffOfType(BuffType.Fear)
            || _Player.HasBuffOfType(BuffType.Charm)
            || _Player.HasBuffOfType(BuffType.Silence)
            || _Player.HasBuffOfType(BuffType.Snare)
            || _Player.HasBuffOfType(BuffType.Taunt)
            || _Player.HasBuffOfType(BuffType.Suppression)
            || _Player.HasBuffOfType(BuffType.Sleep)
            || _Player.HasBuffOfType(BuffType.Polymorph)
            || _Player.HasBuffOfType(BuffType.Frenzy)
            || _Player.HasBuffOfType(BuffType.Disarm)
            || _Player.HasBuffOfType(BuffType.NearSight)
            || _Player.HasBuffOfType(BuffType.Blind))
            {
                R.Cast();
            }
        }

        static float GetSmiteDamage()
        {
            float damage = new float();

            if (_Player.Level < 10) damage = 360 + (_Player.Level - 1) * 30;

            else if (_Player.Level < 15) damage = 280 + (_Player.Level - 1) * 40;

            else if (_Player.Level < 19) damage = 150 + (_Player.Level - 1) * 50;

            return damage;
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            var drawAxePosition = DrawMenu["drawQpos"].Cast<CheckBox>().CurrentValue;

            if (drawAxePosition && olafAxe.Object != null)
            {
                new Circle() { Color = System.Drawing.Color.Green, BorderWidth = 6, Radius = 85 }.Draw(olafAxe.Object.Position);
            }
            if (DrawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Color.LightBlue, Q.Range, Player.Instance.Position);
            }
            if (DrawMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Color.LightBlue, E.Range, Player.Instance.Position);
            }
        }
    }
}
