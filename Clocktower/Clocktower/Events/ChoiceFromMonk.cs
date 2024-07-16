using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class ChoiceFromMonk : IGameEvent
    {
        public ChoiceFromMonk(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var monk in grimoire.GetPlayersWithAbility(Character.Monk))
            {
                var target = await monk.Agent.RequestChoiceFromMonk(grimoire.Players.Where(player => player != monk));
                storyteller.ChoiceFromMonk(monk, target);
                if (!monk.DrunkOrPoisoned)
                {
                    target.Tokens.Add(Token.ProtectedByMonk, monk);
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
