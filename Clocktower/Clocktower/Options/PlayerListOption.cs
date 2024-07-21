using Clocktower.Game;

namespace Clocktower.Options
{
    internal class PlayerListOption : IOption
    {
        public string Name => Players.Any() ? string.Join('/', Players.Select(player => player.Name)) : "(none)";

        public IReadOnlyCollection<Player> Players { get; private set; }

        public PlayerListOption(IEnumerable<Player> players)
        {
            Players = players.ToList();
        }
    }
}
