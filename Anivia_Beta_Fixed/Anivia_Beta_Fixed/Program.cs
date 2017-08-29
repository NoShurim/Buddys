using Anivia_Beta_Fixed.Modes;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Rendering;
using SharpDX;
using System;
using Settings = Anivia_Beta_Fixed.Config.Misc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anivia_Beta_Fixed
{
    class Program
    {
        public static AIHeroClient Ani => Player.Instance;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnComplete;
        }

        private static void Loading_OnComplete(EventArgs args)
        {
            if (Ani.Hero != Champion.Anivia) return;


            Config.Initialize();
            SpellManager.Initialize();
            ModeManager.Initialize();
            SaveMePls.Initialize();
            if (Settings.autolevelskills)
            {
                Player.Instance.Spellbook.LevelSpell(SpellSlot.Q);
            }

            // Listen to events we need
            Drawing.OnDraw += OnDraw;
            Player.OnLevelUp += Anivia_Beta_Fixed.Modes.PermaActive.autoLevelSkills;
            Dash.OnDash += PermaActive.Dash_OnDash;
            Gapcloser.OnGapcloser += PermaActive.antiGapcloser;
            GameObject.OnCreate += PermaActive.GameObject_OnCreate;
            if (Settings._drawQ.CurrentValue || Settings._drawQ.CurrentValue || Settings._drawE.CurrentValue || Settings._drawR.CurrentValue)
                EloBuddy.SDK.Notifications.Notifications.Show(new EloBuddy.SDK.Notifications.SimpleNotification("Q missing", "If Q is missing, turn up the Q accuracy in the settings to about 140 - 150. Good Luck, Summoner"), 20000);
        }


        private static void OnDraw(EventArgs args)
        {
            // Draw range circles of our spells
            if (Settings._drawQ.CurrentValue)
                Circle.Draw(Color.Red, SpellManager.Q.Range, Player.Instance.Position);
            if (Settings._drawW.CurrentValue)
                Circle.Draw(Color.Aqua, SpellManager.W.Range, Player.Instance.Position);
            if (Settings._drawE.CurrentValue)
                Circle.Draw(Color.DarkGreen, SpellManager.E.Range, Player.Instance.Position);
            if (Settings._drawR.CurrentValue)
                Circle.Draw(Color.DarkOrange, SpellManager.R.Range, Player.Instance.Position);

            if (Settings.drawComboDmg)
            {
                foreach (var e in EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget() && e.IsHPBarRendered))
                {
                    //combodamage
                    float damage = SpellManager.DamageToHero(e);
                    if (damage <= 0)
                        continue;

                    var damagePercentage = ((e.Health - damage) > 0 ? (e.Health - damage) : 0) / e.MaxHealth;
                    var currentHealthPercentage = e.Health / e.MaxHealth;

                    var start = new Vector2((int)(e.HPBarPosition.X) + damagePercentage * 100 - 10, (int)(e.HPBarPosition.Y) - 5);
                    var end = new Vector2((int)(e.HPBarPosition.X) + currentHealthPercentage * 100 - 10, (int)(e.HPBarPosition.Y) - 5);

                    // Draw the line
                    Drawing.DrawLine(start, end, 20, System.Drawing.Color.Lime);
                    if (e.Health - damage < 0)
                    {
                        Drawing.DrawText(e.HPBarPosition.X, e.HPBarPosition.Y - 20, System.Drawing.Color.Lime, "KILLABLE", 18);
                    }
                }
            }
        }
    }
}
