using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class NotifyEmpath : IGameEvent
    {
        public NotifyEmpath(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var empath in grimoire.PlayersForWhomWeShouldRunAbility(Character.Empath))
            {
                var (neighbourA, neighbourB) = grimoire.GetLivingNeighbours(empath);
                int evilCount = await GetEmpathNumber(empath, neighbourA, neighbourB);
                await empath.Agent.NotifyEmpath(neighbourA, neighbourB, evilCount);
                storyteller.NotifyEmpath(empath, neighbourA, neighbourB, evilCount);
            }
        }

        private async Task<int> GetEmpathNumber(Player empath, Player neighbourA, Player neighbourB)
        {
            var possibleEmpathNumbers = GetPossibleEmpathNumbers(empath, neighbourA, neighbourB).ToList();
            if (possibleEmpathNumbers.Count == 1)
            {
                return possibleEmpathNumbers[0];
            }

            return await storyteller.GetEmpathNumber(empath, neighbourA, neighbourB, possibleEmpathNumbers);
        }

        private static IEnumerable<int> GetPossibleEmpathNumbers(Player empath, Player neighbourA, Player neighbourB)
        {
            if (empath.DrunkOrPoisoned)
            {
                yield return 0;
                yield return 1;
                yield return 2;
            }
            else
            {
                int minEvilCount = 0;
                int maxEvilCount = 0;
                AdjustPossibleEvilCounts(neighbourA, ref minEvilCount, ref maxEvilCount);
                AdjustPossibleEvilCounts(neighbourB, ref minEvilCount, ref maxEvilCount);
                for (int evilCount = minEvilCount; evilCount <= maxEvilCount; ++evilCount)
                {
                    yield return evilCount;
                }
            }
        }

        private static void AdjustPossibleEvilCounts(Player neighbour, ref int minEvilCount, ref int maxEvilCount)
        {
            if (neighbour.CanRegisterAsEvil)
            {
                maxEvilCount++;
            }
            if (!neighbour.CanRegisterAsGood)
            {
                minEvilCount++;
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
