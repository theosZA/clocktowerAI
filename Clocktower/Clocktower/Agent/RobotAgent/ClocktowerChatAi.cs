using Clocktower.Agent.RobotAgent.Model;
using Clocktower.Game;
using Clocktower.Options;
using OpenAi;

namespace Clocktower.Agent.RobotAgent
{
    /// <summary>
    /// For sending chat to and receiving responses from an Open AI Chat API within the context of a Clocktower game.
    /// Allows you to send chat to the AI with AddMessage() or AddFormattedMessage(), and request chat responses with Request(), RequestDialogue() and RequestChoice().
    /// </summary>
    internal class ClocktowerChatAi
    {
        /// <summary>
        /// Event is triggered whenever a new message is added to the chat. Note that this will include all
        /// messages (though not summaries) even if the actual prompts to the AI don't include all the messages.
        /// </summary>
        public event ChatMessageHandler? OnChatMessage;

        /// <summary>
        /// Event is triggered whenever the AI summarizes the previous day.
        /// </summary>
        public event DaySummaryHandler? OnDaySummary;

        /// <summary>
        /// Event is triggered whenever tokens are used, i.e. whenever a request is made to the AI.
        /// </summary>
        public event TokenCountHandler? OnTokenCount;

        public ClocktowerChatAi(string model, string playerName, string personality, IReadOnlyCollection<string> playerNames, string scriptName, IReadOnlyCollection<Character> script)
        {
            this.playerName = playerName;
            gameChat = new GameChat(model, playerName, personality, playerNames, scriptName, script);
            gameChat.OnChatMessage += InternalOnChatMessage;
            gameChat.OnDaySummary += InternalOnDaySummary;
            gameChat.OnTokenCount += InternalOnTokenCount;
        }

        public async Task Night(int nightNumber)
        {
            await gameChat.NewPhase(Phase.Night, nightNumber);
        }

        public async Task Day(int dayNumber)
        {
            await gameChat.NewPhase(Phase.Day, dayNumber);
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
            gameChat.AddMessage(TextUtilities.FormatText(message, objects));
        }

        public async Task<string> Request(string? prompt = null, params object[] objects)
        {
            if (string.IsNullOrEmpty(prompt))
            {
                return await gameChat.Request<string>(prompt: null) ?? string.Empty;
            }

            return await gameChat.Request<string>(TextUtilities.FormatText(prompt, objects)) ?? string.Empty;
        }

        public async Task<T?> RequestObject<T>(string? prompt = null, params object[] objects)
        {
            if (string.IsNullOrEmpty(prompt))
            {
                return await gameChat.Request<T>(prompt: null);
            }

            return await gameChat.Request<T>(TextUtilities.FormatText(prompt, objects));
        }

        public async Task<(string dialogue, bool endChat)> RequestChatDialogue(string? prompt = null, params object[] objects)
        {
            var dialogue = await RequestDialogue(prompt, objects);
            var endChat = (dialogue.EndsWith("goodbye", StringComparison.InvariantCultureIgnoreCase) ||
                           dialogue.EndsWith("goodbye.", StringComparison.InvariantCultureIgnoreCase) ||
                           dialogue.EndsWith("goodbye!", StringComparison.InvariantCultureIgnoreCase));
            return (dialogue, endChat);
        }

        public async Task<string> RequestDialogue(string? prompt = null, params object[] objects)
        {
            var dialogue = CleanResponse(await Request(prompt, objects));
            if (IsPass(dialogue))
            {
                gameChat.Trim(string.IsNullOrEmpty(prompt) ? 1 : 2);
                return string.Empty;
            }

            // Sometimes the AI likes to add an explanation before and after the actual dialogue.
            // In some cases the actual dialogue is between quotes.
            var quotedDialogue = dialogue.TextBetween('"', '"');
            if (!string.IsNullOrEmpty(quotedDialogue) && quotedDialogue.ContainsWhitespace())
            {
                return quotedDialogue;
            }
            // In other cases the actual dialogue is preceeded by their own name, sometimes emphasized.
            dialogue = dialogue.TextAfter($"{playerName}: ")
                               .TextAfter($"**{playerName}**: ");

            return dialogue;
        }

        public async Task<IOption> RequestChoiceAfterReasoning(IReadOnlyCollection<IOption> options, string? prompt = null, params object[] objects)
        {
            var choiceAsText = await Request(prompt, objects);
            return TextParser.ReadSelectedOptionFromText(choiceAsText, options);
        }

        public async Task<IOption> RequestPlayerSelection(IReadOnlyCollection<IOption> options, string prompt)
        {
            for (int retry = 0; retry < 3; retry++)
            {
                var nomination = await RequestObject<PlayerSelection>(prompt) ?? throw new InvalidDataException($"Robot agent did not respond with a valid {typeof(PlayerSelection).Name} object");
                var result = nomination.PickOption(options);
                if (result != null)
                {
                    return result;
                }
                prompt = nomination.NoMatchingOptionPrompt(options);
            }
            return options.First(option => option is PassOption);
        }

        public async Task<IOption> RequestVote(IReadOnlyCollection<IOption> options, string prompt)
        {
            var vote = await RequestObject<Vote>(prompt) ?? throw new InvalidDataException($"Robot agent did not respond with a valid {typeof(Vote).Name} object");
            return vote.PickOption(options);
        }

        public async Task<IOption> RequestShenanigans(IReadOnlyCollection<IOption> options, string? prompt = null, params object[] objects)
        {
            // This is for a case where there is a variety of different options and we are going to try our best to figure out which one the AI is trying to choose.

            var choiceAsText = await Request(prompt, objects);
            return TextParser.ReadShenaniganOptionFromText(choiceAsText, options);
        }

        public async Task<IOption> RequestChoice(IReadOnlyCollection<IOption> options, string? prompt = null, params object[] objects)
        {
            // Note that the prompt should be specific on how to choose from the options available.

            var choiceAsText = (await Request(prompt, objects)).Trim();

            if (string.IsNullOrEmpty(choiceAsText))
            {
                return options.FirstOrDefault(option => option is PassOption) ?? await RetryRequestChoice(options);
            }

            var choice = GetMatchingOption(options, choiceAsText) ?? await RetryRequestChoice(options);
            if (choice is AlwaysPassOption || choice is PassOption && !options.Any(option => option is VoteOption))
            {   // Trim passes from our chat log.
                gameChat.Trim(string.IsNullOrEmpty(prompt) ? 1 : 2);
            }

            return choice;
        }

        private async Task<IOption> RetryRequestChoice(IReadOnlyCollection<IOption> options)
        {
            string prompt = "That is not a valid option. Please choose one of the following options: " + string.Join(", ", options.Select(option => option.Name));
            var choiceAsText = (await Request(prompt)).Trim();

            if (string.IsNullOrEmpty(choiceAsText))
            {
                return options.FirstOrDefault(option => option is PassOption) ?? throw new Exception($"{playerName} chose to pass but there is no Pass option");
            }

            var option = GetMatchingOption(options, choiceAsText);
            if (option != null)
            {
                return option;
            }
            // AI failed to pick a valid option - default to PASS or the first listed option.
            var passOption = options.FirstOrDefault(option => option is PassOption);
            if (passOption != null)
            {
                return passOption;    
            }
            return options.First();
        }

        private static string CleanResponse(string? textFromAi)
        {
            if (string.IsNullOrEmpty(textFromAi))
            {
                return string.Empty;
            }

            var text = textFromAi.Trim();
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            // Straighten all speech marks to make it easier for us to determine what the AI considers its actual dialogue.
            text = text.Replace('“', '"').Replace('”', '"');

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

        private static bool IsPass(string dialogue)
        {
            return dialogue.StartsWith("pass", StringComparison.InvariantCultureIgnoreCase) ||
                   dialogue.EndsWith("pass", StringComparison.InvariantCultureIgnoreCase) ||
                   dialogue.EndsWith("pass.", StringComparison.InvariantCultureIgnoreCase) ||
                   dialogue.EndsWith("(pass)", StringComparison.InvariantCultureIgnoreCase) ||
                   dialogue.StartsWith("goodbye", StringComparison.InvariantCultureIgnoreCase);
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

        private void InternalOnChatMessage(Role role, string message)
        {
            OnChatMessage?.Invoke(role, message);
        }

        private void InternalOnDaySummary(int dayNumber, string summary)
        {
            OnDaySummary?.Invoke(dayNumber, summary);
        }

        private void InternalOnTokenCount(int promptTokens, int completionTokens, int totalTokens)
        {
            OnTokenCount?.Invoke(promptTokens, completionTokens, totalTokens);
        }

        private readonly string playerName;
        private readonly GameChat gameChat;
    }
}
