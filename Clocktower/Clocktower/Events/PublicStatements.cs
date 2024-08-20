using Clocktower.Agent.Observer;
using Clocktower.Game;

namespace Clocktower.Events
{
    internal class PublicStatements : IGameEvent
    {
        public int StatementsCount { get; private set; } = 0;

        public bool OnlyAlivePlayers { get; set; } = false;

        public PublicStatements(Grimoire grimoire, IGameObserver observers, Random random, bool morning)
        {
            this.grimoire = grimoire;
            this.observers = observers;
            this.random = random;
            this.morning = morning;
        }

        public async Task RunEvent()
        {
            // Give everyone, in random order, a chance to make a public statement.
            var players = GetPlayersWhoCanMakePublicStatements();
            players.Shuffle(random);
            foreach (var player in players)
            {
                var statement = morning ? await player.Agent.GetMorningPublicStatement() : await player.Agent.GetEveningPublicStatement();
                if (!string.IsNullOrEmpty(statement))
                {
                    await observers.PublicStatement(player, statement);
                    StatementsCount++;
                }
            }
        }

        public IList<Player> GetPlayersWhoCanMakePublicStatements()
        {
            if (OnlyAlivePlayers)
            {
                return grimoire.Players.Where(player => player.Alive).ToList();
            }
            return grimoire.Players.ToList();
        }

        private readonly Grimoire grimoire;
        private readonly IGameObserver observers;
        private readonly Random random;
        private readonly bool morning;
    }
}
