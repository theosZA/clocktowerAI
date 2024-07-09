using Clocktower.Game;
using Clocktower.Options;

namespace ClocktowerScenarioTests
{
    internal static class OptionExtensions
    {

        public static Character ToCharacter(this IOption option)
        {
            return option is PlayerOption playerOption ? playerOption.Player.Character : ((CharacterOption)option).Character;
        }

        public static (Character playerA, Character playerB, Character character) ToCharacterForTwoPlayers(this IOption option)
        {
            var current = (CharacterForTwoPlayersOption)option;
            return (current.PlayerA.Character, current.PlayerB.Character, current.Character);
        }
    }
}
