using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using System;
using System.Linq;
using static Kayn.Menus;
using static Kayn.SpellsManager;

namespace Kayn.Modes
{
    internal class KillSeteal
    {
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }

        }

        internal static void Execute()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            var targetw = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            var targetq = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (target == null || target.IsInvulnerable || target.MagicImmune) { return; }

            if (Misc["KS1"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                foreach (var enemy in EntityManager.Heroes.Enemies.Where(x => x.Distance(_Player) <= Q.Range && x.IsValidTarget() && !x.IsInvulnerable && !x.IsZombie))
                {
                    if (targetq.IsValidTarget(Q.Range) && Q.IsReady() && QDamage(enemy) >= enemy.Health)
                    {
                        Q.Cast(targetq);
                    }
                }
                if (Misc["KS2"].Cast<CheckBox>().CurrentValue && W.IsReady())
                {
                    foreach (var enemy in EntityManager.Heroes.Enemies.Where(x => x.Distance(_Player) <= W.Range && x.IsValidTarget() && !x.IsInvulnerable && !x.IsZombie))
                    {
                        if (targetq.IsValidTarget(W.Range) && W.IsReady() && WDamage(enemy) >= enemy.Health)
                        {
                            W.Cast(targetw);
                        }
                    }
                    if (Misc["KS3"].Cast<CheckBox>().CurrentValue && R.IsReady())
                    {
                        foreach (var enemy in EntityManager.Heroes.Enemies.Where(x => x.Distance(_Player) <= R.Range && x.IsValidTarget() && !x.IsInvulnerable && !x.IsZombie))
                        {
                            if (targetq.IsValidTarget(R.Range) && R.IsReady() && RDamage(enemy) >= enemy.Health)
                            {
                                R.Cast(target);
                            }
                        }
                    }
                }
            }
        }
    }
}