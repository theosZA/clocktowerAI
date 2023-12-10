using Clocktower.Game;
using Clocktower.Options;
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
            foreach (var monk in grimoire.Players.Where(player => player.Character == Character.Monk))
            {
                var options = grimoire.Players.Where(player => player != monk).ToOptions();
                var target = (await monk.Agent.RequestChoiceFromMonk(options)).GetPlayer();
                storyteller.ChoiceFromMonk(monk, target);
                if (!monk.DrunkOrPoisoned)
                {
                    target.Tokens.Add(Token.ProtectedByMonk);
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
