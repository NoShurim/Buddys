using System;
using EloBuddy.SDK;

namespace Vayne_Beta_Fixed
{
    internal class ModeBase
    {
        protected Spell.Active Q
        {
            get { return SpellndCast.Q; }
        }
        protected Spell.Targeted E
        {
            get { return SpellndCast.E; }
        }
        protected Spell.Active R
        {
            get { return SpellndCast.R; }
        }

        internal bool ShouldBeExecuted()
        {
            throw new NotImplementedException();
        }

        internal void Execute()
        {
            throw new NotImplementedException();
        }
    }
}