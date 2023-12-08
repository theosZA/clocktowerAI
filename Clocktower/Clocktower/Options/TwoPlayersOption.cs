using Clocktower.Game;

namespace Clocktower.Options
{
    internal class TwoPlayersOption : IOption
    {
        public string Name => $"{PlayerA.Name} and {PlayerB.Name}";

        public Player PlayerA { get; private set; }
        public Player PlayerB { get; private set; }

        public TwoPlayersOption(Player playerA, Player playerB)
        {
            PlayerA = playerA;
            PlayerB = playerB;
        }
    }
}
