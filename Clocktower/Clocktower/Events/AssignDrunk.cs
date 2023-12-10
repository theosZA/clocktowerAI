using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class AssignDrunk : IGameEvent
    {
        public AssignDrunk(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            var drunkCandidates = grimoire.Players.Where(player => player.CharacterType == CharacterType.Townsfolk).ToOptions();
            var drunk = (await storyteller.GetDrunk(drunkCandidates)).GetPlayer();
            drunk.Tokens.Add(Token.IsTheDrunk);
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
