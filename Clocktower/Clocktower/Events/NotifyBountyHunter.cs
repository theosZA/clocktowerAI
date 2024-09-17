using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class NotifyBountyHunter : IGameEvent
    {
        public NotifyBountyHunter(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var bountyHunter in grimoire.PlayersForWhomWeShouldRunAbility(Character.Bounty_Hunter))
            {
                if (!grimoire.Players.Any(player => player.Tokens.HasTokenForPlayer(Token.BountyHunterPing, bountyHunter)))
                {
                    await RunEvent(bountyHunter);
                }
            }
        }

        private async Task RunEvent(Player bountyHunter)
        {
            var bountyHunterPing = (await storyteller.GetBountyHunterPing(bountyHunter, GetValidBountyHunterPings(bountyHunter)));
            bountyHunterPing.Tokens.Add(Token.BountyHunterPing, bountyHunter);
            await bountyHunter.Agent.NotifyBountyHunter(bountyHunterPing);
            storyteller.NotifyBountyHunter(bountyHunter, bountyHunterPing);
        }

        public IEnumerable<Player> GetValidBountyHunterPings(Player bountyHunter)
        {
            if (bountyHunter.DrunkOrPoisoned)
            {
                return grimoire.Players;
            }

            return grimoire.Players.Where(player => player != bountyHunter && player.CanRegisterAsEvil);
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
