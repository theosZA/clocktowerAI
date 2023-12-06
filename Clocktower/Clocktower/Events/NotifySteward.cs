using Clocktower.Agent;
using Clocktower.Game;

namespace Clocktower.Events
{
    internal class NotifySteward : IGameEvent
    {
        public NotifySteward(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public Task RunEvent()
        {
            var steward = grimoire.GetAlivePlayer(Character.Steward);
            if (steward != null)
            {
                // For now we give them a hardcoded player.
                var stewardTarget = steward.DrunkOrPoisoned ? grimoire.GetRequiredPlayer(Character.Imp) : grimoire.GetRequiredPlayer(Character.Ravenkeeper);
                steward.Agent.NotifySteward(stewardTarget);
                storyteller.NotifySteward(steward, stewardTarget);
            }

            return Task.CompletedTask;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
