using Newtonsoft.Json;
using System.Diagnostics;

namespace OpenAi.ChatCompletionApi
{
    internal class SubChat
    {
        public event IChat.ChatMessageAddedHandler? OnChatMessageAdded;
        public event IChat.ChatMessagesRemovedHandler? OnChatMessagesRemoved;
        public event IChat.SubChatSummarizedHandler? OnSubChatSummarized;
        public event IChat.AssistantRequestHandler? OnAssistantRequest;

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
            OnChatMessageAdded?.Invoke(subChatName, role, message);
        }

        public async Task<T?> GetAssistantResponse<T>(IEnumerable<SubChat> previousSubChats)
        {
            var messagesToSend = previousSubChats.SelectMany(subChat => subChat.Messages)
                                                 .Concat(messages)
                                                 .ToList();
            var (response, promptTokens, completionTokens, totalTokens) = await chatCompletionApi.RequestChatCompletion<T>(messagesToSend);
            OnAssistantRequest?.Invoke(subChatName, isSummaryRequest: false, messagesToSend, response, promptTokens, completionTokens, totalTokens);

            AddMessage(Role.Assistant, response);
            return StringToType<T>(response);
        }

        public async Task Summarize(IEnumerable<SubChat> previousSubChats)
        {
            if (summarizePrompt == null)
            {   // No summary if there's no prompt.
                return;
            }
            var messagesToSend = previousSubChats.SelectMany(subChat => subChat.Messages)
                                                 .Concat(messages)
                                                 .Append((Role.User, summarizePrompt))
                                                 .ToList();
            var (summaryResponse, promptTokens, completionTokens, totalTokens) = await chatCompletionApi.RequestChatCompletion<string>(messagesToSend);
            OnAssistantRequest?.Invoke(subChatName, isSummaryRequest: true, messagesToSend, summaryResponse, promptTokens, completionTokens, totalTokens);

            if (!summaryResponse.StartsWith(subChatName))
            {
                summaryResponse = summaryResponse.Insert(0, $"{subChatName}: ");
            }
            summary = summaryResponse;
            OnSubChatSummarized?.Invoke(subChatName, summary);
        }

        public void TrimMessages(int count)
        {
            int startIndex = messages.Count - count;
            messages.RemoveRange(startIndex, count);
            OnChatMessagesRemoved?.Invoke(subChatName, startIndex, count);
        }

        private static T? StringToType<T>(string response)
        {
            try
            {
                if (typeof(T) == typeof(string))
                {
                    return (T)(object)response;
                }

                return JsonConvert.DeserializeObject<T>(response);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.ToString());
                return default;
            }
        }

        private readonly List<(Role role, string message)> messages = new();
        private readonly ChatCompletionApi chatCompletionApi;
        private readonly string subChatName;
        private readonly string? summarizePrompt;
        private string? summary;
    }
}
