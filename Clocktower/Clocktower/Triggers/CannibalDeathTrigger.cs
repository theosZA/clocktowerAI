﻿using Clocktower.Events;
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

        public async Task RunCannibal(Player dyingPlayer, Player cannibal)
        {
            foreach (var player in grimoire.Players)
            {
                // The Cannibal isn't dead, but loses all relevant tokens associated with the previous character.
                player.Tokens.ClearTokensOnPlayerDeath(cannibal);
            }
            dyingPlayer.Tokens.Add(Token.CannibalEaten, cannibal);

            cannibal.Tokens.Remove(Token.CannibalPoisoned);
            cannibal.Tokens.Remove(Token.CannibalDrunk);
            cannibal.Tokens.Add(Token.CannibalFirstNightWithAbility, cannibal);

            if (dyingPlayer.Alignment == Alignment.Evil)
            {
                cannibal.Tokens.Add(Token.CannibalPoisoned, cannibal);
                var fakeCannibalAbility = await storyteller.ChooseFakeCannibalAbility(cannibal, dyingPlayer, scriptCharacters.Where(character => character.Alignment() == Alignment.Good && character != Character.Drunk));
                cannibal.CannibalAbility = fakeCannibalAbility;
            }
            else if (dyingPlayer.RealCharacter == Character.Drunk)
            {
                cannibal.Tokens.Add(Token.CannibalDrunk, cannibal);
                var fakeCannibalAbility = await storyteller.ChooseFakeCannibalAbility(cannibal, dyingPlayer, scriptCharacters.Where(character => character.CharacterType() == CharacterType.Townsfolk));
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
