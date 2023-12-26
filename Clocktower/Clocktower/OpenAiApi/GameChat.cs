using Clocktower.Game;

namespace Clocktower.OpenAiApi
{
    /// <summary>
    /// Holds all messages the player has received and sent this game, can
    /// send and receive additional messages, and will request the player 
    /// summarize messages when appropriate.
    /// </summary>
    internal class GameChat
    {
        public GameChat(string playerName, IReadOnlyCollection<string> playerNames, IReadOnlyCollection<Character> script, ChatCompletionApi chatCompletionApi, IChatLogger chatLogger)
        {
            this.chatCompletionApi = chatCompletionApi;
            this.chatLogger = chatLogger;

            var phase = new PhaseChat(chatCompletionApi, Phase.Setup, 0, chatLogger);
            phase.AddSystemMessage(SystemMessage.GetSystemMessage(playerName, playerNames, script));
            phases.Add(phase);
        }

        public void NewPhase(Phase phase, int dayNumber)
        {
            phases.Add(new PhaseChat(chatCompletionApi, phase, dayNumber, chatLogger));
        }

        public void AddMessage(string message)
        {
            CurrentPhase.AddUserMessage(message);
        }

        public async Task<string> Request(string? prompt)
        {
            await SummarizeIfNeeded();
            return await CurrentPhase.Request(prompt, phases.SkipLast(1).ToList());
        }

        /// <summary>
        /// Removes the last few messages from the list of messages. Useful when you don't want unneeded messages cluttering up the chat log.
        /// Only applies to the current phase.
        /// </summary>
        /// <param name="messageCount">The number of messages to remove.</param>
        public void Trim(int messageCount)
        {
            CurrentPhase.Trim(messageCount);
        }

        private async Task SummarizeIfNeeded()
        {
            // Check all phase chats except the current one.
            for (int i = 0; i < phases.Count - 1; i++)
            {
                // Summarize day phases that haven't already been summarized.
                if (phases[i].Phase == Phase.Day && !phases[i].Summarized)
                {
                    await phases[i].Summarize(phases.Take(i));
                }
            }
        }

        private PhaseChat CurrentPhase => phases.Last();

        private readonly List<PhaseChat> phases = new();
        private readonly ChatCompletionApi chatCompletionApi;
        private readonly IChatLogger chatLogger;
    }
}
