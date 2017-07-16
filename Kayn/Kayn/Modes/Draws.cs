using EloBuddy;
using static Kayn.Menus;
using static Kayn.SpellsManager;
using System;
using EloBuddy.SDK.Menu.Values;
using static EloBuddy.SDK.Geometry.Polygon;
using SharpDX;

namespace Kayn.Modes
{
    internal class Draws
    {
        internal static void Execute()
        {
            Drawing.OnDraw += Draws_Load;
        }

        private static void Draws_Load(EventArgs args)
        {

            if (Menus.Draws["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
            }
            if (Menus.Draws["W"].Cast<CheckBox>().CurrentValue && W.IsReady())
            {
            }
            if (Menus.Draws["R"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
            }               
        }
    }
}