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
                var bluffs = (await storyteller.GetDemonBluffs(demon, GetAvailableBluffs().ToList())).ToList();
                bluffs.Shuffle(random);
                grimoire.DemonBluffs = bluffs;
                await demon.Agent.DemonInformation(minions, bluffs);
                storyteller.DemonInformation(demon, minions, bluffs);
            }
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
