using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Triggers
{
    internal class RavenkeeperDeathTrigger : IDeathTrigger
    {
        public RavenkeeperDeathTrigger(IStoryteller storyteller, Grimoire grimoire, IReadOnlyCollection<Character> scriptCharacters)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.scriptCharacters = scriptCharacters;
        }

        public async Task RunTrigger(DeathInformation deathInformation)
        {
            if (!deathInformation.duringDay && deathInformation.dyingPlayer.ShouldRunAbility(Character.Ravenkeeper))
            {
                await RunRavenkeeper(deathInformation.dyingPlayer);
            }
        }

        private async Task RunRavenkeeper(Player ravenkeeper)
        {
            var target = await ravenkeeper.Agent.RequestChoiceFromRavenkeeper(grimoire.Players);
            var character = await GetCharacterSeenByRavenkeeper(ravenkeeper, target);
            storyteller.ChoiceFromRavenkeeper(ravenkeeper, target, character);
            await ravenkeeper.Agent.NotifyRavenkeeper(target, character);
        }

        private async Task<Character> GetCharacterSeenByRavenkeeper(Player ravenkeeper, Player target)
        {
            if (ravenkeeper.DrunkOrPoisoned)
            {   // Any on-script character is possible.
                return await GetCharacterSeenByRavenkeeperFromList(ravenkeeper, target, scriptCharacters);
            }

            List<Character> characters = new() { target.RealCharacter };

            if (target.CanRegisterAsDemon && target.CharacterType != CharacterType.Demon)
            {
                foreach (var demon in scriptCharacters.OfCharacterType(CharacterType.Demon))
                {
                    characters.Add(demon);
                }
            }
            if (target.CanRegisterAsMinion && target.CharacterType != CharacterType.Minion)
            {
                foreach (var minion in scriptCharacters.OfCharacterType(CharacterType.Minion))
                {
                    characters.Add(minion);
                }
            }
            if (target.CanRegisterAsOutsider && target.CharacterType != CharacterType.Outsider)
            {
                foreach (var outsider in scriptCharacters.OfCharacterType(CharacterType.Outsider))
                {
                    characters.Add(outsider);
                }
            }
            if (target.CanRegisterAsTownsfolk && target.CharacterType != CharacterType.Townsfolk)
            {
                foreach (var townsfolk in scriptCharacters.OfCharacterType(CharacterType.Townsfolk))
                {
                    characters.Add(townsfolk);
                }
            }

            return await GetCharacterSeenByRavenkeeperFromList(ravenkeeper, target, characters);
        }

        private async Task<Character> GetCharacterSeenByRavenkeeperFromList(Player ravenkeeper, Player target, IReadOnlyCollection<Character> characters)
        {
            if (characters.Count == 1)
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
