using System;
using EloBuddy;
using EloBuddy.SDK.Events;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using System.Linq;
using EloBuddy.SDK.Enumerations;
using static Kayn.Menus;
using static Kayn.SpellsManager;

namespace Kayn
{
    class Program
    {

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }

        }
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_On;
        }

        private static void Loading_On(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Kayn) { return; }

            Chat.Print("[Addon]", System.Drawing.Color.LightBlue);
            Chat.Print("[Champion]", System.Drawing.Color.Red, "[Kayn]", System.Drawing.Color.Blue);

            new SpellsManager();
            new Menus();
        }
    }
}

       