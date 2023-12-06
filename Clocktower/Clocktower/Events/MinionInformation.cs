using Clocktower.Agent;
using Clocktower.Game;

namespace Clocktower.Events
{
    internal class MinionInformation : IGameEvent
    {
        public MinionInformation(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public Task RunEvent()
        {
            var demon = grimoire.GetDemon();
            var minions = grimoire.GetMinions().ToList();
            foreach (var minion in minions)
            {
                minion.Agent.MinionInformation(demon, minions.Except(new[] { minion }).ToList());
                storyteller.MinionInformation(minion, demon, minions.Except(new[] { minion }).ToList());
            }

            return Task.CompletedTask;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
