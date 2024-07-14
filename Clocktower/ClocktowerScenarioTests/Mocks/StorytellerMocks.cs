using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Storyteller;

namespace ClocktowerScenarioTests.Mocks
{
    internal static class StorytellerMocks
    {
        public static void MockGetDrunk(this IStoryteller storyteller, Character drunkCharacter)
        {
            storyteller.GetDrunk(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsOptionForCharacterFromArg(drunkCharacter);
        }

        public static void MockGetSweetheartDrunk(this IStoryteller storyteller, Character sweetheartDrunk)
        {
            storyteller.GetSweetheartDrunk(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsOptionForCharacterFromArg(sweetheartDrunk);
        }

        public static List<Character> MockGetStewardPing(this IStoryteller storyteller, Character stewardPing)
        {
            List<Character> stewardPingOptions = new();
            storyteller.GetStewardPing(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .ReturnsMatchingOptionFromOptionsArg(stewardPing, stewardPingOptions, argIndex: 1);
            return stewardPingOptions;
        }

        public static List<(Character playerA, Character playerB, Character character)> MockGetInvestigatorPing(this IStoryteller storyteller, Character investigatorPing, Character investigatorWrong, Character asCharacter)
        {
            List<(Character playerA, Character playerB, Character character)> investigatorPingOptions = new();
            storyteller.GetInvestigatorPings(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .ReturnsMatchingOptionFromOptionsArg((investigatorPing, investigatorWrong, asCharacter), investigatorPingOptions, argIndex: 1);
            return investigatorPingOptions;
        }

        public static List<(Character playerA, Character playerB, Character character)?> MockGetLibrarianPing(this IStoryteller storyteller, Character librarianPing, Character librarianWrong, Character asCharacter)
        {
            List<(Character playerA, Character playerB, Character character)?> librarianPingOptions = new();
            storyteller.GetLibrarianPings(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .ReturnsMatchingOptionFromOptionsArg((librarianPing, librarianWrong, asCharacter), librarianPingOptions, argIndex: 1);
            return librarianPingOptions;
        }

        public static List<(Character playerA, Character playerB, Character character)> MockGetWasherwomanPing(this IStoryteller storyteller, Character washerwomanPing, Character washerwomanWrong, Character asCharacter)
        {
            List<(Character playerA, Character playerB, Character character)> washerwomanPingOptions = new();
            storyteller.GetWasherwomanPings(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .ReturnsMatchingOptionFromOptionsArg((washerwomanPing, washerwomanWrong, asCharacter), washerwomanPingOptions, argIndex: 1);
            return washerwomanPingOptions;
        }

        public static List<Direction> MockGetShugenjaDirection(this IStoryteller storyteller, Direction direction)
        {
            List<Direction> directionOptions = new();
            storyteller.GetShugenjaDirection(Arg.Any<Player>(), Arg.Any<Grimoire>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .ReturnsMatchingOptionFromOptionsArg(direction, directionOptions, argIndex: 2);
            return directionOptions;
        }

        public static List<int> MockGetEmpathNumbers(this IStoryteller storyteller, int empathNumber)
        {
            List<int> empathNumbers = new();
            storyteller.GetEmpathNumber(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .ReturnsMatchingOptionFromOptionsArg(empathNumber, empathNumbers, argIndex: 3);
            return empathNumbers;
        }

        public static List<int> MockGetChefNumber(this IStoryteller storyteller, int chefNumber)
        {
            List<int> chefNumbers = new();
            storyteller.GetChefNumber(Arg.Any<Player>(), Arg.Any<IEnumerable<Player>>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .ReturnsMatchingOptionFromOptionsArg(chefNumber, chefNumbers, argIndex: 2);
            return chefNumbers;
        }

        public static List<Character> MockGetNewImp(this IStoryteller storyteller, Character starPassTarget)
        {
            List<Character> starPassTargets = new();
            storyteller.GetNewImp(Arg.Any<IReadOnlyCollection<IOption>>())
                .ReturnsMatchingOptionFromOptionsArg(starPassTarget, starPassTargets);
            return starPassTargets;
        }

        public static void MockGetCharacterForUndertaker(this IStoryteller storyteller, Character character)
        {
            storyteller.GetCharacterForUndertaker(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .ReturnsOptionForCharacterFromArg(character, argIndex: 2);
        }

        public static void MockGetCharacterForRavenkeeper(this IStoryteller storyteller, Character character)
        {
            storyteller.GetCharacterForRavenkeeper(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .ReturnsOptionForCharacterFromArg(character, argIndex: 2);
        }

        public static void MockShouldKillTinker(this IStoryteller storyteller, bool shouldKill)
        {
            storyteller.ShouldKillTinker(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsYesNoOptionFromArg(shouldKill, argIndex: 1);
        }

        public static void MockShouldKillWithSlayer(this IStoryteller storyteller, bool shouldKill)
        {
            storyteller.ShouldKillWithSlayer(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsYesNoOptionFromArg(shouldKill, argIndex: 2);
        }

        public static void MockShouldExecuteWithVirgin(this IStoryteller storyteller, bool shouldExecute)
        {
            storyteller.ShouldExecuteWithVirgin(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsYesNoOptionFromArg(shouldExecute, argIndex: 2);
        }

        public static void MockFishermanAdvice(this IStoryteller storyteller, string advice)
        {
            storyteller.GetFishermanAdvice(Arg.Any<Player>()).Returns(advice);
        }

        public static void MockFortuneTellerRedHerring(this IStoryteller storyteller, Character redHerring)
        {
            storyteller.GetFortuneTellerRedHerring(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsOptionForCharacterFromArg(redHerring, argIndex: 1);
        }

        public static void MockFortuneTellerReading(this IStoryteller storyteller, bool reading)
        {
            storyteller.GetFortuneTellerReading(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsYesNoOptionFromArg(reading, argIndex: 3);
        }
    }
}
