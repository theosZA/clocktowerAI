namespace OpenAi
{
    /// <summary>
    /// Interface describing a single ongoing conversation with a Chat assistant. 
    /// To help manage the size of the conversation and limit token usage, a chat is divided into multiple sub-chats, each of which
    /// can be summarized by the Chat assistant once the next sub-chat begins.
    /// </summary>
    public interface IChat
    {
        delegate void ChatMessageAddedHandler(string subChatName, Role role, string message);
        /// <summary>
        /// Event triggered when a new message is added to the chat. Messages are always added to the end of the specified sub-chat
        /// except when the role is Role.System for which messages are added to the beginning of the sub-chat.
        /// </summary>
        event ChatMessageAddedHandler OnChatMessageAdded;

        delegate void SubChatSummarizedHandler(string subChatName, string summary);
        /// <summary>
        /// Event triggered when a sub-chat is summarized.
        /// </summary>
        event SubChatSummarizedHandler OnSubChatSummarized;

        delegate void AssistantRequestHandler(string subChatName, bool isSummaryRequest, IReadOnlyCollection<(Role role, string message)> messages,
                                              string response, int promptTokens, int completionTokens, int totalTokens);
        /// <summary>
        /// Event triggered when a response is received for an assistant request. Note that this will include behind-the-scenes requests to
        /// summarize a sub-chat. These can be ignored if desired by looking at the isSummaryRequest delegate parameter.
        /// </summary>
        event AssistantRequestHandler OnAssistantRequest;

        /// <summary>
        /// The single system message that will be included at the start of each chat completion request. It is described by OpenAI as:
        /// The system message helps set the behavior of the assistant. For example, you can modify the personality of the assistant or provide specific
        /// instructions about how it should behave throughout the conversation. However note that the system message is optional and the model’s behavior
        /// without a system message is likely to be similar to using a generic message such as "You are a helpful assistant."
        /// </summary>
        string SystemMessage { get; set; }

        /// <summary>
        /// Starts a new sub-chat with the given name. A sub-chat should correspond to a portion of the overall chat for which a convenient summary
        /// can be made (or for which no summarization is desired).
        /// </summary>
        /// <param name="name">The name by which to identify this sub-chat in summaries and when logging.</param>
        void StartNewSubChat(string name);

        /// <summary>
        /// Adds a new user message to the tail of current sub-chat.
        /// </summary>
        /// <param name="message">The new message to add.</param>
        void AddUserMessage(string message);

        /// <summary>
        /// Provides the full chat (with summarized sub-chats) to the Chat assistant and asynchronously returns an assistant message.
        /// This message is also added to the current sub-chat.
        /// </summary>
        /// <typeparam name="T">If the type is not <see cref="string"/>, then the response is provided in the form of an object of type T.</typeparam>
        /// <param name="model">The Open AI Chat Completion model to use. Refer to the GPT models listed at https://platform.openai.com/docs/models for possible values.</param>
        /// <returns>The resulting assistant message.</returns>
        Task<T?> GetAssistantResponse<T>(string model);

        /// <summary>
        /// Summarizes the specified sub-chat, replacing all messages in the sub-chat with the prompt and newly generated summary.
        /// </summary>
        /// <param name="subChatName">The name of the sub-chat to summarize.</param>
        /// <param name="model">The Open AI Chat Completion model to use. Refer to the GPT models listed at https://platform.openai.com/docs/models for possible values.</param>
        /// <param name="prompt">
        /// The prompt to provide to the Chat assistant to have it generate a summary of the current sub-chat. Note that the full chat (with previously summarized sub-chats) prior
        /// to this sub-chat is also included, so the prompt needs to be specific about which content is to be summarized.
        /// </param>
        Task SummarizeSubChat(string subChatName, string model, string prompt);
    }
}
