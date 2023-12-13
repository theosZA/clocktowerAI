using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class EndDay : IGameEvent
    {
        public EndDay(IStoryteller storyteller, Grimoire grimoire, IGameObserver observers, Nominations nominations)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.observers = observers;
            this.nominations = nominations;
        }

        public Task RunEvent()
        {
            if (nominations.PlayerToBeExecuted == null)
            {
                observers.DayEndsWithNoExecution();
            }
            else
            {
                bool playerDies = nominations.PlayerToBeExecuted.Alive;
                observers.PlayerIsExecuted(nominations.PlayerToBeExecuted, playerDies);
                if (playerDies)
                {
                    new Kills(storyteller, grimoire).Execute(nominations.PlayerToBeExecuted);
                }
            }

            return Task.CompletedTask;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IGameObserver observers;
        private readonly Nominations nominations;
    }
}
