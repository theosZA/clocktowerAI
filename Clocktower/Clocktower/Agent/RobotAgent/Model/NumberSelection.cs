using Clocktower.Options;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Clocktower.Agent.RobotAgent.Model
{
    /// <summary>
    /// AI model for specifying a number learned by a player.
    /// </summary>
    internal class NumberSelection : IOptionSelection
    {
        [Required(AllowEmptyStrings = true)]
        public string Reasoning { get; set; } = string.Empty;

        [Required]
        public int Number { get; set; }

        public IOption? PickOption(IReadOnlyCollection<IOption> options)
        {
            return options.FirstOrDefault(option => option is NumberOption numberOption && numberOption.Number == Number);
        }

        public string NoMatchingOptionPrompt(IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder();

            sb.Append($"'{Number}' is not a valid choice. `{nameof(Number)}` property must be one of ");

            var numbers = options.Where(option => option is NumberOption)
                                 .Select(option => (option as NumberOption)!.Number);
            sb.Append(string.Join(", ", numbers));
            sb.Append('.');

            return sb.ToString();
        }
    }
}
