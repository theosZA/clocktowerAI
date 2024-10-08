using Clocktower.Options;
using System.ComponentModel.DataAnnotations;

namespace Clocktower.Agent.RobotAgent.Model
{
    /// <summary>
    /// AI model for specifying whether to provide a Yes or No response.
    /// </summary>
    internal class YesNoSelection : IOptionSelection
    {
        [Required(AllowEmptyStrings = true)]
        public string Reasoning { get; set; } = string.Empty;

        [Required]
        public bool Yes { get; set; }

        public IOption? PickOption(IReadOnlyCollection<IOption> options)
        {
            if (Yes)
            {
                return options.First(option => option is YesOption);
            }
            return options.First(option => option is NoOption || option is PassOption);
        }

        public string NoMatchingOptionPrompt(IReadOnlyCollection<IOption> options)
        {
            // All responses are valid.
            throw new NotImplementedException();
        }
    }
}
