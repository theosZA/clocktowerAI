namespace Clocktower.Options
{
    /// <summary>
    /// Represents voting in favour of the current nomination.
    /// </summary>
    internal class NumberOption : IOption
    {
        public string Name => $"{Number}";

        public int Number { get; private set; }

        public NumberOption(int number)
        {
            Number = number;
        }
    }
}
