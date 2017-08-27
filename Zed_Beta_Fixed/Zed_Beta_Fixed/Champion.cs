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
using SharpDX;

namespace Zed_Beta_Fixed
{
    class Champion
    {
        //Buffs
        public const string SombraHasBuff = "Shadow";
        public const string Dead = "Zed_Base_R_buf_tell.troy";
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }

        }

        //Base Spells
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Active W2;
        public static Spell.Active E;
        public static Spell.Targeted R;
        public static Spell.Active R2;
        public static Spell.Targeted Ignite;
        public static Vector3 Wpos;
        public static Vector3 Rpos;
        public static int Wtimer;
        public static int GR = 0;
        public static int GW = 0;
        public static double Rtimer;
        public static int global_ticks = 0;
        public static float StartTime = 0f;
        public static int LastCastTime = 0;
        public static float StartTimeR = 0f;
        public static bool Wdmgp = false;
        public static bool Rdmgcheck = false;
        public static bool Rdmgp = false;
        public const int W2Range = 700;
        public static GameObject Object;
        public static Obj_AI_Minion RSombra;
        public static Obj_AI_Minion Wm;


        public static bool IsW1()
        {
            return Zed.Spellbook.GetSpell(SpellSlot.W).Name == "ZedW";
        }
        public static bool IsW2()
        {
            return (Zed.Spellbook.GetSpell(SpellSlot.W).Name == "ZedW2");
        }
        public static bool IsR1()
        {
            return (Zed.Spellbook.GetSpell(SpellSlot.R).Name == "ZedR");
        }

        public bool Enemy;
        public bool Enemy2;
        //Bolls
        private static bool Wcts;
        private static bool Rcts;


        private static AIHeroClient Zed => Player.Instance;
        //Menu
        public static Menu DD, ComboMenu, Auto, Farming, Utimate, Draws, KillSteal;

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
            Ignite = new Spell.Targeted(SpellSlot.Summoner1, 600);
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
            ComboMenu.Add("Key", new KeyBind("Forced [R]", false, KeyBind.BindTypes.HoldActive, (uint)'A'));
            ComboMenu.Add("UseG", new CheckBox("Use Ignite [Firts]"));
            ComboMenu.Add("Q", new CheckBox("Use [Q]"));
            ComboMenu.Add("W", new CheckBox("Use [W]"));
            ComboMenu.Add("E", new CheckBox("Use [E]"));
            ComboMenu.Add("ModeR", new ComboBox("ModSharp", 1, "Normal => [R]", "Static [R]"));
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
            Farming.Add("mini", new Slider("Minion Percent [W] > {0}", 3, 0, 6));
            Farming.AddSeparator();
            Farming.AddLabel("JungleClear");
            Farming.Add("Qj", new CheckBox("Use [Q]"));
            Farming.Add("Wj", new CheckBox("Use [W]"));
            Farming.Add("Ej", new CheckBox("Use [E]"));
            //Utimate
            Utimate = DD.AddSubMenu("Utimate [R]");
            Utimate.AddLabel("Settings Utimate");
            Utimate.Add("AutoR", new CheckBox("Use Auto[R]"));
            Utimate.Add("R", new CheckBox("[R] Utimate [Not use Spells]"));
            Utimate.Add("Rlife", new Slider("Target [R] > {0}", 75, 0));
            //KillSteal
            KillSteal = DD.AddSubMenu("KillSteal");
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
            var target = TargetSelector.GetTarget(900, DamageType.Physical);
            var globalrange = R.IsReady() && IsR1() ? R.Range - 5 : 1500;
            var UseR = ComboMenu["Rc"].Cast<CheckBox>().CurrentValue;
            var modeR = ComboMenu["ModeR"].Cast<ComboBox>().CurrentValue;
            var UseW = ComboMenu["Wc"].Cast<CheckBox>().CurrentValue;
            var UseQ = ComboMenu["Qc"].Cast<CheckBox>().CurrentValue;
            var UseE = ComboMenu["Ec"].Cast<CheckBox>().CurrentValue;
            var Igniteuse = ComboMenu["UseG"].Cast<CheckBox>().CurrentValue;


            if (target == null || !target.IsValid)
            {
                return;
            }
            var Wcastpos = new Vector3();

            if (!Rpos.Equals(new Vector3()))
            {
                switch (modeR)
                {
                    case 0:
                        Wcastpos = (target.ServerPosition + (target.ServerPosition - Rpos).Normalized() * 450);
                        break;
                    case 1:
                        Wcastpos = (target.ServerPosition + (target.ServerPosition - Rpos).Normalized() * 350).To2D().Perpendicular().To3D();
                        break;
                    case 2:
                        Wcastpos = Game.CursorPos;
                        break;
                }
            }
            if (Zed.Distance(target) < globalrange)
            {
                if (Zed.Distance(target) < R.Range)
                {
                    CastR(target);
                }

                if (target.HasBuff("zedrtargetmark"))
                {
                    if (UseW && Zed.Distance(target) < W.Range)
                    {
                        CastW(Wcastpos);
                    }
                }
                else
                {
                    if (UseW && Zed.Distance(target) < W.Range && IsW1())
                    {
                        CastW(target.ServerPosition);
                    }
                    else if (Zed.Distance(target) > W.Range && target.IsValidTarget(W.Range + Q.Range / 2) &&
                             UseW)
                    {
                        var Wposition = Zed.ServerPosition.Extend(target.ServerPosition, 700f);
                        CastW(Wpos);

                        if (UseW)
                        {
                            CastW2();
                        }
                    }
                }

                if (Zed.Distance(target) < Q.Range && UseQ)
                {
                    if (!Wpos.Equals(new Vector3()) && Zed.Spellbook.GetSpell(SpellSlot.W).Name == "ZedW2")
                    {
                        CastQ(target);
                    }
                    else if (!W.IsReady() || Zed.Spellbook.GetSpell(SpellSlot.W).Name != "ZedW2" || Wpos.Equals(new Vector3()))
                    {
                        CastQ(target);
                    }
                }

                if (UseE)
                {
                    CastE();
                }

                if (W.IsReady() &&
                    target.IsValidTarget(1400) && !target.IsValidTarget(850)
                )
                {
                    var Wposition = Zed.ServerPosition.Extend(target.ServerPosition, 700f);
                    W.Cast(Wpos);
                }

                if (Igniteuse)
                {

                    foreach (var tar in EntityManager.Enemies.Where(a => a.IsValidTarget(1200) && !a.IsDead))
                    {
                        var dmgI = (50 + ((Zed.Level) * 20));
                        var health = tar.Health;
                        if (health < dmgI && Zed.Distance(tar) < 600)
                        {
                            if (Ignite.IsReady())
                            {
                                Ignite.Cast(tar);
                            }
                        }
                    }
                }
            }
        }

        private static void ByLane()
        {
            var minios = EntityManager.MinionsAndMonsters.Get(EntityManager.MinionsAndMonsters.EntityType.Minion, EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition, Q.Range);
            var Wf = Farming["Wf"].Cast<CheckBox>().CurrentValue;
            var Ef = Farming["Ef"].Cast<CheckBox>().CurrentValue;
            var Qf = Farming["Qf"].Cast<CheckBox>().CurrentValue;
            var mini = Farming["mini"].Cast<Slider>().CurrentValue;

            foreach (var minion in minios)
            {
                if (minion != null)
                {
                    if (Wf && W.IsReady())
                    {
                        if (W.GetPrediction(minion).CollisionObjects.Where(may => may.IsEnemy && !may.IsDead && may.IsValid && !may.IsInvulnerable).Count() >= mini)
                        {
                            CastW(minion.ServerPosition);
                        }
                    }
                    if (Ef && E.IsReady())
                    {
                        E.Cast(minion);
                    }

                    if (Qf && Q.IsReady())
                    {
                        CastQ(minion);
                    }
                }
            }
        }

        private static void ByJungle()
        {
            throw new NotImplementedException();
        }

        private static void AutoR()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical); //By BestSNA
            if (Utimate["AutoR"].Cast<CheckBox>().CurrentValue)
            {
                if (target != null && target.HealthPercent < 45)
                {
                    if (!target.IsInRange(_Player, R.Range) && R.IsReady() && Q.IsReady() && E.IsReady())
                    {
                        return;
                    }
                    {
                        R.Cast(target);
                    }
                }
            }
        }

        private static void Killteal()
        {

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
        public static void CastQ(Obj_AI_Base unit)
        {
            if (!Q.IsReady())
            {
                return;
            }
            if (!Wpos.Equals(new Vector3()))
            {
                if (Wpos.Distance(unit.ServerPosition) < Q.Range)
                {
                    Q.Cast(unit.ServerPosition);
                }
            }
            else if (Zed.Distance(unit) < Q.Range)
            {
                Q.Cast(unit.ServerPosition);
            }
        }

        public static void CastR(Obj_AI_Base unit)
        {
            if (Zed.Spellbook.GetSpell(SpellSlot.R).Name != "ZedR")
            {
                return;
            }
            if (R.IsReady())
            {
                R.Cast(unit);

            }
        }
        public static void CastW(Vector3 pos)
        {
            if (Zed.Spellbook.GetSpell(SpellSlot.W).Name == "ZedW2")
            {
                return;
            }
            if (W.IsReady() && IsW1() && Game.TicksPerSecond - LastCastTime > 175)
            {
                W.Cast(pos);
                LastCastTime = Game.TicksPerSecond;
            }
        }
        public static void CastW2()
        {
            if (Zed.Spellbook.GetSpell(SpellSlot.W).Name != "ZedW2")
            {
                return;

            }
            if (W.IsReady())
            {
                W.Cast();
            }
        }
        public static void CastE()
        {
            var target = TargetSelector.GetTarget(925, DamageType.Physical);

            if (!E.IsReady())
            {
                return;
            }
            if (!Rpos.Equals(new Vector3()))
            {
                if (target.Distance(Rpos) < E.Range)
                {
                    E.Cast();
                }
            }
            if (!Wpos.Equals(new Vector3()))
            {
                if (target.Distance(Wpos) < E.Range)
                {
                    E.Cast();
                }
            }
            if (Zed.Distance(target) < E.Range)
            {
                E.Cast();

            }
        }
    }
}
