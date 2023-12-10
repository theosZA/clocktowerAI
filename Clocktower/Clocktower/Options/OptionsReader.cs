using Clocktower.Game;

namespace Clocktower.Options
{
    internal static class OptionsReader
    {
        public static Player GetPlayer(this IOption option)
        {
            return ((PlayerOption)option).Player;
        }

        public static Player? GetPlayerOptional(this IOption option)
        {
            return option is PlayerOption playerOption ? playerOption.Player : null;
        }

        public static Character GetCharacter(this IOption option)
        {
            return ((CharacterOption)option).Character;
        }

        public static Character? GetCharacterOptional(this IOption option)
        {
            return option is CharacterOption characterOption ? characterOption.Character : null;
        }

        public static int GetNumber(this IOption option)
        {
            return ((NumberOption)option).Number;
        }

        public static (Player playerA, Player playerB) GetTwoPlayers(this IOption option)
        {
            var twoPlayersOption = (TwoPlayersOption)option;
            return (twoPlayersOption.PlayerA, twoPlayersOption.PlayerB);
        }

        public static IEnumerable<Character> GetThreeCharacters(this IOption option)
        {
            var threeCharactersOption = (ThreeCharactersOption)option;
            yield return threeCharactersOption.CharacterA;
            yield return threeCharactersOption.CharacterB;
            yield return threeCharactersOption.CharacterC;
        }

        public static Player? GetSlayerTargetOptional(this IOption option)
        {
            return option is SlayerShotOption slayerShotOption ? slayerShotOption.Target : null;
        }
    }
}
