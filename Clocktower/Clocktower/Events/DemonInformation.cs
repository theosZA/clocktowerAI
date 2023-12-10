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
                var bluffs = (await GetDemonBluffs(demon)).ToList();
                bluffs.Shuffle(random);
                demon.Agent.DemonInformation(minions, bluffs);
                storyteller.DemonInformation(demon, minions, bluffs);
            }
        }

        private async Task<IEnumerable<Character>> GetDemonBluffs(Player demon)
        {
            return (await storyteller.GetDemonBluffs(demon, GetAvailableBluffs().ToList().ToThreeCharactersOptions())).GetThreeCharacters();
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
