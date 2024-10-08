using Clocktower.Options;
using System.ComponentModel.DataAnnotations;

namespace Clocktower.Agent.RobotAgent.Model
{
    /// <summary>
    /// AI model for specifying a direction, clockwise or counter-clockwise.
    /// </summary>
    internal class DirectionSelection : IOptionSelection
    {
        [Required(AllowEmptyStrings = true)]
        public string Reasoning { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = true)]
        public string ClockwiseOrCounterClockwise { get; set; } = string.Empty;

        public IOption? PickOption(IReadOnlyCollection<IOption> options)
        {
            if (string.Equals(ClockwiseOrCounterClockwise, "clockwise", StringComparison.InvariantCultureIgnoreCase))
            {
                return options.FirstOrDefault(option => option is DirectionOption directionOption && directionOption.Direction == Game.Direction.Clockwise);
            }
            if (string.Equals(ClockwiseOrCounterClockwise, "counterclockwise", StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(ClockwiseOrCounterClockwise, "counter-clockwise", StringComparison.InvariantCultureIgnoreCase))
            {
                return options.FirstOrDefault(option => option is DirectionOption directionOption && directionOption.Direction == Game.Direction.Counterclockwise);
            }
            return null;
        }

        public string NoMatchingOptionPrompt(IReadOnlyCollection<IOption> options)
        {
            return $"'{ClockwiseOrCounterClockwise}' is not a valid choice. `{nameof(ClockwiseOrCounterClockwise)}` property must be either \"clockwise\" or \"counter-clockwise\".";
        }
    }
}
