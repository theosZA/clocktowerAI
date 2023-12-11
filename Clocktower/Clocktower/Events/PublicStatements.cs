using Clocktower.Game;
using Clocktower.Observer;

namespace Clocktower.Events
{
    internal class PublicStatements : IGameEvent
    {
        public PublicStatements(Grimoire grimoire, ObserverCollection observers, Random random, bool morning)
        {
            this.grimoire = grimoire;
            this.observers = observers;
            this.random = random;
            this.morning = morning;
        }

        public async Task RunEvent()
        {
            // Give everyone, in random order, a chance to make a public statement.
            var players = grimoire.Players.ToList();
            players.Shuffle(random);
            foreach (var player in grimoire.Players)
            {
                var statement = morning ? await player.Agent.GetMorningPublicStatement() : await player.Agent.GetEveningPublicStatement();
                if (!string.IsNullOrEmpty(statement))
                {
                    observers.PublicStatement(player, statement);
                }
            }
        }

        private readonly Grimoire grimoire;
        private readonly ObserverCollection observers;
        private readonly Random random;
        private readonly bool morning;
    }
}
