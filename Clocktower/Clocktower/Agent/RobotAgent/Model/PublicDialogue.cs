using System.ComponentModel.DataAnnotations;

namespace Clocktower.Agent.RobotAgent.Model
{
    /// <summary>
    /// Robot agent model for choosing what to say in public.
    /// </summary>
    internal class PublicDialogue
    {
        [Required(AllowEmptyStrings = true)]
        public string Reasoning { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = true)]
        public string Dialogue { get; set; } = string.Empty;
    }
}
