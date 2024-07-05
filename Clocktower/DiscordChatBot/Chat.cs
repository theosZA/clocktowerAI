using Discord;
using Discord.WebSocket;

namespace DiscordChatBot
{
    /// <summary>
    /// Represents a chat between the bot and an individual. We can send and receive messages on this chat.
    /// </summary>
    public class Chat
    {
        public Chat(string name)
        {
            this.name = name;
        }

        public async Task Create(SocketGuild guild)
        {
            channel = guild.TextChannels.FirstOrDefault(channel => channel.Name == name);
            channel ??= await guild.CreateTextChannelAsync(name);
        }

        public async Task SendMessage(string message)
        {
            if (channel != null)
            {
                await channel.SendMessageAsync(message);
            }
        }

        public void MessageReceived(string message)
        {
            lastReceivedMessage = message;
            messageReceivedTsc?.TrySetResult();
        }

        public async Task<string> SendMessageAndGetResponse(string messageToSend)
        {
            messageReceivedTsc = new TaskCompletionSource();
            await SendMessage(messageToSend);
            await messageReceivedTsc.Task;
            return lastReceivedMessage ?? string.Empty;
        }

        private readonly string name;
        private ITextChannel? channel;

        private string? lastReceivedMessage;
        private TaskCompletionSource? messageReceivedTsc;
    }
}
