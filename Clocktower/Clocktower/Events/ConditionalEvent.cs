namespace Clocktower.Events
{
    /// <summary>
    /// Wraps another event, but will only run if the provided condition is met.
    /// </summary>
    internal class ConditionalEvent : IGameEvent
    {
        public ConditionalEvent(IGameEvent wrappedEvent, Func<Task<bool>> condition)
        {
            this.wrappedEvent = wrappedEvent;
            this.condition = condition;
        }

        public async Task RunEvent()
        {
            if (await condition())
            {
                await wrappedEvent.RunEvent();
            }
        }

        private readonly IGameEvent wrappedEvent;
        private readonly Func<Task<bool>> condition;
    }
}
