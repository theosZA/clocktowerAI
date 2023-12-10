using Clocktower.Game;

namespace Clocktower.Options
{
    internal class CharacterForTwoPlayersOption : IOption
    {
        public string Name => $"{TextUtilities.CharacterToText(Character)} for {PlayerA.Name} and {PlayerB.Name}";

        public Character Character { get; private set; }
        public Player PlayerA { get; private set; }
        public Player PlayerB { get; private set; }

        public CharacterForTwoPlayersOption(Character character, Player playerA, Player playerB)
        {
            Character = character;
            PlayerA = playerA;
            PlayerB = playerB;
        }
    }
}
