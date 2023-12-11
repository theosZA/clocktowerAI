namespace Clocktower.Events
{
    /// <summary>
    /// The interface defining an event that happens.
    /// </summary>
    public interface IGameEvent
    {
        public Task RunEvent();
    }
}
