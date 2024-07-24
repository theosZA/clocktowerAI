using Clocktower.Game;
using Clocktower.Storyteller;
using Clocktower.Agent;

namespace Clocktower.Events
{
    internal class ChoiceFromDemon : IGameEvent
    {
        public ChoiceFromDemon(Character demonCharacter, IStoryteller storyteller, Grimoire grimoire)
        {
            this.demonCharacter = demonCharacter;
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            var demons = grimoire.PlayersForWhomWeShouldRunAbility(demonCharacter).ToList();   // Fix the demons first, so that any demons created during this process don't get a kill.
            foreach (var demon in demons)
            {
                var target = await demon.Agent.RequestChoiceFromDemon(demonCharacter, grimoire.Players);

                storyteller.ChoiceFromDemon(demon, target);
                if (!demon.DrunkOrPoisoned && target.Alive)
                {
                    await new Kills(storyteller, grimoire).NightKill(target, demon);
                }
            }
        }

        private readonly Character demonCharacter;
        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
