using EloBuddy;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using LeBlanc_Beta_Fixed;

namespace Modes
{
    class AntiGapcloser : MyHero
    {
        public static void Execute(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender.IsAlly || sender.IsDead || sender.IsMe) return;

            if (CastCheckbox(Menus.AntiGapcloser, "E"))
            {
                if (Lib.E.IsReady())
                {
                    var epred = Lib.E.GetPrediction(sender);
                    if (epred.HitChance >= HitChance.Medium)
                    {
                        Lib.E.Cast(epred.CastPosition);
                    }
                }
            }
            if (CastCheckbox(Menus.AntiGapcloser, "RE"))
            {
                if (Lib.RActive.Name == "LeblancSoulShackleM")
                {
                    if (Lib.RActive.IsReady())
                    {
                        var epred = Lib.E.GetPrediction(sender);
                        if (epred.HitChance >= HitChance.Medium)
                        {
                            Lib.RActive.Cast(epred.CastPosition);
                        }
                    }
                }
            }
        }
    }
}