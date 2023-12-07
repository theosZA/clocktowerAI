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
            foreach (var steward in grimoire.GetLivingPlayers(Character.Steward))
            {
                var stewardTarget = steward.DrunkOrPoisoned ? grimoire.Players.First(player => player.Alignment == Alignment.Evil) : grimoire.Players.First(player => player != steward && player.Alignment == Alignment.Good);

                steward.Agent.NotifySteward(stewardTarget);
                storyteller.NotifySteward(steward, stewardTarget);
            }

            return Task.CompletedTask;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
