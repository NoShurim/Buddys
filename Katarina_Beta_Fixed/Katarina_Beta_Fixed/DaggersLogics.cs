using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Katarina_Beta_Fixed
{
    class DaggersLogics
    {
        public static IEnumerable<Obj_AI_Base> GetDaggers()
        {
            return ObjectManager.Get<Obj_AI_Base>().Where(a => a.Name == "HiddenMinion" && a.IsValid && a.Health == 100);
        }

        public static Vector3 GetClosestDagger()
        {
            var dagger = GetDaggers();
            if (!dagger.Any() || dagger == null || dagger.Count() <= 0) return new Vector3();
            var t = dagger.Where(p => p.Distance(Player.Instance) >= 125).OrderBy(p => p.Distance(Player.Instance.Position)).FirstOrDefault();
            return t == null ? new Vector3() : t.Position;
        }

        public static Vector3 GetBestDaggerPoint(Vector3 position, Obj_AI_Base target)
        {
            if (target.Position.IsInRange(position, 150)) return position;
            return position.Extend(target, 150).To3D();
        }

        public static Vector3 LogicDistacne(Vector3 position, Obj_AI_Base target)
        {
            if (target.Position.IsInRange(position, 200)) return position;
            return position.Extend(target, 200).To3D();
        }

        public static Vector3 LogicInstance(Vector3 position, Obj_AI_Base Player)
        {
            if (Player.ServerPosition.IsInRange(position, -50)) return position;
            return position.Extend(Player, -50).To3D();
        }

        public static Vector3 ELogic(Vector3 position, Obj_AI_Base Player)
        {
            if (Player.ServerPosition.IsInRange(position, 50)) return position;
            return position.Extend(Player, 50).To3D();
        }

    }
}
