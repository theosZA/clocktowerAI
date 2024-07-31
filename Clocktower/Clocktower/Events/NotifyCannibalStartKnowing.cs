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
                switch (cannibal.CannibalAbility)
                {
                    case Character.Chef:
                        await new NotifyChef(storyteller, grimoire).RunEvent(cannibal);
                        break;

                    case Character.Steward:
                        await new NotifySteward(storyteller, grimoire).RunEvent(cannibal);
                        break;

                    case Character.Noble:
                        await new NotifyNoble(storyteller, grimoire).RunEvent(cannibal);
                        break;

                    case Character.Investigator:
                        await new NotifyInvestigator(storyteller, grimoire, scriptCharacters, random).RunEvent(cannibal);
                        break;

                    case Character.Librarian:
                        await new NotifyLibrarian(storyteller, grimoire, scriptCharacters, random).RunEvent(cannibal);
                        break;

                    case Character.Washerwoman:
                        await new NotifyWasherwoman(storyteller, grimoire, scriptCharacters, random).RunEvent(cannibal);
                        break;

                    case Character.Shugenja:
                        await new NotifyShugenja(storyteller, grimoire).RunEvent(cannibal);
                        break;
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IReadOnlyCollection<Character> scriptCharacters;
        private readonly Random random;
    }
}
