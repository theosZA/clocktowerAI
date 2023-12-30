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
        public IChatLogger? Logger
        {
            get => openAiChat.Logger;
            set => openAiChat.Logger = value;
        }

        public ITokenCounter? TokenCounter
        {
            get => openAiChat.TokenCounter;
            set => openAiChat.TokenCounter = value;
        }

        public GameChat(string playerName, IReadOnlyCollection<string> playerNames, IReadOnlyCollection<Character> script, IChatLogger? logger, ITokenCounter? tokenCounter)
        {
            Logger = logger;
            TokenCounter = tokenCounter;

            openAiChat.SystemMessage = SystemMessage.GetSystemMessage(playerName, playerNames, script);
        }

        public async Task NewPhase(Phase phase, int dayNumber)
        {
            var phaseName = phase == Phase.Setup ? "Set-up" : $"{phase} {dayNumber}";
            var summarizePrompt = phase == Phase.Day ? $"Please provide a detailed summary, in bullet-point form, of what happened and what you learned in {phaseName.ToLowerInvariant()}. " +
                                                        "There should be a point for each private chat that you had; a point for the discussion around each nomination; " +
                                                        "as well as points for any general public discussion or abilities publicly used. There's no need to provide any concluding remarks - just the detailed points are enough."
                                                     : null;
            await openAiChat.StartNewSubChat(phaseName, summarizePrompt);
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

        private readonly OpenAiChat openAiChat = new OpenAiChat("gpt-3.5-turbo-1106");
    }
}
