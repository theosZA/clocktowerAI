namespace Clocktower.Events
{
    /// <summary>
    /// The interface defining an event that happens.
    /// </summary>
    internal interface IGameEvent
    {
        public Task RunEvent();
    }
}
