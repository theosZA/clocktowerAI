using Clocktower.Game;
using Clocktower.Observer;

namespace Clocktower.Events
{
    internal class AnnounceNightKills : IGameEvent
    {
        public AnnounceNightKills(Grimoire grimoire, IGameObserver observers, int dayNumber)
        {
            this.grimoire = grimoire;
            this.observers = observers;
            this.dayNumber = dayNumber;
        }

        public Task RunEvent()
        {
            var newlyDeadPlayers = grimoire.Players.Where(player => player.Tokens.Contains(Token.DiedAtNight) || player.Tokens.Contains(Token.KilledByDemon)).ToList();
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
                    newlyDeadPlayer.Tokens.Remove(Token.KilledByDemon);
                    newlyDeadPlayer.Kill();
                }
            }

            return Task.CompletedTask;
        }

        private readonly Grimoire grimoire;
        private readonly IGameObserver observers;
        private readonly int dayNumber;
    }
}
