using EloBuddy;
using EloBuddy.SDK;
using static Gragas_Beta_Fixed.MyHero;

namespace Gragas_Beta_Fixed
{
    internal class DamageLib
    {

        private static readonly AIHeroClient _Player = ObjectManager.Player;
        public static float QCalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)(new[] { 0, 80, 120, 160, 200, 240 }[Q.Level] + 0.6f * _Player.FlatMagicDamageMod
                    ));
        }

        public static float WCalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)(new[] { 0, 20, 50, 80, 110, 140 }[W.Level] + 0.3f * _Player.FlatMagicDamageMod + 0.08f * target.HealthPercent
                    ));
        }

        public static float ECalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)(new[] { 0, 80, 130, 180, 230, 280 }[E.Level] + 0.6f * _Player.FlatMagicDamageMod
                    ));
        }

        public static float RCalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)(new[] { 0, 200, 300, 400 }[R.Level]  + 0.8f * _Player.FlatMagicDamageMod
                    ));
        }
        public static float DmgCalc(AIHeroClient target)
        {
            var damage = 0f;
            if (Q.IsReady() && target.IsValidTarget(Q.Range))
                damage += QCalc(target);
            if (W.IsReady())
                damage += WCalc(target);
            if (E.IsReady() && target.IsValidTarget(E.Range))
                damage += ECalc(target);
            if (R.IsReady() && target.IsValidTarget(R.Range))
                damage += RCalc(target);
            damage += _Player.GetAutoAttackDamage(target, true) * 2;
            return damage;
        }


    }
}