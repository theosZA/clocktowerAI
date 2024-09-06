using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class NotifyOracle : IGameEvent
    {
        public NotifyOracle(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var oracle in grimoire.PlayersForWhomWeShouldRunAbility(Character.Oracle))
            {
                int evilCount = await GetOracleNumber(oracle);
                await oracle.Agent.NotifyOracle(evilCount);
                storyteller.NotifyOracle(oracle, evilCount);
            }
        }

        private async Task<int> GetOracleNumber(Player oracle)
        {
            var possibleOracleNumbers = GetPossibleOracleNumbers(oracle).ToList();
            if (possibleOracleNumbers.Count == 1)
            {
                return possibleOracleNumbers[0];
            }

            return await storyteller.GetOracleNumber(oracle, grimoire.Players.Where(player => !player.Alive), possibleOracleNumbers);
        }

        private IEnumerable<int> GetPossibleOracleNumbers(Player oracle)
        {
            var deadPlayers = grimoire.Players.Where(player => !player.Alive);
            if (oracle.DrunkOrPoisoned)
            {
                return Enumerable.Range(0, deadPlayers.Count() + 1);
            }

            int minEvilCount = 0;
            int maxEvilCount = 0;
            foreach (var deadPlayer in deadPlayers)
            {
                AdjustPossibleEvilCounts(deadPlayer, ref minEvilCount, ref maxEvilCount);
            }
            return Enumerable.Range(minEvilCount, maxEvilCount - minEvilCount + 1);
        }

        private static void AdjustPossibleEvilCounts(Player deadPlayer, ref int minEvilCount, ref int maxEvilCount)
        {
            if (deadPlayer.CanRegisterAsEvil)
            {
                maxEvilCount++;
            }
            if (!deadPlayer.CanRegisterAsGood)
            {
                minEvilCount++;
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
