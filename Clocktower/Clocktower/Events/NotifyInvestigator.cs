﻿using Clocktower.Game;
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
            foreach (var investigator in grimoire.PlayersForWhomWeShouldRunAbility(Character.Investigator))
            {
                await RunEvent(investigator);
            }
        }

        public async Task RunEvent(Player investigator)
        {
            var pings = await GetPings(investigator);
            pings.PlayerA.Tokens.Add(Token.InvestigatorPing, investigator);
            pings.PlayerB.Tokens.Add(Token.InvestigatorWrong, investigator);

            var players = new List<Player> { pings.PlayerA, pings.PlayerB };
            players.Shuffle(random);

            await investigator.Agent.NotifyInvestigator(players[0], players[1], pings.Character);
            storyteller.NotifyInvestigator(investigator, players[0], players[1], pings.Character);
        }

        private async Task<CharacterForTwoPlayersOption> GetPings(Player investigator)
        {
            var options = GetOptions(investigator).ToList();
            return (CharacterForTwoPlayersOption)await storyteller.GetInvestigatorPings(investigator, options);
        }

        private IEnumerable<IOption> GetOptions(Player investigator)
        {
            var minionCharacters = scriptCharacters.OfCharacterType(CharacterType.Minion);
            // Exclude the investigator from their own ping.
            var players = grimoire.Players.Where(player => player != investigator).ToList();

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
