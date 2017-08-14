using EloBuddy;
using EloBuddy.SDK;


namespace Kalista_Beta_Fixed
{
    public abstract class ModeBase
    {
        protected static readonly AIHeroClient Player = EloBuddy.Player.Instance;

        protected Spell.Skillshot Q
        {
            get { return SpellsManager.Q; }
        }
        protected Spell.Targeted W
        {
            get { return SpellsManager.W; }
        }
        protected Spell.Active E
        {
            get { return SpellsManager.E; }
        }
        protected Spell.Active R
        {
            get { return SpellsManager.R; }
        }

        public abstract bool ShouldBeExecuted();
        public abstract void Execute();
    }
}
