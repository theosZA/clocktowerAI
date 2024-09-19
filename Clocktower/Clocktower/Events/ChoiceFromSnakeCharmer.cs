using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class ChoiceFromSnakeCharmer : IGameEvent
    {
        public ChoiceFromSnakeCharmer(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            // Fix list of Snake Charmers up front so that Demons swapped into Snake Charmers won't act this time.
            var snakeCharmers = grimoire.PlayersForWhomWeShouldRunAbility(Character.Snake_Charmer).ToList();
            foreach (var snakeCharmer in snakeCharmers)
            {
                await RunSnakeCharmer(snakeCharmer);
            }
        }

        public async Task RunSnakeCharmer(Player snakeCharmer)
        {
            var target = await snakeCharmer.Agent.RequestChoiceFromSnakeCharmer(grimoire.Players.Where(player => player.Alive));

            bool success = await IsSuccessfulSnakeCharm(snakeCharmer, target);
            storyteller.ChoiceFromSnakeCharmer(snakeCharmer, target, success);

            if (success)
            {
                // We are swapping based on real characters to allow for cases like a Philosopher-Snake Charmer.
                // In such a case, the demon becomes a (poisoned) Philosopher.
                // We don't have to worry about scenarios like Drunk-Snake Charmer or Marionette-Snake Charmer because
                // in those cases we won't have had a successful snake charming.

                var snakeCharmerCharacter = snakeCharmer.RealCharacter;
                var snakeCharmerAlignment = snakeCharmer.Alignment;

                var demonCharacter = target.RealCharacter;
                var demonAlignment = target.Alignment;

                await grimoire.ChangeCharacter(snakeCharmer, demonCharacter, demonAlignment);
                await grimoire.ChangeCharacter(target, snakeCharmerCharacter, snakeCharmerAlignment);

                target.Tokens.Add(Token.SnakeCharmerPoisoned, target);
            }
        }

        public async Task<bool> IsSuccessfulSnakeCharm(Player snakeCharmer, Player target)
        {
            if (snakeCharmer.DrunkOrPoisoned)
            {
                return false;
            }
            
            if (target.CharacterType == CharacterType.Demon)
            {
                return true;
            }

            if (target.CanRegisterAsDemon)
            {
                return await storyteller.ShouldRegisterForSnakeCharmer(snakeCharmer, target);
            }

            return false;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
