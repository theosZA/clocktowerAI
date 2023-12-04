namespace Clocktower.Events
{
    /// <summary>
    /// The interface defining an event that happens.
    /// Many events require asynchronous input from player or storyteller, so they should invoke their onEventFinished callback when done.
    /// </summary>
    internal interface IGameEvent
    {
        public void RunEvent(Action onEventFinished);
    }
}
