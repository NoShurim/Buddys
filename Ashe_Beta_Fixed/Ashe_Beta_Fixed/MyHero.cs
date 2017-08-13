using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Settings = Ashe_Beta_Fixed.MenusSetting.Misc;
using Ashe_Beta_Fixed;

namespace Ashe_Beta_Fixed
{
    public static class MyHero
    {

        private static AIHeroClient Ashe => Player.Instance;

        public static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            if (Ashe.Hero != Champion.Ashe) return;
            Chat.Print("[Addon] [Champion] [Ashe]", System.Drawing.Color.AliceBlue);
  
            MenusSetting.Execute();
            SpellManager.Execute();
            ModeManager.Execute();
            SaveMePls.Execute();

            if (Settings.autolevelskills)
            {
                Player.Instance.Spellbook.LevelSpell(SpellSlot.W);
            }

            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnLevelUp += Modes.ActiveCast.autoLevelSkills;
            
        }

        private static void OnDraw(EventArgs args)
        {
            if (Settings._drawW.CurrentValue)

                Circle.Draw(Color.Aqua, SpellManager.W.Range, Player.Instance.Position);

        }
    }
}
