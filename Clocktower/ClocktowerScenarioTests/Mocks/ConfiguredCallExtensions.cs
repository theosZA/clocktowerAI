using Clocktower.Game;
using Clocktower.Options;
using NSubstitute.Core;

namespace ClocktowerScenarioTests.Mocks
{
    internal static class ConfiguredCallExtensions
    {
        public static ConfiguredCall ReturnsMatchingOptionFromOptionsArg<T>(this Task<IOption> onThisBeingCalled, T valueToMatch, List<T> listToPopulate, int argIndex = 0)
        {
            return onThisBeingCalled.Returns(args => args.GetMatchingOptionFromOptionsArg(valueToMatch, listToPopulate, argIndex));
        }

        public static ConfiguredCall ReturnsMatchingOptionFromOptionsArg<T>(this Task<IOption> onThisBeingCalled, T valueToMatch, int argIndex = 0)
        {
            return onThisBeingCalled.Returns(args => args.GetMatchingOptionFromOptionsArg(valueToMatch, argIndex));
        }

        public static ConfiguredCall ReturnsPassOptionFromArg(this Task<IOption> onThisBeingCalled, int argIndex = 0)
        {
            return onThisBeingCalled.Returns(args => args.GetPassOptionFromArg(argIndex));
        }

        public static ConfiguredCall ReturnsOptionForCharacterFromArg(this Task<IOption> onThisBeingCalled, Character target, int argIndex = 0)
        {
            return onThisBeingCalled.Returns(args => args.GetOptionForCharacterFromArg(target, argIndex));
        }

        public static ConfiguredCall ReturnsYesNoOptionFromArg(this Task<IOption> onThisBeingCalled, bool yesOrNo, int argIndex = 0)
        {
            return onThisBeingCalled.Returns(args => args.GetYesNoOptionFromArg(yesOrNo, argIndex));
        }
    }
}
