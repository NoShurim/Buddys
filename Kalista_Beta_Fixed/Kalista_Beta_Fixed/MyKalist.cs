using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace Kalista_Beta_Fixed
{
    public static class MyKalist
    {
        public static HashSet<Obj_AI_Base> Entities;
        public static HashSet<Obj_AI_Minion> Minions;
        public static HashSet<AIHeroClient> Heroes;

        private static Dictionary<int, BuffInstance> Buffs;
        private static Dictionary<int, float> RDamages;
        private static Dictionary<int, bool> Killable;

        public static bool HasRendBuff(this Obj_AI_Base target)
        {
            return GetRendBuff(target) != null;
        }

        public static BuffInstance GetRendBuff(this Obj_AI_Base target)
        {
            return Buffs.ContainsKey(target.NetworkId) ? Buffs[target.NetworkId] : null;
        }

        public static float GetRendDamage(this Obj_AI_Base target)
        {
            return RDamages.ContainsKey(target.NetworkId) ? RDamages[target.NetworkId] : 0;
        }

        public static bool IsRendKillable(this Obj_AI_Base target)
        {
            return Killable.ContainsKey(target.NetworkId) && Killable[target.NetworkId];
        }

        internal static void Execute()
        {
            Entities = new HashSet<Obj_AI_Base>();
            Minions = new HashSet<Obj_AI_Minion>();
            Heroes = new HashSet<AIHeroClient>();
            Buffs = new Dictionary<int, BuffInstance>();
            RDamages = new Dictionary<int, float>();
            Killable = new Dictionary<int, bool>();

            Game.OnTick += On_Tick;
        }

        private static void On_Tick(EventArgs args)
        {
            Entities.Clear();
            Buffs.Clear();
            foreach (var entity in ObjectManager.Get<Obj_AI_Base>().Where(o => o.IsValidTarget(onlyEnemyTeam: true)))
            {
                var rendBuff = GetRendBuffInternal(entity);
                if (rendBuff != null)
                {
                    Entities.Add(entity);
                    Buffs[entity.NetworkId] = rendBuff;
                }
            }
            Minions = new HashSet<Obj_AI_Minion>(Entities.OfType<Obj_AI_Minion>());
            Heroes = new HashSet<AIHeroClient>(Entities.OfType<AIHeroClient>());

            RDamages.Clear();
            Killable.Clear();
            foreach (var entity in Entities.ToArray())
            {
     
                var rendBuff = entity.GetRendBuff();

                if (rendBuff != null)
                {
                    var damage = Damages.GetRendDamage(entity, rendBuff: rendBuff);
                    var killable = Damages.IsRendKillable(entity, damage);

             
                    RDamages[entity.NetworkId] = damage;
                    Killable[entity.NetworkId] = killable;
                }
                else
                {

                    Entities.Remove(entity);
                    Minions.Remove(entity as Obj_AI_Minion);
                    Heroes.Remove(entity as AIHeroClient);
                }
            }
        }

        private static BuffInstance GetRendBuffInternal(Obj_AI_Base target)
        {
            return target.Buffs.Find(b => b.Caster.IsMe && b.IsValid() && b.DisplayName == "KalistaExpungeMarker");
        }
    }
}
