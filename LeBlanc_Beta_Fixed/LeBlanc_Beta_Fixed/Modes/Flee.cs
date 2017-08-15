using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using LeBlanc_Beta_Fixed;
namespace Modes
{
    class Flee : MyHero
    {
        public static void Execute()
        {
            var target = TargetSelector.GetTarget(Lib.E.Range, DamageType.Magical);
            if (target != null)
            {
                if (CastCheckbox(Menus.Flwe, "E"))
                {
                    var epred = Lib.E.GetPrediction(target);
                    if (epred.HitChance >= HitChance.Medium)
                    {
                        Lib.E.Cast(epred.CastPosition);
                    }
                }
            }
            if (CastCheckbox(Menus.Flwe, "W"))
            {
                var wpos = LeBlanc.Position.Extend(Game.CursorPos, Lib.W.Range).To3D();
                if (Lib.W.IsReady())
                {
                    Lib.CastW(wpos);
                }
            }
            if (CastCheckbox(Menus.Flwe, "R"))
            {
                if (Lib.RActive.IsReady())
                {
                    var wpos = LeBlanc.Position.Extend(Game.CursorPos, Lib.W.Range).To3D();
                    if (Lib.RActive.Name == "LeblancSlideM")
                    {
                        Lib.CastR(wpos);
                    }
                }
            }
        }
    }
}
