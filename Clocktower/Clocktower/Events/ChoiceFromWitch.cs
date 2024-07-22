using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class ChoiceFromWitch : IGameEvent
    {
        public ChoiceFromWitch(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            if (grimoire.Players.Count(player => player.Alive) > 3)
            {
                foreach (var witch in grimoire.GetPlayersWithAbility(Character.Witch))
                {
                    var target = await witch.Agent.RequestChoiceFromWitch(grimoire.Players);
                    storyteller.ChoiceFromWitch(witch, target);
                    if (!witch.DrunkOrPoisoned)
                    {
                        target.Tokens.Add(Token.CursedByWitch, witch);
                    }
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
