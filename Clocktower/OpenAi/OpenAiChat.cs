using OpenAi.ChatCompletionApi;

namespace OpenAi
{
    /// <summary>
    /// An ongoing conversation with an Open AI Chat Completion assistant.
    /// </summary>
    public class OpenAiChat : IChat
    {
        public IChatLogger? Logger
        {
            get => logger;

            set
            {
                logger = value;
                foreach (var subChat in subChats)
                {
                    subChat.Logger = value;
                }
            }
        }

        public ITokenCounter? TokenCounter
        {
            get => tokenCounter;

            set
            {
                tokenCounter = value;
                foreach (var subChat in subChats)
                {
                    subChat.TokenCounter = value;
                }
            }
        }

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

            subChats.Add(new SubChat(chatCompletionApi, name, summarizePrompt)
            {
                Logger = logger,
                TokenCounter = tokenCounter
            });
        }

        public void TrimMessages(int count)
        {
            subChats.Last().TrimMessages(count);
        }

        private IChatLogger? logger;
        private ITokenCounter? tokenCounter;
        private readonly List<SubChat> subChats = new List<SubChat>();
        private readonly ChatCompletionApi.ChatCompletionApi chatCompletionApi;
    }
}
