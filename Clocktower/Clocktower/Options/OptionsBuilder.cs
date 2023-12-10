using Clocktower.Game;

namespace Clocktower.Options
{
    internal static class OptionsBuilder
    {
        public static IReadOnlyCollection<IOption> ToOptions(this IEnumerable<Player> players)
        {
            return players.Select(player => new PlayerOption(player)).ToList();
        }

        public static IReadOnlyCollection<IOption> ToOptions(this IEnumerable<Character> characters)
        {
            return characters.Select(character => new CharacterOption(character)).ToList();
        }

        public static IReadOnlyCollection<IOption> ToOptions(this IEnumerable<int> numbers)
        {
            return numbers.Select(number => new NumberOption(number)).ToList();
        }

        public static IReadOnlyCollection<IOption> YesOrNo => yesOrNo;

        public static IReadOnlyCollection<IOption> DirectionOptions => directionOptions;

        public static IReadOnlyCollection<IOption> ToVoteOptions(this Player player)
        {
            return new IOption[]
            {
                new PassOption(),
                new VoteOption(player)
            };
        }

        public static IReadOnlyCollection<IOption> ToTwoPlayersOptions(this IReadOnlyCollection<Player> players)
        {
            return (from playerA in players
                    from playerB in players
                    where playerA != playerB
                    select new TwoPlayersOption(playerA, playerB)).ToList();
        }

        public static IReadOnlyCollection<IOption> ToThreeCharactersOptions(this IReadOnlyCollection<Character> characters)
        {
            return (from characterA in characters
                    from characterB in characters
                    where characterA != characterB
                    from characterC in characters
                    where characterA != characterC && characterB != characterC
                    select new ThreeCharactersOption(characterA, characterB, characterC)).ToList();
        }

        public static IReadOnlyCollection<IOption> ToSlayerShotOptions(this IEnumerable<Player> players, bool bluff)
        {
            return players.Select(player => new SlayerShotOption(player, bluff)).ToList();
        }

        private static readonly IReadOnlyCollection<IOption> yesOrNo = new IOption[]
        {
            new NoOption(),
            new YesOption(),
        };

        private static readonly IReadOnlyCollection<IOption> directionOptions = new IOption[]
        {
            new ClockwiseOption(),
            new CounterclockwiseOption()
        };
    }
}
