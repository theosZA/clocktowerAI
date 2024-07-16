using Clocktower.Game;
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
            foreach (var steward in grimoire.GetPlayersWithAbility(Character.Steward))
            {
                await RunEvent(steward);
            }
        }

        public async Task RunEvent(Player steward)
        {
            var stewardPing = (await storyteller.GetStewardPing(steward, GetValidStewardPings(steward)));
            stewardPing.Tokens.Add(Token.StewardPing, steward);
            await steward.Agent.NotifySteward(stewardPing);
            storyteller.NotifySteward(steward, stewardPing);
        }

        public IEnumerable<Player> GetValidStewardPings(Player steward)
        {
            if (steward.DrunkOrPoisoned)
            {
                return grimoire.Players.Where(player => player != steward);
            }

            return grimoire.Players.Where(player => player.CanRegisterAsGood);
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
