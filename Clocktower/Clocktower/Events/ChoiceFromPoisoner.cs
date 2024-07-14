using Clocktower.Agent;
using Clocktower.Game;
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
                var target = await poisoner.Agent.RequestChoiceFromPoisoner(grimoire.Players);
                storyteller.ChoiceFromPoisoner(poisoner, target);
                if (!poisoner.DrunkOrPoisoned)
                {
                    target.Tokens.Add(Token.PoisonedByPoisoner, poisoner);
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
