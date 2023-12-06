using Clocktower.Agent;
using Clocktower.Game;

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
            var godfather = grimoire.GetAlivePlayer(Character.Godfather);
            if (godfather != null)
            {
                var outsiders = grimoire.GetOutsiders().ToList();
                godfather.Agent.NotifyGodfather(outsiders);
                storyteller.NotifyGodfather(godfather, outsiders);
            }

            return Task.CompletedTask;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
