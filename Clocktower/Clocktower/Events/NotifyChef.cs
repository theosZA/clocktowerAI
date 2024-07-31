using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class NotifyChef : IGameEvent
    {
        public NotifyChef(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var chef in grimoire.PlayersForWhomWeShouldRunAbility(Character.Chef))
            {
                await RunEvent(chef);
            }
        }

        public async Task RunEvent(Player chef)
        {
            int chefCount = await GetChefNumber(chef);
            await chef.Agent.NotifyChef(chefCount);
            storyteller.NotifyChef(chef, chefCount);
        }

        private async Task<int> GetChefNumber(Player chef)
        {
            if (chef.DrunkOrPoisoned)
            {
                return await storyteller.GetChefNumber(chef, Array.Empty<Player>(), Enumerable.Range(0, grimoire.Players.Count + 1));
            }

            var possibleChefNumbers = GetPossibleChefNumbers().ToList();
            if (possibleChefNumbers.Count == 1)
            {
                return possibleChefNumbers[0];
            }

            return await storyteller.GetChefNumber(chef, GetPossibleMisregistrations(), possibleChefNumbers);
        }

        private IEnumerable<int> GetPossibleChefNumbers()
        {
            int minEvilPairs = 0;
            int maxEvilPairs = 0;

            var players = grimoire.Players.ToList();
            for (int i = 0; i < players.Count; i++)
            {
                var playerA = players[i];
                var playerB = players[(i + 1) % players.Count];

                if (playerA.CanRegisterAsEvil && playerB.CanRegisterAsEvil)
                {
                    maxEvilPairs++;
                }
                if (!playerA.CanRegisterAsGood && !playerB.CanRegisterAsGood)
                {
                    minEvilPairs++;
                }
            }

            return Enumerable.Range(minEvilPairs, maxEvilPairs - minEvilPairs + 1);
        }

        private IEnumerable<Player> GetPossibleMisregistrations()
        {
            // Only include misregistrations that could change the chef number.
            // A player will count for this if they can misregister and are seated next to a player that could register as evil.

            var players = grimoire.Players.ToList();
            for (int i = 0; i < players.Count; i++)
            {
                var leftNeighbour = players[i];
                var player = players[(i + 1) % players.Count];
                var rightNeighbour = players[(i + 2) % players.Count];

                if ((player.CanRegisterAsEvil && player.Alignment != Alignment.Evil) ||
                    (player.CanRegisterAsGood && player.Alignment != Alignment.Good))
                {
                    // Player can misregister. Check if they could be part of an evil pair.
                    if (leftNeighbour.CanRegisterAsEvil || rightNeighbour.CanRegisterAsEvil)
                    {
                        yield return player;
                    }
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
