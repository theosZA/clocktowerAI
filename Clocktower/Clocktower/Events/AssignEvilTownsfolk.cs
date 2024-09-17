using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class AssignEvilTownsfolk : IGameEvent
    {
        public AssignEvilTownsfolk(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            foreach (var bountyHunter in grimoire.Players.Where(player => player.RealCharacter == Character.Bounty_Hunter))
            {
                var evilTownsfolk = await storyteller.GetEvilTownsfolk(bountyHunter, grimoire.Players.WithCharacterType(CharacterType.Townsfolk));
                await evilTownsfolk.ChangeAlignment(Alignment.Evil);
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
