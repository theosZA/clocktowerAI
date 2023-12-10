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
            var drunkCandidates = grimoire.Players.Where(player => player.CharacterType == CharacterType.Townsfolk)
                                                  .Select(player => new PlayerOption(player))
                                                  .ToList();
            var drunk = ((PlayerOption)await storyteller.GetDrunk(drunkCandidates)).Player;
            drunk.Tokens.Add(Token.IsTheDrunk);
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
