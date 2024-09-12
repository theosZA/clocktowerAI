using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class NotifyPhilosopherStartKnowing : IGameEvent
    {
        public NotifyPhilosopherStartKnowing(IStoryteller storyteller, Grimoire grimoire, IReadOnlyCollection<Character> scriptCharacters, Random random)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.scriptCharacters = scriptCharacters;
            this.random = random;
        }

        public async Task RunEvent()
        {
            // Check if we have a Philosopher who just turned tonight.
            foreach (var philosopher in grimoire.Players.Where(player => player.Alive && IsNewPhilosopher(player)))
            {
                await StartKnowing.Notify(philosopher, philosopher.Character, storyteller, grimoire, scriptCharacters, random);
            }
        }

        private static bool IsNewPhilosopher(Player player)
        {
            return (player.Tokens.HasToken(Token.IsThePhilosopher) || player.Tokens.HasToken(Token.IsTheBadPhilosopher))
                && player.Tokens.HasToken(Token.PhilosopherUsedAbilityTonight);
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IReadOnlyCollection<Character> scriptCharacters;
        private readonly Random random;
    }
}
