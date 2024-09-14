using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class NotifyJuggler : IGameEvent
    {
        public NotifyJuggler(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var juggler in grimoire.PlayersForWhomWeShouldRunAbility(Character.Juggler).Where(juggler => juggler.Tokens.HasToken(Token.JugglerFirstDay)))
            {
                await RunJuggler(juggler);
            }

            // Special handling for Cannibals who gain the Juggler ability.
            foreach (var cannibal in grimoire.PlayersForWhomWeShouldRunAbility(Character.Cannibal).Where(player => player.CannibalAbility == Character.Juggler))
            {
                var executedJuggler = grimoire.Players.First(player => player.Tokens.HasTokenForPlayer(Token.CannibalEaten, cannibal));
                await RunCannibalJuggler(cannibal, executedJuggler);
            }

            foreach (var deadJuggler in grimoire.Players.Where(player => !player.Alive && player.Tokens.HasToken(Token.JugglerFirstDay)))
            {
                deadJuggler.Tokens.Remove(Token.JugglerFirstDay);
            }
        }

        private async Task RunJuggler(Player juggler)
        {
            juggler.Tokens.Remove(Token.JugglerFirstDay);
            await RunJuggler(juggler, juggler);
        }

        private async Task RunCannibalJuggler(Player cannibal, Player executedJuggler)
        {
            // Jinx - Cannibal / Juggler: If the Juggler guesses on their first day and dies by execution, tonight the living Cannibal learns how many guesses the Juggler got correct.
            if (executedJuggler.Tokens.HasToken(Token.JugglerFirstDay))
            {
                executedJuggler.Tokens.Remove(Token.JugglerFirstDay);
                await RunJuggler(executedJuggler, cannibal);                
            }

            // Is this the Cannibal's first night with Cannibal ability? If so, tomorrow is their "first day" for the purpose of being the Juggler.
            if (cannibal.Tokens.HasToken(Token.CannibalFirstNightWithAbility))
            {
                cannibal.Tokens.Add(Token.JugglerFirstDay, cannibal);
            }
        }

        private async Task RunJuggler(Player playerWhoMadeJuggles, Player playerToReceiveJuggleNumber)
        {
            int jugglerCount = GetRealJugglerCount(playerWhoMadeJuggles);
            if (playerToReceiveJuggleNumber.DrunkOrPoisoned)
            {
                jugglerCount = await storyteller.GetJugglerNumber(playerToReceiveJuggleNumber, jugglerCount);
            }
            await playerToReceiveJuggleNumber.Agent.NotifyJuggler(jugglerCount);
            storyteller.NotifyJuggler(playerToReceiveJuggleNumber, jugglerCount);
        }

        private int GetRealJugglerCount(Player juggler)
        {
            return grimoire.Players.Sum(player => player.Tokens.CountTokensForPlayer(Token.JuggledCorrectly, juggler));
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
