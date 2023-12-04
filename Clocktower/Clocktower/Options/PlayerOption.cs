using Clocktower.Game;

namespace Clocktower.Options
{
    internal class PlayerOption : IOption
    {
        public string Name => Player.Name;

        public Player Player { get; private set; }

        public PlayerOption(Player player)
        {
            Player = player;
        }
    }
}
