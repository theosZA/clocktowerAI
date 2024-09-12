using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class NotifyCannibalStartKnowing : IGameEvent
    {
        public NotifyCannibalStartKnowing(IStoryteller storyteller, Grimoire grimoire, IReadOnlyCollection<Character> scriptCharacters, Random random)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.scriptCharacters = scriptCharacters;
            this.random = random;
        }

        public async Task RunEvent()
        {
            // Check if we have a Cannibal who just gained a start knowning ability.
            foreach (var cannibal in grimoire.Players.Where(player => player.Alive && player.ShouldRunAbility(Character.Cannibal) && player.Tokens.HasToken(Token.CannibalFirstNightWithAbility)))
            {
                if (cannibal.CannibalAbility.HasValue)
                {
                    await StartKnowing.Notify(cannibal, cannibal.CannibalAbility.Value, storyteller, grimoire, scriptCharacters, random);
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IReadOnlyCollection<Character> scriptCharacters;
        private readonly Random random;
    }
}
