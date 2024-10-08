using System.ComponentModel.DataAnnotations;

namespace Clocktower.Agent.RobotAgent.Model
{
    /// <summary>
    /// AI Storyteller model for providing a text response to a player.
    /// </summary>
    internal class TextResponse
    {
        [Required(AllowEmptyStrings = true)]
        public string Reasoning { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = true)]
        public string Response { get; set; } = string.Empty;
    }
}
