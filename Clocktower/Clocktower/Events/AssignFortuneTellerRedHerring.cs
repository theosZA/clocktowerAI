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
            // Ensure that there is a real Fortune Teller.
            if (!grimoire.Players.Any(player => player.RealCharacter == Character.Fortune_Teller))
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
