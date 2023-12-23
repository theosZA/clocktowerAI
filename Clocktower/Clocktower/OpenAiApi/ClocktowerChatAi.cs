using Clocktower.Game;
using Clocktower.Options;
using System.ComponentModel.Design;
using System.Text;

namespace Clocktower.OpenAiApi
{
    /// <summary>
    /// Keeps track of the progress of the game using all night info and summaries generated for each day,
    /// plus everything that has happened in the current day.
    /// </summary>
    internal class ClocktowerChatAi
    {
        public IEnumerable<(Role role, string message)> Messages => phases.SelectMany(phase => phase.Messages);

        public ClocktowerChatAi(string playerName, IReadOnlyCollection<string> playersNames, IReadOnlyCollection<Character> script, IChatLogger chatLogger, ITokenCounter tokenCounter)
        {
            this.playerName = playerName;
            chatCompletionApi = new(tokenCounter);
            this.chatLogger = chatLogger;

            var phase = new PhaseMessages(chatCompletionApi, OpenAiApi.Phase.Setup, 0, chatLogger);
            phase.AddSystemMessage(SystemMessage.GetSystemMessage(playerName, playersNames, script));

            phases.Add(phase);
        }

        public void Night(int nightNumber)
        {
            phases.Add(new PhaseMessages(chatCompletionApi, OpenAiApi.Phase.Night, nightNumber, chatLogger));
        }

        public void Day(int dayNumber)
        {
            phases.Add(new PhaseMessages(chatCompletionApi, OpenAiApi.Phase.Day, dayNumber, chatLogger));
        }

        public void AddMessage(string message)
        {
            Phase.Add(message);
        }

        public void AddFormattedMessage(string message, params object[] objects)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText(message, objects);
            Phase.Add(sb.ToString());
        }

        public async Task<string> Request(string? prompt = null, params object[] objects)
        {
            if (!string.IsNullOrEmpty(prompt))
            {
                var sb = new StringBuilder();
                sb.AppendFormattedText(prompt, objects);
                prompt = sb.ToString();
            }

            return await Phase.Request(prompt, phases.SkipLast(1).ToList());
        }

        public async Task<string> RequestDialogue(string? prompt = null, params object[] objects)
        {
            var dialogue = CleanResponse(await Request(prompt, objects));

            if (dialogue.StartsWith("pass", StringComparison.InvariantCultureIgnoreCase))
            {
                return string.Empty;
            }

            return dialogue;
        }

        public async Task<IOption> RequestChoice(IReadOnlyCollection<IOption> options, string? prompt = null, params object[] objects)
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
            string prompt = "That is not a valid option. Please choice one of the following options: " + string.Join(", ", options.Select(option => option.Name));
            var choiceAsText = (await Request(prompt)).Trim();

            if (string.IsNullOrEmpty(choiceAsText))
            {
                return options.FirstOrDefault(option => option is PassOption) ?? throw new Exception($"{playerName} chose to pass but there is no Pass option");
            }

            return GetMatchingOption(options, choiceAsText) ?? throw new Exception($"{playerName} chose \"{choiceAsText}\" but there is no matching option");
        }

        private static string CleanResponse(string? textFromAi)
        {
            if (string.IsNullOrEmpty(textFromAi))
            {
                return string.Empty;
            }

            var text = textFromAi.Trim();
            if (text.EndsWith('.'))
            {
                text = text[..^1].TrimEnd();
            }

            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            // If there's a colon before the second word, it means the player has presumably prepended a speaker name (which may or may not be correct).
            if (text.IndexOf(':') < text.IndexOf(' '))
            {
                text = text[(text.IndexOf(':') + 1)..].TrimStart();

                if (string.IsNullOrEmpty(text))
                {
                    return string.Empty;
                }
            }

            return text;
        }

        private static IOption? GetMatchingOption(IReadOnlyCollection<IOption> options, string choiceAsText)
        {
            return options.FirstOrDefault(option => MatchesOption(choiceAsText, option)) ?? options.FirstOrDefault(option => MatchesOptionRelaxed(choiceAsText, option));
        }

        private static bool MatchesOption(string choiceAsText, IOption option)
        {
            return option switch
            {
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
            return string.Equals(choiceAsText[..splitIndex].Trim(), player1, StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(choiceAsText[(splitIndex + 5)..].Trim(), player2, StringComparison.InvariantCultureIgnoreCase);
        }

        private PhaseMessages Phase => phases.Last();

        private readonly string playerName;

        private readonly ChatCompletionApi chatCompletionApi;
        private readonly IChatLogger chatLogger;

        private readonly List<PhaseMessages> phases = new();
    }
}
