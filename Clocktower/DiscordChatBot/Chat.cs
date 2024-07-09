using Discord;
using Discord.WebSocket;

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

        public async Task SendMessage(string message, string? imageFileName = null)
        {
            if (message.Length > 2000)
            {
                // Split the message so that length is below 2000.
                var endPosition = message.LastIndexOf('\n', 1999);
                await SendMessage(message[..endPosition], imageFileName);
                await SendMessage(message[(endPosition + 1)..]);
                return;
            }

            if (channel != null)
            {
                if (string.IsNullOrEmpty(imageFileName) || !File.Exists(imageFileName))
                {
                    await channel.SendMessageAsync(message);
                }
                else
                {
                    var embed = new EmbedBuilder().WithImageUrl($"attachment://{Path.GetFileName(imageFileName)}")
                                                  .Build();
                    await channel.SendFileAsync(imageFileName, message, false, embed);
                }
            }
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

        private ITextChannel? channel;

        private string? lastReceivedMessage;
        private TaskCompletionSource? messageReceivedTsc;
    }
}
