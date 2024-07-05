using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class NotifyWasherwoman : IGameEvent
    {
        public NotifyWasherwoman(IStoryteller storyteller, Grimoire grimoire, IReadOnlyCollection<Character> scriptCharacters, Random random)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.scriptCharacters = scriptCharacters;
            this.random = random;
        }

        public async Task RunEvent()
        {
            foreach (var washerwoman in grimoire.GetLivingPlayers(Character.Washerwoman))
            {
                await RunEvent(washerwoman);
            }
        }

        public async Task RunEvent(Player washerwoman)
        {
            var pings = await GetPings(washerwoman);
            var players = new List<Player> { pings.PlayerA, pings.PlayerB };
            players.Shuffle(random);

            await washerwoman.Agent.NotifyWasherwoman(players[0], players[1], pings.Character);
            storyteller.NotifyWasherwoman(washerwoman, players[0], players[1], pings.Character);
        }

        private async Task<CharacterForTwoPlayersOption> GetPings(Player washerwoman)
        {
            var options = GetOptions(washerwoman).ToList();
            return (CharacterForTwoPlayersOption)await storyteller.GetWasherwomanPings(washerwoman, options);
        }

        private IEnumerable<IOption> GetOptions(Player washerwoman)
        {
            var townsfolkCharacters = scriptCharacters.OfCharacterType(CharacterType.Townsfolk);
            // Exclude the washerwoman from their own ping.
            var players = grimoire.Players.Where(player => player != washerwoman).ToList();

            if (washerwoman.DrunkOrPoisoned)
            {   // Drunk or poisoned. They can see any two players as any townsfolk.
                return from townsfolkCharacter in townsfolkCharacters
                       from playerA in players
                       from playerB in players
                       where playerA != playerB
                       select (IOption)new CharacterForTwoPlayersOption(townsfolkCharacter, playerA, playerB);
            }

            // Consider each real townsfolk as player A, and combine with all other players for player B.
            return from townsfolkCharacter in townsfolkCharacters
                   from playerA in players
                   where playerA.CanRegisterAsTownsfolk
                   where playerA.Character == townsfolkCharacter || playerA.CharacterType != CharacterType.Townsfolk
                   from playerB in players
                   where playerA != playerB
                   select (IOption)new CharacterForTwoPlayersOption(townsfolkCharacter, playerA, playerB);
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IReadOnlyCollection<Character> scriptCharacters;
        private readonly Random random;
    }
}
