using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class NotifySteward : IGameEvent
    {
        public NotifySteward(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var steward in grimoire.GetLivingPlayers(Character.Steward))
            {
                var options = grimoire.Players.Where(player => player != steward && (player.Alignment == Alignment.Good || steward.DrunkOrPoisoned))
                                              .Select(player => new PlayerOption(player))
                                              .ToList();
                var stewardTarget = ((PlayerOption)await storyteller.GetStewardPing(steward, options)).Player;

                steward.Agent.NotifySteward(stewardTarget);
                storyteller.NotifySteward(steward, stewardTarget);
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
