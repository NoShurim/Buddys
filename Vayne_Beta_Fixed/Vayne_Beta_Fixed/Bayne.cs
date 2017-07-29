using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace Vayne_Beta_Fixed
{
    class Bayne
    {
        private static AIHeroClient Vayne => Player.Instance;
        public static AIHeroClient thuTarget;
        public static float Seen = Game.Time;
        public static Vector3 predictedPos;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_Vayne;
        }

        private static void Loading_Vayne(EventArgs args)
        {
            if (Vayne.Hero != Champion.Vayne) return;
            Chat.Print("[Vayne Is Beta]");

            Menus.Execute();
            SpellndCast.Execute();
            ModeManager.Execute();
            LogicOber.Execute();
            Beta.Execute();
        }
    }
}
