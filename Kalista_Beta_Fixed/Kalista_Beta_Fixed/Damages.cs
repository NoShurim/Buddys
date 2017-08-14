using EloBuddy;
using EloBuddy.SDK;
using static Kalista_Beta_Fixed.MyKalist;

namespace Kalista_Beta_Fixed
{
    public static class Damages
    {
        public static readonly Damage.DamageSourceBoundle QDamage = new Damage.DamageSourceBoundle();

        private static readonly float[] Damage1 = { 20, 30, 40, 50, 60 };
        private static readonly float[] Damage2 = { 0.6f, 0.6f, 0.6f, 0.6f, 0.6f };
        private static readonly float[] Damage3 = { 5, 9, 14, 20, 27 };
        private static readonly float[] Damage4 = { 0.15f, 0.18f, 0.21f, 0.24f, 0.27f };

        static Damages()
        {
            QDamage.Add(new Damage.DamageSource(SpellSlot.Q, DamageType.Physical)
            {
                Damages = new float[] { 10, 70, 130, 190, 250 }
            });
            QDamage.Add(new Damage.BonusDamageSource(SpellSlot.Q, DamageType.Physical)
            {
                DamagePercentages = new float[] { 1, 1, 1, 1, 1 }
            });
        }

        public static bool IsRendKillable(Obj_AI_Base target, float? damage = null)
        {
            if (target == null || !target.IsValidTarget() || !target.HasRendBuff())
            {
                return false;
            }

            var totalHealth = target.TotalShieldHealth();

            var hero = target as AIHeroClient;
            if (hero != null)
            {
                if (hero.HasUndyingBuff() || hero.HasSpellShield())
                {
                    return false;
                }

                if (hero.ChampionName == "Blitzcrank" && !target.HasBuff("BlitzcrankManaBarrierCD") && !target.HasBuff("ManaBarrier"))
                {
                    totalHealth += target.Mana / 2;
                }
            }

            return (damage ?? GetRendDamage(target)) > totalHealth;
        }

        public static float GetRendDamage(AIHeroClient target)
        {
            return GetRendDamage(target, -1);
        }

        public static float GetRendDamage(Obj_AI_Base target, int customStacks = -1, BuffInstance rendBuff = null)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, GetRawRendDamage(target, customStacks, rendBuff) - Menus.Misc.DamageReductionE) *
                   (Player.Instance.HasBuff("SummonerExhaustSlow") ? 0.6f : 1); 
        }

        public static float GetRawRendDamage(Obj_AI_Base target, int customStacks = -1, BuffInstance rendBuff = null)
        {
            rendBuff = rendBuff ?? target.GetRendBuff();
            var stacks = (customStacks > -1 ? customStacks : rendBuff != null ? rendBuff.Count : 0) - 1;
            if (stacks > -1)
            {
                var index = SpellsManager.E.Level - 1;
                return Damage1[index] + stacks * Damage3[index] +
                       Player.Instance.TotalAttackDamage * (Damage2[index] + stacks * Damage4[index]);
            }

            return 0;
        }
    }
}
