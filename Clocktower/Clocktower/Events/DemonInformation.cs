using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class DemonInformation : IGameEvent
    {
        public DemonInformation(IStoryteller storyteller, Grimoire grimoire, IReadOnlyCollection<Character> scriptCharacters, Random random)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.scriptCharacters = scriptCharacters;
            this.random = random;
        }

        public async Task RunEvent()
        {
            var minions = grimoire.Players.Where(player => player.CharacterType == CharacterType.Minion).ToList();
            foreach (var demon in grimoire.Players.Where(player => player.CharacterType == CharacterType.Demon))
            {
                var choice = await GetDemonBluffs(demon);
                var bluffs = new List<Character> { choice.CharacterA, choice.CharacterB, choice.CharacterC };
                bluffs.Shuffle(random);
                demon.Agent.DemonInformation(minions, bluffs);
                storyteller.DemonInformation(demon, minions, bluffs);
            }
        }

        private async Task<ThreeCharactersOption> GetDemonBluffs(Player demon)
        {
            var options = GetOptions(demon).ToList();
            return (ThreeCharactersOption)await storyteller.GetDemonBluffs(demon, options);
        }

        private IEnumerable<IOption> GetOptions(Player demon)
        {
            var availableBluffs = GetAvailableBluffs();

            return from bluffA in availableBluffs
                   from bluffB in availableBluffs
                   where bluffA != bluffB
                   from bluffC in availableBluffs
                   where bluffA != bluffC && bluffB != bluffC
                   select new ThreeCharactersOption(bluffA, bluffB, bluffC);
        }

        private IEnumerable<Character> GetAvailableBluffs()
        {
            return from character in scriptCharacters
                   where (int)character < 2000  // good character 
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
