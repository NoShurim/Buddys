using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using static Thresh_Beta_Fixed.Menus;
using EloBuddy.SDK.Events;
using System;
using SharpDX;
using System.Linq;

namespace Thresh_Beta_Fixed
{
    internal class Spells
    {
        public static Spell.Skillshot Q;
        public static Spell.Active Q2;
        public static Spell.Skillshot E;
        public static Spell.Skillshot W;
        public static Spell.Active R;

        private static AIHeroClient _Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        internal static void Execute()
        {
            Q = new Spell.Skillshot(spellSlot: SpellSlot.Q, spellRange: 1050, skillShotType: SkillShotType.Linear, castDelay: 250, spellSpeed: 1550, spellWidth: 60)
            {
                AllowedCollisionCount = 0
            };
            W = new Spell.Skillshot(spellSlot: SpellSlot.W, spellRange: 950, skillShotType: SkillShotType.Circular, castDelay: 250, spellSpeed: 1550, spellWidth: 60)
            {
                AllowedCollisionCount = 1
            };
            E = new Spell.Skillshot(spellSlot: SpellSlot.W, spellRange: 775, skillShotType: SkillShotType.Linear, castDelay: 250, spellSpeed: 1550, spellWidth: 60)
            {
                AllowedCollisionCount = int.MaxValue
            };
            Q2 = new Spell.Active(SpellSlot.Q);

            R = new Spell.Active(SpellSlot.R, 450);
        }

        internal static void CastQ(Obj_AI_Base target)
        {
            var preq = Q.GetPrediction(target);
            if (preq.HitChancePercent >= Pq())
            {
                Q.Cast();
            }
            else
            {
                Q2.Cast();
            }

            if (Wc() && W.IsReady())
            {
                foreach (var ally in EntityManager.Heroes.Allies.Where(x => x.IsValidTarget(W.Range)))
                {
                    W.Cast(ally);
                }
            }
        }

        internal static void CastE(Obj_AI_Base targete)
        {
            if (E.IsReady() && targete.IsInRange(_Player, E.Range))
            {
                E.Cast(targete.Position.Extend(_Player.Position, Vector3.Distance(targete.Position, _Player.Position) + 400).To3D());
            }
        }
    }
}