using Clocktower.Game;

namespace Clocktower.Options
{
    /// <summary>
    /// Represents voting in favour of the current nomination.
    /// </summary>
    internal class VoteOption : IOption
    {
        public string Name => $"Vote to execute {Nominee.Name}";

        public Player Nominee { get; private set; }

        public VoteOption(Player nominee)
        {
            Nominee = nominee;
        }
    }
}
