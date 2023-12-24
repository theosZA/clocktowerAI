using Clocktower.Game;
using Clocktower.Options;
using System.Text;

namespace Clocktower.OpenAiApi
{
    /// <summary>
    /// For sending chat to and receiving responses from an Open AI Chat API within the context of a Clocktower game.
    /// Allows you to send chat to the AI with AddMessage() or AddFormattedMessage(), and request chat responses with Request(), RequestDialogue() and RequestChoice().
    /// </summary>
    internal class ClocktowerChatAi
    {
        public ClocktowerChatAi(string playerName, IReadOnlyCollection<string> playerNames, IReadOnlyCollection<Character> script, IChatLogger chatLogger, ITokenCounter tokenCounter)
        {
            this.playerName = playerName;
            gameChat = new GameChat(playerName, playerNames, script, new ChatCompletionApi(tokenCounter), chatLogger);
        }

        public void Night(int nightNumber)
        {
            gameChat.NewPhase(Phase.Night, nightNumber);
        }

        public void Day(int dayNumber)
        {
            gameChat.NewPhase(Phase.Day, dayNumber);
        }

        public void AddMessage(string message)
        {
            gameChat.AddMessage(message);
        }

        /// <summary>
        /// Adds a message to the current chat with formatting using format specifier '%'.
        /// </summary>
        /// <param name="message">
        /// Writes the given text to the chat, only substituting variables as 
        /// %n: normal, %b: bold,
        /// %p: formatted as a player, %P: formatted as a player list,
        /// %c: formatted as a character, %C: formatted as a character list.
        /// %a: formatted as an alignment
        /// </param>
        /// <param name="objects">
        /// Objects to substitute into the output text. For %p the object must be a Player, for %P the object must be an IEnumerable<Player>, for %c the object must be a Character, and for %C the object must be an IEnumerable<Character>.
        /// </param>
        public void AddFormattedMessage(string message, params object[] objects)
        {
            gameChat.AddMessage(FormatText(message, objects));
        }

        public async Task<string> Request(string? prompt = null, params object[] objects)
        {
            if (string.IsNullOrEmpty(prompt))
            {
                return await gameChat.Request(prompt: null);
            }

            return await gameChat.Request(FormatText(prompt, objects));
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

        private static string FormatText(string text, params object[] objects)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText(text, objects);
            return sb.ToString();
        }

        private readonly string playerName;
        private readonly GameChat gameChat;
    }
}
