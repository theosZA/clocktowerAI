using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Storyteller;
using NSubstitute;

namespace ClocktowerScenarioTests
{
    internal class ClocktowerGameBuilder
    {
        public IGameSetup Setup { get; } = Substitute.For<IGameSetup>();

        public IStoryteller Storyteller { get; } = Substitute.For<IStoryteller>();

        public List<IAgent> Agents { get; }

        public ClocktowerGameBuilder(int playerCount)
        {
            Agents = CreateAgents(playerCount).ToList();

            Setup.PlayerCount.Returns(playerCount);
            Setup.Script.Returns(Enum.GetValues<Character>());

            Storyteller.GetDemonBluffs(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>()).Returns(args => args.ArgAt<IReadOnlyCollection<IOption>>(1).First());
        }

        public ClocktowerGame Build()
        {
            return new ClocktowerGame(Setup, Storyteller, Agents, new Random());
        }

        public static (ClocktowerGameBuilder, ClocktowerGame) BuildDefault(string charactersCommaSeparatedList)
        {
            return BuildDefault(charactersCommaSeparatedList.Split(',')
                                                            .Select(characterText => Enum.Parse<Character>(characterText))
                                                            .ToList());
        }

        public static (ClocktowerGameBuilder, ClocktowerGame) BuildDefault(IReadOnlyCollection<Character> characters)
        {
            var setup = new ClocktowerGameBuilder(playerCount: characters.Count);
            var game = setup.WithDefaultAgents()
                            .WithCharacters(characters)
                            .Build();
            return (setup, game);
        }

        public ClocktowerGameBuilder WithDefaultAgents()
        {
            foreach (var agent in Agents)
            {
                agent.OfferPrivateChat(Arg.Any<IReadOnlyCollection<IOption>>()).Returns(args => args.ArgAt<IReadOnlyCollection<IOption>>(0).First());
                agent.GetPrivateChat(Arg.Any<Player>()).Returns((string.Empty, true));

                // By default the agents won't nominate anyone, but if that is overridden in a test with a nomination, the agents will vote for that nomination.
                agent.GetNomination(Arg.Any<IReadOnlyCollection<IOption>>()).Returns(args => args.ArgAt<IReadOnlyCollection<IOption>>(0).First(option => option is PassOption));
                agent.GetVote(Arg.Any<IReadOnlyCollection<IOption>>(), Arg.Any<bool>()).Returns(args => args.ArgAt<IReadOnlyCollection<IOption>>(0).First(option => option is VoteOption));
            }

            return this;
        }

        public ClocktowerGameBuilder WithCharacters(IReadOnlyCollection<Character> characters)
        {
            Setup.Characters.Returns(characters.ToArray());
            return this;
        }

        private static IEnumerable<IAgent> CreateAgents(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return Substitute.For<IAgent>();
            }
        }
    }
}
