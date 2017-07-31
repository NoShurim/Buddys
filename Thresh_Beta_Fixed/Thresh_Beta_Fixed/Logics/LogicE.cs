using EloBuddy;
using static Thresh_Beta_Fixed.Spells;

namespace Thresh_Beta_Fixed.Logics
{
    public static class LogicE
    {
        private static float TimeElogic;
        private static float TimeE2Last;
        public static Obj_AI_Base ET;
        private static bool IsE
        {
            get { return E.Name.Equals("TE"); }
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
            if (args.Buff.Name.Equals("TE"))
            {
                ET = null;
            }
        }
        private static void Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!args.Buff.Caster.IsMe) return;
            if (args.Buff.Name.Equals("TE"))
            {
                ET = sender;
                TimeElogic = Game.Time;
            }
        }
        private static void Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(sender is AIHeroClient) || !sender.IsAlly) return;
            if (sender.IsMe)
            {
                if (args.Slot == SpellSlot.E)
                {
                    if (args.SData.Name.Equals("TE"))
                    {
                        TimeE2Last = Game.Time;
                    }
                }
            }
            else
            {
                if (args.SData.Name.Equals("LanternWAlly") && !IsE)
                {
                    MyChampion.MyThresh.Spellbook.CastSpell(SpellSlot.E);
                }
            }
        }
    }
}