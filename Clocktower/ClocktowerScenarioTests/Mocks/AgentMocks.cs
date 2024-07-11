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
                    .Do(args =>
                        {
                            receivedStewardPing.Value = args.ArgAt<Player>(0).Character;
                            gameToEnd?.EndGame(Alignment.Good);
                        });
            return receivedStewardPing;
        }

        public static Wrapper<(Character playerA, Character playerB, Character seenCharacter)> MockNotifyInvestigator(this IAgent agent, ClocktowerGame? gameToEnd = null)
        {
            Wrapper<(Character playerA, Character playerB, Character seenCharacter)> receivedInvestigatorPing = new();
            agent.When(agent => agent.NotifyInvestigator(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<Character>()))
                .Do(args =>
                    {
                        receivedInvestigatorPing.Value = (args.ArgAt<Player>(0).Character, args.ArgAt<Player>(1).Character, args.ArgAt<Character>(2));
                        gameToEnd?.EndGame(Alignment.Good);
                    });
            return receivedInvestigatorPing;
        }

        public static Wrapper<(Character playerA, Character playerB, Character seenCharacter)> MockNotifyLibrarian(this IAgent agent, ClocktowerGame? gameToEnd = null)
        {
            Wrapper<(Character playerA, Character playerB, Character seenCharacter)> receivedLibrarianPing = new();
            agent.When(agent => agent.NotifyLibrarian(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<Character>()))
                .Do(args =>
                {
                    receivedLibrarianPing.Value = (args.ArgAt<Player>(0).Character, args.ArgAt<Player>(1).Character, args.ArgAt<Character>(2));
                    gameToEnd?.EndGame(Alignment.Good);
                });
            return receivedLibrarianPing;
        }

        public static Wrapper<(Character playerA, Character playerB, Character seenCharacter)> MockNotifyWasherwoman(this IAgent agent, ClocktowerGame? gameToEnd = null)
        {
            Wrapper<(Character playerA, Character playerB, Character seenCharacter)> receivedWasherwomanPing = new();
            agent.When(agent => agent.NotifyWasherwoman(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<Character>()))
                .Do(args =>
                {
                    receivedWasherwomanPing.Value = (args.ArgAt<Player>(0).Character, args.ArgAt<Player>(1).Character, args.ArgAt<Character>(2));
                    gameToEnd?.EndGame(Alignment.Good);
                });
            return receivedWasherwomanPing;
        }

        public static Wrapper<Direction> MockNotifyShugenja(this IAgent agent, ClocktowerGame? gameToEnd = null)
        {
            Wrapper<Direction> receivedShugenjaDirection = new();
            agent.When(agent => agent.NotifyShugenja(Arg.Any<Direction>()))
                .Do(args =>
                {
                    receivedShugenjaDirection.Value = args.ArgAt<Direction>(0);
                    gameToEnd?.EndGame(Alignment.Good);
                });
            return receivedShugenjaDirection;
        }

        public static Wrapper<int> MockNotifyEmpath(this IAgent agent, ClocktowerGame? gameToEnd = null)
        {
            Wrapper<int> receivedEmpathNumber = new();
            agent.When(agent => agent.NotifyEmpath(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<int>()))
                .Do(args =>
                {
                    receivedEmpathNumber.Value = args.ArgAt<int>(2);
                    gameToEnd?.EndGame(Alignment.Good);
                });
            return receivedEmpathNumber;
        }
    }
}
