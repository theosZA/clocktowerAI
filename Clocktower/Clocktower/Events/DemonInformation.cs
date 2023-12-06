using Clocktower.Agent;
using Clocktower.Game;

namespace Clocktower.Events
{
    internal class DemonInformation : IGameEvent
    {
        public DemonInformation(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public Task RunEvent()
        {
            var demon = grimoire.GetDemon();
            var minions = grimoire.GetMinions().ToList();
            var notInPlayCharacters = new[] { Character.Fortune_Teller, Character.Philosopher, Character.Soldier };  // hardcoded list for now
            demon.Agent.DemonInformation(minions, notInPlayCharacters);
            storyteller.DemonInformation(demon, minions, notInPlayCharacters);

            return Task.CompletedTask;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
