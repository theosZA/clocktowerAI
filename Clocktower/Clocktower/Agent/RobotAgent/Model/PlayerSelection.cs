using Clocktower.Options;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Clocktower.Agent.RobotAgent.Model
{
    /// <summary>
    /// Robot agent model for specifying a single player selected by the agent.
    /// </summary>
    internal class PlayerSelection
    {
        [Required(AllowEmptyStrings = true)]
        public string Reasoning { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = true)]
        public string Player { get; set; } = string.Empty;

        public IOption? PickOption(IReadOnlyCollection<IOption> options)
        {
            if (string.IsNullOrEmpty(Player))
            {
                return options.FirstOrDefault(option => option is PassOption);
            }
            return options.FirstOrDefault(option => option.Name == Player);
        }

        public string NoMatchingOptionPrompt(IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder();

            sb.Append($"'{Player}' is not a valid choice. `Player` property must be one of ");

            var players = options.Where(option => option is PlayerOption)
                                 .Select(option => option.Name);
            sb.Append(string.Join(", ", players));

            if (options.Any(option => option is PassOption))
            {
                sb.Append(" or can be left blank to pass");
            }
            sb.Append('.');

            return sb.ToString();
        }
    }
}
