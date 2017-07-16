using EloBuddy;
using EloBuddy.SDK;
using static Kayn_Official.Champi;
using System;

namespace Kayn_Official
{
    internal class DamageIndicator
    {
        public static float Wdama(Obj_AI_Base target)
        {
            if (W.IsReady())
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, new[] { 0f, 70, 110f, 150f, 190f, 230f }[W.Level] + 0.6f * ObjectManager.Player.FlatMagicDamageMod);
            else
                return 0f;
        }
        public static float Rdama(Obj_AI_Base target)
        {
            if (R.IsReady())
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, new[] { 150f, 250f, 350f }[R.Level] + 1f * ObjectManager.Player.FlatPhysicalDamageMod + 0.15f * target.Health);
            else
                return 0f;
        }
        public static float SpellsDMG(Obj_AI_Base target)
        {
            if (target == null)
            {
                return 0;
            }
            else if (target != null)
            {
                return Wdama(target) + Rdama(target);
            }
            else return 0f;
        }
        internal static void Indicator(EventArgs args)
        {
            
        }
    }
}