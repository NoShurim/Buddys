using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using LeBlanc_Beta_Fixed;
namespace Modes
{
    class Harass : MyHero
    {
        public static void Execute()
        {
            var target = TargetSelector.GetTarget(Lib.W.Range * 2, DamageType.Magical);
            if (target == null) return;
            var _Q = CastCheckbox(Menus.Haras, "Q") && Lib.Q.IsReady() && CastSlider(Menus.Haras, "QMana") < LeBlanc.ManaPercent;
            var _W = CastCheckbox(Menus.Haras, "W") && Lib.W.Name != "leblancslidereturn" && Lib.W.IsReady() && CastSlider(Menus.Haras, "WMana") < LeBlanc.ManaPercent;
            var _E = CastCheckbox(Menus.Haras, "E") && Lib.E.IsReady() && CastSlider(Menus.Haras, "EMana") < LeBlanc.ManaPercent;
            var extW = CastCheckbox(Menus.Haras, "extW");
            var wpos = LeBlanc.Position.Extend(target, Lib.W.Range).To3D();


            if (CastCheckbox(Menus.Haras, "AutoW"))
            {
                if (Lib.W.Name == "leblancslidereturn" && !_Q && !_E)
                {
                   LeBlanc.Spellbook.CastSpell(SpellSlot.W);
                }
            }
            if (_Q)
            {
                if (Lib.Q.IsInRange(target))
                {
                    Lib.Q.Cast(target);
                }
                else if (extW && LeBlanc.IsInRange(target, Lib.Q.Range + Lib.W.Range))
                {
                    Lib.CastW(wpos);
                }
            }
            else if (_W)
            {
                var wpred = Lib.W.GetPrediction(target);
                Lib.W.Cast(wpred.CastPosition);
            }
            else if (_E)
            {
                var epred = Lib.E.GetPrediction(target);
                if (epred.HitChance >= HitChance.High)
                {
                    Lib.E.Cast(epred.CastPosition);
                }
            }
        }
    }
}