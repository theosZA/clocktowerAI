using Clocktower.Options;

namespace Clocktower.Agent.RobotAgent.Model
{
    internal interface IOptionSelection
    {
        IOption? PickOption(IReadOnlyCollection<IOption> options);

        string NoMatchingOptionPrompt(IReadOnlyCollection<IOption> options);
    }
}
