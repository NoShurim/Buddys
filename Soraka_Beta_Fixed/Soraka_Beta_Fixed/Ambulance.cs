using System;
using EloBuddy.SDK.Events;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Enumerations;
using System.Linq;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace Soraka_Beta_Fixed
{
    class Ambulance
    {
        public static AIHeroClient Soraka => Player.Instance;

        public static bool isChecked(Menu obj, string value)
        {
            return obj[value].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderValue(Menu obj, string value)
        {
            return obj[value].Cast<Slider>().CurrentValue;
        }
        private const int XOffset = 10;
        private const int YOffset = 20;
        private const int Width = 103;
        private const int Height = 8;

        private static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnComplete;
        }

        private static void Loading_OnComplete(EventArgs args)
        {
            if (Soraka.Hero != Champion.Soraka) return;
            Bootstrap.Init(null);
            Spells.Execute();
            Menus.fodeu();
            Game.OnTick += Game_OnTick;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            var unit = sender;
            var spell = e;
            var useE = isChecked(Menus.Misc, "eInterrupt");
            if (!useE || spell.DangerLevel != DangerLevel.High) return;
            if (!unit.IsValidTarget(Spells.E.Range)) return;
            if (!Spells.E.IsReady()) return;
            Spells.E.Cast(unit);
        }

        private static void Gapcloser_OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            var useQ = isChecked(Menus.Misc, "useQGapCloser");
            var useE = isChecked(Menus.Misc, "useEGapCloser");
            if (useQ && Spells.Q.IsReady() && sender.IsEnemy && !sender.IsZombie)
            {
                Spells.Q.Cast(e.End);
            }
            if (useE && Spells.E.IsReady() && sender.IsEnemy && !sender.IsZombie)
            {
                Spells.E.Cast(e.End);
            }
        }
        private static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            var useAAminion = isChecked(Menus.Misc, "AttackMinions");
            if (useAAminion && target.Type == GameObjectType.obj_AI_Minion)
            {
                var allyinrange = EntityManager.Heroes.Allies.Count(x => !x.IsMe && x.Distance(_Player) <= 1200);
                if (allyinrange > 0)
                {
                    args.Process = false;
                }
            }
        }
        private static void Game_OnTick(EventArgs args)
        {
            var autoW =  isChecked(Menus.Healing, "useW");
            if (autoW && Spells.W.IsReady())
            {
                AutoW();
            }
            if (Spells.R.IsReady())
            {
                AutoR();
            }
            switch (Orbwalker.ActiveModesFlags)
            {
                case Orbwalker.ActiveModes.Combo:
                    Combo();
                    return;
                case Orbwalker.ActiveModes.Harass:
                    Harass();
                    return;
                case Orbwalker.ActiveModes.None:
                    break;
                case Orbwalker.ActiveModes.LastHit:
                    break;
                case Orbwalker.ActiveModes.JungleClear:
                    break;
                case Orbwalker.ActiveModes.LaneClear:
                    break;
                case Orbwalker.ActiveModes.Flee:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void Combo()
        {
            var autoW = isChecked(Menus.Healing, "useW");
            if (autoW && Spells.W.IsReady())
            {
                AutoW();
            }
            if (Spells.R.IsReady())
            {
                AutoR();
            }
            var useQ = isChecked(Menus.Combo, "useQCombo");
            var useE = isChecked(Menus.Combo, "useECombo");
            var minMana = getSliderValue(Menus.Combo, "minMcombo");

            var target = TargetSelector.GetTarget(Spells.Q.Range, DamageType.Magical);
            if (target == null) return;
            if (useQ && Spells.Q.IsReady() && _Player.ManaPercent >= minMana)
            {
                Spells.Q.Cast(target);
            }
            if (useE && Spells.E.IsReady() && _Player.ManaPercent >= minMana)
            {
                Spells.E.Cast(target);
            }
        }
        private static void Harass()
        {
            var autoW = Menus.Healing["useW"].Cast<CheckBox>().CurrentValue;
            if (autoW && Spells.W.IsReady())
            {
                AutoW();
            }
            if (Spells.R.IsReady())
            {
                AutoR();
            }
            var useQ = isChecked(Menus.Harass, "useQHarass");
            var useE = isChecked(Menus.Harass, "useEHarass");
            var minMana = getSliderValue(Menus.Harass, "minMharass");

            var target = TargetSelector.GetTarget(Spells.Q.Range, DamageType.Magical);
            if (target == null) return;
            if (useQ && Spells.Q.IsReady() && _Player.ManaPercent >= minMana &&
                Spells.Q.GetPrediction(target).HitChance >= getPred())
            {
                Spells.Q.Cast(target);
            }
            if (useE && Spells.E.IsReady() && _Player.ManaPercent >= minMana &&
                Spells.E.GetPrediction(target).HitChance >= getPred())
            {
                Spells.E.Cast(target);
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            var hBar = isChecked(Menus.Drawing, "drawH");
            if (hBar)
            {
                foreach (var pos in from hero in EntityManager.Heroes.Allies let pos = hero.HPBarPosition where !hero.IsDead && !hero.IsMe && hero.HealthPercent <= getSliderValue(Menus.Healing, "wpct" + hero.ChampionName) select pos)
                {
                    Drawing.DrawText(pos.X + 110, pos.Y - 5, System.Drawing.Color.Tomato, "H");
                }
            }
            var QRange = isChecked(Menus.Drawing, "drawQ");
            var ERange = isChecked(Menus.Drawing, "drawE");
            if (QRange)
            {
                Circle.Draw(Color.LightSkyBlue, Spells.Q.Range, Player.Instance.Position);
            }
            if (ERange)
            {
                Circle.Draw(Color.DeepSkyBlue, Spells.E.Range, Player.Instance.Position);
            }
        }

        #region Auto R

        public static void AutoR()
        {
            var useR = isChecked(Menus.Healing, "useR");
            if (!Spells.R.IsReady() && useR) return;
            if (ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && x.IsValidTarget(float.MaxValue)).Select(x => (int)x.Health / x.MaxHealth * 100).Select(friendHealth => new { friendHealth, health = getSliderValue(Menus.Healing, "useRslider") }).Where(x => x.friendHealth <= x.health).Select(x => x.friendHealth).Any())
            {
                Spells.R.Cast();
            }
        }

        #endregion

        #region Auto W

        public static void AutoW()
        {
            var test = EntityManager.Heroes.Allies.Where(hero => !hero.IsMe && !hero.IsDead && !hero.IsInShopRange() && !hero.IsZombie && hero.Distance(_Player) <= Spells.W.Range && Menus.Healing["w" + hero.ChampionName].Cast<CheckBox>().CurrentValue && hero.HealthPercent <= Menus.Healing["wpct" + hero.ChampionName].Cast<Slider>().CurrentValue).ToList();
            var allytoheal = test.OrderBy(x => x.Health).FirstOrDefault(x => !x.IsInShopRange());
            if (allytoheal != null)
            {
                Spells.W.Cast(allytoheal);
            }
        }

        #endregion

        #region Calcs

        public static HitChance getPred()
        {
            var preDS = getSliderValue(Menus.Harass, "predNeeded");
            switch (preDS)
            {
                case 1:
                    return HitChance.Low;
                case 2:
                    return HitChance.Medium;
                case 3:
                    return HitChance.High;
            }
            return HitChance.Unknown;
        }

        #endregion

        #region Healing Bar Calcs

        private static float WHeal(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical, (float)(new[] { 120, 150, 180, 210, 240 }[Spells.W.Level] + 0.6 * _Player.FlatMagicDamageMod));
        }

        public static void DrawWbar()
        {
            foreach (var unit in ObjectManager.Get<AIHeroClient>().Where(h => h.IsValid && h.IsHPBarRendered && h.IsAlly))
            {
                var barPos = unit.HPBarPosition;
                var healing = WHeal(unit);
                var pctgAfterHeal = Math.Max(0, unit.Health + healing) / unit.MaxHealth;
                var yPos = barPos.Y + YOffset;
                var xPosDamage = barPos.X + XOffset + Width * pctgAfterHeal;
                var xPosCurrentHp = barPos.X + XOffset + Width * unit.Health / unit.MaxHealth;

                if (healing > unit.Health)
                {
                    Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + Height, 2, System.Drawing.Color.Lime);
                }
                var diffhp = xPosCurrentHp + xPosDamage;
                var pos1 = barPos.X + 9 + (107 * pctgAfterHeal);
                for (var i = 0; i < diffhp; i++)
                {
                    Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, System.Drawing.Color.Goldenrod);
                }
            }
        }

        #endregion
    }
}
