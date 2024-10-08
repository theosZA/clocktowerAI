using Clocktower.Options;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Clocktower.Agent.RobotAgent.Model
{
    /// <summary>
    /// Robot agent model for specifying exactly two players selected by the agent.
    /// </summary>
    internal class TwoPlayerSelection : IOptionSelection
    {
        [Required(AllowEmptyStrings = true)]
        public string Reasoning { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = true)]
        public string Player1 { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = true)]
        public string Player2 { get; set; } = string.Empty;

        public IOption? PickOption(IReadOnlyCollection<IOption> options)
        {
            if (string.IsNullOrEmpty(Player1) && string.IsNullOrEmpty(Player2))
            {
                return options.FirstOrDefault(option => option is PassOption);
            }
            if (string.IsNullOrEmpty(Player1) || string.IsNullOrEmpty(Player2))
            {
                return null;
            }
            return options.FirstOrDefault(option => option is TwoPlayersOption twoPlayersOption && 
                                                    string.Equals(twoPlayersOption.PlayerA.Name, Player1, StringComparison.InvariantCultureIgnoreCase) &&
                                                    string.Equals(twoPlayersOption.PlayerB.Name, Player2, StringComparison.InvariantCultureIgnoreCase));
        }

        public string NoMatchingOptionPrompt(IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder();

            var validPlayer1Names = options.Where(option => option is TwoPlayersOption twoPlayersOption)
                                           .Select(option => ((TwoPlayersOption)option).PlayerA.Name)
                                           .ToList();
            var validPlayer2Names = options.Where(option => option is TwoPlayersOption twoPlayersOption)
                                           .Select(option => ((TwoPlayersOption)option).PlayerB.Name)
                                           .ToList();

            if (!validPlayer1Names.Any(playerName => string.Equals(playerName, Player1, StringComparison.InvariantCultureIgnoreCase)))
            {
                sb.Append($"'{Player1}' is not a valid choice. `{nameof(Player1)}` property must be one of ");
                sb.Append(string.Join(", ", validPlayer1Names));
                sb.Append(". ");
            }
            if (!validPlayer2Names.Any(playerName => string.Equals(playerName, Player2, StringComparison.InvariantCultureIgnoreCase)))
            {
                sb.Append($"'{Player2}' is not a valid choice. `{nameof(Player2)}` property must be one of ");
                sb.Append(string.Join(", ", validPlayer1Names));
                sb.Append(". ");
            }
            if (options.Any(option => option is PassOption))
            {
                sb.Append("Alternatively both player names can be left blank to pass.");
            }

            return sb.ToString();
        }
    }
}
