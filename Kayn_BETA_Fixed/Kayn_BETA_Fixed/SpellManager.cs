using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using static Kayn_BETA_Fixed.Program;
using static Kayn_BETA_Fixed.Menus;
using System.Linq;
using System.Collections.Generic;

namespace Kayn_BETA_Fixed
{
    public static class SpellManager
    {
        public static Spell.Skillshot Qk { get; private set; }
        public static Spell.Skillshot Wk { get; private set; }
        public static Spell.Active Ek { get; private set; }
        public static Spell.Targeted Rk { get; private set; }

        public static Spell.Skillshot Wa { get; private set; }
        public static Spell.Targeted Ra { get; private set; }

        public static Spell.SpellBase W
        {
            get { return Player.Instance.IsShadown() ? (Spell.SpellBase)Wa : Wk; }
        }

        public static Spell.SpellBase R
        {
            get { return Player.Instance.IsShadown() ? (Spell.SpellBase)Ra : Rk; }
        }


        internal static void Execute()
        {
            Qk = new Spell.Skillshot(spellSlot: SpellSlot.Q, spellRange: 550, skillShotType: SkillShotType.Circular, castDelay: 250, spellSpeed: 1550, spellWidth: 60)
            {
                AllowedCollisionCount = 0
            };
            Wk = new Spell.Skillshot(spellSlot: SpellSlot.W, spellRange: 700, skillShotType: SkillShotType.Cone, castDelay: 250, spellSpeed: 1550, spellWidth: 60)
            {
                AllowedCollisionCount = int.MaxValue
            };
            Ek = new Spell.Active(SpellSlot.E, 2000);
            Rk = new Spell.Targeted(SpellSlot.R, 550);

            //Assasin
            Wa = new Spell.Skillshot(SpellSlot.W, 900, SkillShotType.Linear);
            Ra = new Spell.Targeted(SpellSlot.W, 750);
        }

        public static Spell.SpellBase[] Spells
        {
            get { return new[] { Qk, W, Ek, R }; }
        }

        public static bool IsEnabled(this Spell.SpellBase spell, Orbwalker.ActiveModes mode)
        {
            var useQ = CastCheckbox(Combo, "Q");
            var useW = CastCheckbox(Combo, "W");
            var useE = CastCheckbox(Combo, "E");
            var useR1 = CastCheckbox(Combo, "R");
            //Harass
            var usehW = CastCheckbox(AutoHara, "AutoW");
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
                            return useR1;
                    }
                    break;
                case Orbwalker.ActiveModes.Harass:
                    switch (spell.Slot)
                    {
                        case SpellSlot.W:
                            return usehW;
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

        public static EntityManager.MinionsAndMonsters.FarmLocation? GetFarmLocation(
            this Spell.SpellBase spell,
            EntityManager.UnitTeam team = EntityManager.UnitTeam.Enemy,
            EntityManager.MinionsAndMonsters.EntityType type = EntityManager.MinionsAndMonsters.EntityType.Minion,
            IEnumerable<Obj_AI_Minion> targets = null)
        {
            // Get minions if not set
            if (targets == null)
            {
                switch (type)
                {
                    case EntityManager.MinionsAndMonsters.EntityType.Minion:
                        targets = EntityManager.MinionsAndMonsters.GetLaneMinions(team, Player.Instance.ServerPosition, spell.Range, false);
                        break;
                    case EntityManager.MinionsAndMonsters.EntityType.Monster:
                        targets = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition, spell.Range, false);
                        break;
                    default:
                        targets = EntityManager.MinionsAndMonsters.CombinedAttackable.Where(o => o.IsInRange(Player.Instance, spell.Range));
                        break;
                }
            }
            var allTargets = targets.ToArray();

            // Validate
            var skillshot = spell as Spell.Skillshot;
            if (skillshot == null || allTargets.Length == 0)
            {
                return null;
            }

            // Get best location to shoot
            var farmLocation = EntityManager.MinionsAndMonsters.GetLineFarmLocation(allTargets, skillshot.Width, (int)spell.Range);
            if (farmLocation.HitNumber == 0)
            {
                return null;
            }
            return farmLocation;
        }
    }
}