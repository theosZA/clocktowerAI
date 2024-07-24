using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class NotifyGodfather : IGameEvent
    {
        public NotifyGodfather(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public Task RunEvent()
        {
            foreach (var godfather in grimoire.PlayersForWhomWeShouldRunAbility(Character.Godfather))
            {
                var outsiders = grimoire.Players.Where(player => player.CharacterType == CharacterType.Outsider)
                                                .Select(player => player.RealCharacter)
                                                .OrderBy(character => character.ToString())
                                                .ToList();
                godfather.Agent.NotifyGodfather(outsiders);
                storyteller.NotifyGodfather(godfather, outsiders);
            }

            return Task.CompletedTask;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
