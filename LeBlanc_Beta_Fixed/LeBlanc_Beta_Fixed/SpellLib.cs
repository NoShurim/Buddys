using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using SharpDX;

namespace LeBlanc_Beta_Fixed
{
    class Lib
    {
        public static Spell.Targeted Q, QUtimat, RTargeted;
        public static Spell.Skillshot W, E;
        public static Spell.Active RActive;
        public static float QlasTick;


        static Lib()
        {
            Q = new Spell.Targeted(SpellSlot.Q, 700);

            W = new Spell.Skillshot(SpellSlot.W, 600, SkillShotType.Circular, 0, 1600, 260)
            {
                AllowedCollisionCount = -1
            };

            E = new Spell.Skillshot(SpellSlot.E, 950, SkillShotType.Linear, 250, 1750, 55)
            {
                AllowedCollisionCount = 0
            };

            RActive = new Spell.Active(SpellSlot.R);
            QUtimat = new Spell.Targeted(SpellSlot.Q, 700);
            RTargeted = new Spell.Targeted(SpellSlot.R, int.MaxValue);
        }

        public static void CastW(Obj_AI_Base target)
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "leblancslidereturn") return;

            var wpred = W.GetPrediction(target);
            if (wpred.HitChance >= HitChance.Medium)
            {
                W.Cast(wpred.CastPosition);
            }
        }
        public static void CastW(Vector3 target)
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name != "leblancslidereturn")
            {
                W.Cast(target);
            }
        }
        public static void CastR(Obj_AI_Base target)
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancR") // Q
            {
                if (target.IsValidTarget(Q.Range))
                {
                    RActive.Cast(target);
                }
            }
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancR") // W
            {
                if (target.IsValidTarget(W.Range))
                {
                    var wpred = W.GetPrediction(target).CastPosition;
                    RActive.Cast(wpred);
                }
            }
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancR") // E
            {
                if (target.IsValidTarget(E.Range))
                {
                    var epred = E.GetPrediction(target);
                    if (epred.HitChance >= HitChance.Medium)
                    {
                        RActive.Cast(epred.CastPosition);
                    }
                }
            }
        }
        public static void CastR(Vector3 target)
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSlideM") // W
            {
                RTargeted.Cast(target);
            }
        }
    }    
}