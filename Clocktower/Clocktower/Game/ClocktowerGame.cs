﻿using Clocktower.Agent;
using Clocktower.Agent.Observer;
using Clocktower.Events;
using Clocktower.Setup;
using Clocktower.Storyteller;

namespace Clocktower.Game
{
    /// <summary>
    /// An instance of a Blood on the Clocktower game.
    /// </summary>
    internal class ClocktowerGame
    {
        public bool Finished => grimoire.Finished;
        public Alignment? Winner => grimoire.Winner;

        public ClocktowerGame(IGameSetup setup, IStoryteller storyteller, IReadOnlyCollection<IAgent> agents, Random random)
        {
            this.agents = agents;
            this.storyteller = storyteller;
            observers = ProxyCollection<IGameObserver>.CreateProxy(agents.Select(agent => agent.Observer).Append(storyteller.Observer));
            grimoire = new Grimoire(agents, setup.Characters);

            gameEventFactory = new GameEventFactory(storyteller, grimoire, observers, setup, random);
        }

        public async Task StartGame()
        {
            await storyteller.StartGame();

            var startGameTasks = agents.Select(async agent => await agent.StartGame());
            await Task.WhenAll(startGameTasks);

            await grimoire.AssignCharacters(storyteller);
        }

        public void EndGame(Alignment winner)
        {
            grimoire.EndGame(winner);
        }

        public async Task AnnounceWinner()
        {
            if (Winner.HasValue)
            {
                await observers.AnnounceWinner(Winner.Value,
                                               grimoire.Players.Where(player => player.Alignment == Winner.Value).ToList(),
                                               grimoire.Players.Where(player => player.Alignment != Winner.Value).ToList());
            }

            var endGameTasks = agents.Select(async agent => await agent.EndGame());
            await Task.WhenAll(endGameTasks);
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
