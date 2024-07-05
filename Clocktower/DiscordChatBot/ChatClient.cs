using Discord;
using Discord.WebSocket;
using System.Diagnostics;

namespace DiscordChatBot
{
    /// <summary>
    /// The ChatClient wraps all <see cref="Chat"/> instances.
    /// </summary>
    /// <remarks>Currently this client is limited to a single guild named "ClocktowerVsAI".</remarks>
    public class ChatClient
    {
        public const string GuildName = "ClocktowerVsAI";

        public async Task Start()
        {
            client.Log += Log;
            client.Ready += Ready;
            client.MessageReceived += MessageReceived;

            var token = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN", EnvironmentVariableTarget.User);
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            await readyTsc.Task;
        }

        public async Task<Chat> CreateChat(string name)
        {
            var guild = client.Guilds.FirstOrDefault(guild => guild.Name == GuildName) ?? throw new Exception($"Not a member of guild {GuildName}");
            var chat = new Chat(name);
            chats.Add(chat);
            await chat.Create(guild);
            return chat;
        }

        private static Task Log(LogMessage logMessage)
        {
            Debug.WriteLine(logMessage.ToString());
            return Task.CompletedTask;
        }

        private Task Ready()
        {
            var guilds = client.Guilds.ToList();
            Debug.WriteLine($"Guilds: {string.Join(", ", guilds.Select(guild => guild.Name))}");
            if (guilds.Count > 0)
            {
                var guild = guilds[0];
                var channels = guild.TextChannels.ToList();
                Debug.WriteLine($"Channels: {string.Join(", ", channels.Select(channel => channel.Name))}");
            }
            readyTsc.TrySetResult();

            return Task.CompletedTask;
        }

        private Task MessageReceived(SocketMessage message)
        {
            Debug.WriteLine($"Message: {message.Channel.Name} - {message.Author.GlobalName} - {message.Content}");

            if (!message.Author.IsBot)
            {
                var chat = chats.FirstOrDefault(chat => string.Equals(message.Channel.Name, chat.Name, StringComparison.InvariantCultureIgnoreCase));
                chat?.MessageReceived(message.Content);
            }

            return Task.CompletedTask;
        }

        private static DiscordSocketClient BuildClient()
        {
            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };
            return new DiscordSocketClient(config);
        }

        private readonly DiscordSocketClient client = BuildClient();
        private readonly TaskCompletionSource readyTsc = new();

        private readonly List<Chat> chats = new();
    }
}