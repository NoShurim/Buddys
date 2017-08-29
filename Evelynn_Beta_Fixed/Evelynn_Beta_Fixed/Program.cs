using System;
using EloBuddy.SDK.Events;
using EloBuddy;

namespace Evelynn_Beta_Fixed
{
    class Program
    {
        public static AIHeroClient Evely => Player.Instance;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_LoadingBasic;
        }

        private static void Loading_LoadingBasic(EventArgs args)
        {
            if (Evely.Hero != Champion.Evelynn) return;
        }
    }
}
