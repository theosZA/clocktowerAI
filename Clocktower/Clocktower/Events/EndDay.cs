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
                CheckForMayorWin();
            }
            else
            {
                bool playerDies = grimoire.PlayerToBeExecuted.Alive;
                await observers.PlayerIsExecuted(grimoire.PlayerToBeExecuted, playerDies);
                if (playerDies)
                {
                    await new Kills(storyteller, grimoire).Execute(grimoire.PlayerToBeExecuted);
                }
                grimoire.PlayerToBeExecuted = null;
            }

            if (!grimoire.Finished)
            {
                await observers.AnnounceLivingPlayers(grimoire.Players);
            }
        }

        public void CheckForMayorWin()
        {
            if (grimoire.Players.Alive() != 3)
            {
                return;
            }
            var mayors = grimoire.GetPlayersWithAbility(Character.Mayor)
                                 .Where(mayor => !mayor.DrunkOrPoisoned)
                                 .ToList();
            // There really shouldn't be more than one player with a Mayor ability here, but just in case
            // we apply the rule that any good mayor win trumps any evil mayor win.
            if (mayors.Any(mayor => mayor.Alignment == Alignment.Good))
            {
                grimoire.EndGame(Alignment.Good);
            }
            else if (mayors.Any(mayor => mayor.Alignment == Alignment.Evil))
            {
                grimoire.EndGame(Alignment.Evil);
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IGameObserver observers;
    }
}
