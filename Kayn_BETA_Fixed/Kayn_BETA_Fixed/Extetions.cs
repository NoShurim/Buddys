using EloBuddy;
using System;

namespace Kayn_BETA_Fixed
{
    public static class Extetions
    {
        public static bool IsKayn(this AIHeroClient target)
        {
            return target.Model == "Kayn";
        }

        public static bool IsRhast(this AIHeroClient target)
        {
            return target.Model == "Rhaast";
        }

        public static bool IsShadown(this AIHeroClient target)
        {
            return target.Model == "ShadownAssassin";
        }

        public static bool IsAboutToTransform(this AIHeroClient target)
        {
            return target.IsKayn() && (Math.Abs(target.Mana - target.MaxMana) < float.Epsilon && (target.HasBuff("kayntransformrhaast") || target.HasBuff("kayntransshadownassassin")));
        }
    }
}
