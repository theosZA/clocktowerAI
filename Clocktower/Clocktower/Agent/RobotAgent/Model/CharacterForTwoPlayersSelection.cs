using Clocktower.Options;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Clocktower.Agent.RobotAgent.Model
{
    /// <summary>
    /// AI model for specifying two players and a character that one of them is seen as.
    /// </summary>
    internal class CharacterForTwoPlayersSelection : IOptionSelection
    {
        [Required(AllowEmptyStrings = true)]
        public string Reasoning { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = true)]
        public string PlayerA { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = true)]
        public string CharacterA { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = true)]
        public string PlayerB { get; set; } = string.Empty;

        public IOption? PickOption(IReadOnlyCollection<IOption> options)
        {
            if (string.IsNullOrEmpty(PlayerA) || string.IsNullOrEmpty(PlayerB) || string.IsNullOrEmpty(CharacterA))
            {
                return options.FirstOrDefault(option => option is NoOutsiders);
            }
            return options.FirstOrDefault(option => option is CharacterForTwoPlayersOption o &&
                                                    string.Equals(o.PlayerA.Name, PlayerA, StringComparison.InvariantCultureIgnoreCase) &&
                                                    string.Equals(o.PlayerB.Name, PlayerB, StringComparison.InvariantCultureIgnoreCase) &&
                                                    string.Equals(o.Character.ToString(), CharacterA, StringComparison.InvariantCultureIgnoreCase));
        }

        public string NoMatchingOptionPrompt(IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"'{PlayerA}' or '{PlayerB}' being seen as the '{CharacterA}' is not a valid choice. These are the possible combinations:");
            foreach (var option in options)
            {
                if (option is CharacterForTwoPlayersOption characterForTwoPlayersOption)
                {
                    sb.AppendLine($"- \"{nameof(PlayerA)}\"=\"{characterForTwoPlayersOption.PlayerA.Name}\"" +
                                  $", \"{nameof(CharacterA)}\"=\"{characterForTwoPlayersOption.Character}\"" +
                                  $", \"{nameof(PlayerB)}\"=\"{characterForTwoPlayersOption.PlayerB.Name}\"");
                }
                else if (option is NoOutsiders)
                {
                    sb.AppendLine($"- {nameof(PlayerA)}, {nameof(CharacterA)}, {nameof(PlayerB)} can all be left to blank to indicate that there are no Outsiders in this game.");
                }
            }

            return sb.ToString();
        }
    }
}
