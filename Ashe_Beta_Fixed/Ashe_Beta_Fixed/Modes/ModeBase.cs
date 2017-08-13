using EloBuddy.SDK;

namespace Ashe_Beta_Fixed.Modes
{
    public abstract class ModeBase
    {
        protected Spell.Active Q
        {
            get { return SpellManager.Q; }
        }
        protected Spell.Skillshot W
        {
            get { return SpellManager.W; }
        }
        protected Spell.Skillshot E
        {
            get { return SpellManager.E; }
        }
        protected Spell.Skillshot R
        {
            get { return SpellManager.R; }
        }
        protected Spell.Active heal
        {
            get { return SpellManager.heal; }
        }
        protected Spell.Targeted Ignite
        {
            get { return SpellManager.ignite; }
        }
        public abstract bool ExecuteOnComands();

        public abstract void Execute();
    }
}
