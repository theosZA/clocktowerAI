using Clocktower.Game;
using Clocktower.Observer;

namespace Clocktower.Events
{
    internal class StartNight : IGameEvent
    {
        public StartNight(Grimoire grimoire, IGameObserver observers, int nightNumber)
        {
            this.grimoire = grimoire;
            this.observers = observers;
            this.nightNumber = nightNumber;
        }

        public Task RunEvent()
        {
            observers.Night(nightNumber);

            // Clear expired tokens.
            foreach (var player in grimoire.Players)
            {
                player.Tokens.Remove(Token.PoisonedByPoisoner);
                player.Tokens.Remove(Token.ProtectedByMonk);
                player.Tokens.Remove(Token.AlreadyClaimedSlayer);   // We allow players to claim Slayer once each day to allow for Philosopher into Slayer.
            }

            return Task.CompletedTask;
        }

        private readonly Grimoire grimoire;
        private readonly IGameObserver observers;
        private readonly int nightNumber;
    }
}
