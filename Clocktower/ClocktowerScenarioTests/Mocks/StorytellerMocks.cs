using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Storyteller;

namespace ClocktowerScenarioTests.Mocks
{
    internal static class StorytellerMocks
    {
        public static List<Character> MockGetStewardPing(this IStoryteller storyteller, Character stewardPing)
        {
            List<Character> stewardPingOptions = new();
            storyteller.GetStewardPing(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args =>
                {
                    var options = args.ArgAt<IReadOnlyCollection<IOption>>(1).ToList();
                    stewardPingOptions.AddRange(options.Select(option => option.ToCharacter()));
                    return options.First(option => option.ToCharacter() == stewardPing);
                });
            return stewardPingOptions;
        }

        public static List<(Character playerA, Character playerB, Character character)> MockGetInvestigatorPing(this IStoryteller storyteller, Character investigatorPing, Character investigatorWrong, Character asCharacter)
        {
            List<(Character playerA, Character playerB, Character character)> investigatorPingOptions = new();
            storyteller.GetInvestigatorPings(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args =>
                {
                    var options = args.ArgAt<IReadOnlyCollection<IOption>>(1).ToList();
                    investigatorPingOptions.AddRange(options.Select(option => option.ToCharacterForTwoPlayers()));
                    return options.First(option => option.ToCharacterForTwoPlayers() == (investigatorPing, investigatorWrong, asCharacter));
                });
            return investigatorPingOptions;
        }

        public static List<(Character playerA, Character playerB, Character character)?> MockGetLibrarianPing(this IStoryteller storyteller, Character librarianPing, Character librarianWrong, Character asCharacter)
        {
            List<(Character playerA, Character playerB, Character character)?> librarianPingOptions = new();
            storyteller.GetLibrarianPings(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args =>
                {
                    var options = args.ArgAt<IReadOnlyCollection<IOption>>(1).ToList();
                    librarianPingOptions.AddRange(options.Select(option => option.ToOptionalCharacterForTwoPlayers()));
                    return options.First(option => option.ToCharacterForTwoPlayers() == (librarianPing, librarianWrong, asCharacter));
                });
            return librarianPingOptions;
        }

        public static List<(Character playerA, Character playerB, Character character)> MockGetWasherwomanPing(this IStoryteller storyteller, Character washerwomanPing, Character washerwomanWrong, Character asCharacter)
        {
            List<(Character playerA, Character playerB, Character character)> washerwomanPingOptions = new();
            storyteller.GetWasherwomanPings(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args =>
                {
                    var options = args.ArgAt<IReadOnlyCollection<IOption>>(1).ToList();
                    washerwomanPingOptions.AddRange(options.Select(option => option.ToCharacterForTwoPlayers()));
                    return options.First(option => option.ToCharacterForTwoPlayers() == (washerwomanPing, washerwomanWrong, asCharacter));
                });
            return washerwomanPingOptions;
        }

        public static List<Direction> MockGetShugenjaDirection(this IStoryteller storyteller, Direction direction)
        {
            List<Direction> directionOptions = new();
            storyteller.GetShugenjaDirection(Arg.Any<Player>(), Arg.Any<Grimoire>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args =>
                {
                    var options = args.ArgAt<IReadOnlyCollection<IOption>>(2).ToList();
                    directionOptions.AddRange(options.Select(option => ((DirectionOption)option).Direction));
                    return options.First(option => ((DirectionOption)option).Direction ==  direction);
                });
            return directionOptions;
        }

        public static List<int> MockGetEmpathNumbers(this IStoryteller storyteller, int empathNumber)
        {
            List<int> empathNumbers = new();
            storyteller.GetEmpathNumber(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args =>
                {
                    var options = args.ArgAt<IReadOnlyCollection<IOption>>(3).ToList();
                    empathNumbers.AddRange(options.Select(option => ((NumberOption)option).Number));
                    return options.First(option => ((NumberOption)option).Number == empathNumber);
                });
            return empathNumbers;
        }
    }
}
