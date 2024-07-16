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
            foreach (var player in grimoire.Players.WithCharacter(Character.Fortune_Teller))
            {
                await AssignRedHerring(player);
            }
        }

        public async Task AssignRedHerring(Player fortuneTeller)
        {
            var redHerringCandidates = grimoire.Players.Where(player => player.CanRegisterAsGood);
            var redHerring = await storyteller.GetFortuneTellerRedHerring(fortuneTeller, redHerringCandidates);
            redHerring.Tokens.Add(Token.FortuneTellerRedHerring, fortuneTeller);
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
