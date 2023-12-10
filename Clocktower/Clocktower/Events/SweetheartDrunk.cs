using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class SweetheartDrunk : IGameEvent
    {
        public SweetheartDrunk(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            // Any newly dead Sweethearts that need to drunk someone?
            if (grimoire.Players.Any(player => !player.Alive && player.Character == Character.Sweetheart) &&
                !grimoire.Players.Any(player => player.Tokens.Contains(Token.SweetheartDrunk)))
            {
                (await storyteller.GetSweetheartDrunk(grimoire.Players)).Tokens.Add(Token.SweetheartDrunk);
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
