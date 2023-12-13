using Clocktower.Game;

namespace Clocktower.Events
{
    internal class EndNight : IGameEvent
    {
        public EndNight(Grimoire grimoire)
        {
            this.grimoire = grimoire;
        }

        public Task RunEvent()
        {
            // Clear expired tokens.
            foreach (var player in grimoire.Players)
            {
                player.Tokens.Remove(Token.Executed);
                player.Tokens.Remove(Token.PhilosopherUsedAbilityTonight);
            }

            return Task.CompletedTask;
        }

        private readonly Grimoire grimoire;
    }
}
