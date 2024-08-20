using Clocktower.Agent.Observer;
using Clocktower.Game;

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
            await observers.StartRollCall(grimoire.Players.Count(player => player.Alive));

            // Give everyone, in grimoire order, a chance to make a public statement about their character and information.
            foreach (var player in grimoire.Players)
            {
                var statement = await player.Agent.GetRollCallStatement();
                if (!string.IsNullOrEmpty(statement))
                {
                    await observers.PublicStatement(player, statement);
                }
            }
        }

        private readonly Grimoire grimoire;
        private readonly IGameObserver observers;
    }
}
