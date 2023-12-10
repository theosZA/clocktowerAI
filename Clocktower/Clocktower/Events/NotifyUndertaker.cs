using Clocktower.Game;
using Clocktower.Options;
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
                    undertaker.Agent.NotifyUndertaker(executedPlayer, executedCharacter);
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
            var options = possibleCharacters.Select(character => (IOption)new CharacterOption(character)).ToList();
            var choice = (CharacterOption)await storyteller.GetCharacterForUndertaker(undertaker, executedPlayer, options);
            return choice.Character;
        }

        private IReadOnlyCollection<Character> GetPossibleCharacters(Player undertaker, Player executedPlayer)
        {
            if (undertaker.DrunkOrPoisoned)
            {
                return scriptCharacters;
            }

            var possibleCharacters = new List<Character> { executedPlayer.Character };
            if (executedPlayer.CanRegisterAsDemon && executedPlayer.CharacterType != CharacterType.Demon)
            {
                foreach (var demon in scriptCharacters.Where(character => (int)character >= 3000))
                {
                    possibleCharacters.Add(demon);
                }
            }
            if (executedPlayer.CanRegisterAsMinion && executedPlayer.CharacterType != CharacterType.Minion)
            {
                foreach (var minion in scriptCharacters.Where(character => 2000 <= (int)character && (int)character < 3000))
                {
                    possibleCharacters.Add(minion);
                }
            }

            return possibleCharacters;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IReadOnlyCollection<Character> scriptCharacters;
    }
}
