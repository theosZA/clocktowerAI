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
                switch (philosopher.Character)
                {
                    case Character.Chef:
                        await new NotifyChef(storyteller, grimoire).RunEvent(philosopher);
                        break;

                    case Character.Steward:
                        await new NotifySteward(storyteller, grimoire).RunEvent(philosopher);
                        break;

                    case Character.Noble:
                        await new NotifyNoble(storyteller, grimoire, random).RunEvent(philosopher);
                        break;

                    case Character.Investigator:
                        await new NotifyInvestigator(storyteller, grimoire, scriptCharacters, random).RunEvent(philosopher);
                        break;

                    case Character.Librarian:
                        await new NotifyLibrarian(storyteller, grimoire, scriptCharacters, random).RunEvent(philosopher);
                        break;

                    case Character.Washerwoman:
                        await new NotifyWasherwoman(storyteller, grimoire, scriptCharacters, random).RunEvent(philosopher);
                        break;

                    case Character.Shugenja:
                        await new NotifyShugenja(storyteller, grimoire).RunEvent(philosopher);
                        break;
                }
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
