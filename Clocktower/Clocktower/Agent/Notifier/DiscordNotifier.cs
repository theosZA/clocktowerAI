using Clocktower.Game;
using DiscordChatBot;
using System.Text;
using System.Text.RegularExpressions;

namespace Clocktower.Agent.Notifier
{
    internal class DiscordNotifier : IMarkupNotifier
    {
        public DiscordNotifier(ChatClient chatClient)
        {
            this.chatClient = chatClient;
        }

        public async Task Start(string playerName, IReadOnlyCollection<string> players, string scriptName, IReadOnlyCollection<Character> script)
        {
            chat = await chatClient.CreateChat(playerName);

            await Notify($"Welcome {playerName} to a game of Blood on the Clocktower.");
            await Notify(TextBuilder.ScriptToText(scriptName, script));
            var scriptPdf = $"Scripts/{scriptName}.pdf";
            if (File.Exists(scriptPdf))
            {
                await chat.SendFile(scriptPdf);
            }
            await Notify(TextBuilder.SetupToText(players.Count, script));
            await Notify(TextBuilder.PlayersToText(players));
        }

        public async Task Notify(string markupText)
        {
            if (chat != null)
            {
                await chat.SendMessage(CleanMarkupText(markupText));
            }
        }

        public async Task NotifyWithImage(string markupText, string imageFileName)
        {
            if (chat != null)
            {
                await chat.SendMessage(CleanMarkupText(markupText), imageFileName);
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

        public async Task<string> SendMessageAndGetResponse(string markupText)
        {
            if (chat == null)
            {
                return string.Empty;
            }

            return await chat.SendMessageAndGetResponse(CleanMarkupText(markupText));
        }

        private static string CleanMarkupText(string markupText)
        {
            // Replace coloured text with bold text since Discord doesn't support colours.
            string pattern = @"(\[color:[^\]]+\])|(\[\/color\])";
            return Regex.Replace(markupText, pattern, "**");
        }

        private readonly ChatClient chatClient;
        private Chat? chat;
    }
}
