using Clocktower.Game;
using Clocktower.Observer;

namespace Clocktower.Events
{
    internal class RollCall : IGameEvent
    {
        public RollCall(Grimoire grimoire, IGameObserver observers)
        {
            this.grimoire = grimoire;
            this.observers = observers;
        }

        public async Task RunEvent()
        {
            int playersAlive = grimoire.Players.Count(player => player.Alive);
            if (playersAlive > 4)
            {
                return;
            }
            observers.StartRollCall(playersAlive);

            // Give everyone, in grimoire order, a chance to make a public statement about their character and information.
            foreach (var player in grimoire.Players)
            {
                var statement = await player.Agent.GetRollCallStatement();
                if (!string.IsNullOrEmpty(statement))
                {
                    observers.PublicStatement(player, statement);
                }
            }
        }

        private readonly Grimoire grimoire;
        private readonly IGameObserver observers;
    }
}
