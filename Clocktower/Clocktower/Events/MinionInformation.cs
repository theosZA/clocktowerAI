using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class MinionInformation : IGameEvent
    {
        public MinionInformation(IStoryteller storyteller, Grimoire grimoire, IReadOnlyCollection<Character> scriptCharacters, Random random)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.scriptCharacters = scriptCharacters;
            this.random = random;
        }

        public async Task RunEvent()
        {
            var minions = grimoire.Players.Where(player => player.CharacterType == CharacterType.Minion).ToList();
            var demon = grimoire.Players.First(player => player.CharacterType == CharacterType.Demon);
            foreach (var minion in minions)
            {
                var minionBluffs = await GetMinionBluffs(minion);
                await minion.Agent.MinionInformation(demon, minions.Except(new[] { minion }).ToList(), minionBluffs);
                storyteller.MinionInformation(minion, demon, minions.Except(new[] { minion }).ToList(), minionBluffs);
            }
        }

        private async Task<IReadOnlyCollection<Character>> GetMinionBluffs(Player minion)
        {
            if (!grimoire.PlayersWithHealthyAbility(Character.Snitch).Any())
            {
                return Array.Empty<Character>();
            }

            var chosenBluffs = (await storyteller.GetMinionBluffs(minion, GetAvailableBluffs().ToList())).ToList();
            chosenBluffs.Shuffle(random);
            return chosenBluffs;
        }

        private IEnumerable<Character> GetAvailableBluffs()
        {
            return from character in scriptCharacters.OfAlignment(Alignment.Good)
                   where !grimoire.Players.Any(player => player.RealCharacter == character)
                   orderby TextUtilities.CharacterToText(character)
                   select character;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IReadOnlyCollection<Character> scriptCharacters;
        private readonly Random random;
    }
}
