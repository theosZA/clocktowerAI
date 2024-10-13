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

        public GameChat(string chatModel, string reasoningModel, string playerName, string personality, IReadOnlyCollection<string> playerNames, string scriptName, IReadOnlyCollection<Character> script)
        {
            chatLogger = new(playerName);

            chat = new OpenAiChat();
            chat.OnChatMessageAdded += OnChatMessageAdded;
            chat.OnSubChatSummarized += OnSubChatSummarized;
            chat.OnAssistantRequest += OnAssistantRequest;
            chat.SystemMessage = SystemMessage.GetSystemMessage(playerName, personality, playerNames, scriptName, script);

            this.chatModel = chatModel;
            this.reasoningModel = reasoningModel;
        }

        public async Task NewPhase(Phase phase, int dayNumber)
        {
            chat.StartNewSubChat(PhaseName(phase, dayNumber));

            if (phase == Phase.Night && dayNumber > 1)
            {
                // Reason about all your information.
                var reasoningPrompt = "Use all the information you've learned so far together with logical reasoning to determine each player's likely alignment and character. " +
                                      "Format your answer as a list, one item for each player with the following information: name; dead or alive; whether you believe they're good or evil " +
                                      "(and in brackets how confident you are); character or characters that you think they're most likely to be (and in brackets how confident you are); " +
                                      "and the main pieces of information that led you to this conclusion. For example:\r\n" +
                                      "- **Zeke** - Alive - Good (very likely) - Slayer (almost certain) or Imp (unlikely) - I learned they were good on night 1 and so, " +
                                      "unless I was drunk or poisoned, I trust the private claim that they made to me during day 1.";
                chat.AddUserMessage(reasoningPrompt);
                await chat.GetAssistantResponse<string>(reasoningModel);

                // Summarize the previous day.
                var phaseToSummarize = PhaseName(Phase.Day, dayNumber - 1);
                var summarizePrompt = $"Please provide a detailed summary, in bullet-point form, of what happened and what you learned in {phaseToSummarize.ToLowerInvariant()}. " +
                                       "There should be a point for each private chat that you had; a point for the discussion around each nomination; " +
                                       "as well as points for any general public discussion or abilities publicly used.";
                await chat.SummarizeSubChat(phaseToSummarize, chatModel, summarizePrompt);
            }
        }

        public void AddMessage(string message)
        {
            chat.AddUserMessage(message);
        }

        public async Task<T?> Request<T>(string? prompt)
        {
            if (!string.IsNullOrEmpty(prompt))
            {
                AddMessage(prompt);
            }
            return await chat.GetAssistantResponse<T>(chatModel);
        }

        private static string PhaseName(Phase phase, int dayNumber)
        {
            return phase == Phase.Setup ? "Set-up" : $"{phase} {dayNumber}";
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

        private readonly IChat chat;
        private readonly ChatLogger chatLogger;
        private readonly string chatModel;
        private readonly string reasoningModel;
    }
}
