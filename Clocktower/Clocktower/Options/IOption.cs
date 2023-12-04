namespace Clocktower.Options
{
    /// <summary>
    /// Represents an option that a player (or storyteller) can choose for a given event.
    /// The agent will be given a collection of options and must choose exactly one.
    /// </summary>
    public interface IOption
    {
        string Name { get; }
    }
}
