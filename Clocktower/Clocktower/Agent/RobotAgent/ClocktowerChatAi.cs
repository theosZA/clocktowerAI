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

        public async Task<(string dialogue, bool endChat)> RequestChatDialogue(string prompt)
        {
            var response = await RequestObject<PrivateDialogue>(prompt);
            return (response.Dialogue, response.TerminateConversation);
        }

        public async Task<string> RequestDialogue(string prompt)
        {
            return (await RequestObject<PublicDialogue>(prompt)).Dialogue;
        }

        public async Task<IOption> RequestUseAbility(IReadOnlyCollection<IOption> options, string prompt)
        {
            return await RequestOptionFromJson<UseAbilityChoice>(options, prompt);
        }

        public async Task<IOption> RequestPlayerSelection(IReadOnlyCollection<IOption> options, string prompt)
        {
            return await RequestOptionFromJson<PlayerSelection>(options, prompt);
        }

        public async Task<IOption> RequestTwoPlayersSelection(IReadOnlyCollection<IOption> options, string prompt)
        {
            return await RequestOptionFromJson<TwoPlayerSelection>(options, prompt);
        }

        public async Task<IOption> RequestCharacterSelection(IReadOnlyCollection<IOption> options, string prompt)
        {
            return await RequestOptionFromJson<CharacterSelection>(options, prompt);
        }

        public async Task<IOption> RequestVote(IReadOnlyCollection<IOption> options, string prompt)
        {
            return await RequestOptionFromJson<Vote>(options, prompt);
        }

        public async Task<IOption> RequestShenanigans(IReadOnlyCollection<IOption> options, string prompt)
        {
            return await RequestOptionFromJson<PublicAction>(options, prompt);
        }

        private async Task<IOption> RequestOptionFromJson<T>(IReadOnlyCollection<IOption> options, string prompt) where T: IOptionSelection
        {
            for (int retry = 0; retry < 3; retry++)
            {
                var response = await RequestObject<T>(prompt);
                var result = response.PickOption(options);
                if (result != null)
                {
                    return result;
                }
                prompt = response.NoMatchingOptionPrompt(options);
            }
            return options.First(option => option is PassOption);
        }

        private async Task<T> RequestObject<T>(string prompt)
        {
            return await gameChat.Request<T>(prompt) ?? throw new InvalidDataException($"Robot agent did not respond with a valid {typeof(T).Name} object");
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

        private readonly GameChat gameChat;
    }
}
