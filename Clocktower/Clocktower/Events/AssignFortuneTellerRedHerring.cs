using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class AssignFortuneTellerRedHerring : IGameEvent
    {
        public AssignFortuneTellerRedHerring(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            // Ensure that there is at least one real Fortune Teller.
            // We currently have a limitation that multiple Fortune Tellers share the same red herring (possibly an issue for the Philosopher).
            if (!grimoire.Players.Any(player => player.RealCharacter == Character.Fortune_Teller))  // This currently doesn't handle a Philosopher-Fortune Teller
            {
                return;
            }

            var redHerringCandidates = grimoire.Players.Where(player => player.CharacterType != CharacterType.Demon);
            (await storyteller.GetFortuneTellerRedHerring(redHerringCandidates)).Tokens.Add(Token.FortuneTellerRedHerring);
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
