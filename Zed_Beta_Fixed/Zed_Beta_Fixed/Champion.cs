using System;
using EloBuddy.SDK.Events;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using EloBuddy.SDK;
using EloBuddy.SDK.Spells;
using EloBuddy.SDK.Enumerations;
using System.Linq;

namespace Zed_Beta_Fixed
{
    class Champion
    {
        //Buffs
        public const string SombraHasBuff = "Shadow";
        public const string Dead = "Zed_Base_R_buf_tell.troy";

        //Base Spells
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Active W2;
        public static Spell.Active E;
        public static Spell.Targeted R;
        public static Spell.Active R2;

        public const int W2Range = 700;
        public static GameObject Object;
        public static Obj_AI_Minion RSombra;
        public static Obj_AI_Minion Wm;

        public bool Enemy;
        public bool Enemy2;
        //Bolls
        private static bool Wcts;
        private static bool Rcts;


        private static AIHeroClient Zed => Player.Instance;
        //Menu
        public static Menu DD, ComboMenu, Auto, Farming, Utimate, Draws;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_Beta;
        }

        private static void Loading_Beta(EventArgs args)
        {
            if (Zed.Hero != EloBuddy.Champion.Zed) return;

            Menus();

            Q = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear, 0, 900, 125);
            W = new Spell.Skillshot(SpellSlot.W, 625, SkillShotType.Linear, 0, 1725, 125);
            W2 = new Spell.Active(SpellSlot.W, 625);
            //
            E = new Spell.Active(SpellSlot.E, 290);
            R = new Spell.Targeted(SpellSlot.R, 625);
            R2 = new Spell.Active(SpellSlot.R, 625);
        }

        private static void Menus()
        {
            DD = MainMenu.AddMenu("Zed", "Zed");
            //ComboMenu
            ComboMenu = DD.AddSubMenu("Combo");
            ComboMenu.AddLabel("Settings Combo");
            ComboMenu.Add("Q", new CheckBox("Use [Q]"));
            ComboMenu.Add("W", new CheckBox("Use [W]"));
            ComboMenu.Add("E", new CheckBox("Use [E]"));
            ComboMenu.AddSeparator();
            ComboMenu.AddLabel("Settings Logic");
            ComboMenu.Add("Ql", new CheckBox("Use [Logic Q]"));
            ComboMenu.Add("Wl", new CheckBox("Use [Logic W]"));
            ComboMenu.Add("El", new CheckBox("Use [Logic E]"));
            ComboMenu.AddSeparator();
            ComboMenu.AddLabel("Settings Logic [Q]");
            ComboMenu.Add("Q2", new CheckBox("Fist [Q] Showdown"));
            ComboMenu.Add("Qp", new Slider("Use Prediction Lane > {0}", 50, 1));
            ComboMenu.AddSeparator();
            ComboMenu.AddLabel("Settings Logic [W]");
            ComboMenu.Add("W2", new CheckBox("Fist [W] Showdown"));
            ComboMenu.Add("Wp", new Slider("Use Prediction Lane > {0}", 70, 1));
            ComboMenu.Add("We", new CheckBox("Use [W] To reach out to enemies"));
            ComboMenu.AddSeparator();
            ComboMenu.AddLabel("Settings Logic [E]");
            ComboMenu.Add("E2", new CheckBox("Fist [E] Showdown"));
            ComboMenu.Add("Ep", new Slider("Use Prediction Lane > {0}", 70, 1));
            //Auto
            Auto = DD.AddSubMenu("Auto");
            Auto.AddLabel("Settings Auto");
            Auto.Add("AutoQ", new CheckBox("Auto [Q]"));
            //Farming
            Farming = DD.AddSubMenu("LaneClear");
            Farming.Add("Qf", new CheckBox("Use [Q] Farme"));
            Farming.Add("Wf", new CheckBox("Use [W] Farme"));
            Farming.Add("Ef", new CheckBox("Use [E] Farme"));
            Farming.AddSeparator();
            Farming.AddLabel("[Minion Settings]");
            Farming.Add("minion", new Slider("Minion Percent [W] > {0}", 3, 0, 6));
            Farming.Add("minion2", new Slider("Minion Percent [E] > {0}", 3, 0, 6));
            Farming.AddSeparator();
            Farming.AddLabel("JungleClear");
            Farming.Add("Qj", new CheckBox("Use [Q]"));
            Farming.Add("Wj", new CheckBox("Use [W]"));
            Farming.Add("Ej", new CheckBox("Use [E]"));
            //Utimate
            Utimate = DD.AddSubMenu("Utimate [R]");
            Utimate.AddLabel("Settings Utimate");
            Utimate.Add("R", new CheckBox("[R] Utimate [Not use Spells]"));
            Utimate.Add("Rlife", new Slider("Target [R] > {0}", 75, 0));
            //Draws
            Draws = DD.AddSubMenu("Drawings");
            Draws.Add("DQ", new CheckBox("[Q] Draws"));
            Draws.Add("DW", new CheckBox("[W] Draws"));
            Draws.Add("DE", new CheckBox("[E] Draws"));
            Draws.Add("DR", new CheckBox("[R] Draws"));

            Obj_AI_Base.OnBuffGain += Showndows;
            Obj_AI_Base.OnPlayAnimation += PlayerAnimation;
            Obj_AI_Base.OnProcessSpellCast += CastObjetcSpells;
            GameObject.OnCreate += Create_ObJectic;
            GameObject.OnDelete += Delete_Objectic;
            AttackableUnit.OnDamage += DamageAutoAtack;
            Drawing.OnDraw += OnDraw_Firts;
            Game.OnTick += GameOnTick;
            //Evade...


        }

        private static void GameOnTick(EventArgs args)
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
            AutoR();
            Killteal();
        
        }

        private static void ByCombo()
        {
            throw new NotImplementedException();
        }

        private static void ByLane()
        {
            throw new NotImplementedException();
        }

        private static void ByJungle()
        {
            throw new NotImplementedException();
        }

        private static void AutoR()
        {
            throw new NotImplementedException();
        }

        private static void Killteal()
        {
            throw new NotImplementedException();
        }

        private static void OnDraw_Firts(EventArgs args)
        {
            if (Draws["DQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                Q.DrawRange(System.Drawing.Color.LightBlue);
            }
            if (Draws["DW"].Cast<CheckBox>().CurrentValue && W.IsReady())
            {
                W.DrawRange(System.Drawing.Color.Crimson);
            }
            if (Draws["DE"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                E.DrawRange(System.Drawing.Color.Crimson);
            }
            if (Draws["DR"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                R.DrawRange(System.Drawing.Color.LawnGreen);
            }
        }

        private static void DamageAutoAtack(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            //First Spells Auto Atack
        }
        private static void CastObjetcSpells(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }
            if (args.Slot == SpellSlot.W && args.SData.Name == "ZedW")
            {
                Rcts = false;
            }
            else if (args.Slot == SpellSlot.R && args.SData.Name == "ZedR")
            {
                Wcts = false;
                Rcts = true;
            }
        }
        private static void Delete_Objectic(GameObject sender, EventArgs args)
        {
            var minion = sender as Obj_AI_Minion;
            if (minion != null && minion.IsAlly && minion.BaseSkinName == SombraHasBuff)
            {
                if (Wm.IdEquals(minion))
                {
                    Wm = null;
                }
                else if (RSombra.IdEquals(minion))
                {
                    RSombra = null;
                }
            }
            else if (sender.IdEquals(Object))
            {
                Object = null;
            }
        }

        private static void Create_ObJectic(GameObject sender, EventArgs args)
        {
            //Obj Evade
        }
        private static void PlayerAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            var minion = sender as Obj_AI_Minion;
            if (minion != null && minion.IsAlly && minion.BaseSkinName == SombraHasBuff)
            {
                if (args.Animation == "Death")
                {
                    if (Wm.IdEquals(minion))
                    {
                        Wm = null;
                    }
                    //Target Null forced, Player Animation Crashed... :/
                    else if (RSombra.IdEquals(minion))
                    {
                        RSombra = null;
                    }
                }
            }
        }
        private static void Showndows(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            var minion = sender as Obj_AI_Minion;
            if (minion != null && minion.IsAlly && minion.BaseSkinName == SombraHasBuff && args.Buff.Caster.IsMe)
            {
                switch (args.Buff.Name)
                {
                    case "zedwshadowbuff":
                        Wm = minion;
                        break;
                    case "zedrshadowbuff":
                        Wm = minion;
                        break;
                }
            }
        }
    }
}
        