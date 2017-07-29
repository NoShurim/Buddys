using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Settings = Vayne_Beta_Fixed.Menus.Modes.Combo;
namespace Vayne_Beta_Fixed
{
    internal class byCombo : ModeBase
    {
        public static bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        public void Execute()
        {
            if (Settings.UseR && R.IsReady() && Player.Instance.CountEnemiesInRange(1000) >= Settings.UseREnemies)
            {
                R.Cast();
            }
            if (Settings.UseQ && Q.IsReady() && Player.Instance.CountEnemiesInRange(550) == 0 && Player.Instance.CountEnemiesInRange(850) > 0 && Menus.QSettings.GapcloserQ)
            {
                foreach (var e in EntityManager.Heroes.Enemies.Where(t => t.HealthPercent > 0 && !t.IsInvulnerable && !t.IsZombie).OrderBy(t => t.Distance(Player.Instance)))
                {
                    Player.CastSpell(SpellSlot.Q, e.Position);
                }
            }
            if (Settings.UseE && E.IsReady())
            {
                LogicOber.CastE();
            }
            if (Settings.useBOTRK)
            {
                if (!castBOTRK())
                    castBilgewater();
            }
            if (Settings.useYOUMOUS)
            {
                castYoumous();
            }
            getVision();
        }

        private void getVision()
        {
            if (Player.Instance.IsDead || Player.Instance.IsInvulnerable || !Player.Instance.IsTargetable || Player.Instance.IsZombie || Player.Instance.IsInShopRange())
                return;
            else if (Settings.useTrinketVision && Bayne.thuTarget != null && Bayne.thuTarget.Position.Distance(Player.Instance) < 600 && Game.Time - Bayne.Seen > 2)
            {
                InventorySlot[] inv = Player.Instance.InventoryItems;
                foreach (var item in inv)
                {
                    if (item.Id == ItemId.Greater_Stealth_Totem_Trinket || item.Id == ItemId.Greater_Vision_Totem_Trinket || item.Id == ItemId.Warding_Totem_Trinket)
                    {
                        item.Cast(Bayne.predictedPos);
                    }
                }
            }
            else if (Settings.useWardVision && Bayne.thuTarget != null && Bayne.thuTarget.Position.Distance(Player.Instance) < R.Range && Game.Time -Bayne.Seen > 2)
            {
                InventorySlot[] inv = Player.Instance.InventoryItems;
                foreach (var item in inv)
                {
                    if (item.Id == ItemId.Wardens_Mail)
                    {
                        item.Cast(Bayne.predictedPos);
                    }
                }
            }
        }

        private bool castYoumous()
        {
            if (Player.Instance.IsDead || !Player.Instance.CanCast || Player.Instance.IsInvulnerable || !Player.Instance.IsTargetable || Player.Instance.IsZombie || Player.Instance.IsInShopRange())
                return false;
            InventorySlot[] inv = Player.Instance.InventoryItems;
            foreach (var item in inv)
            {
                if ((item.Id == ItemId.Youmuus_Ghostblade) && item.CanUseItem() && Player.Instance.CountEnemiesInRange(700) > 0)
                {
                    return item.Cast();
                }
            }
            return false;
        }

        private bool castBilgewater()
        {
            if (Player.Instance.IsDead || !Player.Instance.CanCast || Player.Instance.IsInvulnerable || !Player.Instance.IsTargetable || Player.Instance.IsZombie || Player.Instance.IsInShopRange())
                return false;
            InventorySlot[] inv = Player.Instance.InventoryItems;
            foreach (var item in inv)
            {
                if ((item.Id == ItemId.Bilgewater_Cutlass) && item.CanUseItem())
                {
                    var target = TargetSelector.GetTarget(550, DamageType.Magical);
                    if (target != null)
                        return item.Cast(target);
                }
            }
            return false;
        }

        private bool castBOTRK()
        {
            if (Player.Instance.IsDead || !Player.Instance.CanCast || Player.Instance.IsInvulnerable || !Player.Instance.IsTargetable || Player.Instance.IsZombie || Player.Instance.IsInShopRange())
                return false;
            InventorySlot[] inv = Player.Instance.InventoryItems;
            foreach (var item in inv)
            {
                if ((item.Id == ItemId.Blade_of_the_Ruined_King) && item.CanUseItem())
                {
                    var target = TargetSelector.GetTarget(550, DamageType.Physical);
                    if (target != null && Player.Instance.Health <= DamageLibrary.GetItemDamage(Player.Instance, target, ItemId.Blade_of_the_Ruined_King))
                        return item.Cast(target);
                }
            }
            return false;
        }

        internal static void Spellblade(AttackableUnit target, EventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || Player.Instance.CountEnemiesInRange(850) == 0)
                return;
            if (Settings.UseQ && SpellndCast.Q.IsReady())
            {
                LogicOber.CastQ(true);
            }
        }
    }
}
   