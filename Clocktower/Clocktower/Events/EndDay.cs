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

        public async Task RunEvent()
        {
            if (grimoire.PlayerToBeExecuted == null)
            {
                await observers.DayEndsWithNoExecution();
            }
            else
            {
                bool playerDies = grimoire.PlayerToBeExecuted.Alive;
                await observers.PlayerIsExecuted(grimoire.PlayerToBeExecuted, playerDies);
                if (playerDies)
                {
                    new Kills(storyteller, grimoire).Execute(grimoire.PlayerToBeExecuted);
                }
                grimoire.PlayerToBeExecuted = null;
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IGameObserver observers;
    }
}
