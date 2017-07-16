using EloBuddy;
using EloBuddy.SDK.Events;
using EloBuddy.SDK;
using System;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System.Linq;
using SharpDX;

namespace Kayn_Official
{
    class Champi
    {
        public static Spell.Skillshot Q,W;
        public static Spell.Active E;
        public static Spell.Targeted R;
        public static Menu kay, comb, hara, lane, jungle;
        private static AIHeroClient Kayn => Player.Instance;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += LoadingKayn;
        }

        private static void LoadingKayn(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Kayn) return;
            Chat.Print("Kayn");
            Q = new Spell.Skillshot(SpellSlot.Q, 300, EloBuddy.SDK.Enumerations.SkillShotType.Circular);
            W = new Spell.Skillshot(SpellSlot.W, 700, EloBuddy.SDK.Enumerations.SkillShotType.Linear);
            E = new Spell.Active(SpellSlot.E);
            R = new Spell.Targeted(SpellSlot.R, 450);
            kay = MainMenu.AddMenu("Kayn", "Kayn");
            comb = kay.AddSubMenu("Combo");
            comb.Add("Qc", new CheckBox("Use [Q] in Combo"));
            comb.Add("Wc", new CheckBox("Use [W] in Combo"));
            comb.Add("Ec", new CheckBox("Use [E] in Combo", false));
            comb.Add("Rc", new CheckBox("Use [R] in Combo"));
            comb.AddLabel("[R] Settings");
            comb.Add("RKS", new CheckBox("Use [R] Always ends"));
            hara = kay.AddSubMenu("Harass");
            hara.Add("Wh", new CheckBox("Use [W] in Harass"));
            hara.AddLabel("Harass Mana Manager");
            hara.Add("Mana", new Slider("Mana [W]", 50, 1, 100));
            lane = kay.AddSubMenu("Lane");
            lane.Add("Ql", new CheckBox("Use [Q] in Lane"));
            lane.Add("Wl", new CheckBox("Use [W] in Lane"));
            jungle = kay.AddSubMenu("Jungle");
            jungle.Add("Qj", new CheckBox("Use [Q] in Jungle"));
            jungle.Add("Wj", new CheckBox("Use [W] in Jungle"));
            //
            Game.OnTick += Kayn_Tick;


        }

        private static void Kayn_Tick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.Equals(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (Orbwalker.ActiveModesFlags.Equals(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags.Equals(Orbwalker.ActiveModes.LaneClear))
            {
                LaneCLear();
            }
            if (Orbwalker.ActiveModesFlags.Equals(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
            }
        }

        private static void JungleClear()
        {
            var frammontres = EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(x => x.IsValidTarget(Kayn.GetAutoAttackRange())).ToList();
            if (!frammontres.Any()) return;
            string[] monsters = { "SRU_Gromp", "SRU_Blue", "SRU_Red", "SRU_Razorbeak", "SRU_Krug", "SRU_Murkwolf", "Sru_Crab", "SRU_RiftHerald", "SRU_Dragon", "SRU_Baron" };
            if (Q.IsReady() && jungle["Qj"].Cast<CheckBox>().CurrentValue)
            {
                Q.Cast();
            }
            if (!W.IsReady() || !jungle["Wj"].Cast<CheckBox>().CurrentValue) return;
            {
                var farmW = frammontres.FirstOrDefault(x => monsters.Contains(x.BaseSkinName, StringComparer.CurrentCultureIgnoreCase));
                if (farmW == null || !(farmW.Health > Kayn.GetAutoAttackDamage(farmW, true) * 2)) return;
                var pred = W.GetPrediction(farmW);
                W.Cast(pred.CastPosition);
            }
        }

        private static void LaneCLear()
        {
            var farmQ = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Kayn.ServerPosition).Where(x => x.IsValidTarget(Q.Range - 100)).ToList();
            if (!farmQ.Any()) return;
            if ((Q.IsReady() && lane["Ql"].Cast<CheckBox>().CurrentValue))
            {
                Q.Cast();
            }
            if (!W.IsReady() || !lane["Wl"].Cast<CheckBox>().CurrentValue) return;
            var farmW = W.GetBestLinearCastPosition(farmQ);
            if (farmW.CastPosition != Vector3.Zero)
            {
                W.Cast(farmW.CastPosition);
            }
        }
        private static void Harass()
        {
            throw new NotImplementedException();
        }

        private static void Combo()
        {
            if (comb["Qc"].Cast<CheckBox>().CurrentValue)
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                var qpred = Q.GetPrediction(target);
                if (target.IsValidTarget(Q.Range) && Q.IsReady() && qpred.HitChance >= EloBuddy.SDK.Enumerations.HitChance.High)
                {
                    Q.Cast(target);
                }
            }
            if (comb["Wc"].Cast<CheckBox>().CurrentValue)
            {
                var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                var wpred = W.GetPrediction(target);
                if (target.IsValidTarget(W.Range) && W.IsReady() && wpred.HitChance >= EloBuddy.SDK.Enumerations.HitChance.Medium)
                {
                    W.Cast(target);
                }
            }
            if (comb["Rc"].Cast<CheckBox>().CurrentValue)
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (target.IsValidTarget(R.Range) && R.IsReady())
                {
                    R.Cast(target);
                }
            }
        }
    }
}
