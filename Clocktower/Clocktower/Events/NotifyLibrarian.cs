using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class NotifyLibrarian : IGameEvent
    {
        public NotifyLibrarian(IStoryteller storyteller, Grimoire grimoire, IReadOnlyCollection<Character> scriptCharacters, Random random)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.scriptCharacters = scriptCharacters;
            this.random = random;
        }

        public async Task RunEvent()
        {
            foreach (var librarian in grimoire.GetLivingPlayers(Character.Librarian))
            {
                var pings = await GetPings(librarian);
                var players = new List<Player> { pings.PlayerA, pings.PlayerB };
                players.Shuffle(random);

                librarian.Agent.NotifyLibrarian(players[0], players[1], pings.Character);
                storyteller.NotifyLibrarian(librarian, players[0], players[1], pings.Character);
            }
        }

        public async Task<CharacterForTwoPlayersOption> GetPings(Player librarian)
        {
            var options = GetOptions(librarian).ToList();
            return (CharacterForTwoPlayersOption)await storyteller.GetLibrarianPings(librarian, options);
        }

        public IEnumerable<IOption> GetOptions(Player librarian)
        {
            // Exclude the librarian from their own ping.
            var players = grimoire.Players.Where(player => player != librarian);

            if (librarian.DrunkOrPoisoned)
            {   // Drunk or poisoned. They can see any two players as any outsider.
                var outsiders = scriptCharacters.Where(character => 1000 <= (int)character && (int)character < 2000);
                return from outsider in outsiders
                       from playerA in players
                       from playerB in players
                       where playerA != playerB
                       select (IOption)new CharacterForTwoPlayersOption(outsider, playerA, playerB);
            }

            // Consider each real outsider as player A, and combine with all other players for player B.
            return from playerA in players
                   where playerA.CharacterType == CharacterType.Outsider
                   from playerB in players
                   where playerA != playerB
                   select (IOption)new CharacterForTwoPlayersOption(playerA.Tokens.Contains(Token.IsTheDrunk) ? Character.Drunk : playerA.Character, playerA, playerB);
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IReadOnlyCollection<Character> scriptCharacters;
        private readonly Random random;
    }
}
