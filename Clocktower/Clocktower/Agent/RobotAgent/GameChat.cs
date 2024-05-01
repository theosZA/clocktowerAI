using Clocktower.Game;
using OpenAi;

namespace Clocktower.Agent.RobotAgent
{
    /// <summary>
    /// Holds all messages the player has received and sent this game, can send and receive additional messages, 
    /// and will request the player summarize messages at the end of each day.
    /// </summary>
    internal class GameChat
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

        public GameChat(string model, string playerName, string personality, IReadOnlyCollection<string> playerNames, IReadOnlyCollection<Character> script)
        {
            chatLogger = new(playerName);

            openAiChat = new(model);
            openAiChat.OnChatMessageAdded += OnChatMessageAdded;
            openAiChat.OnSubChatSummarized += OnSubChatSummarized;
            openAiChat.OnAssistantRequest += OnAssistantRequest;
            openAiChat.SystemMessage = SystemMessage.GetSystemMessage(playerName, personality, playerNames, script);
        }

        public async Task NewPhase(Phase phase, int dayNumber)
        {
            var phaseName = phase == Phase.Setup ? "Set-up" : $"{phase} {dayNumber}";
            var summarizePrompt = phase == Phase.Day ? $"Please provide a detailed summary, in bullet-point form, of what happened and what you learned in {phaseName.ToLowerInvariant()}. " +
                                                        "There should be a point for each private chat that you had; a point for the discussion around each nomination; " +
                                                        "as well as points for any general public discussion or abilities publicly used. " +
                                                        "Conclude with a list of all players in the game, and for each player list whether they're alive or dead, " +
                                                        "whether you believe they're good or evil (and in brackets how confident you are of this opinion), " +
                                                        "and the character or characters that you think they're most likely to be (and in brackets how confident you are of this opinion). " +
                                                        "For example: * Zeke - Alive - Good (very likely) - Slayer (almost certain) or Imp (unlikely)."
                                                     : null;
            await openAiChat.StartNewSubChat(phaseName, summarizePrompt);
            openAiChat.AddUserMessage(phaseName);
        }

        public void AddMessage(string message)
        {
            openAiChat.AddUserMessage(message);
        }

        public async Task<string> Request(string? prompt)
        {
            if (!string.IsNullOrEmpty(prompt))
            {
                AddMessage(prompt);
            }
            return await openAiChat.GetAssistantResponse();
        }

        /// <summary>
        /// Removes the last few messages from the list of messages. Useful when you don't want unneeded messages cluttering up the chat log.
        /// Only applies to the current phase.
        /// </summary>
        /// <param name="messageCount">The number of messages to remove.</param>
        public void Trim(int messageCount)
        {
            openAiChat.TrimMessages(messageCount);
        }

        private void OnChatMessageAdded(string subChatName, Role role, string message)
        {
            chatLogger.MessageAdded(role, message);
            OnChatMessage?.Invoke(role, message);
        }

        private void OnSubChatSummarized(string subChatName, string summary)
        {
            chatLogger.SubChatSummarized(subChatName, summary);

            // The sub-chat name should by "Day NN".
            if (int.TryParse(subChatName[4..], out int dayNumber))
            {
                OnDaySummary?.Invoke(dayNumber, summary);
            }
        }

        private void OnAssistantRequest(string subChatName, bool isSummaryRequest, IReadOnlyCollection<(Role role, string message)> messages,
                                        string response, int promptTokens, int completionTokens, int totalTokens)
        {
            chatLogger.AssistantRequest(messages, response, promptTokens, completionTokens);
            OnTokenCount?.Invoke(promptTokens, completionTokens, totalTokens);
        }

        private readonly OpenAiChat openAiChat;
        private readonly ChatLogger chatLogger;
    }
}
