namespace Clocktower.Triggers
{
    /// <summary>
    /// Interface for effects that need to happen when a player dies.
    /// </summary>
    internal interface IDeathTrigger
    {
        Task RunTrigger(DeathInformation deathInformation);
    }
}
