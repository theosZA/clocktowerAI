using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class ChoiceFromWidow : IGameEvent
    {
        public ChoiceFromWidow(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var widow in grimoire.PlayersWithHealthyAbility(Character.Widow))
            {
                await RunEvent(widow);
            }
        }

        public async Task RunEvent(Player widow)
        {
            await widow.Agent.ShowGrimoire(Character.Widow, grimoire);
            var target = await widow.Agent.RequestChoiceFromWidow(grimoire.Players);
            storyteller.ChoiceFromWidow(widow, target);
            target.Tokens.Add(Token.PoisonedByWidow, widow);

            if (!widow.DrunkOrPoisoned) // They may have become drunk or poisoned at this step depending on their choice of targets, e.g. if they chose themself.
            {
                var ping = await storyteller.GetWidowPing(widow, grimoire.Players.WithAlignment(Alignment.Good).ToList());
                await ping.Agent.LearnOfWidow();
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
