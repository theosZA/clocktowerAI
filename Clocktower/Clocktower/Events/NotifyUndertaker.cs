using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class NotifyUndertaker : IGameEvent
    {
        public NotifyUndertaker(IStoryteller storyteller, Grimoire grimoire, IReadOnlyCollection<Character> scriptCharacters)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.scriptCharacters = scriptCharacters;
        }

        public async Task RunEvent()
        {
            foreach (var undertaker in grimoire.GetLivingPlayers(Character.Undertaker))
            {
                var executedPlayer = grimoire.Players.FirstOrDefault(player => player.Tokens.Contains(Token.Executed)); // There should be at most 1 executed player.
                if (executedPlayer != null)
                {
                    var executedCharacter = await GetExecutedCharacter(undertaker, executedPlayer);
                    await undertaker.Agent.NotifyUndertaker(executedPlayer, executedCharacter);
                    storyteller.NotifyUndertaker(undertaker, executedPlayer, executedCharacter);
                }
            }
        }

        private async Task<Character> GetExecutedCharacter(Player undertaker, Player executedPlayer)
        {
            var possibleCharacters = GetPossibleCharacters(undertaker, executedPlayer);
            if (possibleCharacters.Count == 1)
            {
                return possibleCharacters.First();
            }
            return await storyteller.GetCharacterForUndertaker(undertaker, executedPlayer, possibleCharacters);
        }

        private IReadOnlyCollection<Character> GetPossibleCharacters(Player undertaker, Player executedPlayer)
        {
            if (undertaker.DrunkOrPoisoned)
            {
                return scriptCharacters;
            }

            var possibleCharacters = new List<Character> { executedPlayer.RealCharacter };
            if (executedPlayer.CanRegisterAsDemon && executedPlayer.CharacterType != CharacterType.Demon)
            {
                foreach (var demon in scriptCharacters.OfCharacterType(CharacterType.Demon))
                {
                    possibleCharacters.Add(demon);
                }
            }
            if (executedPlayer.CanRegisterAsMinion && executedPlayer.CharacterType != CharacterType.Minion)
            {
                foreach (var minion in scriptCharacters.OfCharacterType(CharacterType.Minion))
                {
                    possibleCharacters.Add(minion);
                }
            }
            if (executedPlayer.CanRegisterAsOutsider && executedPlayer.CharacterType != CharacterType.Outsider)
            {
                foreach (var outsider in scriptCharacters.OfCharacterType(CharacterType.Outsider))
                {
                    possibleCharacters.Add(outsider);
                }
            }
            if (executedPlayer.CanRegisterAsTownsfolk && executedPlayer.CharacterType != CharacterType.Townsfolk)
            {
                foreach (var townsfolk in scriptCharacters.OfCharacterType(CharacterType.Townsfolk))
                {
                    possibleCharacters.Add(townsfolk);
                }
            }

            return possibleCharacters;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IReadOnlyCollection<Character> scriptCharacters;
    }
}
