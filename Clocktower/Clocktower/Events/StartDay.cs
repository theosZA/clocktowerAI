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

        public async Task RunEvent()
        {
            await observers.Day(dayNumber);
        }

        private readonly IGameObserver observers;
        private readonly int dayNumber;
    }
}
