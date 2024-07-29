using Clocktower.Game;

namespace Clocktower.Options
{
    internal class ThreePlayersOption : IOption
    {
        public string Name => $"{PlayerA.Name}, {PlayerB.Name} and {PlayerC.Name}";

        public Player PlayerA { get; private set; }
        public Player PlayerB { get; private set; }
        public Player PlayerC { get; private set; }

        public ThreePlayersOption(Player playerA, Player playerB, Player playerC)
        {
            PlayerA = playerA;
            PlayerB = playerB;
            PlayerC = playerC;
        }
    }
}
