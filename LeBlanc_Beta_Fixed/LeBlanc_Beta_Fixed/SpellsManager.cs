using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using SharpDX;

namespace LeBlanc_Beta_Fixed
{
    class Lib
    {



        public static Spell.Targeted Q;
        public static Spell.Skillshot W, E;
        public static Spell.Active RActive, WAc;
        public static Spell.Targeted Ignite;


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
            WAc = new Spell.Active(SpellSlot.W);

            Ignite = new Spell.Targeted(SpellSlot.Summoner1, 600);

        }
        public static void CastQ(Obj_AI_Base unit)
        {
            if (!Q.IsReady())
            {
                return;
            }
            if (MyHero.LeBlanc.Distance(unit) < Q.Range)
            {
                Q.Cast(unit);
            }
        }

        public static void CastW(Vector3 pos)
        {
            if (!W.IsReady() || MyHero.LeBlanc.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() != "leblancw")
            {
                return;
            }
            if (MyHero.LeBlanc.Distance(pos) < W.Range)
            {
                W.Cast(pos);
            }
        }

        public void CastW2()
        {
            if (!W.IsReady() || MyHero.LeBlanc.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() != "leblancwreturn")
            {
                return;
            }
            W.Cast();
        }

        public static void CastE(Obj_AI_Base unit)
        {
            if (!E.IsReady())
            {
                return;
            }
            if (MyHero.LeBlanc.Distance(unit) < E.Range)
            {
                E.Cast(unit);
            }
        }
        public static void CastR(string mode, Obj_AI_Base unit, Vector3 Rpos = new Vector3())
        {

            if (!RActive.IsReady())
            {
                return;
            }
            switch (mode)
            {
                case "D":
                    if (MyHero.LeBlanc.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancrtoggle")
                    {
                        RActive.Cast();
                    }
                    else
                    {
                        RActive.Cast(Rpos);
                    }
                    break;
                case "RQ":
                    if (MyHero.LeBlanc.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancrtoggle")
                    {
                        RActive.Cast();
                        CastQ(unit);
                    }
                    break;
                case "RW":
                    if (MyHero.LeBlanc.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancrtoggle")
                    {
                        RActive.Cast();
                        CastW(unit.ServerPosition);
                    }
                    break;
                case "RE":
                    if (MyHero.LeBlanc.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancrtoggle")
                    {
                        RActive.Cast();
                        CastE(unit);
                    }
                    break;
                case "Return":
                    if (MyHero.LeBlanc.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancrtoggle")
                    {
                        RActive.Cast();
                        if (MyHero.LeBlanc.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancrwreturn")
                        {
                            RActive.Cast();
                        }
                    }
                    break;
            }
        }
    }
}