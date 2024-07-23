using Clocktower.Game;

namespace Clocktower.Options
{
    /// <summary>
    /// Represents claiming Slayer and shooting a player. Of course this will have no effect if the player is not
    /// the Slayer, but it needs to be available to everyone so that they can bluff the character.
    /// </summary>
    internal class SlayerShotOption : IOption
    {
        public string Name => $"Slayer: {Target.Name}";

        public Player Target { get; private set; }

        public SlayerShotOption(Player target)
        {
            Target = target;
        }
    }
}
