using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class ChoiceFromButler : IGameEvent
    {
        public ChoiceFromButler(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var butler in grimoire.GetLivingPlayers(Character.Butler))
            {
                // Note that the Butler's choice still takes effect even if they are drunk or poisoned.
                // This is because they can't ever be 100% certain that they are drunk or poisoned,
                // and voting without their chosen master would be a violation of game rules otherwise.
                // As such the Butler is being run as if "even when drunk or poisoned" is part of their ability.

                var target = await butler.Agent.RequestChoiceFromButler(grimoire.Players.Where(player => player != butler));
                storyteller.ChoiceFromButler(butler, target);
                target.Tokens.Add(Token.ChosenByButler, butler);
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
