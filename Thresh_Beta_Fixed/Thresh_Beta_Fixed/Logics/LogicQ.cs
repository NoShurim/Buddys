using System;
using EloBuddy;
using static Thresh_Beta_Fixed.Spells;

namespace Thresh_Beta_Fixed.Logics
{
    public static class LogicQ
    {
        public static float TimeQlogic;
        private static float TimeQ2Last;
        public static Obj_AI_Base QT;
        public static bool Q1
        {
            get { return Q.Name.Equals("TQ"); }
        }
        static void Logic()
        {
            Obj_AI_Base.OnProcessSpellCast += Base_OnProcessSpellCast;
            Obj_AI_Base.OnBuffGain += Base_OnBuffGain;
            Obj_AI_Base.OnBuffLose += Base_OnBuffLose;
        }

        private static void Base_OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (!args.Buff.Caster.IsMe) return;
            if (args.Buff.Name.Equals("TQ"))
            {
                QT = null;
            }
        }
        private static void Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!args.Buff.Caster.IsMe) return;
            if (args.Buff.Name.Equals("TQ"))
            {
                QT = sender;
                TimeQlogic = Game.Time;
            }
        }
        private static void Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(sender is AIHeroClient) || !sender.IsAlly) return;
            if (sender.IsMe)
            {
                if (args.Slot == SpellSlot.Q)
                {
                    if (args.SData.Name.Equals("TQ"))
                    {
                        TimeQ2Last = Game.Time;
                    }
                }
            }
            else
            {
                if (args.SData.Name.Equals("LanternWAlly") && !Q1)
                {
                    MyChampion.MyThresh.Spellbook.CastSpell(SpellSlot.Q);
                }
            }
        }
    }
}
