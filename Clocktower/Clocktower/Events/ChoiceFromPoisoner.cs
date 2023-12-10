using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class ChoiceFromPoisoner : IGameEvent
    {
        public ChoiceFromPoisoner(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var poisoner in grimoire.GetLivingPlayers(Character.Poisoner))
            {
                var options = grimoire.Players.Select(player => new PlayerOption(player)).ToList();
                var choice = (PlayerOption)await poisoner.Agent.RequestChoiceFromPoisoner(options);
                var target = choice.Player;

                storyteller.ChoiceFromPoisoner(poisoner, target);
                if (!poisoner.DrunkOrPoisoned)
                {
                    target.Tokens.Add(Token.PoisonedByPoisoner);
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
