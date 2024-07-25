using Clocktower.Game;
using Clocktower.Options;

namespace ClocktowerScenarioTests.Mocks
{
    internal static class OptionExtensions
    {
        public static bool IsEquivalentTo<T>(this IOption option, T value)
        {
            var type = typeof(T);
            if (type == typeof(IReadOnlyCollection<Character>))
            {
                var characterSet = value as IReadOnlyCollection<Character> ?? Array.Empty<Character>();
                var optionCharacterSet = option is PlayerListOption playerListOption ? playerListOption.GetPlayers().Select(player => player.Character) : Enumerable.Repeat(option.AsType<Character>(), 1);
                var symmetricDifference = characterSet.Except(optionCharacterSet).Union(optionCharacterSet.Except(characterSet));
                return !symmetricDifference.Any();
            }
            return EqualityComparer<T>.Default.Equals(option.AsType<T>(), value);
        }

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
            if (type == typeof((Character playerA, Character playerB)))
            {
                return (T)(object)option.ToTwoPlayers();
            }
            if (type == typeof((Character, Character, Character)))
            {
                return (T)(object)option.ToThreeCharacters();
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

        public static Character? ToOptionalRealCharacter(this IOption option)
        {
            return option is PlayerOption playerOption ? playerOption.Player.RealCharacter
                 : option is CharacterOption characterOption ? characterOption.Character
                 : null;
        }

        public static (Character playerA, Character playerB) ToTwoPlayers(this IOption option)
        {
            var current = (TwoPlayersOption)option;
            return (current.PlayerA.Character, current.PlayerB.Character);
        }

        public static (Character, Character, Character) ToThreeCharacters(this IOption option)
        {
            if (option is ThreeCharactersOption threeCharactersOption)
            {
                return (threeCharactersOption.CharacterA, threeCharactersOption.CharacterB, threeCharactersOption.CharacterC);
            }

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
