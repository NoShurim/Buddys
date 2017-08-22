using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu.Values;
using static Xerath_Beta_Fixed.HeroChampion;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Xerath_Beta_Fixed
{
    public static class SpellManager
    {

        public static Spell.Chargeable Q { get; private set; }
        public static Spell.Skillshot W { get; private set; }
        public static Spell.Skillshot E { get; private set; }
        public static Spell.Skillshot R { get; private set; }

        public static Spell.SpellBase[] Spells { get; private set; }
        public static Dictionary<SpellSlot, Color> ColorTranslation { get; private set; }

        public static bool IsCastingUlt
        {
            get { return Player.Instance.Buffs.Any(b => b.Caster.IsMe && b.IsValid() && b.DisplayName == "XerathR"); }
        }
        public static int LastChargeTime { get; private set; }
        public static Vector3 LastChargePosition { get; private set; }
        public static int MaxCharges
        {
            get { return !R.IsLearned ? 3 : 2 + R.Level; }
        }
        public static int ChargesRemaining { get; private set; }

        public static bool TapKeyPressed { get; private set; }

        public static void Initialize()
        {
            Q = new Spell.Chargeable(SpellSlot.Q, 750, 1500, 1500, 500, int.MaxValue, 95);
            W = new Spell.Skillshot(SpellSlot.W, 1100, SkillShotType.Circular, 250, int.MaxValue, 500);
            E = new Spell.Skillshot(SpellSlot.E, 1050, SkillShotType.Linear, 250, 1400, 60);
            R = new Spell.Skillshot(SpellSlot.R, 3520, SkillShotType.Circular, 500, int.MaxValue, 120);

            Q.AllowedCollisionCount = int.MaxValue;
            W.AllowedCollisionCount = int.MaxValue;
            R.AllowedCollisionCount = int.MaxValue;

            Game.OnTick += OnTick;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private static float _previousLevel;

        private static void OnTick(EventArgs args)
        {
            // Adjust R range
            if (_previousLevel < R.Level)
            {
                R.Range = Convert.ToUInt32(2000 + 1200 * R.Level);
                _previousLevel = R.Level;
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                switch (args.SData.Name)
                {
                    // Ult activation
                    case "XerathLocusOfPower2":
                        LastChargePosition = Vector3.Zero;
                        LastChargeTime = 0;
                        ChargesRemaining = MaxCharges;
                        TapKeyPressed = false;
                        break;
                    // Ult charge usage
                    case "xerathlocuspulse":
                        LastChargePosition = args.End;
                        LastChargeTime = Environment.TickCount;
                        ChargesRemaining--;
                        TapKeyPressed = false;
                        break;
                }
            }
        }
        public static bool IsEnabled(this Spell.SpellBase spell, Orbwalker.ActiveModes mode)
        {
            var useQ = CastCheckbox(Combo, "Qc");
            var useW = CastCheckbox(Combo, "Wc");
            var useE = CastCheckbox(Combo, "Ec");
            var useR1 = CastCheckbox(Combo, "Rc");
            var useR = CastKey(Utimate, "Key");
            //Harass
            var usehQ = CastCheckbox(Harass, "Hq");
            var usehW = CastCheckbox(Harass, "Hw");
            var usehE = CastCheckbox(Harass, "He");
            //Lane
            var uselQ = CastCheckbox(Lane, "Ql");
            var uselW = CastCheckbox(Lane, "Wl");
            //Jungle
            var usejQ = CastCheckbox(Jungle, "Qj");
            var usejW = CastCheckbox(Jungle, "Wj");


            switch (mode)
            {
                case Orbwalker.ActiveModes.Combo:
                    switch (spell.Slot)
                    {
                        case SpellSlot.Q:
                            return useQ;
                        case SpellSlot.W:
                            return useW;
                        case SpellSlot.E:
                            return useE;
                        case SpellSlot.R:
                            return useR && useR1;
                    }
                    break;
                case Orbwalker.ActiveModes.Harass:
                    switch (spell.Slot)
                    {
                        case SpellSlot.Q:
                            return usehQ;
                        case SpellSlot.W:
                            return usehW;
                        case SpellSlot.E:
                            return usehE;
                    }
                    break;
                case Orbwalker.ActiveModes.LaneClear:
                    switch (spell.Slot)
                    {
                        case SpellSlot.Q:
                            return uselQ;
                        case SpellSlot.W:
                            return uselW;
                    }
                    break;
                case Orbwalker.ActiveModes.JungleClear:
                    switch (spell.Slot)
                    {
                        case SpellSlot.Q:
                            return usejQ;
                        case SpellSlot.W:
                            return usejW;

                    }
                    break;
            }

            return false;
        }

        public static bool IsBackRange(this Spell.SpellBase spell, Orbwalker.ActiveModes mode)
        {
            return spell.IsEnabled(mode) && spell.IsReady();
        }

        public static AIHeroClient GetTarget(this Spell.SpellBase spell, params AIHeroClient[] excludeTargets)
        {
            var targets = EntityManager.Heroes.Enemies.Where(o => o.IsValidTarget() && !excludeTargets.Contains(o) && spell.IsInRange(o)).ToArray();
            return TargetSelector.GetTarget(targets, DamageType.Magical);
        }

        public static bool CastOnBestTarget(this Spell.SpellBase spell)
        {
            var target = spell.GetTarget();
            return target != null && spell.Cast(target);
        }

       public static bool CastR()
        {
            if (!R.IsReady() || Game.Time < lastR + 0.8f)
                return false;
            int range = 2200 + (R.Level * 1320);
            var enemy = TargetSelector.GetTarget(range, DamageType.Magical);
            if (!enemy.IsValidTarget())
                return false;
            int enemyid = enemy.NetworkId;
            Vector2 enemypos = enemy.Position.To2D();
            float enemyspeed = enemy.MoveSpeed;
            Vector3[] path = enemy.Path;
            int lenght = path.Length;
            Vector3 predpos = Vector3.Zero;
            if (lenght > 1)
            {
                float s_in_time = enemyspeed * (Game.Time - Timers[enemyid] + (Game.Ping * 0.001f));
                float d = 0f;
                for (int i = 0; i < lenght - 1; i++)
                {
                    Vector2 vi = path[i].To2D();
                    Vector2 vi1 = path[i + 1].To2D();
                    d += vi.Distance(vi1);
                    if (d >= s_in_time)
                    {
                        float dd = enemypos.Distance(vi1);
                        float ss = enemyspeed * 0.5f;
                        if (dd >= ss)
                        {
                            predpos = (enemypos + ((vi1 - enemypos).Normalized() * ss)).To3D();
                            break;
                        }
                        if (i + 1 == lenght - 1)
                        {
                            predpos = (enemypos + ((vi1 - enemypos).Normalized() * enemypos.Distance(vi1))).To3D();
                            break;
                        }
                        for (int j = i + 1; j < lenght - 1; j++)
                        {
                            Vector2 vj = path[j].To2D();
                            Vector2 vj1 = path[j + 1].To2D();
                            ss -= dd;
                            dd = vj.Distance(vj1);
                            if (dd >= ss)
                            {
                                predpos = (vj + ((vj1 - vj).Normalized() * ss)).To3D();
                                break;
                            }
                            if (j + 1 == lenght - 1)
                            {
                                predpos = (vj + ((vj1 - vj).Normalized() * dd)).To3D();
                                break;
                            }
                        }
                        break;
                    }
                    if (i + 1 == lenght - 1)
                    {
                        predpos = (vi + ((vi1 - vi).Normalized() * vi.Distance(vi1))).To3D();
                        break;
                    }
                }
            }
            else
            {
                predpos = enemy.Position;
            }
            if (predpos.IsZero || predpos.Distance(Player.Instance.Position) > range || (int)path.LastOrDefault().X != (int)enemy.Path.LastOrDefault().X)
                return false;
            Player.Instance.Spellbook.CastSpell(SpellSlot.R, predpos);
            lastR = Game.Time;
            return true;
        }
    }
}