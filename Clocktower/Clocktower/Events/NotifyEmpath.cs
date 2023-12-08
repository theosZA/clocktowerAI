using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Options;
using System.Diagnostics.Eventing.Reader;

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
            foreach (var empath in grimoire.GetLivingPlayers(Character.Empath))
            {
                var (neighbourA, neighbourB) = grimoire.GetLivingNeighbours(empath);
                int evilCount = await GetEmpathNumber(empath, neighbourA, neighbourB);
                empath.Agent.NotifyEmpath(neighbourA, neighbourB, evilCount);
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

            var choice = await storyteller.GetEmpathNumber(empath, neighbourA, neighbourB, possibleEmpathNumbers.Select(number => new NumberOption(number)).ToList());
            return ((NumberOption)choice).Number;
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
                if (neighbourA.Alignment == Alignment.Evil)
                {
                    ++minEvilCount;
                    ++maxEvilCount;
                }
                else if (neighbourA.CanRegisterAsEvil)
                {
                    ++maxEvilCount;
                }
                if (neighbourB.Alignment == Alignment.Evil)
                {
                    ++minEvilCount;
                    ++maxEvilCount;
                }
                else if (neighbourB.CanRegisterAsEvil)
                {
                    ++maxEvilCount;
                }
                for (int evilCount = minEvilCount; evilCount <= maxEvilCount; ++evilCount)
                {
                    yield return evilCount;
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
