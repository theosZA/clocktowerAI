using Clocktower.Agent;
using Clocktower.Events;
using Clocktower.Observer;
using Clocktower.Storyteller;

namespace Clocktower.Game
{
    /// <summary>
    /// An instance of a Blood on the Clocktower game.
    /// </summary>
    internal class ClocktowerGame
    {
        public bool Finished => grimoire.Finished;

        public ClocktowerGame(IGameSetup setup, IReadOnlyCollection<IAgent> agents, Random random)
        {
            this.agents = agents;
            storyteller = new StorytellerForm(random);
            observers = ProxyCollection<IGameObserver>.CreateProxy(agents.Select(agent => agent.Observer).Append(storyteller.Observer));
            grimoire = new Grimoire(agents, setup.Characters);

            gameEventFactory = new GameEventFactory(storyteller, grimoire, observers, setup, random);
        }

        public async Task StartGame()
        {
            storyteller.Start();

            var startGameTasks = agents.Select(async agent => await agent.StartGame());
            await Task.WhenAll(startGameTasks);

            grimoire.AssignCharacters(storyteller);
        }

        public void AnnounceWinner()
        {
            var winner = grimoire.Winner;
            if (winner.HasValue)
            {
                observers.AnnounceWinner(winner.Value,
                                         grimoire.Players.Where(player => player.Alignment == winner.Value).ToList(),
                                         grimoire.Players.Where(player => player.Alignment != winner.Value).ToList());
            }
        }

        public async Task RunNightAndDay()
        {
            ++dayNumber;

            await gameEventFactory.BuildNightEvents(dayNumber).RunEvent();
            await gameEventFactory.BuildDayEvents(dayNumber).RunEvent();
        }

        private readonly IReadOnlyCollection<IAgent> agents;
        private readonly IStoryteller storyteller;
        private readonly IGameObserver observers;
        private readonly Grimoire grimoire;

        private readonly GameEventFactory gameEventFactory;
    
        private int dayNumber = 0;
    }
}
