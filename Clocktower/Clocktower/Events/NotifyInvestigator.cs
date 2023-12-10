using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class NotifyInvestigator : IGameEvent
    {
        public NotifyInvestigator(IStoryteller storyteller, Grimoire grimoire, IReadOnlyCollection<Character> scriptCharacters, Random random)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.scriptCharacters = scriptCharacters;
            this.random = random;
        }

        public async Task RunEvent()
        {
            foreach (var investigator in grimoire.GetLivingPlayers(Character.Investigator))
            {
                var pings = await GetPings(investigator);
                var players = new List<Player> { pings.PlayerA, pings.PlayerB };
                players.Shuffle(random);

                investigator.Agent.NotifyInvestigator(players[0], players[1], pings.Character);
                storyteller.NotifyInvestigator(investigator, players[0], players[1], pings.Character);
            }
        }

        public async Task<CharacterForTwoPlayersOption> GetPings(Player investigator)
        {
            var options = GetOptions(investigator).ToList();
            return (CharacterForTwoPlayersOption)await storyteller.GetInvestigatorPings(investigator, options);
        }

        public IEnumerable<IOption> GetOptions(Player investigator)
        {
            var minionCharacters = scriptCharacters.Where(character => 2000 <= (int)character && (int)character < 3000);
            // Exclude the investigator from their own ping.
            var players = grimoire.Players.Where(player => player != investigator);

            if (investigator.DrunkOrPoisoned)
            {   // Drunk or poisoned. They can see any two players as any minion.
                return from minionCharacter in minionCharacters
                       from playerA in players
                       from playerB in players
                       where playerA != playerB
                       select (IOption)new CharacterForTwoPlayersOption(minionCharacter, playerA, playerB);
            }

            // Consider each real minion as player A, and combine with all other players for player B.
            return from minionCharacter in minionCharacters
                   from playerA in players
                   where playerA.CanRegisterAsMinion
                   where playerA.Character == minionCharacter || playerA.CharacterType != CharacterType.Minion
                   from playerB in players
                   where playerA != playerB
                   select (IOption)new CharacterForTwoPlayersOption(minionCharacter, playerA, playerB);
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IReadOnlyCollection<Character> scriptCharacters;
        private readonly Random random;
    }
}
