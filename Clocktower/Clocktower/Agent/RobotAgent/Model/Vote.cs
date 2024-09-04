using Clocktower.Options;
using System.ComponentModel.DataAnnotations;

namespace Clocktower.Agent.RobotAgent.Model
{
    /// <summary>
    /// Robot agent model for specifying a vote to execute or to pass.
    /// </summary>
    internal class Vote : IOptionSelection
    {
        [Required(AllowEmptyStrings = true)]
        public string Reasoning { get; set; } = string.Empty;

        [Required]
        public bool VoteForExecution { get; set; }

        public IOption? PickOption(IReadOnlyCollection<IOption> options)
        {
            if (VoteForExecution)
            {
                return options.First(option => option is VoteOption);
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
