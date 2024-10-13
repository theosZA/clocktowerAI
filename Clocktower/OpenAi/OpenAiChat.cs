using OpenAi.ChatCompletionApi;

namespace OpenAi
{
    /// <summary>
    /// An ongoing conversation with an Open AI Chat Completion assistant.
    /// </summary>
    public class OpenAiChat : IChat
    {
        public event IChat.ChatMessageAddedHandler? OnChatMessageAdded;
        public event IChat.SubChatSummarizedHandler? OnSubChatSummarized;
        public event IChat.AssistantRequestHandler? OnAssistantRequest;

        public string SystemMessage
        {
            get => subChats[0].Messages.FirstOrDefault(message => message.role == Role.System).message;

            set => subChats[0].AddMessage(Role.System, value);
        }

        /// <summary>
        /// Constructor for a conversation with an Open AI Chat Completion assistant. A default unnamed sub-chat is created.
        /// </summary>
        /// <param name="messages">An optional list of messages added to the default sub-chat.</param>
        public OpenAiChat(IEnumerable<(Role role, string message)>? messages = null)
        {
            AddNewSubChat(string.Empty, messages);
        }

        public void StartNewSubChat(string name)
        {
            AddNewSubChat(name);
        }

        public void AddUserMessage(string message)
        {
            subChats.Last().AddMessage(Role.User, message);
        }

        public async Task<T?> GetAssistantResponse<T>(string model)
        {
            return await subChats.Last().GetAssistantResponse<T>(model, subChats.SkipLast(1));
        }

        public async Task SummarizeSubChat(string subChatName, string model, string prompt)
        {
            var subChatIndex = subChats.FindIndex(subChat => subChat.Name == subChatName);
            await subChats[subChatIndex].Summarize(model, prompt, subChats.GetRange(0, subChatIndex));
        }

        private void AddNewSubChat(string name, IEnumerable<(Role role, string message)>? messages = null)
        {
            var subChat = new SubChat(name);
            subChat.OnChatMessageAdded += InternalChatMessageAddedHandler;
            subChat.OnSubChatSummarized += InternalSubChatSummarizedHandler;
            subChat.OnAssistantRequest += InternalAssistantRequestHandler;

            if (messages != null)
            {
                foreach (var (role, message) in messages)
                {
                    subChat.AddMessage(role, message);
                }
            }

            subChats.Add(subChat);
        }

        private void InternalChatMessageAddedHandler(string subChatName, Role role, string message)
        {
            OnChatMessageAdded?.Invoke(subChatName, role, message);
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

        private readonly List<SubChat> subChats = new();
    }
}
