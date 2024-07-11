using Clocktower.Game;
using Clocktower.Options;
using NSubstitute.Core;

namespace ClocktowerScenarioTests.Mocks
{
    internal static class CallInfoExtensions
    {
        public static IOption GetPassOptionFromArg(this CallInfo args, int argIndex = 0)
        {
            return args.ArgAt<IReadOnlyCollection<IOption>>(argIndex).First(option => option is PassOption);
        }

        public static IOption GetOptionForCharacterFromArg(this CallInfo args, Character target, int argIndex = 0)
        {
            return args.ArgAt<IReadOnlyCollection<IOption>>(argIndex).First(option => option.ToOptionalCharacter() == target);
        }
    }
}
