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
            if (Player.Instance.ChampionName != "Kayn") { return; }

            Chat.Print("[Addon]", System.Drawing.Color.LightBlue);
            Chat.Print("[Champion]", System.Drawing.Color.Red, "[Kayn]", System.Drawing.Color.Blue);

            new SpellsManager();
            new Menus();
            Interrupter.OnInterruptableSpell += Interrupter_Spell;
        }

        private static void Interrupter_Spell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            var wtarget = TargetSelector.GetTarget(W.Range, DamageType.Mixed);

            if (Misc["Inter"].Cast<CheckBox>().CurrentValue && W.IsReady() && W.GetPrediction(wtarget).HitChance >= HitChance.High)
            {
                if (_Player.Distance(_Player.ServerPosition, true) <= W.Range && W.GetPrediction(wtarget).HitChance >= HitChance.High)
                {
                    Q.Cast(wtarget);
                }
            }
        }
    }
}

       