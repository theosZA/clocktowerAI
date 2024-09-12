using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class ChoiceFromHuntsman : IGameEvent
    {
        public ChoiceFromHuntsman(IStoryteller storyteller, Grimoire grimoire, IReadOnlyCollection<Character> scriptCharacters, Random random, bool firstNight)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.scriptCharacters = scriptCharacters;
            this.random = random;
            this.firstNight = firstNight;
        }

        public async Task RunEvent()
        {
            foreach (var huntsman in grimoire.PlayersForWhomWeShouldRunAbility(Character.Huntsman))
            {
                var target = await huntsman.Agent.RequestChoiceFromHuntsman(grimoire.Players.Where(player => player.Alive));
                if (target != null)
                {
                    huntsman.Tokens.Add(Token.UsedOncePerGameAbility, huntsman);
                    if (huntsman.DrunkOrPoisoned || target.Character != Character.Damsel)
                    {
                        storyteller.FailedHuntsmanGuess(huntsman, target);
                    }
                    else
                    {
                        var notInPlayTownsfolk = scriptCharacters.OfCharacterType(CharacterType.Townsfolk).Except(grimoire.Players.Select(player => player.RealCharacter));
                        var newCharacter = await storyteller.ChooseDamselCharacter(target, huntsman, notInPlayTownsfolk);
                        await grimoire.ChangeCharacter(target, newCharacter);
                        storyteller.AssignCharacter(target);

                        if (!firstNight)
                        {
                            await StartKnowing.Notify(target, newCharacter, storyteller, grimoire, scriptCharacters, random);
                        }
                    }
                }
            }   
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IReadOnlyCollection<Character> scriptCharacters;
        private readonly Random random;
        private readonly bool firstNight;
    }
}
