using Clocktower.Game;

namespace Clocktower.Options
{
    internal class ThreeCharactersOption : IOption
    {
        public string Name => $"{TextUtilities.CharacterToText(CharacterA)}, {TextUtilities.CharacterToText(CharacterB)} and {TextUtilities.CharacterToText(CharacterC)}";

        public Character CharacterA { get; private set; }
        public Character CharacterB { get; private set; }
        public Character CharacterC { get; private set; }

        public ThreeCharactersOption(Character characterA, Character characterB, Character characterC)
        {
            CharacterA = characterA;
            CharacterB = characterB;
            CharacterC = characterC;
        }
    }
}
