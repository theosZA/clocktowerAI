namespace OpenAi
{
    /// <summary>
    /// Interface describing a single ongoing conversation with a Chat assistant. 
    /// To help manage the size of the conversation and limit token usage, a chat is divided into multiple sub-chats, each of which
    /// can be summarized by the Chat assistant once the next sub-chat begins.
    /// </summary>
    public interface IChat
    {
        /// <summary>
        /// The logger to which all messages in the chat and all summaries made of sub-chats will be directed.
        /// </summary>
        IChatLogger? Logger { get; set; }

        /// <summary>
        /// The counter to which all updates on token usage will be directed.
        /// </summary>
        ITokenCounter? TokenCounter { get; set; }

        /// <summary>
        /// The single system message that will be included at the start of each chat completion request. It is described by OpenAI as:
        /// The system message helps set the behavior of the assistant. For example, you can modify the personality of the assistant or provide specific
        /// instructions about how it should behave throughout the conversation. However note that the system message is optional and the model’s behavior
        /// without a system message is likely to be similar to using a generic message such as "You are a helpful assistant."
        /// </summary>
        string SystemMessage { get; set; }

        /// <summary>
        /// Starts a new sub-chat with the given name. A sub-chat should correspond to a portion of the overall chat for which a convenient summary
        /// can be made (or for which no summarization is desired). This will automatically close off the previous sub-chat and summarize it if possible.
        /// </summary>
        /// <param name="name">The name by which to identify this sub-chat in summaries and when logging.</param>
        /// <param name="summarizePrompt">A prompt to provide the Chat assistant to have it summarize the sub-chat. If left as null, then no summarization of this new sub-chat will be done.</param>
        /// <remarks>If no sub-chat is created before the first message is added, then a new non-summarizable, unnamed sub-chat will be created.</remarks>
        Task StartNewSubChat(string name, string? summarizePrompt = null);

        /// <summary>
        /// Adds a new user message to the tail of current sub-chat.
        /// </summary>
        /// <param name="message">The new message to add.</param>
        void AddUserMessage(string message);

        /// <summary>
        /// Provides the full chat (with sub-chats summarized if possible) to the Chat assistant and asynchronously returns an assistant message.
        /// This message is also added to the current sub-chat.
        /// </summary>
        /// <returns>The resulting assistant message.</returns>
        Task<string> GetAssistantResponse();

        /// <summary>
        /// Removes the last few messages from the current sub-chat. Useful when you don't want valueless messages cluttering up the chat history.
        /// </summary>
        /// <param name="count">The number of messages to remove.</param>
        void TrimMessages(int count);
    }
}
