namespace Clocktower.OpenAiApi
{
    /// <summary>
    /// All the messages that a player has seen or sent in a particular phase (day or night).
    /// The messages for an entire day will be summarized once the day concludes.
    /// </summary>
    internal class PhaseChat
    {
        public Phase Phase { get; private set; }
        public int DayNumber { get; private set; }

        public bool Summarized => summary != null;
        public IReadOnlyCollection<(Role role, string message)> Messages => summary == null ? messages
                                                                                            : new[] { (Role.Assistant, summary) };

        public PhaseChat(ChatCompletionApi chatCompletionApi, Phase phase, int dayNumber, IChatLogger chatLogger)
        {
            this.chatCompletionApi = chatCompletionApi;
            this.chatLogger = chatLogger;

            Phase = phase;
            DayNumber = dayNumber;
            Add(Role.User, PhaseText);
        }

        public void AddSystemMessage(string message)
        {
            messages.Insert(0, (Role.System, message));
            chatLogger.Log(Role.System, message);
        }

        public void AddUserMessage(string message)
        {
            Add(Role.User, message);
        }

        public async Task<string> Request(string? prompt, IReadOnlyCollection<PhaseChat> previousPhases)
        {
            if (!string.IsNullOrEmpty(prompt))
            {
                Add(Role.User, prompt);
            }

            var messagesToSend = previousPhases.SelectMany(phase => phase.Messages)
                                               .Concat(messages);
            var response = await chatCompletionApi.RequestChatCompletion(messagesToSend);
            Add(Role.Assistant, response);
            return response;
        }

        /// <summary>
        /// Removes the last few messages from the list of messages. Useful when you don't want unneeded messages cluttering up the chat log.
        /// </summary>
        /// <param name="messageCount">The number of messages to remove.</param>
        public void Trim(int messageCount)
        {
            messages.RemoveRange(messages.Count - messageCount, messageCount);
        }

        /// <summary>
        /// Creates a summarized version of the chat for this phase.
        /// </summary>
        public async Task Summarize(IEnumerable<PhaseChat> previousPhases)
        {
            var messagesToSend = previousPhases.SelectMany(phase => phase.Messages)
                                               .Concat(messages)
                                               .Append((Role.User, $"Please provide a detailed summary, in bullet-point form, of what happened and what you learned in {PhaseText.ToLowerInvariant()}. " +
                                                                    "There should be a point for each private chat that you had; a point for the discussion around each nomination; " +
                                                                    "as well as points for any general public discussion or abilities publicly used. There's no need to provide any concluding remarks - just the detailed points are enough."));
            var summaryResponse = await chatCompletionApi.RequestChatCompletion(messagesToSend);
            if (!summaryResponse.StartsWith(PhaseText))
            {
                summaryResponse = summaryResponse.Insert(0, $"{PhaseText}: ");
            }
            summary = summaryResponse;
            chatLogger.LogSummary(Phase, DayNumber, summaryResponse);
        }

        private void Add(Role role, string message)
        {
            messages.Add((role, message));
            chatLogger.Log(role, message);
        }

        private string PhaseText => Phase switch
        {
            Phase.Setup => "Setup",
            Phase.Night => $"Night {DayNumber}",
            Phase.Day => $"Day {DayNumber}",
            _ => throw new InvalidOperationException($"Unknown phase {Phase}")
        };

        private readonly ChatCompletionApi chatCompletionApi;
        private readonly IChatLogger chatLogger;

        private readonly List<(Role role, string message)> messages = new();
        private string? summary;
    }
}
