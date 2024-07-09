using Clocktower.Agent;
using Clocktower.Game;

namespace ClocktowerScenarioTests.Mocks
{
    internal static class AgentMocks
    {
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
    }
}
