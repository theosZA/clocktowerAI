namespace Clocktower.Options
{
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
