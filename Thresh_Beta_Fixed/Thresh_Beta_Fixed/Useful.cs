using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using SharpDX;
using static Thresh_Beta_Fixed.Heros.Assassin;
using static Thresh_Beta_Fixed.Heros.Atiradores;
using static Thresh_Beta_Fixed.Heros.NewClass;
using static Thresh_Beta_Fixed.Heros.Supports;
using static Thresh_Beta_Fixed.Heros.Thanks;

namespace Thresh_Beta_Fixed
{
   public static class Useful
    {
        public static float HealthPercent(this List<AIHeroClient> list, float range)
        {
            return list.Where(h => h.IsValidTarget(range)).Sum(h => h.HealthPercent);
        }

        public static int CountEnemiesInside(this AIHeroClient unit, float range = float.MaxValue)
        {
            return EntityManager.Heroes.Enemies.Count(h => h.IsValidTarget() && unit.IsInRange(h, range));
        }
        public static bool IsInEnemyTurret(this Obj_AI_Base unit)
        {
            if (unit == null || !unit.IsValid || unit.IsDead) return false;
            var turret =
                EntityManager.Turrets.Enemies.FirstOrDefault(
                    m => m != null && m.Health > 0 && m.IsValid && unit.Distance(m, true) <= Math.Pow(750f + 80, 2));
            return turret != null;
        }

        private static Vector3 Source(this Spell.Skillshot s)
        {
            return s.SourcePosition ?? MyChampion.MyThresh.Position;
        }

        public static Obj_AI_Base JungleClear(this Spell.Skillshot s, bool useCast = true, int numberOfHits = 1)
        {
            if (!s.IsReady() || numberOfHits <= 0) return null;
            var minions =
                EntityManager.MinionsAndMonsters.GetJungleMonsters(s.Source(), s.Range + s.Width)
                    .OrderBy(m => m.MaxHealth);
            if (!minions.Any() || minions.Count() < numberOfHits) return null;
            switch (s.Type)
            {
                case SkillShotType.Linear:
                    var t = s.GetBestLineTarget(minions.ToList<Obj_AI_Base>());
                    if (t.Item1 >= numberOfHits)
                    {
                        if (useCast)
                        {
                            s.Cast(t.Item2);
                        }
                        return t.Item2;
                    }
                    break;
                case SkillShotType.Circular:
                    var t2 = s.GetBestCircularTarget(minions.ToList<Obj_AI_Base>());
                    if (t2.Item1 < numberOfHits) return null;
                    if (useCast)
                    {
                        s.Cast(t2.Item2);
                    }
                    return t2.Item2;
            }
            return null;
        }

        public static Obj_AI_Base LaneClear(this Spell.Skillshot s, int numberOfHits = 1, bool useCast = true)
        {
            if (!s.IsReady() || numberOfHits <= 0) return null;
            var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, s.Source(),
                s.Range + s.Width);
            if (!minions.Any() || minions.Count() < numberOfHits) return null;
            switch (s.Type)
            {
                case SkillShotType.Linear:
                    var t = s.GetBestLineTarget(minions.ToList<Obj_AI_Base>());
                    if (t.Item1 >= numberOfHits)
                    {
                        if (useCast)
                        {
                            s.Cast(t.Item2);
                        }
                        return t.Item2;
                    }
                    break;
                case SkillShotType.Circular:
                    var t2 = s.GetBestCircularTarget(minions.ToList<Obj_AI_Base>());
                    if (t2.Item1 >= numberOfHits)
                    {
                        if (useCast)
                        {
                            s.Cast(t2.Item2);
                        }
                        return t2.Item2;
                    }
                    break;
            }
            return null;
        }

        private static int CountObjectsOnLineSegment(this Spell.Skillshot s, List<Obj_AI_Base> list, Vector3 endPosition)
        {
            return (from obj in list
                    let info = obj.Position.To2D().ProjectOn(s.Source().To2D(), endPosition.To2D())
                    where info.IsOnSegment && info.SegmentPoint.Distance(obj.Position.To2D(), true) <= s.Width
                    select obj).Count();
        }

        public static Tuple<int, Obj_AI_Base> GetBestLineTarget(this Spell.Skillshot s, List<Obj_AI_Base> list)
        {
            Obj_AI_Base bestTarget = null;
            var bestHit = -1;
            var List =
                list.Where(m => m.IsValidTarget() && s.Source().Distance(m, true) <= Math.Pow(s.Range, 2)).ToList();
            foreach (var obj in List)
            {
                var endPosition = s.Source() + (obj.Position - s.Source()).Normalized() * s.Range;
                var hit = s.CountObjectsOnLineSegment(List, endPosition);
                if (hit <= bestHit) continue;
                bestHit = hit;
                bestTarget = obj;
                if (bestHit == List.Count)
                {
                    break;
                }
            }
            return new Tuple<int, Obj_AI_Base>(bestHit, bestTarget);
        }

        private static int CountObjectsNearTo(this Spell.Skillshot s, List<Obj_AI_Base> list, Obj_AI_Base target)
        {
            return list.Count(obj => target.Distance(obj, true) <= Math.Pow(s.Width, 2));
        }

        public static Tuple<int, Obj_AI_Base> GetBestCircularTarget(this Spell.Skillshot s, List<Obj_AI_Base> list)
        {
            Obj_AI_Base bestTarget = null;
            var bestHit = -1;
            var List =
                list.Where(m => m.IsValidTarget() && s.Source().Distance(m, true) <= Math.Pow(s.Range + s.Width, 2))
                    .ToList();
            foreach (var obj in List)
            {
                var hit = s.CountObjectsNearTo(List, obj);
                if (hit <= bestHit) continue;
                bestHit = hit;
                bestTarget = obj;
                if (bestHit == List.Count)
                {
                    break;
                }
            }
            return new Tuple<int, Obj_AI_Base>(bestHit, bestTarget);
        }
        public static int GetPriority(this AIHeroClient hero)
        {
            var championName = hero.ChampionName;

            if (tanks.Contains(championName))
            {
                return 1;
            }
            if (Support.Contains(championName))
            {
                return 2;
            }
            if (Assasinos.Contains(championName))
            {
                return 3;
            }
            if (Magos.Contains(championName))
            {
                return 4;
            }
            return Atirador.Contains(championName) ? 5 : 1;
        }
    }
}

