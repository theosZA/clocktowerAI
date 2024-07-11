using Clocktower.Game;
using Clocktower.Options;

namespace ClocktowerScenarioTests.Mocks
{
    internal static class OptionExtensions
    {
        public static T AsType<T>(this IOption option)
        {
            var type = typeof(T);
            if (type == typeof(Character))
            {
                return (T)(object)option.ToCharacter();
            }
            if (type == typeof(Character?))
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
                return (T)(object?)option.ToOptionalCharacter();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8603 // Possible null reference return.
            }
            if (type == typeof((Character playerA, Character playerB, Character character)))
            {
                return (T)(object)option.ToCharacterForTwoPlayers();
            }
            if (type == typeof((Character playerA, Character playerB, Character character)?))
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
                return (T)(object?)option.ToOptionalCharacterForTwoPlayers();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8603 // Possible null reference return.
            }
            if (type == typeof(int))
            {
                return (T)(object)option.GetNumber();
            }
            if (type == typeof(Direction))
            {
                return (T)(object)option.GetDirection();
            }
            throw new NotImplementedException($"No conversion from IOption to {typeof(T)} has been implemented");
        }

        public static Character ToCharacter(this IOption option)
        {
            return option is PlayerOption playerOption ? playerOption.Player.Character : ((CharacterOption)option).Character;
        }

        public static Character? ToOptionalCharacter(this IOption option)
        {
            return option is PlayerOption playerOption ? playerOption.Player.Character
                 : option is CharacterOption characterOption ? characterOption.Character
                 : null;
        }

        public static (Character playerA, Character playerB, Character character) ToCharacterForTwoPlayers(this IOption option)
        {
            var current = (CharacterForTwoPlayersOption)option;
            return (current.PlayerA.Character, current.PlayerB.Character, current.Character);
        }

        public static (Character playerA, Character playerB, Character character)? ToOptionalCharacterForTwoPlayers(this IOption option)
        {
            if (option is not CharacterForTwoPlayersOption current)
            {
                return null;
            }
            return (current.PlayerA.Character, current.PlayerB.Character, current.Character);
        }
    }
}
