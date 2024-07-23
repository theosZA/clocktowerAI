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

        public static IReadOnlyCollection<IOption> ToAllPossibleSubsetsAsOptions(this IEnumerable<Player> players)
        {
            return SubsetsOf(players).Select(playerSet => playerSet.ToList())
                                     .OrderBy(playerList => playerList.Count())
                                     .Select(playerList => new PlayerListOption(playerList))
                                     .ToList();
        }

        private static IEnumerable<IEnumerable<T>> SubsetsOf<T>(IEnumerable<T> source)
        {
            if (!source.Any())
            {
                return Enumerable.Repeat(Array.Empty<T>(), 1);
            }

            var element = source.Take(1);

            var haveNots = SubsetsOf(source.Skip(1));
            var haves = haveNots.Select(set => element.Concat(set));

            return haves.Concat(haveNots);
        }

        private static readonly IReadOnlyCollection<IOption> yesOrNo = new IOption[]
        {
            new NoOption(),
            new YesOption(),
        };

        private static readonly IReadOnlyCollection<IOption> directionOptions = new IOption[]
        {
            new DirectionOption(Direction.Clockwise),
            new DirectionOption(Direction.Counterclockwise)
        };
    }
}
