using Clocktower.Game;

namespace Clocktower.Triggers
{
    internal struct DeathInformation
    {
        public Player dyingPlayer;
        public Player? killingPlayer;
        public bool duringDay;
        public bool executed;

        public bool hasScarletWomanJustBecomeDemon; // if true, then that counts as the Imp's star pass
    }
}
