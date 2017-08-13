using System;
using EloBuddy;
using EloBuddy.SDK.Events;

namespace Nunu_Beta_Fixed
{
    public static class MyHero
    {
        private static AIHeroClient Nunu => Player.Instance;

        public static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            if (Nunu.Hero != Champion.Nunu) return;
            Chat.Print("[Addon] [Champion] [Nunu]", System.Drawing.Color.AliceBlue);

            MenusSettings.Execute();
            SpellManager.Execute();
            ModeManager.Execute();
            SmiteDamage.Execute();
            Damage.Execute();
            Events.Execute();
        }
    }
}
