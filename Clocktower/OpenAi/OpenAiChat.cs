﻿using OpenAi.ChatCompletionApi;

namespace OpenAi
{
    /// <summary>
    /// An ongoing conversation with an Open AI Chat Completion assistant.
    /// </summary>
    public class OpenAiChat : IChat
    {
        public event IChat.ChatMessageAddedHandler? OnChatMessageAdded;
        public event IChat.ChatMessagesRemovedHandler? OnChatMessagesRemoved;
        public event IChat.SubChatSummarizedHandler? OnSubChatSummarized;
        public event IChat.AssistantRequestHandler? OnAssistantRequest;

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
            AddNewSubChat(string.Empty, summarizePrompt: null);
        }

        /// <summary>
        /// Constructor to provide an in-progress conversation with an Open AI Chat Completion assistant
        /// where all chat has happened on the default sub-chat.
        /// </summary>
        /// <param name="model">The Open AI Chat Completion model to use. Refer to the GPT models listed at https://platform.openai.com/docs/models for possible values.</param>
        /// <param name="messages">A list of messages (with the role whose message it is), all in the default sub-chat.</param>
        public OpenAiChat(string model, IEnumerable<(Role role, string message)> messages)
        {
            chatCompletionApi = new ChatCompletionApi.ChatCompletionApi(model);
            AddNewSubChat(string.Empty, summarizePrompt: null, messages);
        }

        public void AddUserMessage(string message)
        {
            subChats.Last().AddMessage(Role.User, message);
        }

        public async Task<T?> GetAssistantResponse<T>()
        {
            return await subChats.Last().GetAssistantResponse<T>(subChats.SkipLast(1));
        }

        public async Task StartNewSubChat(string name, string? summarizePrompt = null)
        {
            await subChats.Last().Summarize(subChats.SkipLast(1));
            AddNewSubChat(name, summarizePrompt);
        }

        public void TrimMessages(int count)
        {
            subChats.Last().TrimMessages(count);
        }

        private void AddNewSubChat(string name, string? summarizePrompt = null, IEnumerable<(Role role, string message)>? messages = null)
        {
            var subChat = new SubChat(chatCompletionApi, name, summarizePrompt);
            subChat.OnChatMessageAdded += InternalChatMessageAddedHandler;
            subChat.OnChatMessagesRemoved += InternalChatMessagesRemovedHandler;
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

        private void InternalChatMessagesRemovedHandler(string subChatName, int startIndex, int count)
        {
            OnChatMessagesRemoved?.Invoke(subChatName, startIndex, count);
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

        private readonly List<SubChat> subChats = new List<SubChat>();
        private readonly ChatCompletionApi.ChatCompletionApi chatCompletionApi;
    }
}
