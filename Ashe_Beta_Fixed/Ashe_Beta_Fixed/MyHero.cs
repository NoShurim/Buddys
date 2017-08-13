using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Rendering;
using static Ashe_Beta_Fixed.SpellManager;
using SharpDX;
using Settings = Ashe_Beta_Fixed.MenusSetting.Misc;
using EloBuddy.SDK.Enumerations;

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
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;
            Interrupter.OnInterruptableSpell += Interupt;
            Orbwalker.OnPostAttack += ResetAttack;
            GameObject.OnCreate += GameObject_OnCreate;
        }

        private static void Gapcloser_OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender.IsEnemy && e.Sender.Distance(Ashe) <= 350)
            {
                W.Cast(e.Sender);
            }
        }


        private static void Interupt(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (!sender.IsEnemy || !(sender is AIHeroClient) || Player.Instance.IsRecalling())
            {
                return;
            }

            if (R.IsReady() && e.DangerLevel == DangerLevel.High && Ashe.Distance(sender) <= 1500)
            {
                R.Cast(sender);
            }
        }

        private static void ResetAttack(AttackableUnit target, EventArgs args)
        {
            throw new NotImplementedException();
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            var khazix = EntityManager.Heroes.Enemies.Find(e => e.ChampionName.Equals("Khazix"));
            var rengar = EntityManager.Heroes.Enemies.Find(e => e.ChampionName.Equals("Rengar"));
            if (rengar != null)
            {
                if (sender.Name == ("Rengar_LeapSound.troy") && R.IsReady() && sender.Position.Distance(Ashe) <= 300)
                {
                    R.Cast(rengar);
                }
            }

            if (khazix != null)
            {
                if (sender.Name == ("Khazix_Base_E_Tar.troy") && R.IsReady() && sender.Position.Distance(Ashe) <= 300)
                {
                    R.Cast(khazix);
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Settings._drawW.CurrentValue)

                Circle.Draw(Color.Aqua, SpellManager.W.Range, Player.Instance.Position);

        }
    }
}
