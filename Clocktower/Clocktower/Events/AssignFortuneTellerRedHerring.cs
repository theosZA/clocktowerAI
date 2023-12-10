using Clocktower.Game;
using Clocktower.Options;
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
            if (!grimoire.Players.Any(player => player.Character == Character.Fortune_Teller && !player.Tokens.Contains(Token.IsTheDrunk)))
            {
                return;
            }

            var redHerringOptions = grimoire.Players.Where(player => player.CharacterType != CharacterType.Demon)
                                                    .Select(player => new PlayerOption(player))
                                                    .ToList();
            var redHerring = ((PlayerOption)await storyteller.GetFortuneTellerRedHerring(redHerringOptions)).Player;
            redHerring.Tokens.Add(Token.FortuneTellerRedHerring);
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
