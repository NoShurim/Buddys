using System;
using System.Collections.Generic;
using static Vayne_Beta_Fixed.ModeBase;
using EloBuddy;


namespace Vayne_Beta_Fixed
{
    internal class ModeManager
    {
        private static List<ModeBase> Modes { get; set; }

        static ModeManager()
        {
            Modes = new List<ModeBase>();
            Modes.AddRange(new ModeBase[]
            {
                new byCombo(),
                new byHarass(),
                new byLaneClear(),
                new byJungleClear(),

            });
            Game.OnTick += OnTick;
        }
        internal static void Execute()
        {
            //
        }
        private static void OnTick(EventArgs args)
        {
            Modes.ForEach(mode =>
            {
                try
                {
                    if (mode.ShouldBeExecuted())
                    {
                        mode.Execute();
                    }
                }
                catch (Exception)
                {
                }
            });
        }
    }
}