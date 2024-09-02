using Clocktower.Game;

namespace Clocktower.Options
{
    internal static class OptionsReader
    {
        public static Player GetPlayer(this IOption option)
        {
            return ((PlayerOption)option).Player;
        }

        public static IEnumerable<Player> GetPlayers(this IOption option)
        {
            return ((PlayerListOption)option).Players;
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

        public static Direction GetDirection(this IOption option)
        {
            return ((DirectionOption)option).Direction;
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

        public static IEnumerable<Player> GetThreePlayers(this IOption option)
        {
            var threePlayersOption = (ThreePlayersOption)option;
            yield return threePlayersOption.PlayerA;
            yield return threePlayersOption.PlayerB;
            yield return threePlayersOption.PlayerC;
        }

        public static (Player? target, bool alwaysPass) GetSlayerTargetOptional(this IOption option)
        {
            return option switch
            {
                SlayerShotOption slayerShotOption => (slayerShotOption.Target, false),
                AlwaysPassOption _ => (null, true),
                _ => (null, false)
            };
        }

        public static IEnumerable<Player> GetPlayers(this IEnumerable<IOption> options)
        {
            return options.Select(option => option.GetPlayerOptional())
                          .Where(player => player != null)
                          .Select(player => player!);
        }
    }
}
