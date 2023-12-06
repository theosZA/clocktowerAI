using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Options;
using System.Diagnostics;

namespace Clocktower.Events
{
    internal class ChoiceFromGodfather : IGameEvent
    {
        public ChoiceFromGodfather(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            var godfather = grimoire.GetPlayer(Character.Godfather);
            if (godfather == null || !godfather.Tokens.Contains(Token.GodfatherKillsTonight))
            {
                return;
            }

            var options = grimoire.Players.Select(player => new PlayerOption(player)).ToList();

            var choice = await godfather.Agent.RequestChoiceFromGodfather(options);
            var playerOption = choice as PlayerOption;
            Debug.Assert(playerOption != null);
            var player = playerOption.Player;
            storyteller.ChoiceFromGodfather(godfather, player);

            if (!godfather.DrunkOrPoisoned)
            {
                player.Tokens.Add(Token.DiedAtNight);
            }
            godfather.Tokens.Remove(Token.GodfatherKillsTonight);
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
