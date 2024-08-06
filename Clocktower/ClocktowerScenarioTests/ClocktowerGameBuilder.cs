using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Setup;
using Clocktower.Storyteller;
using ClocktowerScenarioTests.Mocks;

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
            for (int i = 0; i < Agents.Count; i++)
            {
                agents.Add(Setup.Characters[i], Agents[i]);
            }
            return new ClocktowerGame(Setup, Storyteller, Agents, new Random());
        }

        public static (ClocktowerGameBuilder, ClocktowerGame) BuildDefault(string charactersCommaSeparatedList)
        {
            return BuildDefault(ReadCharactersFromCommaSeparatedList(charactersCommaSeparatedList).ToList());
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
                agent.GetNomination(Arg.Any<IReadOnlyCollection<IOption>>()).Returns(args => args.GetPassOptionFromArg());
                agent.MockVote(voteToExecute: true);
            }

            return this;
        }

        public ClocktowerGameBuilder WithCharacters(string charactersCommaSeparatedList)
        {
            return WithCharacters(ReadCharactersFromCommaSeparatedList(charactersCommaSeparatedList).ToList());
        }

        public ClocktowerGameBuilder WithCharacters(IReadOnlyCollection<Character> characters)
        {
            Setup.Characters.Returns(characters.ToArray());
            return this;
        }

        public ClocktowerGameBuilder WithDrunk(Character character)
        {
            Setup.IsCharacterSelected(Character.Drunk).Returns(true);
            Setup.CanCharacterBeTheDrunk(character).Returns(true);
            Storyteller.MockGetDrunk(character);
            return this;
        }

        public ClocktowerGameBuilder WithMarionette(Character character)
        {
            Setup.IsCharacterSelected(Character.Marionette).Returns(true);
            Storyteller.MockGetMarionette(character);
            return this;
        }

        public IAgent Agent(Character character)
        {
            return agents[character];
        }

        private static IEnumerable<IAgent> CreateAgents(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return Substitute.For<IAgent>();
            }
        }

        private static IEnumerable<Character> ReadCharactersFromCommaSeparatedList(string charactersCommaSeparatedList)
        {
            return charactersCommaSeparatedList.Split(',').Select(characterText => Enum.Parse<Character>(characterText));
        }

        private readonly Dictionary<Character, IAgent> agents = new();
    }
}
