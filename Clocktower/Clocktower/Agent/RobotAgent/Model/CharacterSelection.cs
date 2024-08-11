using Clocktower.Options;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Clocktower.Agent.RobotAgent.Model
{
    /// <summary>
    /// Robot agent model for specifying a single character selected by the agent.
    /// </summary>
    internal class CharacterSelection : IOptionSelection
    {
        [Required(AllowEmptyStrings = true)]
        public string Reasoning { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = true)]
        public string Character { get; set; } = string.Empty;

        public IOption? PickOption(IReadOnlyCollection<IOption> options)
        {
            if (string.IsNullOrEmpty(Character))
            {
                return options.FirstOrDefault(option => option is PassOption);
            }
            return options.FirstOrDefault(option => option.Name == Character);
        }

        public string NoMatchingOptionPrompt(IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder();

            sb.Append($"'{Character}' is not a valid choice. `{nameof(Character)}` property must be one of ");

            var characters = options.Where(option => option is CharacterOption)
                                    .Select(option => option.Name);
            sb.Append(string.Join(", ", characters));

            if (options.Any(option => option is PassOption))
            {
                sb.Append(" or can be left blank to pass");
            }
            sb.Append('.');

            return sb.ToString();
        }
    }
}
