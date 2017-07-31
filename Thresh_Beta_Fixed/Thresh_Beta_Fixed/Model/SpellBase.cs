using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using System.Collections.Generic;

namespace Thresh_Beta_Fixed.Model
{
        public enum SpellType
        {
            Self,
            Targeted,
            Linear,
            Circular,
            Cone,
            Unknown
        }
        public class CustomSettings
        {
            public int AllowedCollisionCount = int.MaxValue;
            public int CastDelay;
            public int HitChancePercent;
            public int Range;
            public GameObject Source;
            public int Speed;
            public SpellType Type = SpellType.Unknown;
            public int Width;
        }
    public class SpellBase
    {
        private readonly Dictionary<Obj_AI_Base, float> _cachedDamage = new Dictionary<Obj_AI_Base, float>();
        private readonly Dictionary<int, Dictionary<int, Dictionary<int, bool>>> _cachedIsOnSegment =
            new Dictionary<int, Dictionary<int, Dictionary<int, bool>>>();
        private readonly Dictionary<int, Dictionary<int, bool>> _cachedObjectsInRange =
            new Dictionary<int, Dictionary<int, bool>>();
        private readonly List<Obj_AI_Base> _enemyHeroes = new List<Obj_AI_Base>();
        private readonly List<Obj_AI_Base> _enemyMinions = new List<Obj_AI_Base>();
        private readonly List<Obj_AI_Base> _killableMinions = new List<Obj_AI_Base>();
        private readonly List<Obj_AI_Base> _laneClearMinions = new List<Obj_AI_Base>();
        private readonly List<Obj_AI_Base> _monsters = new List<Obj_AI_Base>();
        public readonly Dictionary<int, Dictionary<int, PredictionResult>> CachedPredictions =
            new Dictionary<int, Dictionary<int, PredictionResult>>();
        public bool EnemyHeroesCanBeCalculated;
        public bool EnemyMinionsCanBeCalculated;
        public bool LaneClearMinionsCanBeCalculated;
        public bool MonstersCanBeCalculated;
        private string _name;
        private int _speed;
        private int _width;
        public int AllowedCollisionCount = int.MaxValue;
        public bool Aoe;
        public int CastDelay;
        public bool CollidesWithYasuoWall = true;
        public int LastCastTime;
        public Vector3 LastEndPosition;
        public int LastSentTime;
        public Vector3 LastStartPosition;
        public float MinHitChancePercent = 60f;
        public int Range;
        public GameObject RangeCheckSourceObject = MyChampion.MyThresh;
        public Slider Slider;
        public SpellSlot Slot;
        public GameObject SourceObject = MyChampion.MyThresh;
        public SpellType Type;

    }
}
