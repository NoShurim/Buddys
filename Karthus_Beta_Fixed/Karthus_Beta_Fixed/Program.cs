using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX.Direct3D9;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Rendering;
using SettingsGap = Karthus_Beta_Fixed.SettingsMenus.Modes.GapCloseMenu;
using SettingsDraw = Karthus_Beta_Fixed.SettingsMenus.Modes.DrawingMenu;
using SettingsCombo = Karthus_Beta_Fixed.SettingsMenus.Modes.Combo;
using SharpDX;

namespace Karthus_Beta_Fixed
{
    class Program
    {
        private static AIHeroClient Karthus => Player.Instance;
        private static Font Tahoma13, Tahoma13B, Tahoma16B, TextBold;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnComplete;

        }

        private static void Loading_OnComplete(EventArgs args)
        {
            if (Karthus.Hero != Champion.Karthus) return;
            Chat.Print("[Addon] [Champion] [Karthus]", System.Drawing.Color.AliceBlue);
            SpellManager.Execute();
            ModeManager.Execute();
            MyLogic.Execute();
            //Comands
            Drawing.OnDraw += OnDraw;
            Gapcloser.OnGapcloser += GapCloser;
            Orbwalker.OnPreAttack += BeforeAttack;
            Game.OnUpdate += OnUpdate;
        }
        private static void GapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (SettingsGap.UseWonGap)
            {
                if (!sender.IsValidTarget(SpellManager.W.Range) || sender.IsMinion || sender.IsAlly)
                {
                    return;
                }
                if (SpellManager.W.IsReady() && sender.Distance(Player.Instance.Position) <= SpellManager.W.Range - 50)
                    SpellManager.W.Cast(sender.Position);
            }
        }

        private static void BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && SettingsCombo.useAA)
            {
                if (Player.Instance.Mana > 44)
                {
                    if (target.Type == GameObjectType.AIHeroClient)
                    {
                        args.Process = false;
                    }
                }
            }
        }

        public static float RDamage(Obj_AI_Base target)
        {
            var DMG = 0f;

            if (SpellManager.R.Level == 1)
            {
                DMG = 250f + (0.60f * Player.Instance.FlatMagicDamageMod);
                DMG = Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, DMG);
            }
            if (SpellManager.R.Level == 2)
            {
                DMG = 400f + (0.60f * Player.Instance.FlatMagicDamageMod);
                DMG = Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, DMG);
            }
            if (SpellManager.R.Level == 3)
            {
                DMG = 550f + (0.60f * Player.Instance.FlatMagicDamageMod);
                DMG = Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, DMG);
            }

            return DMG;
        }

        public static void drawText(string msg, Vector3 Hero, System.Drawing.Color Green, int weight = 0)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1] + weight, Green, msg);
        }

        public static void DrawFontTextScreen(Font vFont, string vText, float vPosX, float vPosY, ColorBGRA vColor)
        {
            vFont.DrawText(null, vText, (int)vPosX, (int)vPosY, vColor);
        }

        public static void DrawFontTextMap(Font vFont, string vText, Vector3 Pos, ColorBGRA vColor)
        {
            var wts = Drawing.WorldToScreen(Pos);
            vFont.DrawText(null, vText, (int)wts[0], (int)wts[1], vColor);
        }

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, System.Drawing.Color color)
        {
            var wts1 = Drawing.WorldToScreen(pos1);
            var wts2 = Drawing.WorldToScreen(pos2);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], bold, color);
        }

        class ChampionInfo
        {
            public int NetworkId { get; set; }

            public Vector3 LastVisablePos { get; set; }
            public float LastVisableTime { get; set; }
            public Vector3 PredictedPos { get; set; }

            public float StartRecallTime { get; set; }
            public float AbortRecallTime { get; set; }
            public float FinishRecallTime { get; set; }

            public ChampionInfo()
            {
                LastVisableTime = Game.Time;
                StartRecallTime = 0;
                AbortRecallTime = 0;
                FinishRecallTime = 0;
            }
        }

        static List<ChampionInfo> ChampionInfoList = new List<ChampionInfo>();
        public static Obj_AI_Base jungler;
        public static int timer, HitChanceNum = 4, tickNum = 4, tickIndex = 0;

        public static void LoadOKTW()
        {
            foreach (var hero in ObjectManager.Get<Obj_AI_Base>())
            {
                if (hero.IsEnemy && !hero.IsMinion && !hero.IsMe && !hero.IsStructure())
                {
                    ChampionInfoList.Add(new ChampionInfo() { NetworkId = hero.NetworkId, LastVisablePos = hero.Position });
                    if (IsJungler(hero))
                        jungler = hero;
                }
            }

            Game.OnUpdate += OnUpdate;
        }

        public static bool LagFree(int offset)
        {
            if (tickIndex == offset)
                return true;
            else
                return false;
        }

        private static void OnUpdate(EventArgs args)
        {
            tickIndex++;

            if (tickIndex > 4)
                tickIndex = 0;

            if (!LagFree(0))
                return;

            if (!Program.LagFree(3))
                return;

            foreach (var enemy in EntityManager.Heroes.Enemies.Where(enemy => enemy.IsValid))
            {
                var ChampionInfoOne = ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);
                if (enemy.IsVisible && !enemy.IsDead && enemy != null && enemy.IsValidTarget())
                {

                    if (ChampionInfoOne == null)
                    {
                        ChampionInfoList.Add(new ChampionInfo() { NetworkId = enemy.NetworkId, LastVisablePos = enemy.Position, LastVisableTime = Game.Time/*, PredictedPos = prepos */});
                    }
                    else
                    {
                        ChampionInfoOne.NetworkId = enemy.NetworkId;
                        ChampionInfoOne.LastVisablePos = enemy.Position;
                        ChampionInfoOne.LastVisableTime = Game.Time;
                    }
                }
                if (enemy.IsDead)
                {
                    if (ChampionInfoOne != null)
                    {
                        ChampionInfoOne.NetworkId = enemy.NetworkId;
                        ChampionInfoOne.LastVisablePos = ObjectManager.Get<Obj_SpawnPoint>().FirstOrDefault(x => x.IsEnemy).Position;
                        ChampionInfoOne.LastVisableTime = Game.Time;
                    }
                }
            }
        }

        private static bool IsJungler(Obj_AI_Base hero) { return hero.Spellbook.Spells.Any(spell => spell.Name.ToLower().Contains("smite")); }

        static bool IsInPassiveForm()
        {
            return ObjectManager.Player.IsZombie; //!ObjectManager.Player.IsHPBarRendered;
        }

        /*public static void UltKS()
        {
            if (!SpellManager.R.IsReady())
                return;
            var time = System.Environment.TickCount;

            if (SettingsCombo.useUltKS)
            {

                List<Obj_AI_Base> ultTargets = new List<Obj_AI_Base>();

                foreach (EnemyInfo target in Program.Helper.EnemyInfo.Where(x =>
                    x.Player.IsValid &&
                    !x.Player.IsDead &&
                    x.Player.IsEnemy &&
                    ((!x.Player.IsVisible && time - x.LastSeen < 10000) || x.Player.IsVisible) &&
                    !x.Player.IsMinion &&
                    ObjectManager.Player.GetSpellDamage(x.Player, SpellManager.R.Slot) >= Program.Helper.GetTargetHealth(x, (int)(SpellManager.R.CastDelay))))
                {
                    if (target.Player.IsVisible || (!target.Player.IsVisible && time - target.LastSeen < 2750)) //allies still attacking target? prevent overkill
                        if (Program.Helper.OwnTeam.Any(x => !x.IsMe && x.Distance(target.Player) < 1600))
                            continue;

                    if (IsInPassiveForm() || !Program.Helper.EnemyTeam.Any(x => x.IsValid && !x.IsDead && (x.IsVisible || (!x.IsVisible && time - Program.Helper.GetPlayerInfo(x).LastSeen < 2750)) && ObjectManager.Player.Distance(x) < 1600)) //any other enemies around? dont ult unless in passive form
                        ultTargets.Add(target.Player);
                }

                int targets = ultTargets.Count();

                if (targets > 0)
                {
                    //dont ult if Zilean is nearby the target/is the target and his ult is up
                    var zilean = Program.Helper.EnemyTeam.FirstOrDefault(x => x.BaseSkinName == "Zilean" && (x.IsVisible || (!x.IsVisible && time - Program.Helper.GetPlayerInfo(x).LastSeen < 3000)) && (x.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready ||
                                (x.Spellbook.GetSpell(SpellSlot.R).Level > 0 &&
                                x.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Surpressed /*&&
                                x.Mana >= x.Spellbook.GetSpell(SpellSlot.R).ManaCost*//*)));

                    if (zilean != null)
                    {
                        int inZileanRange = ultTargets.Count(x => x.Distance(zilean) < 2500); //if multiple, shoot regardless

                        if (inZileanRange > 0)
                            targets--; //remove one target, because zilean can save one
                    }

                    if (targets > 0)
                        SpellManager.R.Cast();
                }
            }
        }*/

        private static void OnDraw(EventArgs args)
        {
            try

            {

                //if ((int)(Game.Time * 10) % 2 == 0)
                // blink = false;

                float posY = ((float)SettingsDraw.PosY * 0.01f) * Drawing.Height;
                float posX = ((float)SettingsDraw.PosX * 0.01f) * Drawing.Width;
                float positionDraw = 0;
                var FillColor = System.Drawing.Color.GreenYellow;
                var Color = System.Drawing.Color.Azure;
                //float offset = 0;

                foreach (var enemy in EntityManager.Heroes.Enemies.Where(a => a.IsEnemy && a.IsValid))
                {
                    var kolor = System.Drawing.Color.GreenYellow;

                    if (enemy.IsDead)
                        kolor = System.Drawing.Color.Gray;
                    else if (!enemy.IsVisible)
                        kolor = System.Drawing.Color.OrangeRed;

                    var kolorHP = System.Drawing.Color.GreenYellow;

                    if (enemy.IsDead)
                        kolorHP = System.Drawing.Color.GreenYellow;
                    else if ((int)enemy.HealthPercent < 30)
                        kolorHP = System.Drawing.Color.Red;
                    else if ((int)enemy.HealthPercent < 60)
                        kolorHP = System.Drawing.Color.Orange;

                    if (SettingsDraw.ChampInfo)
                    {
                        positionDraw += 15;
                        DrawFontTextScreen(Tahoma13B, "Lv." + enemy.Level + "-", posX - 43, posY + positionDraw, SharpDX.Color.White);
                        DrawFontTextScreen(Tahoma13B, enemy.ChampionName, posX, posY + positionDraw, SharpDX.Color.White);

                        if (true)
                        {
                            var fSlot = enemy.Spellbook.Spells[4];
                            if (fSlot.Name != "summonerflash")
                                fSlot = enemy.Spellbook.Spells[5];

                            if (fSlot.Name == "summonerflash")
                            {
                                var fT = fSlot.CooldownExpires - Game.Time;
                                if (fT < 0)
                                    DrawFontTextScreen(Tahoma13B, "F: Ready", posX + 97, posY + positionDraw, SharpDX.Color.GreenYellow);
                                else
                                    DrawFontTextScreen(Tahoma13B, "F: Off / CD :" + (int)fT + "s.", posX + 97, posY + positionDraw, SharpDX.Color.Yellow);
                            }

                            if (enemy.Level > 5)
                            {
                                var rSlot = enemy.Spellbook.Spells[3];
                                var t = rSlot.CooldownExpires - Game.Time;

                                if (t < 0)
                                    DrawFontTextScreen(Tahoma13B, "R(Ult): Ready", posX + 170, posY + positionDraw, SharpDX.Color.GreenYellow);
                                else
                                    DrawFontTextScreen(Tahoma13B, "R(Ult): Off / CD : " + (int)t + "s.", posX + 170, posY + positionDraw, SharpDX.Color.Yellow);
                            }
                            else
                                DrawFontTextScreen(Tahoma13B, "R(Ult): Off", posX + 170, posY + positionDraw, SharpDX.Color.Yellow);
                        }

                    }
                }
                //DRAW OTKW END

                // Draw range circles of our spells and attack range
                if (SettingsDraw.DrawMyRange)
                {
                    Circle.Draw(SharpDX.Color.Blue, Player.Instance.AttackRange, Player.Instance.Position);
                }
                if (SettingsDraw.DrawEnemyRange)
                {
                    foreach (var enemy_rg in ObjectManager.Get<Obj_AI_Base>().Where(x => x.CountEnemiesInRange(2500) >= Player.Instance.CountEnemiesInRange(2500) && x.IsEnemy))
                    {
                        if (!enemy_rg.IsValidTarget(2500))
                        {
                            continue;
                        }

                        Drawing.DrawCircle(enemy_rg.Position, enemy_rg.AttackRange, System.Drawing.Color.Red);
                    }
                }
                // TODO: Uncomment if you want those enabled aswell, but remember to enable them
                // TODO: in the SpellManager aswell, otherwise you will get a NullReferenceException
                if (SettingsDraw.DrawQ)
                {
                    Circle.Draw(SharpDX.Color.Green, SpellManager.Q.Range, Player.Instance.Position);
                }
                if (SettingsDraw.DrawW)
                {
                    Circle.Draw(SharpDX.Color.Yellow, SpellManager.W.Range, Player.Instance.Position);
                }
                if (SettingsDraw.DrawE)
                {
                    Circle.Draw(SharpDX.Color.Purple, SpellManager.E.Range, Player.Instance.Position);
                }

                // DRAW LAST HIT
                if (SettingsDraw.DrawLH)
                {
                    foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(x => x.CountEnemiesInRange(2500) >= Player.Instance.CountEnemiesInRange(2500) && x.IsEnemy))
                    {
                        if (!minion.IsValidTarget(2500))
                        {
                            continue;
                        }

                        if (minion.Health <= Player.Instance.GetAutoAttackDamage(minion, true))
                        {
                            if (SettingsDraw.DrawLH)
                            {
                                var x = 5;
                                var y = 15;
                                var z = 20;

                                while (x >= 1)
                                {
                                    Drawing.DrawCircle(minion.Position, minion.BoundingRadius + x, System.Drawing.Color.Red);
                                    x--;
                                }

                                while (y >= 7)
                                {
                                    Drawing.DrawCircle(minion.Position, minion.BoundingRadius + y, System.Drawing.Color.Red);
                                    y--;
                                }

                                while (z >= 16)
                                {
                                    Drawing.DrawCircle(minion.Position, minion.BoundingRadius + y, System.Drawing.Color.Red);
                                    z--;
                                }
                            }
                        }
                    }
                }
                // END LAST HIT

                // DRAW R ALERT
                //var EnemiesTxt = "";

                //Show Enemies
                //var enemies = HeroManager.Enemies.Where(a => a.IsEnemy && a.IsValid);
                //Vector2 WTS = Drawing.WorldToScreen(Player.Instance.Position);

                /*var time = System.Environment.TickCount;

                foreach (EnemyInfo target in Program.Helper.EnemyInfo.Where(x =>
                    x.Player.IsValid &&
                    !x.Player.IsDead &&
                    x.Player.IsEnemy &&
                    !x.Player.IsMinion &&
                    !x.Player.IsMe &&
                    !x.Player.IsStructure() &&
                    ((!x.Player.IsVisible && time - x.LastSeen < 10000) || (x.Player.IsVisible)) &&
                    ObjectManager.Player.GetSpellDamage(x.Player, SpellSlot.R) >= Program.Helper.GetTargetHealth(x, (int)(SpellManager.R.CastDelay))))
                {
                    EnemiesTxt += target.Player.BaseSkinName + " ";
                }*/

                /*foreach (var enemy in enemies)
                {

                    if ((enemy.Health - RDamage(enemy)) <= 0)
                    {
                        if (!enemy.IsDead)
                        {
                            EnemiesTxt = enemy.BaseSkinName + " | ";

                        }
                    }
                }*/

                //Drawing.DrawText(WTS[0] - 100, WTS[1] + 100, System.Drawing.Color.Green, "State E :" + Player.Instance.Spellbook.GetSpell(SpellSlot.E).ToggleState.ToString());

                /*if (EnemiesTxt != "")
                {
                    if (SpellManager.R.IsLearned && SpellManager.R.IsReady() && SettingsDraw.DrawRalert)
                    {
                        DrawFontTextScreen(Tahoma16B, "R Alert : " + EnemiesTxt + "Killable", (float)(WTS[0] - 150), (float)(WTS[1] + 80), SharpDX.Color.Red);
                    }
                }*/

                var EnemiesTxt = "";

                //Show Enemies
                var enemies = EntityManager.Heroes.Enemies.Where(a => a.IsEnemy && a.IsValid);
                Vector2 WTS = Drawing.WorldToScreen(Player.Instance.Position);

                foreach (var enemy in enemies)
                {
                    if ((GetTargetHealth(enemy) - RDamage(enemy)) <= 0)
                    {
                        if (!enemy.IsDead)
                        {
                            EnemiesTxt = enemy.BaseSkinName + " | ";

                        }
                    }
                }

                //Drawing.DrawText(WTS[0] - 100, WTS[1] + 100, System.Drawing.Color.Green, "State E :" + Player.Instance.Spellbook.GetSpell(SpellSlot.E).ToggleState.ToString());

                if (EnemiesTxt != "")
                {
                    if (SpellManager.R.IsLearned && SpellManager.R.IsReady() && SettingsDraw.DrawRalert)
                    {
                        DrawFontTextScreen(Tahoma16B, "R Alert : " + EnemiesTxt + "Killable", (float)(WTS[0] - 150), (float)(WTS[1] + 80), SharpDX.Color.Red);
                    }
                }

                //END DRAW R ALERT

            }
            catch (Exception e)
            {
                // Please enable the debug window to see and solve the exceptions that might occur!
                //Logger.Log(LogLevel.Error, "Error executing mode '{0}'\n{1}", mode.GetType().Name, e);
            }

        }

        public static float GetTargetHealth(AIHeroClient playerInfo)
        {
            if (playerInfo.IsVisible)
                return playerInfo.Health;

            var predictedhealth = playerInfo.Health + playerInfo.HPRegenRate * (SpellManager.R.CastDelay / 1000);

            return predictedhealth > playerInfo.MaxHealth ? playerInfo.MaxHealth : predictedhealth;
        }

    }
}

    