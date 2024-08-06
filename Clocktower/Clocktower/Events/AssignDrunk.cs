using Clocktower.Game;
using Clocktower.Setup;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class AssignDrunk : IGameEvent
    {
        public AssignDrunk(IStoryteller storyteller, IGameSetup gameSetup, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.gameSetup = gameSetup;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            var drunkCandidates = grimoire.Players.Where(player => gameSetup.CanCharacterBeTheDrunk(player.Character));
            var drunk = await storyteller.GetDrunk(drunkCandidates);
            drunk.Tokens.Add(Token.IsTheDrunk, drunk);
        }

        private readonly IStoryteller storyteller;
        private readonly IGameSetup gameSetup;
        private readonly Grimoire grimoire;
    }
}
