using System.ComponentModel.DataAnnotations;

namespace Clocktower.Agent.RobotAgent.Model
{
    /// <summary>
    /// Robot agent model for specifying a player and their character.
    /// </summary>
    public class PlayerAsCharacter
    {
        [Required(AllowEmptyStrings = true)]
        public string Player { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = true)]
        public string Character { get; set; } = string.Empty;
    }
}
