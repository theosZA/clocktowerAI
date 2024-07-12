using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Options;

namespace ClocktowerScenarioTests.Mocks
{
    internal static class AgentMocks
    {
        public static void MockNomination(this IAgent agent, Character nominee)
        {
            agent.GetNomination(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsOptionForCharacterFromArg(nominee);
        }

        public static void MockImp(this IAgent agent, Character target)
        {
            agent.RequestChoiceFromImp(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsOptionForCharacterFromArg(target);
        }

        public static void MockImp(this IAgent agent, IReadOnlyCollection<Character> targets)
        {
            int targetIndex = 0;
            agent.RequestChoiceFromImp(Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args =>
                {
                    var target = targets.ElementAt(targetIndex++);
                    return args.GetOptionForCharacterFromArg(target);
                });
        }

        public static void MockAssassin(this IAgent agent, Character? target)
        {
            if (target == null)
            {
                agent.RequestChoiceFromAssassin(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsPassOptionFromArg();
            }
            else
            {
                agent.RequestChoiceFromAssassin(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsOptionForCharacterFromArg(target.Value);
            }
        }

        public static void MockGodfather(this IAgent agent, Character target)
        {
            agent.RequestChoiceFromGodfather(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsOptionForCharacterFromArg(target);
        }

        public static Wrapper<Character> MockNotifySteward(this IAgent agent, ClocktowerGame? gameToEnd = null)
        {
            Wrapper<Character> receivedStewardPing = new();
            agent.When(agent => agent.NotifySteward(Arg.Any<Player>()))
                    .Do(args => args.PopulateFromArg(receivedStewardPing, gameToEnd: gameToEnd));
            return receivedStewardPing;
        }

        public static Wrapper<(Character playerA, Character playerB, Character seenCharacter)> MockNotifyInvestigator(this IAgent agent, ClocktowerGame? gameToEnd = null)
        {
            Wrapper<(Character playerA, Character playerB, Character seenCharacter)> receivedInvestigatorPing = new();
            agent.When(agent => agent.NotifyInvestigator(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<Character>()))
                .Do(args => args.PopulateFromArgs(receivedInvestigatorPing, gameToEnd));
            return receivedInvestigatorPing;
        }

        public static Wrapper<(Character playerA, Character playerB, Character seenCharacter)> MockNotifyLibrarian(this IAgent agent, ClocktowerGame? gameToEnd = null)
        {
            Wrapper<(Character playerA, Character playerB, Character seenCharacter)> receivedLibrarianPing = new();
            agent.When(agent => agent.NotifyLibrarian(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<Character>()))
                .Do(args => args.PopulateFromArgs(receivedLibrarianPing, gameToEnd));
            return receivedLibrarianPing;
        }

        public static Wrapper<(Character playerA, Character playerB, Character seenCharacter)> MockNotifyWasherwoman(this IAgent agent, ClocktowerGame? gameToEnd = null)
        {
            Wrapper<(Character playerA, Character playerB, Character seenCharacter)> receivedWasherwomanPing = new();
            agent.When(agent => agent.NotifyWasherwoman(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<Character>()))
                .Do(args => args.PopulateFromArgs(receivedWasherwomanPing, gameToEnd));
            return receivedWasherwomanPing;
        }

        public static Wrapper<Direction> MockNotifyShugenja(this IAgent agent, ClocktowerGame? gameToEnd = null)
        {
            Wrapper<Direction> receivedShugenjaDirection = new();
            agent.When(agent => agent.NotifyShugenja(Arg.Any<Direction>()))
                .Do(args => args.PopulateFromArg(receivedShugenjaDirection, gameToEnd: gameToEnd));
            return receivedShugenjaDirection;
        }

        public static Wrapper<int> MockNotifyEmpath(this IAgent agent, ClocktowerGame? gameToEnd = null)
        {
            Wrapper<int> receivedEmpathNumber = new();
            agent.When(agent => agent.NotifyEmpath(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<int>()))
                .Do(args => args.PopulateFromArg(receivedEmpathNumber, argIndex: 2, gameToEnd: gameToEnd));
            return receivedEmpathNumber;
        }

        public static Wrapper<int> MockNotifyChef(this IAgent agent, ClocktowerGame? gameToEnd = null)
        {
            Wrapper<int> receivedChefNumber = new();
            agent.When(agent => agent.NotifyChef(Arg.Any<int>()))
                .Do(args => args.PopulateFromArg(receivedChefNumber, gameToEnd: gameToEnd));
            return receivedChefNumber;
        }

        public static List<Character> MockRavenkeeperChoice(this IAgent agent, Character choice)
        {
            List<Character> ravenkeeperOptions = new();
            agent.RequestChoiceFromRavenkeeper(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsMatchingOptionFromOptionsArg(choice, ravenkeeperOptions);
            return ravenkeeperOptions;
        }

        public static List<Character> MockMonkChoice(this IAgent agent, Character choice)
        {
            List<Character> monkOptions = new();
            agent.RequestChoiceFromMonk(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsMatchingOptionFromOptionsArg(choice, monkOptions);
            return monkOptions;
        }
    }
}
