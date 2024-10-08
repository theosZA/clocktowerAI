using Clocktower.Options;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Clocktower.Agent.RobotAgent.Model
{
    /// <summary>
    /// AI model for specifying any number of players.
    /// </summary>
    internal class PlayersSelection : IOptionSelection
    {
        [Required(AllowEmptyStrings = true)]
        public string Reasoning { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = true)]
        public IEnumerable<string> Players { get; set; } = Enumerable.Empty<string>();

        public IOption? PickOption(IReadOnlyCollection<IOption> options)
        {
            var players = Players.ToList();
            return options.FirstOrDefault(option => (option is TwoPlayersOption twoPlayersOption && IsMatchingOption(twoPlayersOption, players))
                                                 || (option is ThreePlayersOption threePlayersOption && IsMatchingOption(threePlayersOption, players))
                                                 || (option is PlayerListOption playerListOption && IsMatchingOption(playerListOption, players)));
        }

        public string NoMatchingOptionPrompt(IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"'{string.Join(",", Players)}' is not a valid choice. `{nameof(Players)}` property must be one of the following arrays:");
            foreach (var option in options)
            {
                if (option is PlayerListOption playerListOption)
                {
                    sb.AppendLine($"- [{string.Join(",", playerListOption.Players.Select(player => $"\"{player}\""))}]");
                }
                else if (option is TwoPlayersOption twoPlayersOption)
                {
                    sb.AppendLine($"- [\"{twoPlayersOption.PlayerA}\",\"{twoPlayersOption.PlayerB}\"]");
                }
                else if (option is ThreePlayersOption threePlayersOption)
                {
                    sb.AppendLine($"- [\"{threePlayersOption.PlayerA}\",\"{threePlayersOption.PlayerB}\",\"{threePlayersOption.PlayerC}\"]");
                }
            }

            return sb.ToString();
        }

        private static bool IsMatchingOption(TwoPlayersOption option, List<string> players)
        {
            if (players.Count != 2)
            {
                return false;
            }
            return players.Contains(option.PlayerA.Name) && players.Contains(option.PlayerB.Name);
        }

        private static bool IsMatchingOption(ThreePlayersOption option, List<string> players)
        {
            if (players.Count != 3)
            {
                return false;
            }
            return players.Contains(option.PlayerA.Name) && players.Contains(option.PlayerB.Name) && players.Contains(option.PlayerC.Name);
        }

        private static bool IsMatchingOption(PlayerListOption option, List<string> players)
        {
            if (option.Players.Count != players.Count)
            {
                return false;
            }
            return option.Players.All(option => players.Contains(option.Name));
        }
    }
}
