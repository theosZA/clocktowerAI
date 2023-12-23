using Clocktower.OpenAiApi.Model;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Clocktower.OpenAiApi
{
    internal class ChatCompletionApi
    {
        static ChatCompletionApi()
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("OPENAI_APIKEY", EnvironmentVariableTarget.User));
        }

        public ChatCompletionApi(ITokenCounter tokenCounter)
        {
            this.tokenCounter = tokenCounter;
        }

        public async Task<string> RequestChatCompletion(IEnumerable<(Role role, string message)> messages)
        {
            using var response = await httpClient.PostAsJsonAsync("chat/completions", BuildChatCompletionRequest(messages), jsonSerializerOptions);
            response.EnsureSuccessStatusCode();
            var chatResponse = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>() ?? throw new Exception("No chat completion received from Open API");
            var usage = chatResponse.Usage;
            tokenCounter.NewTokenUsage(usage.PromptTokens, usage.CompletionTokens, usage.TotalTokens);
            return chatResponse.Choices.First().Message.Content;
        }

        private static ChatCompletionRequest BuildChatCompletionRequest(IEnumerable<(Role role, string message)> messages)
        {
            return new ChatCompletionRequest
            {
                Model = "gpt-3.5-turbo-1106",
                Messages = messages.Select(pair => BuildChatMessage(pair.role, pair.message)).ToList()
            };
        }

        private static ChatMessage BuildChatMessage(Role role, string message)
        {
            return new ChatMessage
            {
                Role = role.ToString().ToLowerInvariant(),
                Content = message
            };
        }

        private readonly ITokenCounter tokenCounter;

        private static readonly HttpClient httpClient = new(new LoggingHandler(new StreamWriter($"ChatCompletions-{DateTime.UtcNow.ToString("yyyyMMddTHHmmss")}.log"), new HttpClientHandler()))
        {
            BaseAddress = new Uri("https://api.openai.com/v1/")
        };

        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition =System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull 
        };
    }
}
