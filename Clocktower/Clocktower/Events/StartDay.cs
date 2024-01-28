using Clocktower.Game;
using Clocktower.Observer;

namespace Clocktower.Events
{
    internal class StartDay : IGameEvent
    {
        public StartDay(Grimoire grimoire, IGameObserver observers, int dayNumber)
        {
            this.grimoire = grimoire;
            this.observers = observers;
            this.dayNumber = dayNumber;
        }

        public async Task RunEvent()
        {
            await observers.Day(dayNumber);

            // Announce night kills.
            var newlyDeadPlayers = grimoire.Players.WithToken(Token.DiedAtNight).ToList();
            if (newlyDeadPlayers.Count == 0)
            {
                if (dayNumber > 1)  // No need to announce that there were no deaths on the first day.
                {
                    observers.NoOneDiedAtNight();
                }
            }
            else
            {
                foreach (var newlyDeadPlayer in newlyDeadPlayers)
                {
                    observers.PlayerDiedAtNight(newlyDeadPlayer);
                    newlyDeadPlayer.Tokens.Remove(Token.DiedAtNight);
                    newlyDeadPlayer.Kill();
                }
                observers.LivingPlayerCount(grimoire.Players.Count(player => player.Alive));
            }
        }

        private readonly Grimoire grimoire;
        private readonly IGameObserver observers;
        private readonly int dayNumber;
    }
}
