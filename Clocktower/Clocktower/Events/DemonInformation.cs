using Clocktower.Game;
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
                var bluffs = await GetDemonBluffs(demon);
                bluffs.Shuffle(random);
                grimoire.DemonBluffs = bluffs;
                await demon.Agent.DemonInformation(minions, bluffs);
                storyteller.DemonInformation(demon, minions, bluffs);
            }
        }

        private async Task<List<Character>> GetDemonBluffs(Player demon)
        {
            var bluffs = (await storyteller.GetDemonBluffs(demon, GetAvailableBluffs(Array.Empty<Character>()).ToList())).ToList();

            // Marionette-Snitch jinx - The Marionette does not learn 3 not in-play characters. The Demon learns an extra 3 instead.
            var snitch = grimoire.GetPlayerWithHealthyAbility(Character.Snitch);
            if (snitch != null && grimoire.Players.Any(player => player.RealCharacter == Character.Marionette))
            {
                var additionalBluffs = await storyteller.GetAdditionalDemonBluffs(demon, snitch, GetAvailableBluffs(bluffs).ToList());
                bluffs.AddRange(additionalBluffs);
            }

            return bluffs;
        }

        private IEnumerable<Character> GetAvailableBluffs(IEnumerable<Character> excludingCharacters)
        {
            return from character in scriptCharacters.OfAlignment(Alignment.Good)
                   where !grimoire.Players.Any(player => player.RealCharacter == character)
                   where !excludingCharacters.Contains(character)
                   orderby TextUtilities.CharacterToText(character)
                   select character;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IReadOnlyCollection<Character> scriptCharacters;
        private readonly Random random;
    }
}
