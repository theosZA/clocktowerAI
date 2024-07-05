using Clocktower.Game;

namespace Clocktower.Events
{
    /// <summary>
    /// Wraps a number of events to be called in sequence.
    /// </summary>
    internal class SequenceEvent : IGameEvent
    {
        public SequenceEvent(Grimoire grimoire, IEnumerable<IGameEvent> events)
        {
            this.grimoire = grimoire;
            this.events = events;
        }

        public async Task RunEvent()
        {
            foreach (var gameEvent in events)
            {
                await gameEvent.RunEvent();
                // Early return if the game is over.
                if (grimoire.Finished)
                {
                    return;
                }
                // Early return if the phase is over.
                if (grimoire.PhaseShouldEndImmediately)
                {
                    grimoire.PhaseShouldEndImmediately = false;
                    return;
                }
            }
        }

        private readonly Grimoire grimoire;
        private readonly IEnumerable<IGameEvent> events;
    }
}
