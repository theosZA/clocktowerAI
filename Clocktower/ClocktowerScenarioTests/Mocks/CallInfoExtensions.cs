using Clocktower.Game;
using Clocktower.Options;
using NSubstitute.Core;

namespace ClocktowerScenarioTests.Mocks
{
    internal static class CallInfoExtensions
    {
        public static void PopulateFromArg<T>(this CallInfo args, Wrapper<T> wrapperToPopulate, int argIndex = 0, ClocktowerGame? gameToEnd = null)
        {
            wrapperToPopulate.Value = args.GetArg<T>(args.ArgTypes(), argIndex);
            gameToEnd?.EndGame(Alignment.Good);
        }

        public static void PopulateFromArgs<T0, T1, T2>(this CallInfo args, Wrapper<(T0, T1, T2)> wrapperToPopulate, ClocktowerGame? gameToEnd = null)
        {
            var argTypes = args.ArgTypes();
            wrapperToPopulate.Value = (args.GetArg<T0>(argTypes, 0), args.GetArg<T1>(argTypes, 1), args.GetArg<T2>(argTypes, 2));
            gameToEnd?.EndGame(Alignment.Good);
        }

        public static IOption GetMatchingOptionFromOptionsArg<T>(this CallInfo args, T valueToMatch, List<T> listToPopulate, int argIndex = 0)
        {
            var options = args.ArgAt<IReadOnlyCollection<IOption>>(argIndex).ToList();
            listToPopulate.AddRange(options.Select(option => (T)option.AsType<T>()));
            return options.First(option => EqualityComparer<T>.Default.Equals(option.AsType<T>(), valueToMatch));
        }

        public static IOption GetPassOptionFromArg(this CallInfo args, int argIndex = 0)
        {
            return args.ArgAt<IReadOnlyCollection<IOption>>(argIndex).First(option => option is PassOption);
        }

        public static IOption GetOptionForCharacterFromArg(this CallInfo args, Character target, int argIndex = 0)
        {
            return args.ArgAt<IReadOnlyCollection<IOption>>(argIndex).First(option => option.ToOptionalCharacter() == target);
        }

        public static IOption GetYesNoOptionFromArg(this CallInfo args, bool yesOrNo, int argIndex = 0)
        {
            return args.ArgAt<IReadOnlyCollection<IOption>>(argIndex).First(option => yesOrNo ? option is YesOption : option is NoOption);
        }

        private static T GetArg<T>(this CallInfo args, Type[] argTypes, int argIndex)
        {
            var argType = argTypes[argIndex];

            if (typeof(T) == argType)
            {
                return args.ArgAt<T>(argIndex);
            }
            
            // The only other matching we support is from Player to Character.
            if (argType == typeof(Player) && typeof(T) == typeof(Character))
            {
                return (T)(object)args.ArgAt<Player>(argIndex).Character;
            }

            throw new InvalidCastException($"Can't populate from argument with type {argType} to wrapper with type {typeof(T)}");
        }
    }
}
