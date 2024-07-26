using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class ChoiceFromDevilsAdvocate : IGameEvent
    {
        public ChoiceFromDevilsAdvocate(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var advocate in grimoire.PlayersForWhomWeShouldRunAbility(Character.Devils_Advocate))
            {
                var previousPick = grimoire.Players.FirstOrDefault(player => player.Tokens.HasTokenForPlayer(Token.PickedByDevilsAdvocate, advocate));
                var excludedPicks = new List<Player>();
                if (previousPick != null)
                {
                    excludedPicks.Add(previousPick);
                    previousPick.Tokens.Remove(Token.PickedByDevilsAdvocate, advocate);
                }

                var pick = await advocate.Agent.RequestChoiceFromDevilsAdvocate(grimoire.Players.Except(excludedPicks));
                pick.Tokens.Add(Token.PickedByDevilsAdvocate, advocate);
                storyteller.ChoiceFromDevilsAdvocate(advocate, pick);
                if (!advocate.DrunkOrPoisoned)
                {
                    pick.Tokens.Add(Token.ProtectedByDevilsAdvocate, advocate);
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
