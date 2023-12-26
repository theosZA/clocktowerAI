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

        public ClocktowerGame(IGameSetup setup, Random random)
        {
            var agents = AgentFactory.CreateAgentsFromConfig(setup, random).ToList();

            storyteller = new StorytellerForm(random);
            observers = ProxyCollection<IGameObserver>.CreateProxy(agents.Select(agent => agent.Observer).Append(storyteller.Observer));
            grimoire = new Grimoire(agents, setup.Characters);

            gameEventFactory = new GameEventFactory(storyteller, grimoire, observers, setup, random);

            StartGame(agents);
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
            
            await gameEventFactory.BuildNightEvents(dayNumber).RunEvents(grimoire);
            await gameEventFactory.BuildDayEvents(dayNumber).RunEvents(grimoire);
        }

        private void StartGame(IEnumerable<IAgent> agents)
        {
            storyteller.Start();
            foreach (var agent in agents)
            {
                agent.StartGame();
            }

            grimoire.AssignCharacters(storyteller);
        }

        private readonly IStoryteller storyteller;
        private readonly IGameObserver observers;
        private readonly Grimoire grimoire;

        private readonly GameEventFactory gameEventFactory;
    
        private int dayNumber = 0;
    }
}
