using Clocktower.Game;

namespace Clocktower.Options
{
    internal class CharacterOption : IOption
    {
        public string Name => TextUtilities.CharacterToText(Character);

        public Character Character { get; private set; }

        public CharacterOption(Character character)
        {
            Character = character;
        }
    }
}
