using Clocktower.Events;
using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Triggers
{
    internal class CannibalDeathTrigger : IDeathTrigger
    {
        public CannibalDeathTrigger(IStoryteller storyteller, Grimoire grimoire, IReadOnlyCollection<Character> scriptCharacters)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.scriptCharacters = scriptCharacters;
        }

        public async Task RunTrigger(DeathInformation deathInformation)
        {
            if (!deathInformation.executed)
            {
                return;
            }

            foreach (var cannibal in grimoire.PlayersForWhomWeShouldRunAbility(Character.Cannibal))
            {
                await RunCannibal(deathInformation.dyingPlayer, cannibal);
            }
        }

        private async Task RunCannibal(Player dyingPlayer, Player cannibal)
        {
            foreach (var player in grimoire.Players)
            {
                player.Tokens.Remove(Token.CannibalEaten, cannibal);
            }
            dyingPlayer.Tokens.Add(Token.CannibalEaten, cannibal);
            cannibal.Tokens.Add(Token.CannibalFirstNightWithAbility, cannibal);
            cannibal.Tokens.Remove(Token.CannibalPoisoned);
            if (dyingPlayer.Alignment == Alignment.Evil || dyingPlayer.RealCharacter == Character.Drunk)
            {
                cannibal.Tokens.Add(Token.CannibalPoisoned, cannibal);
                var fakeCannibalAbility = await storyteller.ChooseFakeCannibalAbility(cannibal, dyingPlayer, scriptCharacters.Where(character => character.Alignment() == Alignment.Good));
                cannibal.CannibalAbility = fakeCannibalAbility;
            }
            else
            {
                cannibal.CannibalAbility = dyingPlayer.RealCharacter;
                if (cannibal.CannibalAbility == Character.Fortune_Teller)
                {
                    await new AssignFortuneTellerRedHerring(storyteller, grimoire).AssignRedHerring(fortuneTeller: cannibal);
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IReadOnlyCollection<Character> scriptCharacters;
    }
}
