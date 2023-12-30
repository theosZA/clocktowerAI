namespace OpenAi.ChatCompletionApi
{
    internal class SubChat
    {
        /// <summary>
        /// The logger to which all messages in the chat and all summaries made of sub-chats will be directed.
        /// </summary>
        public IChatLogger? Logger { get; set; }

        /// <summary>
        /// The counter to which all updates on token usage will be directed.
        /// </summary>
        public ITokenCounter? TokenCounter { get; set; }

        public IReadOnlyCollection<(Role role, string message)> Messages => summary == null ? messages
                                                                                            : new[] { (Role.Assistant, summary) };

        public SubChat(ChatCompletionApi chatCompletionApi, string subChatName, string? summarizePrompt)
        {
            this.chatCompletionApi = chatCompletionApi;
            this.subChatName = subChatName;
            this.summarizePrompt = summarizePrompt;
        }

        public void AddMessage(Role role, string message)
        {
            if (role == Role.System)
            {   // Add the system message to the head of the sub-chat.
                messages.Insert(0, (role, message));
            }
            else
            {   // All other messages go to the tail of the sub-chat.
                messages.Add((role, message));
            }
            Logger?.Log(subChatName, role, message);
        }

        public async Task<string> GetAssistantResponse(IEnumerable<SubChat> previousSubChats)
        {
            var messagesToSend = previousSubChats.SelectMany(subChat => subChat.Messages)
                                                 .Concat(messages);
            var response = await chatCompletionApi.RequestChatCompletion(messagesToSend, TokenCounter);
            AddMessage(Role.Assistant, response);
            return response;
        }

        public async Task Summarize(IEnumerable<SubChat> previousSubChats)
        {
            if (summarizePrompt == null)
            {   // No summary if there's no prompt.
                return;
            }
            var messagesToSend = previousSubChats.SelectMany(subChat => subChat.Messages)
                                                 .Concat(messages)
                                                 .Append((Role.User, summarizePrompt));
            var summaryResponse = await chatCompletionApi.RequestChatCompletion(messagesToSend, TokenCounter);
            if (!summaryResponse.StartsWith(subChatName))
            {
                summaryResponse = summaryResponse.Insert(0, $"{subChatName}: ");
            }
            summary = summaryResponse;
            Logger?.LogSummary(subChatName, summaryResponse);
        }

        public void TrimMessages(int count)
        {
            messages.RemoveRange(messages.Count - count, count);
        }

        private readonly List<(Role role, string message)> messages = new();
        private readonly ChatCompletionApi chatCompletionApi;
        private readonly string subChatName;
        private readonly string? summarizePrompt;
        private string? summary;
    }
}
