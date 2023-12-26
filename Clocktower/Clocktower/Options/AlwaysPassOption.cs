namespace Clocktower.Options
{
    /// <summary>
    /// An option to always pass for this choice, e.g. if you never want to bluff the ability for a particular character.
    /// </summary>
    internal class AlwaysPassOption : IOption
    {
        public string Name => "ALWAYS PASS";
    }
}
