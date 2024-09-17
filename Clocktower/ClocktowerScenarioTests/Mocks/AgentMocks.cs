using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Selection;

namespace ClocktowerScenarioTests.Mocks
{
    internal static class AgentMocks
    {
        public static void MockNomination(this IAgent agent, Character nominee)
        {
            agent.GetNomination(Arg.Any<Player?>(), Arg.Any<int?>(), Arg.Any<int?>(), Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsOptionForCharacterFromArg(nominee, argIndex: 3);
        }

        public static void MockVote(this IAgent agent, bool voteToExecute)
        {
            agent.GetVote(Arg.Any<IReadOnlyCollection<IOption>>(), Arg.Any<bool>()).Returns(args => voteToExecute ? args.GetVoteOptionFromArg() : args.GetPassOptionFromArg());
        }

        public static void MockDemonKill(this IAgent agent, Character target)
        {
            agent.RequestChoiceFromDemon(Arg.Any<Character>(), Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsOptionForCharacterFromArg(target, argIndex: 1);
        }

        public static void MockDemonKill(this IAgent agent, IReadOnlyCollection<Character> targets)
        {
            int targetIndex = 0;
            agent.RequestChoiceFromDemon(Arg.Any<Character>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args =>
                {
                    var target = targets.ElementAt(targetIndex++);
                    return args.GetOptionForCharacterFromArg(target, argIndex: 1);
                });
        }

        public static void MockOjo(this IAgent agent, Character target)
        {
            agent.RequestChoiceFromOjo(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsOptionForCharacterFromArg(target);
        }

        public static Wrapper<int> MockKazaliMinionChoice(this IAgent agent, IReadOnlyCollection<(Character currentCharacter, Character minionCharacter)> assignment)
        {
            Wrapper<int> minionCount = new();
            agent.When(agent => agent.RequestSelectionOfKazaliMinions(Arg.Any<KazaliMinionsSelection>()))
                .Do(args =>
                {
                    var kazaliMinionsSelection = args.ArgAt<KazaliMinionsSelection>(0);
                    minionCount.Value = kazaliMinionsSelection.MinionCount;
                    var minions = assignment.Select(assignment => (kazaliMinionsSelection.PossiblePlayers.First(player => player.RealCharacter == assignment.currentCharacter), assignment.minionCharacter)).ToList();
                    kazaliMinionsSelection.SelectMinions(minions);
                });
            return minionCount;
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

        public static void MockPoisoner(this IAgent agent, Character target)
        {
            agent.RequestChoiceFromPoisoner(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsOptionForCharacterFromArg(target);
        }

        public static void MockWidow(this IAgent agent, Character target)
        {
            agent.RequestChoiceFromWidow(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsOptionForCharacterFromArg(target);
        }

        public static void MockWitch(this IAgent agent, Character target)
        {
            agent.RequestChoiceFromWitch(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsOptionForCharacterFromArg(target);
        }

        public static void MockDevilsAdvocate(this IAgent agent, Character target)
        {
            agent.RequestChoiceFromDevilsAdvocate(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsOptionForCharacterFromArg(target);
        }

        public static void MockPhilosopher(this IAgent agent, Character characterAbility)
        {
            agent.RequestChoiceFromPhilosopher(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsOptionForCharacterFromArg(characterAbility);
        }

        public static Wrapper<Character> MockNotifySteward(this IAgent agent, ClocktowerGame? gameToEnd = null)
        {
            Wrapper<Character> receivedStewardPing = new();
            agent.When(agent => agent.NotifySteward(Arg.Any<Player>()))
                    .Do(args => args.PopulateFromArg(receivedStewardPing, gameToEnd: gameToEnd));
            return receivedStewardPing;
        }

        public static List<Character> MockNotifyNoble(this IAgent agent, ClocktowerGame? gameToEnd = null)
        {
            List<Character> noblePings = new();
            agent.When(agent => agent.NotifyNoble(Arg.Any<IReadOnlyCollection<Player>>()))
                    .Do(args => 
                    {
                        noblePings.AddRange(args.ArgAt<IReadOnlyCollection<Player>>(0).Select(player => player.Character));
                        gameToEnd?.EndGame(Alignment.Good);
                    });
            return noblePings;
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

        public static Wrapper<int> MockNotifyOracle(this IAgent agent, ClocktowerGame? gameToEnd = null)
        {
            Wrapper<int> receivedOracleNumber = new();
            agent.When(agent => agent.NotifyOracle(Arg.Any<int>()))
                .Do(args => args.PopulateFromArg(receivedOracleNumber, gameToEnd: gameToEnd));
            return receivedOracleNumber;
        }

        public static Wrapper<Character> MockNotifyBalloonist(this IAgent agent, ClocktowerGame? gameToEnd = null)
        {
            Wrapper<Character> receivedBalloonistPlayer = new();
            agent.When(agent => agent.NotifyBalloonist(Arg.Any<Player>()))
                .Do(args => args.PopulateFromArg(receivedBalloonistPlayer, gameToEnd: gameToEnd));
            return receivedBalloonistPlayer;
        }

        public static Wrapper<Character> MockNotifyUndertaker(this IAgent agent, ClocktowerGame? gameToEnd = null)
        {
            Wrapper<Character> receivedCharacter = new();
            agent.When(agent => agent.NotifyUndertaker(Arg.Any<Player>(), Arg.Any<Character>()))
                .Do(args => args.PopulateFromArg(receivedCharacter, argIndex: 1, gameToEnd: gameToEnd));
            return receivedCharacter;
        }

        public static Wrapper<int> MockNotifyChef(this IAgent agent, ClocktowerGame? gameToEnd = null)
        {
            Wrapper<int> receivedChefNumber = new();
            agent.When(agent => agent.NotifyChef(Arg.Any<int>()))
                .Do(args => args.PopulateFromArg(receivedChefNumber, gameToEnd: gameToEnd));
            return receivedChefNumber;
        }

        public static Wrapper<bool> MockNotifyFortuneTeller(this IAgent agent, ClocktowerGame? gameToEnd = null)
        {
            Wrapper<bool> receivedFortuneTellerResult = new();
            agent.When(agent => agent.NotifyFortuneTeller(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<bool>()))
                .Do(args => args.PopulateFromArg(receivedFortuneTellerResult, argIndex: 2, gameToEnd: gameToEnd));
            return receivedFortuneTellerResult;
        }

        public static List<(Character, Character)> MockFortuneTellerChoice(this IAgent agent, Character choiceA, Character choiceB)
        {
            List<(Character, Character)> fortuneTellerOptions = new();
            agent.RequestChoiceFromFortuneTeller(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsMatchingOptionFromOptionsArg((choiceA, choiceB), fortuneTellerOptions);
            return fortuneTellerOptions;
        }

        public static List<Character> MockRavenkeeperChoice(this IAgent agent, Character choice)
        {
            List<Character> ravenkeeperOptions = new();
            agent.RequestChoiceFromRavenkeeper(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsMatchingOptionFromOptionsArg(choice, ravenkeeperOptions);
            return ravenkeeperOptions;
        }

        public static List<Character> MockSnakeCharmerChoice(this IAgent agent, Character choice)
        {
            List<Character> snakeCharmerOptions = new();
            agent.RequestChoiceFromSnakeCharmer(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsMatchingOptionFromOptionsArg(choice, snakeCharmerOptions);
            return snakeCharmerOptions;
        }

        public static List<Character> MockMonkChoice(this IAgent agent, Character choice)
        {
            List<Character> monkOptions = new();
            agent.RequestChoiceFromMonk(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsMatchingOptionFromOptionsArg(choice, monkOptions);
            return monkOptions;
        }

        public static List<Character> MockButlerChoice(this IAgent agent, Character master)
        {
            List<Character> masterOptions = new();
            agent.RequestChoiceFromButler(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsMatchingOptionFromOptionsArg(master, masterOptions);
            return masterOptions;
        }

        public static void MockSlayerOption(this IAgent agent, Character target)
        {
            agent.PromptShenanigans(Arg.Any<IReadOnlyCollection<IOption>>(), Arg.Any<bool>(), Arg.Any<Player?>())
                .Returns(args =>
                {
                    var options = args.ArgAt<IReadOnlyCollection<IOption>>(0);
                    var slayerOption = (SlayerShotOption)options.First(option => option is SlayerShotOption);
                    slayerOption.SetTarget(slayerOption.PossiblePlayers.First(player => player.Character == target));
                    return slayerOption;
                });
        }

        public static void MockJugglerOption(this IAgent agent, IEnumerable<(Character player, Character character)> juggles)
        {
            agent.PromptShenanigans(Arg.Any<IReadOnlyCollection<IOption>>(), Arg.Any<bool>(), Arg.Any<Player?>())
                .Returns(args =>
                {
                    var options = args.ArgAt<IReadOnlyCollection<IOption>>(0);
                    var jugglerOption = (JugglerOption)options.First(option => option is JugglerOption);
                    jugglerOption.AddJuggles(juggles.Select(juggle => (jugglerOption.PossiblePlayers.First(player => player.Character == juggle.player), juggle.character)));
                    return jugglerOption;
                });
        }

        public static void MockMinionDamselGuess(this IAgent agent, Character target)
        {
            agent.PromptShenanigans(Arg.Any<IReadOnlyCollection<IOption>>(), Arg.Any<bool>(), Arg.Any<Player?>())
                .Returns(args =>
                {
                    var options = args.ArgAt<IReadOnlyCollection<IOption>>(0);
                    var damselOption = (MinionGuessingDamselOption)options.First(option => option is MinionGuessingDamselOption);
                    damselOption.SetTarget(damselOption.PossiblePlayers.First(player => player.Character == target));
                    return damselOption;
                });
        }

        public static void MockFishermanOption(this IAgent agent, bool getAdvice)
        {
            agent.PromptFishermanAdvice(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsYesNoOptionFromArg(getAdvice);
        }

        public static void MockNightwatchmanOption(this IAgent agent, Character target)
        {
            agent.RequestChoiceFromNightwatchman(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsOptionForCharacterFromArg(target);
        }

        public static void MockHuntsmanOption(this IAgent agent, Character target)
        {
            agent.RequestChoiceFromHuntsman(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsOptionForCharacterFromArg(target);
        }

        public static Wrapper<string> MockFishermanAdvice(this IAgent agent, ClocktowerGame? gameToEnd = null)
        {
            Wrapper<string> advice = new();
            agent.When(agent => agent.ResponseForFisherman(Arg.Any<string>()))
                .Do(args => args.PopulateFromArg(advice, gameToEnd: gameToEnd));
            return advice;
        }
    }
}
