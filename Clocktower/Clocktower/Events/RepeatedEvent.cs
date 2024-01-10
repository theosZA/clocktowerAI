namespace Clocktower.Events
{
    /// <summary>
    /// Wraps another event that will run repeatedly as long as the provided condition remains true.
    /// </summary>
    internal class RepeatedEvent : IGameEvent
    {
        public int IterationsRun { get; private set; } = 0;

        public RepeatedEvent(IGameEvent wrappedEvent, Func<int, bool> condition)
        {
            this.wrappedEvent = wrappedEvent;
            this.condition = condition;
        }

        public async Task RunEvent()
        {
            for (IterationsRun = 0; condition(IterationsRun); ++IterationsRun)
            {
                await wrappedEvent.RunEvent();
            }
        }

        private readonly IGameEvent wrappedEvent;
        private readonly Func<int, bool> condition;
    }
}
