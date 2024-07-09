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
    }
}
