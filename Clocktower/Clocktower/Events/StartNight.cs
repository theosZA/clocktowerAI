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

        public async Task RunEvent()
        {
            await observers.Night(nightNumber);

            // Clear expired tokens.
            foreach (var player in grimoire.Players)
            {
                player.Tokens.ClearTokensForEndOfDay();
            }
        }

        private readonly Grimoire grimoire;
        private readonly IGameObserver observers;
        private readonly int nightNumber;
    }
}
