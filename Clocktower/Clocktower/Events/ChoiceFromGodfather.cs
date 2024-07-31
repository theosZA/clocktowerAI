using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Storyteller;
using Clocktower.Triggers;

namespace Clocktower.Events
{
    internal class ChoiceFromGodfather : IGameEvent
    {
        public ChoiceFromGodfather(IStoryteller storyteller, Grimoire grimoire, Deaths deaths)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.deaths = deaths;
        }

        public async Task RunEvent()
        {
            foreach (var godfather in grimoire.PlayersForWhomWeShouldRunAbility(Character.Godfather).WithToken(Token.GodfatherKillsTonight))
            {
                var target = await godfather.Agent.RequestChoiceFromGodfather(grimoire.Players);
                storyteller.ChoiceFromGodfather(godfather, target);

                godfather.Tokens.Remove(Token.GodfatherKillsTonight);
                if (!godfather.DrunkOrPoisoned && target.Alive)
                {
                    await deaths.NightKill(target, godfather);
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly Deaths deaths;
    }
}
