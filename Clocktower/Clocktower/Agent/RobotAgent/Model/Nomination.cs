using NJsonSchema.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Clocktower.Agent.RobotAgent.Model
{
    /// <summary>
    /// Robot agent model for specifying a nomination or pass.
    /// </summary>
    internal class Nomination
    {
        [Required(AllowEmptyStrings = true)]
        public string Reasoning { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = true)]
        [CanBeNull]
        public string? Player { get; set; }
    }
}
