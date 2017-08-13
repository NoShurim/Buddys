using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

using Settings = Ashe_Beta_Fixed.MenusSetting.Modes.LaneClear;
namespace Ashe_Beta_Fixed.Modes
{
    public sealed class LaneClear : ModeBase
    {
        public override bool ExecuteOnComands()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear);
        }

        public override void Execute()
        {
            if (Settings.mana >= Player.Instance.ManaPercent)
                return;

            if (Settings.UseQ && Q.IsReady())
            {
                foreach(var b in Player.Instance.Buffs)
                    if (b.Name == "asheqcastready")
                    {
                        Q.Cast();
                    }
            }

            if (Settings.UseW && W.IsReady())
            {
                Obj_AI_Base minion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, Q.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).FirstOrDefault();
                if (minion != null)
                {
                    W.Cast(minion);
                }
            }
        }
    }
}
