using System.Linq;
using EloBuddy;
using static Gragas_Beta_Fixed.MyHero;

namespace Gragas_Beta_Fixed
{
    public sealed class QLogic
    {
        public static bool CastedQ;
        public static bool ShouldBeExecuted()
        {
            return true;
        }

        public static void Execute()
        {
            if (CastedQ)
            {
                if (Player.Instance.Spellbook.GetSpell(SpellSlot.Q).ToggleState == 2 || Player.Instance.Spellbook.GetSpell(SpellSlot.Q).ToggleState == 1)
                {
                    Q.Cast(Player.Instance);
                    CastedQ = false;
                }
                else
                {
                    CastedQ = false;
                }
            }
        }
    }





}

