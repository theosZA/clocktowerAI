using System.ComponentModel.DataAnnotations;

namespace Clocktower.Agent.RobotAgent.Model
{
    /// <summary>
    /// Robot agent model for choosing what to say in a private chat.
    /// </summary>
    internal class PrivateDialogue
    {
        [Required(AllowEmptyStrings = true)]
        public string Reasoning { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = true)]
        public string Dialogue { get; set; } = string.Empty;

        [Required]
        public bool TerminateConversation { get; set; }
    }
}
