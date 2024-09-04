using System.ComponentModel.DataAnnotations;

namespace Clocktower.Agent.RobotAgent.Model
{
    /// <summary>
    /// Robot agent model for specifying which players are to be which minions.
    /// </summary>
    internal class KazaliMinions
    {
        [Required(AllowEmptyStrings = true)]
        public string Reasoning { get; set; } = string.Empty;

        [Required]
        public IEnumerable<PlayerAsCharacter> MinionAssignments { get; set; } = Enumerable.Empty<PlayerAsCharacter>();
    }
}
