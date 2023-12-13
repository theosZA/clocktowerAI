using Clocktower.Observer;

namespace Clocktower.Events
{
    internal class StartDay : IGameEvent
    {
        public StartDay(IGameObserver observers, int dayNumber)
        {
            this.observers = observers;
            this.dayNumber = dayNumber;
        }

        public Task RunEvent()
        {
            observers.Day(dayNumber);

            return Task.CompletedTask;
        }

        private readonly IGameObserver observers;
        private readonly int dayNumber;
    }
}
