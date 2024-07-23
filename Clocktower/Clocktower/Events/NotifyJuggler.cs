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
            foreach (var juggler in grimoire.GetPlayersWithAbility(Character.Juggler).Where(juggler => juggler.Tokens.HasToken(Token.JugglerFirstDay)))
            {
                await RunJuggler(juggler);
            }
        }

        private async Task RunJuggler(Player juggler)
        {
            juggler.Tokens.Remove(Token.JugglerFirstDay);

            int jugglerCount = grimoire.Players.Sum(player => player.Tokens.CountTokensForPlayer(Token.JuggledCorrectly, juggler));
            if (juggler.DrunkOrPoisoned)
            {
                jugglerCount = await storyteller.GetJugglerNumber(juggler, jugglerCount);
            }
            await juggler.Agent.NotifyJuggler(jugglerCount);
            storyteller.NotifyJuggler(juggler, jugglerCount);
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
