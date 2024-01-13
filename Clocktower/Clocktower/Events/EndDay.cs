using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class EndDay : IGameEvent
    {
        public EndDay(IStoryteller storyteller, Grimoire grimoire, IGameObserver observers)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.observers = observers;
        }

        public Task RunEvent()
        {
            if (grimoire.PlayerToBeExecuted == null)
            {
                observers.DayEndsWithNoExecution();
            }
            else
            {
                bool playerDies = grimoire.PlayerToBeExecuted.Alive;
                observers.PlayerIsExecuted(grimoire.PlayerToBeExecuted, playerDies);
                if (playerDies)
                {
                    new Kills(storyteller, grimoire).Execute(grimoire.PlayerToBeExecuted);
                }
                grimoire.PlayerToBeExecuted = null;
            }

            return Task.CompletedTask;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IGameObserver observers;
    }
}
