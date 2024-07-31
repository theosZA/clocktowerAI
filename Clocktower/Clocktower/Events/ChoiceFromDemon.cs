using Clocktower.Game;
using Clocktower.Storyteller;
using Clocktower.Agent;
using Clocktower.Triggers;

namespace Clocktower.Events
{
    internal class ChoiceFromDemon : IGameEvent
    {
        public ChoiceFromDemon(Character demonCharacter, IStoryteller storyteller, Grimoire grimoire, Deaths deaths)
        {
            this.demonCharacter = demonCharacter;
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.deaths = deaths;
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
                    await deaths.NightKill(target, demon);
                }
            }
        }

        private readonly Character demonCharacter;
        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly Deaths deaths;
    }
}
