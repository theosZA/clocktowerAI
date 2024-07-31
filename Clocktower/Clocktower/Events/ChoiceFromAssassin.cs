using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Storyteller;
using Clocktower.Triggers;

namespace Clocktower.Events
{
    internal class ChoiceFromAssassin : IGameEvent
    {
        public ChoiceFromAssassin(IStoryteller storyteller, Grimoire grimoire, Deaths deaths)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.deaths = deaths;
        }

        public async Task RunEvent()
        {
            foreach (var assassin in grimoire.PlayersForWhomWeShouldRunAbility(Character.Assassin))
            {
                var target = await assassin.Agent.RequestChoiceFromAssassin(grimoire.Players);
                storyteller.ChoiceFromAssassin(assassin, target);

                if (target != null)
                {
                    assassin.Tokens.Add(Token.UsedOncePerGameAbility, assassin);
                    if (!assassin.DrunkOrPoisoned && target.Alive)
                    {
                        await deaths.NightKill(target, assassin);
                    }
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly Deaths deaths;
    }
}
