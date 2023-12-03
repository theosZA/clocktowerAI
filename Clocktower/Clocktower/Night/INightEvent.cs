namespace Clocktower.Night
{
    /// <summary>
    /// The interface defining an event that happens at night.
    /// Many events require asynchronous input from player or storyteller, so they should invoke their EventFinished handler when done.
    /// </summary>
    internal interface INightEvent
    {
        public void RunEvent(Action onEventFinished);
    }
}
