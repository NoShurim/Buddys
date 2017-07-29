using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using static Vayne_Beta_Fixed.SpellndCast;
using EloBuddy.SDK.Events;
using SharpDX;

namespace Vayne_Beta_Fixed
{
    internal class LogicOber
    {
        internal static void Execute()
        {
            //Logic

        }
        internal static void CastQ(bool onChamps)
        {
            if (!Q.IsReady())
                return;
            if (Menus.QSettings.useQToMouse
                || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) && Menus.QSettings.lastHitQToMouse
                || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) && Menus.QSettings.laneClearQToMouse
                || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) && Menus.QSettings.laneClearQToMouse
                || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && Menus.QSettings.comboQToMouse
                || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && Menus.QSettings.harassQToMouse
                )
            {
                Player.CastSpell(SpellSlot.Q, Game.CursorPos);
                return;
            }
        }       
        private static float damage(AIHeroClient t)
        {
            float dmg = 0;
            if (E.IsReady())
            {
                dmg += DamageLibrary.GetSpellDamage(Player.Instance, t, SpellSlot.E);
            }
            dmg += DamageLibrary.GetSpellDamage(Player.Instance, t, SpellSlot.Q);
            dmg += Player.Instance.GetAutoAttackDamage(t);
            return dmg;
        }

        private static bool defQ()
        {
            if (Player.Instance.HealthPercent < Menus.QSettings.lowHPQ)
                return true;

            if (Player.Instance.CountEnemiesInRange(550) - (Player.Instance.CountAlliesInRange(450) + 1) >= Menus.QSettings.defQ)
            {
                return true;
            }
            return false;
        }

        private static AIHeroClient condemnable(Vector3 myPosition)
        {
            List<AIHeroClient> condemnables = new List<AIHeroClient>();
            foreach (var e in EntityManager.Heroes.Enemies.Where(t => t.IsInRange(Player.Instance, E.Range) && t.HealthPercent > 0).OrderBy(t => t.Health))
            {
                if (e.HasBuffOfType(BuffType.SpellImmunity) || e.HasBuffOfType(BuffType.SpellShield))
                    continue;

                Vector2 castTo = Prediction.Position.PredictUnitPosition(e, 600);
                Vector3 pos = e.ServerPosition;
                for (float i = e.BoundingRadius / 2; i < 410; i += e.BoundingRadius)
                {
                    var coll = Player.Instance.ServerPosition.Extend(castTo, Player.Instance.Distance(castTo) + i).To3D();
                    var collOrigin = Player.Instance.ServerPosition.Extend(pos, Player.Instance.Distance(pos) + i).To3D();
                    if ((coll.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Wall) || coll.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Building)) ||
                        (collOrigin.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Wall) || collOrigin.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Building)))
                    {
                        condemnables.Add(e);
                    }
                }
            }
            foreach (var e in condemnables)
            {
                var pred = E2.GetPrediction(e);
                if (pred.HitChance < HitChance.High)
                    continue;
                for (float i = e.BoundingRadius / 2; i < 410; i += e.BoundingRadius)
                {
                    var coll = Player.Instance.Position.To2D().Extend(pred.UnitPosition.To2D(), i + Player.Instance.Distance(pred.UnitPosition));
                    var collOrigin = Player.Instance.Position.To2D().Extend(pred.CastPosition.To2D(), i + Player.Instance.Distance(pred.CastPosition));
                    if ((coll.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Wall) || coll.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Building)) ||
                        (collOrigin.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Wall) || collOrigin.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Building)))
                    {
                        return e;
                    }
                }
            }
            return null;
        }

        internal static void CastE()
        {
            if (!E.IsReady())
                return;

            if (!Player.Instance.IsDashing() && (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && Menus.ESettings.comboPinToWall) || (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && Menus.ESettings.harassPinToWall))
            {
                AIHeroClient t = condemnable(Player.Instance.Position);
                if (t != null)
                    E.Cast(t);
            }
            if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && Menus.ESettings.comboEProcW) || (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && Menus.ESettings.harassEProcW))
            {
                foreach (var e in EntityManager.Heroes.Enemies.Where(t => t.IsInRange(Player.Instance, E.Range) && t.HealthPercent > 0))
                {
                    if (e.GetBuffCount("vaynesilvereddebuff") == 2)
                    {
                        E.Cast(e);
                        return;
                    }
                }
            }
        }
    }
}

 