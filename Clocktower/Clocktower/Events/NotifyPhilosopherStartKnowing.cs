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
                var gameEvent = GetStartKnowingEvent(philosopher.Character);
                if (gameEvent != null)
                {
                    await gameEvent.RunEvent();
                }
            }
        }

        private static bool IsNewPhilosopher(Player player)
        {
            return (player.Tokens.Contains(Token.IsThePhilosopher) || player.Tokens.Contains(Token.IsTheBadPhilosopher))
                && player.Tokens.Contains(Token.PhilosopherUsedAbilityTonight);
        }

        private IGameEvent? GetStartKnowingEvent(Character character)
        {
            return character switch
            {
                Character.Steward => new NotifySteward(storyteller, grimoire),
                Character.Investigator => new NotifyInvestigator(storyteller, grimoire, scriptCharacters, random),
                Character.Librarian => new NotifyLibrarian(storyteller, grimoire, scriptCharacters, random),
                Character.Shugenja => new NotifyShugenja(storyteller, grimoire),
                _ => null,
            };
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IReadOnlyCollection<Character> scriptCharacters;
        private readonly Random random;
    }
}
