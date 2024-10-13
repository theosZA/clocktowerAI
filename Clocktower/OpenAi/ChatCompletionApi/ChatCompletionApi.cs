﻿using OpenAi.ChatCompletionApi.Model;
using Polly;
using Polly.Retry;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace OpenAi.ChatCompletionApi
{
    internal static class ChatCompletionApi
    {
        static ChatCompletionApi()
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("OPENAI_APIKEY", EnvironmentVariableTarget.User));
        }

        public static async Task<(string response, int promptTokens, int completionTokens, int totalTokens)> RequestChatCompletion<T>(string model, IEnumerable<(Role role, string message)> messages)
        {
            var request = BuildChatCompletionRequest<T>(model, messages);
            using var response = await RequestChatCompletion(request);
            var chatResponse = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>() ?? throw new Exception("No chat completion received from Open API");
            var usage = chatResponse.Usage;
            return (chatResponse.Choices.First().Message.Content, usage.PromptTokens, usage.CompletionTokens, usage.TotalTokens);
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

        private static ChatCompletionRequest BuildChatCompletionRequest<T>(string model, IEnumerable<(Role role, string message)> messages)
        {
            var request = new ChatCompletionRequest
            {
                Model = model,
                Messages = messages.Select(pair => BuildChatMessage(pair.role, pair.message)).ToList()
            };
            if (typeof(T) != typeof(string))
            {
                request.ResponseFormat = ResponseFormat.ResponseFormatFromType<T>();
            }
            return request;
        }

        private static ChatMessage BuildChatMessage(Role role, string message)
        {
            return new ChatMessage
            {
                Role = role.ToString().ToLowerInvariant(),
                Content = message
            };
        }

        private static readonly HttpClient httpClient = new(new LoggingHandler(new StreamWriter($"ChatCompletions-{DateTime.UtcNow.ToString("yyyyMMddTHHmmss")}.log"), new HttpClientHandler()))
        {
            BaseAddress = new Uri("https://api.openai.com/v1/")
        };

        private static readonly AsyncRetryPolicy policy = Policy.Handle<HttpRequestException>()
                                                                .Or<TaskCanceledException>()
                                                                .WaitAndRetryAsync(retryCount: 9,
                                                                                   sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(5 * Math.Pow(2, retryAttempt))); // Exponential backoff formula

        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }
}
