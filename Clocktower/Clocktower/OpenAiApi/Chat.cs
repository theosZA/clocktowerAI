namespace Clocktower.OpenAiApi
{
    internal class Chat
    {
        public Chat(string systemMessage)
        {
            messages.Add((Role.System, systemMessage));
        }

        public void AddUserMessage(string userMessage)
        {
            messages.Add((Role.User, userMessage));
        }

        public async Task<string> RequestChatResponse(string? userMessage = null)
        {
            if (!string.IsNullOrWhiteSpace(userMessage))
            {
                AddUserMessage(userMessage);
            }

            var response = await ChatCompletionApi.RequestChatCompletion(messages);
            messages.Add((Role.Assistant, response));
            return response;
        }

        private readonly List<(Role role, string message)> messages = new();
    }
}
