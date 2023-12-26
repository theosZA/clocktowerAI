using Clocktower.OpenAiApi.Model;
using Polly;
using Polly.Retry;
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
            var request = BuildChatCompletionRequest(messages);
            using var response = await RequestChatCompletion(request);
            var chatResponse = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>() ?? throw new Exception("No chat completion received from Open API");
            var usage = chatResponse.Usage;
            tokenCounter.NewTokenUsage(usage.PromptTokens, usage.CompletionTokens, usage.TotalTokens);
            return chatResponse.Choices.First().Message.Content;
        }

        private static async Task<HttpResponseMessage> RequestChatCompletion(ChatCompletionRequest request)
        {
            return await policy.ExecuteAsync(async () =>
            {
                HttpResponseMessage? response = null;
                try
                {
                    response = await httpClient.PostAsJsonAsync("chat/completions", request, jsonSerializerOptions);
                    response.EnsureSuccessStatusCode();
                    return response;
                }
                catch (Exception)
                {
                    response?.Dispose();
                    throw;
                }
            });
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

        private static readonly AsyncRetryPolicy policy = Policy.Handle<HttpRequestException>()
                                                                .WaitAndRetryAsync(retryCount: 9,
                                                                                   sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))); // Exponential backoff formula

        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition =System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull 
        };
    }
}
