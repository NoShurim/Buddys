using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using LeBlanc_Beta_Fixed;
namespace Modes
{
    class Killsteal : MyHero
    {
        public static void Execute()
        {
            if (CastSlider(Menus.Misc, "AutoW") > LeBlanc.HealthPercent)
            {
                if (Lib.W.Name == "leblancslidereturn")
                {
                    LeBlanc.Spellbook.CastSpell(SpellSlot.W);
                }
            }
            var target = TargetSelector.GetTarget(Lib.W.Range * 2 + Lib.Q.Range, DamageType.Magical);
            if (target == null || !target.IsValidTarget()) return;

            var RReady = Lib.RActive.Name.Equals("LeblancChaosOrbM") || Lib.RActive.Name.Equals("LeblancSoulShackleM") || Lib.RActive.Name.Equals("LeblancSlideM");
            var WReady = Lib.W.Name != "leblancslidereturn" && Lib.W.IsReady();
            var ksm = Menus.KillStea;
            var wpos = LeBlanc.Position.Extend(target, Lib.W.Range).To3D();


            var QDmg = Lib.Q.GetDamage(target);
            var WDmg = Lib.W.GetDamage(target);
            var EDmg = Lib.E.GetDamage(target);
            var RDmg = Lib.RActive.GetDamage(target);

            if (Lib.RActive.IsReady() && CastCheckbox(ksm, "R") && Lib.RActive.IsInRange(target))
            {
                if (QDmg < target.Health || !Lib.Q.IsReady())
                {
                    if (RDmg > target.Health)
                    {
                        Lib.CastR(target);
                    }
                }
            }
            if (QDmg + WDmg + EDmg > target.Health)
            {
                var epred = Lib.E.GetPrediction(target);
                if (epred.HitChance >= HitChance.Medium)
                {
                    if (Lib.Q.IsInRange(target) && CastCheckbox(ksm, "Q"))
                    {
                        Lib.Q.Cast(target);
                    }
                    if (Lib.W.IsInRange(target) && CastCheckbox(ksm, "W"))
                    {
                        Lib.CastW(target);
                    }
                    if (Lib.E.IsInRange(target) && CastCheckbox(ksm, "E"))
                    {
                        Lib.E.Cast(epred.CastPosition);
                    }
                }
            }
            if (Lib.Q.IsReady() && CastCheckbox(ksm, "Q") && QDmg > target.Health)
            {
                if (LeBlanc.IsInRange(target, Lib.Q.Range + Lib.W.Range))
                {
                    if (Lib.Q.IsInRange(target))
                    {
                        Lib.Q.Cast(target);
                    }
                    else if (WReady)
                    {
                        if (Lib.W.Range + Lib.Q.Range > LeBlanc.Distance(target) && CastCheckbox(ksm, "W") && CastCheckbox(ksm, "extW"))
                            Lib.CastW(wpos);
                    }
                    else if (RReady)
                    {
                        if (Lib.W.Range + Lib.Q.Range > LeBlanc.Distance(target) && CastCheckbox(ksm, "R") && CastCheckbox(ksm, "extW"))
                            Lib.CastR(wpos);
                    }
                }
                else if (LeBlanc.IsInRange(target, Lib.Q.Range + Lib.W.Range * 2) && Lib.Q.IsReady() && WReady && RReady)
                {
                    if (Lib.Q.IsInRange(target))
                    {
                        Lib.Q.Cast(target);
                    }
                    else if (CastCheckbox(ksm, "W") && CastCheckbox(ksm, "wr"))
                    {
                        Lib.CastW(wpos);
                        Core.DelayAction(() =>
                        Lib.CastR(wpos), (int)LeBlanc.Distance(wpos) / Lib.W.Speed + Game.Ping / 2);
                    }
                }
            }

            else if (Lib.E.IsReady() && CastCheckbox(ksm, "E") && EDmg > target.Health)
            {
                var epred = Lib.E.GetPrediction(target);

                if (LeBlanc.IsInRange(target, Lib.E.Range + Lib.W.Range))
                {
                    if (Lib.E.IsInRange(target) && epred.HitChance >= HitChance.High)
                    {
                        Lib.E.Cast(epred.CastPosition);
                    }
                    else if (WReady)
                    {
                        if (Lib.W.Range + Lib.E.Range > LeBlanc.Distance(target) && CastCheckbox(ksm, "W") && CastCheckbox(ksm, "extW"))
                            Lib.CastW(wpos);
                    }
                    else if (RReady)
                    {
                        if (Lib.W.Range + Lib.E.Range > LeBlanc.Distance(target) && CastCheckbox(ksm, "R") && CastCheckbox(ksm, "extW"))
                            Lib.CastR(wpos);
                    }
                }
                else if (LeBlanc.IsInRange(target, Lib.E.Range + Lib.W.Range * 2) && Lib.E.IsReady() && WReady && RReady)
                {
                    if (Lib.E.IsInRange(target) && epred.HitChance >= HitChance.High)
                    {
                        Lib.E.Cast(epred.CastPosition);
                    }
                    else if (CastCheckbox(ksm, "W") && CastCheckbox(ksm, "wr"))
                    {
                        Lib.CastW(wpos);
                        Core.DelayAction(() =>
                        Lib.CastR(wpos), (int)LeBlanc.Distance(wpos) / Lib.W.Speed + Game.Ping / 2);
                    }
                }
            }
            else if (WReady && CastCheckbox(ksm, "W") && WDmg > target.Health)
            {
                if (Lib.W.IsInRange(target))
                {
                    Lib.CastW(target);
                }
                else if (CastCheckbox(ksm, "wr"))
                {
                    if (LeBlanc.IsInRange(target, Lib.W.Range * 2))
                    {
                        Lib.CastR(wpos);
                    }
                }
            }
        }
    }
}

