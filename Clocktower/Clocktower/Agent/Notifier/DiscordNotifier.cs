using Clocktower.Game;
using DiscordChatBot;
using System.Text;
using System.Text.RegularExpressions;

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
                await Chat.SendMessage(CleanMarkupText(markupText));
            }
        }

        public async Task NotifyWithImage(string markupText, string imageFileName)
        {
            if (Chat != null)
            {
                await Chat.SendMessage(CleanMarkupText(markupText), imageFileName);
            }
        }

        public string CreatePlayerRoll(IReadOnlyCollection<Player> players, bool storytellerView)
        {
            var sb = new StringBuilder();

            foreach (var player in players)
            {
                if (player.Alive)
                {
                    sb.AppendFormattedText("%p", player, storytellerView);
                    sb.AppendLine();
                }
                else
                {
                    var playerText = new StringBuilder();
                    playerText.AppendFormattedText("%p", player, storytellerView);
                    sb.AppendFormattedText("👻 %n 👻", playerText.Replace("**", string.Empty).ToString());   // dead players will not be bolded
                    if (player.HasGhostVote)
                    {
                        sb.AppendLine(" (ghost vote available)");
                    }
                    else
                    {
                        sb.AppendLine();
                    }
                }
            }

            return sb.ToString();
        }

        private static string CleanMarkupText(string markupText)
        {
            // Replace coloured text with bold text since Discord doesn't support colours.
            string pattern = @"(\[color:[^\]]+\])|(\[\/color\])";
            return Regex.Replace(markupText, pattern, "**");
        }

        private readonly ChatClient chatClient;
        private readonly Func<Chat, Task> onChatStart;
    }
}
