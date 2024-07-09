using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Storyteller;
using NSubstitute;

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
                                    stewardPingOptions.AddRange(options.Select(option => option.GetPlayer().Character));
                                    return options.First(option => option.GetPlayer().Character == stewardPing);
                                });
            return stewardPingOptions;
        }
    }
}
