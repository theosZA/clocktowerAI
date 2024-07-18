using Discord;
using Discord.WebSocket;
using System.Text;

namespace DiscordChatBot
{
    /// <summary>
    /// Represents a chat between the bot and an individual. We can send and receive messages on this chat.
    /// </summary>
    public class Chat
    {
        public string Name { get; private init; }

        public Chat(string name)
        {
            Name = name;
        }

        public async Task Create(SocketGuild guild)
        {
            channel = guild.TextChannels.FirstOrDefault(channel => string.Equals(channel.Name, Name, StringComparison.InvariantCultureIgnoreCase));
            channel ??= await guild.CreateTextChannelAsync(Name);
        }

        public async Task SendMessage(string messageToSend, string? imageFileName = null)
        {
            string message;
            if (!string.IsNullOrEmpty(imageFileName) && messageQueue.Any())
            {
                await SendMessageToChannel(PrependMessageQueue(null));
                message = messageToSend;
            }
            else
            {
                message = PrependMessageQueue(messageToSend);
            }

            if (message.Length > 2000)
            {
                // Split the message so that length is below 2000.
                var endPosition = message.LastIndexOf('\n', 1999);
                await SendMessage(message[..endPosition], imageFileName);
                await SendMessage(message[(endPosition + 1)..]);
                return;
            }

            await SendMessageToChannel(message, imageFileName);
        }

        public void QueueMessage(string message)
        {
            messageQueue.Enqueue(message);
        }

        public void MessageReceived(string message)
        {
            lastReceivedMessage = message;
            messageReceivedTsc?.TrySetResult();
        }

        public async Task<string> SendMessageAndGetResponse(string messageToSend, string? imageFileName = null)
        {
            messageReceivedTsc = new TaskCompletionSource();
            await SendMessage(messageToSend, imageFileName);
            await messageReceivedTsc.Task;
            return lastReceivedMessage ?? string.Empty;
        }

        private async Task SendMessageToChannel(string messageToSend, string? imageFileName = null)
        {
            if (channel != null)
            {
                if (string.IsNullOrEmpty(imageFileName) || !File.Exists(imageFileName))
                {
                    await channel.SendMessageAsync(messageToSend);
                }
                else
                {
                    var embed = new EmbedBuilder().WithImageUrl($"attachment://{Path.GetFileName(imageFileName)}")
                                                  .Build();
                    await channel.SendFileAsync(imageFileName, messageToSend, false, embed);
                }
            }
        }

        private string PrependMessageQueue(string? messageAtEnd)
        {
            StringBuilder sb = new();
            
            while (messageQueue.TryDequeue(out var message))
            {
                sb.AppendLine(message);
            }

            if (messageAtEnd != null)
            {
                sb.AppendLine(messageAtEnd);
            }

            return sb.ToString();
        }

        private ITextChannel? channel;

        private string? lastReceivedMessage;
        private TaskCompletionSource? messageReceivedTsc;
        private readonly Queue<string> messageQueue = new();
    }
}
