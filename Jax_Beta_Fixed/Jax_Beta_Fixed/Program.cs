using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Jax_Beta_Fixed
{
    class Program
    {
        //Champion
        private static AIHeroClient Jax => Player.Instance;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnComplete;
        }

        private static void Loading_OnComplete(EventArgs args)
        {
            if (Jax.Hero != Champion.Jax) return;
            Chat.Print("[Addon] [Champion] [Jax]", System.Drawing.Color.AliceBlue);
        }
    }
}