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
    }
}
