using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class ChoiceFromNightwatchman : IGameEvent
    {
        public ChoiceFromNightwatchman(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var nightwatchman in grimoire.PlayersForWhomWeShouldRunAbility(Character.Nightwatchman))
            {
                var target = await nightwatchman.Agent.RequestChoiceFromNightwatchman(grimoire.Players);
                if (target != null)
                {
                    nightwatchman.Tokens.Add(Token.UsedOncePerGameAbility, nightwatchman);
                    storyteller.ShowNightwatchman(nightwatchman, target, shown: !nightwatchman.DrunkOrPoisoned);
                    if (!nightwatchman.DrunkOrPoisoned)
                    {
                        await target.Agent.ShowNightwatchman(nightwatchman);
                    }
                }
            }   
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
