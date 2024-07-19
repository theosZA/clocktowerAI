using Clocktower.Options;
using System.Text;

namespace Clocktower.Agent
{
    internal class TextPlayerPrompter
    {
        public Func<string, Task<string>>? SendMessageAndGetResponse { get; set; }

        public TextPlayerPrompter(string playerName)
        {
            this.playerName = playerName;
        }

        public async Task<string> RequestDialogue(string prompt, params object[] objects)
        {
            var response = await Request(prompt, objects);
            if (string.Equals("PASS", response, StringComparison.InvariantCultureIgnoreCase))
            {
                return string.Empty;
            }
            return response;
        }

        public async Task<(string dialogue, bool endChat)> RequestChatDialogue(string prompt, params object[] objects)
        {
            var dialogue = await RequestDialogue(prompt, objects);
            var endChat = (dialogue.EndsWith("goodbye", StringComparison.InvariantCultureIgnoreCase) ||
                           dialogue.EndsWith("goodbye.", StringComparison.InvariantCultureIgnoreCase) ||
                           dialogue.EndsWith("goodbye!", StringComparison.InvariantCultureIgnoreCase));
            return (dialogue, endChat);
        }

        public async Task<IOption> RequestChoice(IReadOnlyCollection<IOption> options, string prompt, params object[] objects)
        {
            // Note that the prompt should be specific on how to choose from the options available.

            var choiceAsText = (await Request(prompt, objects)).Trim();

            if (string.IsNullOrEmpty(choiceAsText))
            {
                return options.FirstOrDefault(option => option is PassOption) ?? await RetryRequestChoice(options);
            }

            return GetMatchingOption(options, choiceAsText) ?? await RetryRequestChoice(options);
        }

        private async Task<IOption> RetryRequestChoice(IReadOnlyCollection<IOption> options)
        {
            string prompt = "That is not a valid option. Please choose one of the following options: " + string.Join(", ", options.Select(AsPromptText));
            var choiceAsText = (await Request(prompt)).Trim();

            if (string.IsNullOrEmpty(choiceAsText))
            {
                return options.FirstOrDefault(option => option is PassOption) ?? throw new Exception($"{playerName} chose to pass but there is no Pass option");
            }

            return GetMatchingOption(options, choiceAsText) ?? await RetryRequestChoice(options);
        }

        private async Task<string> Request(string prompt, params object[] objects)
        {
            if (SendMessageAndGetResponse == null)
            {
                throw new Exception($"Prompt requested for {playerName} but no handler has been configured for this player");
            }
            return await SendMessageAndGetResponse(FormatText(prompt, objects));
        }

        private static string FormatText(string text, params object[] objects)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedMarkupText(text, objects);
            return sb.ToString();
        }

        private static IOption? GetMatchingOption(IReadOnlyCollection<IOption> options, string choiceAsText)
        {
            return options.FirstOrDefault(option => MatchesOption(choiceAsText, option)) ?? options.FirstOrDefault(option => MatchesOptionRelaxed(choiceAsText, option));
        }

        private static bool MatchesOption(string choiceAsText, IOption option)
        {
            return option switch
            {
                AlwaysPassOption _ => choiceAsText.StartsWith("always", StringComparison.InvariantCultureIgnoreCase),
                PassOption _ => choiceAsText.StartsWith("pass", StringComparison.InvariantCultureIgnoreCase),
                VoteOption _ => choiceAsText.StartsWith("execute", StringComparison.InvariantCultureIgnoreCase),
                SlayerShotOption slayerShotOption => choiceAsText.StartsWith(slayerShotOption.Target.Name, StringComparison.InvariantCultureIgnoreCase),
                TwoPlayersOption twoPlayersOption => MatchesTwoPlayers(choiceAsText, twoPlayersOption.PlayerA.Name, twoPlayersOption.PlayerB.Name),
                _ => choiceAsText.StartsWith(option.Name, StringComparison.InvariantCultureIgnoreCase),
            };
        }

        private static bool MatchesOptionRelaxed(string choiceAsText, IOption option)
        {
            return option switch
            {
                AlwaysPassOption _ => choiceAsText.Contains("always", StringComparison.InvariantCultureIgnoreCase),
                PassOption _ => choiceAsText.Contains("pass", StringComparison.InvariantCultureIgnoreCase),
                VoteOption _ => choiceAsText.Contains("execute", StringComparison.InvariantCultureIgnoreCase),
                SlayerShotOption slayerShotOption => choiceAsText.Contains(slayerShotOption.Target.Name, StringComparison.InvariantCultureIgnoreCase),
                TwoPlayersOption twoPlayersOption => choiceAsText.Contains(twoPlayersOption.PlayerA.Name) && choiceAsText.Contains(twoPlayersOption.PlayerB.Name),
                _ => choiceAsText.Contains(option.Name, StringComparison.InvariantCultureIgnoreCase),
            };
        }

        private static bool MatchesTwoPlayers(string choiceAsText, string player1, string player2)
        {
            int splitIndex = choiceAsText.IndexOf(" and ", StringComparison.InvariantCultureIgnoreCase);
            return splitIndex >= 0
                && string.Equals(choiceAsText[..splitIndex].Trim(), player1, StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(choiceAsText[(splitIndex + 5)..].Trim(), player2, StringComparison.InvariantCultureIgnoreCase);
        }

        private static string AsPromptText(IOption option)
        {
            return option switch
            {
                AlwaysPassOption _ => "`ALWAYS PASS`",
                CharacterOption characterOption => TextUtilities.FormatMarkupText("%c", characterOption.Character),
                PassOption _ => "`PASS`",
                PlayerOption playerOption => TextUtilities.FormatMarkupText("%p", playerOption.Player),
                SlayerShotOption slayerShotOption => TextUtilities.FormatMarkupText("%p", slayerShotOption.Target),
                TwoPlayersOption twoPlayersOption => TextUtilities.FormatMarkupText("%p and %p", twoPlayersOption.PlayerA, twoPlayersOption.PlayerB),
                VoteOption voteOption => "`EXECUTE`",
                _ => throw new ArgumentException($"Unknown option type {option.GetType()}", nameof(option))
            };
        }

        private readonly string playerName;
    }
}
