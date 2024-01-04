using OpenAi.ChatCompletionApi;

namespace OpenAi
{
    /// <summary>
    /// An ongoing conversation with an Open AI Chat Completion assistant.
    /// </summary>
    public class OpenAiChat : IChat
    {
        public event IChat.ChatMessageAddedHandler? OnChatMessageAdded;
        public event IChat.ChatMessagesRemovedHandler? OnChatMessagesRemoved;
        public event IChat.SubChatSummarizedHandler? OnSubChatSummarized;
        public event IChat.AssistantRequestHandler? OnAssistantRequest;

        public string SystemMessage
        {
            get => subChats[0].Messages.FirstOrDefault(message => message.role == Role.System).message;

            set => subChats[0].AddMessage(Role.System, value);
        }

        /// <summary>
        /// Constructor to initiate a conversation with an Open AI Chat Completion assistant.
        /// </summary>
        /// <param name="model">The Open AI Chat Completion model to use. Refer to the GPT models listed at https://platform.openai.com/docs/models for possible values.</param>
        public OpenAiChat(string model)
        {
            chatCompletionApi = new ChatCompletionApi.ChatCompletionApi(model);
            // Start with a default sub-chat useful for holding the system message and any other non-summarizable pre-chat messages.
            subChats.Add(new SubChat(chatCompletionApi, string.Empty, summarizePrompt: null));
        }

        public void AddUserMessage(string message)
        {
            subChats.Last().AddMessage(Role.User, message);
        }

        public async Task<string> GetAssistantResponse()
        {
            return await subChats.Last().GetAssistantResponse(subChats.SkipLast(1));
        }

        public async Task StartNewSubChat(string name, string? summarizePrompt = null)
        {
            await subChats.Last().Summarize(subChats.SkipLast(1));

            var subChat = new SubChat(chatCompletionApi, name, summarizePrompt);
            subChat.OnChatMessageAdded += InternalChatMessageAddedHandler;
            subChat.OnChatMessagesRemoved += InternalChatMessagesRemovedHandler;
            subChat.OnSubChatSummarized += InternalSubChatSummarizedHandler;
            subChat.OnAssistantRequest += InternalAssistantRequestHandler;
            subChats.Add(subChat);
        }

        public void TrimMessages(int count)
        {
            subChats.Last().TrimMessages(count);
        }

        private void InternalChatMessageAddedHandler(string subChatName, Role role, string message)
        {
            OnChatMessageAdded?.Invoke(subChatName, role, message);
        }

        private void InternalChatMessagesRemovedHandler(string subChatName, int startIndex, int count)
        {
            OnChatMessagesRemoved?.Invoke(subChatName, startIndex, count);
        }

        private void InternalSubChatSummarizedHandler(string subChatName, string summary)
        {
            OnSubChatSummarized?.Invoke(subChatName, summary);
        }

        private void InternalAssistantRequestHandler(string subChatName, bool isSummaryRequest, IReadOnlyCollection<(Role role, string message)> messages,
                                                     string response, int promptTokens, int completionTokens, int totalTokens)
        {
            OnAssistantRequest?.Invoke(subChatName, isSummaryRequest, messages, response, promptTokens, completionTokens, totalTokens);
        }

        private readonly List<SubChat> subChats = new List<SubChat>();
        private readonly ChatCompletionApi.ChatCompletionApi chatCompletionApi;
    }
}
