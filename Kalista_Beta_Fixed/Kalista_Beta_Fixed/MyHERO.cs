using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using SharpDX;
using System;
using static Kalista_Beta_Fixed.SpellsManager;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK.Rendering;

namespace Kalista_Beta_Fixed
{
    class MyHero
    {
        private static AIHeroClient Kalista => Player.Instance;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnComple;
        }

        private static void Loading_OnComple(EventArgs args)
        {
            if (Kalista.Hero != Champion.Kalista) return;
            Chat.Print("[Addon] [Kalista]", System.Drawing.Color.AliceBlue);

            Menus.Execute();
            GameProcessoIntGames.Execute();
            DamageIndicator.Initialize(Damages.GetRendDamage);
            DamageIndicator.DrawingColor = System.Drawing.Color.Goldenrod;
            Logics.Execute();
            Drawing.OnDraw += On_Draw;
            Spellbook.OnCastSpell += OnCastSpell;
            SpellsManager.Execute();
            MyKalist.Execute();
        }

        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe && args.Slot == SpellSlot.Q && Player.Instance.IsDashing())
            {
                args.Process = false;
            }
        }

        private static void On_Draw(EventArgs args)
        {
            if (Menus.Drawing.DrawQ)
            {
                Circle.Draw(Color.LightBlue, Q.Range, Player.Instance.Position);
            }
            if (Menus.Drawing.DrawW)
            {
                Circle.Draw(Color.LightBlue, W.Range, Player.Instance.Position);
            }
            if (Menus.Drawing.DrawE)
            {
                Circle.Draw(Color.LightBlue, E.Range, Player.Instance.Position);
            }
            if (Menus.Drawing.DrawR)
            {
                Circle.Draw(Color.LightBlue, R.Range, Player.Instance.Position);
            }
          
        }
    }
}
