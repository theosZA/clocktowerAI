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
    public class ClocktowerChatAi
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

        public async Task<T?> RequestObjectWithRetries<T>(string? prompt = null, params object[] objects)
        {
            if (string.IsNullOrEmpty(prompt))
            {
                return await gameChat.Request<T>(prompt: null);
            }

            return await gameChat.Request<T>(TextUtilities.FormatText(prompt, objects));
        }

        public async Task<(string dialogue, bool endChat)> RequestChatDialogue(string? prompt = null, params object[] objects)
        {
            if (prompt != null)
            {
                prompt = TextUtilities.FormatMarkupText(prompt, objects);
            }
            var response = await RequestObject<PrivateDialogue>(prompt) ?? throw new InvalidDataException($"Robot agent did not respond with a valid {typeof(PrivateDialogue).Name} object");
            return (response.Dialogue, response.TerminateConversation);
        }

        public async Task<string> RequestDialogue(string? prompt = null, params object[] objects)
        {
            if (prompt != null)
            {
                prompt = TextUtilities.FormatMarkupText(prompt, objects);
            }
            var response = await RequestObject<PublicDialogue>(prompt) ?? throw new InvalidDataException($"Robot agent did not respond with a valid {typeof(PublicDialogue).Name} object");
            return response.Dialogue;
        }

        public async Task<IOption> RequestUseAbility(IReadOnlyCollection<IOption> options, string? prompt = null, params object[] objects)
        {
            return await RequestOptionFromJson<UseAbilityChoice>(options, prompt, objects);
        }

        public async Task<IOption> RequestPlayerSelection(IReadOnlyCollection<IOption> options, string? prompt = null, params object[] objects)
        {
            return await RequestOptionFromJson<PlayerSelection>(options, prompt, objects);
        }

        public async Task<IOption> RequestTwoPlayersSelection(IReadOnlyCollection<IOption> options, string? prompt = null, params object[] objects)
        {
            return await RequestOptionFromJson<TwoPlayerSelection>(options, prompt, objects);
        }

        public async Task<IOption> RequestCharacterSelection(IReadOnlyCollection<IOption> options, string? prompt = null, params object[] objects)
        {
            return await RequestOptionFromJson<CharacterSelection>(options, prompt, objects);
        }

        public async Task<IOption> RequestVote(IReadOnlyCollection<IOption> options, string? prompt = null, params object[] objects)
        {
            return await RequestOptionFromJson<Vote>(options, prompt, objects);
        }

        public async Task<IOption> RequestShenanigans(IReadOnlyCollection<IOption> options, string? prompt = null, params object[] objects)
        {
            return await RequestOptionFromJson<PublicAction>(options, prompt, objects);
        }

        private async Task<IOption> RequestOptionFromJson<T>(IReadOnlyCollection<IOption> options, string? prompt = null, params object[] objects) where T: IOptionSelection
        {
            if (prompt != null)
            {
                prompt = TextUtilities.FormatMarkupText(prompt, objects);
            }
            for (int retry = 0; retry < 3; retry++)
            {
                var response = await RequestObject<T>(prompt) ?? throw new InvalidDataException($"Robot agent did not respond with a valid {typeof(T).Name} object");
                var result = response.PickOption(options);
                if (result != null)
                {
                    return result;
                }
                prompt = response.NoMatchingOptionPrompt(options);
            }
            return options.First(option => option is PassOption);
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
