using Clocktower.Game;
using DiscordChatBot;

namespace Clocktower.Agent.Notifier
{
    internal class DiscordNotifier : IMarkupNotifier
    {
        public Chat? Chat { get; private set; }

        public DiscordNotifier(ChatClient chatClient, Func<Chat, Task> onChatStart)
        {
            this.chatClient = chatClient;
            this.onChatStart = onChatStart;
        }

        public async Task Start(string playerName, IReadOnlyCollection<string> players, string scriptName, IReadOnlyCollection<Character> script)
        {
            Chat = await chatClient.CreateChat(playerName);

            await onChatStart(Chat);

            await Chat.SendMessage($"Welcome {playerName} to a game of Blood on the Clocktower.");
            await Chat.SendMessage(TextBuilder.ScriptToText(scriptName, script, markup: true));
            var scriptPdf = $"Scripts/{scriptName}.pdf";
            if (File.Exists(scriptPdf))
            {
                await Chat.SendFile(scriptPdf);
            }
            await Chat.SendMessage(TextBuilder.SetupToText(players.Count, script));
            await Chat.SendMessage(TextBuilder.PlayersToText(players, markup: true));
        }

        public async Task Notify(string markupText)
        {
            if (Chat != null)
            {
                await Chat.SendMessage(markupText);
            }
        }

        public async Task NotifyWithImage(string markupText, string imageFileName)
        {
            if (Chat != null)
            {
                await Chat.SendMessage(markupText, imageFileName);
            }
        }

        private readonly ChatClient chatClient;
        private readonly Func<Chat, Task> onChatStart;
    }
}
