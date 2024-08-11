using Clocktower.Options;
using System.ComponentModel.DataAnnotations;

namespace Clocktower.Agent.RobotAgent.Model
{
    /// <summary>
    /// Robot agent model for specifying whether or not to use the player's character ability.
    /// </summary>
    internal class UseAbilityChoice : IOptionSelection
    {
        [Required(AllowEmptyStrings = true)]
        public string Reasoning { get; set; } = string.Empty;

        [Required]
        public bool UseAbility { get; set; }

        public IOption? PickOption(IReadOnlyCollection<IOption> options)
        {
            if (UseAbility)
            {
                return options.First(option => option is YesOption);
            }
            return options.First(option => option is PassOption);
        }

        public string NoMatchingOptionPrompt(IReadOnlyCollection<IOption> options)
        {
            // All responses are valid.
            throw new NotImplementedException();
        }

    }
}
