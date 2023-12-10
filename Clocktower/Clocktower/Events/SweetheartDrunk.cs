using Clocktower.Game;
using Clocktower.Options;
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
                var options = grimoire.Players.Select(player => new PlayerOption(player))   // Technically all players (including dead players or the demon) can become Sweetheart-drunk.
                                              .ToList();
                var choice = (PlayerOption)await storyteller.GetSweetheartDrunk(options);
                choice.Player.Tokens.Add(Token.SweetheartDrunk);
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
