﻿using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class ChoiceFromRavenkeeper : IGameEvent
    {
        public ChoiceFromRavenkeeper(IStoryteller storyteller, Grimoire grimoire, IReadOnlyCollection<Character> scriptCharacters)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.scriptCharacters = scriptCharacters;
        }

        public async Task RunEvent()
        {
            foreach (var ravenkeeper in grimoire.Players.Where(player => player.Character == Character.Ravenkeeper && player.Tokens.Contains(Token.KilledByDemon)))
            {
                var target = await ravenkeeper.Agent.RequestChoiceFromRavenkeeper(grimoire.Players);
                var character = await GetTargetCharacter(ravenkeeper, target);
                storyteller.ChoiceFromRavenkeeper(ravenkeeper, target, character);
                ravenkeeper.Agent.NotifyRavenkeeper(target, character);
            }
        }

        private async Task<Character> GetTargetCharacter(Player ravenkeeper, Player target)
        {
            if (ravenkeeper.DrunkOrPoisoned)
            {   // Any on-script character is possible.
                return await GetCharacterFromList(ravenkeeper, target, scriptCharacters);
            }

            List<Character> characters = new() { target.Character };

            if (target.CanRegisterAsDemon && target.CharacterType != CharacterType.Demon)
            {
                foreach (var demon in scriptCharacters.Where(character => (int)character >= 3000))
                {
                    characters.Add(demon);
                }
            }
            if (target.CanRegisterAsMinion && target.CharacterType != CharacterType.Minion)
            {
                foreach (var minion in scriptCharacters.Where(character => 2000 <= (int)character && (int)character < 3000))
                {
                    characters.Add(minion);
                }
            }

            return await GetCharacterFromList(ravenkeeper, target, characters);
        }

        private async Task<Character> GetCharacterFromList(Player ravenkeeper, Player target, IReadOnlyCollection<Character> characters)
        {
            if (characters.Count() == 1)
            {
                return characters.First();
            }

            return await storyteller.GetCharacterForRavenkeeper(ravenkeeper, target, characters);
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IReadOnlyCollection<Character> scriptCharacters;
    }
}
