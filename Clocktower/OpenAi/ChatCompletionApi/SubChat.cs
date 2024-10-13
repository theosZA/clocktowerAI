using Newtonsoft.Json;
using System.Diagnostics;

namespace OpenAi.ChatCompletionApi
{
    internal class SubChat
    {
        public event IChat.ChatMessageAddedHandler? OnChatMessageAdded;
        public event IChat.SubChatSummarizedHandler? OnSubChatSummarized;
        public event IChat.AssistantRequestHandler? OnAssistantRequest;

        public string Name { get; private init; }
        public IReadOnlyCollection<(Role role, string message)> Messages => messages;

        public SubChat(string name)
        {
            Name = name;
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
            OnChatMessageAdded?.Invoke(Name, role, message);
        }

        public async Task<T?> GetAssistantResponse<T>(string model, IEnumerable<SubChat> previousSubChats)
        {
            var messagesToSend = previousSubChats.SelectMany(subChat => subChat.Messages)
                                                 .Concat(messages)
                                                 .ToList();

            if (model == "o1-preview" || model == "o1-mini")
            {   // These models do not currently support system messages. Replace the system messages with user messages.
                messagesToSend = messagesToSend.Select(m => (m.role == Role.System ? Role.User : m.role, m.message))
                                               .ToList();
            }

            var (response, promptTokens, completionTokens, totalTokens) = await ChatCompletionApi.RequestChatCompletion<T>(model, messagesToSend);
            OnAssistantRequest?.Invoke(Name, isSummaryRequest: false, messagesToSend, response, promptTokens, completionTokens, totalTokens);

            AddMessage(Role.Assistant, response);
            return StringToType<T>(response);
        }

        public async Task Summarize(string model, string prompt, IEnumerable<SubChat> previousSubChats)
        {
            var messagesToSend = previousSubChats.SelectMany(subChat => subChat.Messages)
                                                 .Concat(messages)
                                                 .Append((Role.User, prompt))
                                                 .ToList();
            var (summaryResponse, promptTokens, completionTokens, totalTokens) = await ChatCompletionApi.RequestChatCompletion<string>(model, messagesToSend);
            OnAssistantRequest?.Invoke(Name, isSummaryRequest: true, messagesToSend, summaryResponse, promptTokens, completionTokens, totalTokens);

            messages.Clear();
            messages.Add((Role.User, prompt));
            messages.Add((Role.Assistant, summaryResponse));

            OnSubChatSummarized?.Invoke(Name, summaryResponse);
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
    }
}
