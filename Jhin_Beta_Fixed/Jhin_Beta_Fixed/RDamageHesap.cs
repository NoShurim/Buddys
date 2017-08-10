using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Color = System.Drawing.Color;

namespace Jhin_Beta_Fixed
{
    public static class RDamageHesap
    {
        public static float CalculateDamage(Obj_AI_Base target, bool R)
        {
            var totaldamage = 0f;

            if (Jhin_Beta_Fixed.Program.R.IsReady())
            {
                totaldamage = totaldamage + RDamage(target);
            }

            return totaldamage;
        }

       

        private static float RDamage(Obj_AI_Base target)
        {
            if (!Jhin_Beta_Fixed.Program.R.IsLearned) return 0;
            var level = Jhin_Beta_Fixed.Program.R.Level - 1;

            if (target.Distance(Program._Player) < 1350)
            {
                return Program._Player.CalculateDamageOnUnit(target, DamageType.Physical,
                    (float)
                        (new double[] { 25, 35, 45 }[level] +
                         new double[] { 25, 30, 35 }[level] / 100 * (target.MaxHealth - target.Health) +
                         0.1 * Program._Player.TotalAttackDamage));
            }

            return Program._Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)
                    (new double[] { 250, 350, 450 }[level] +
                     new double[] { 25, 30, 35 }[level] / 100 * (target.MaxHealth - target.Health) +
                     1 * Program._Player.TotalAttackDamage));
        }
    }
}
        

