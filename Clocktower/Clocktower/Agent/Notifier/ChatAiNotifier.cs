using Clocktower.Agent.RobotAgent;
using Clocktower.Game;
using System.Text;
using System.Text.RegularExpressions;

namespace Clocktower.Agent.Notifier
{
    internal class ChatAiNotifier : IMarkupNotifier
    {
        public ChatAiNotifier(ClocktowerChatAi chat)
        {
            this.chat = chat;
        }

        public Task Start(string playerName, IReadOnlyCollection<string> players, string scriptName, IReadOnlyCollection<Character> script)
        {
            // Nothing
            return Task.CompletedTask;
        }

        public Task Notify(string markupText)
        {
            chat?.AddMessage(CleanMarkupText(markupText));
            return Task.CompletedTask;
        }

        public async Task NotifyWithImage(string markupText, string imageFileName)
        {
            // No point in showing images to the AI.
            await Notify(markupText);
        }

        public string CreatePlayerRoll(IReadOnlyCollection<Player> players, bool storytellerView)
        {
            var sb = new StringBuilder();

            bool firstPlayer = true;
            foreach (var player in players)
            {
                if (!firstPlayer)
                {
                    sb.Append(", ");
                }
                sb.AppendFormattedText($"%p - {(player.Alive ? "ALIVE" : "DEAD")}", player, storytellerView);
                firstPlayer = false;
            }

            return sb.ToString();
        }

        private static string CleanMarkupText(string markupText)
        {
            // Remove coloured text as the AI won't necessarily understand how to interpret the codes.
            string colourPattern = @"(\[color:[^\]]+\])|(\[\/color\])";
            markupText = Regex.Replace(markupText, colourPattern, string.Empty);

            // Remove quote blocks.
            string quotePattern = @">>>\s?";
            markupText = Regex.Replace(markupText, quotePattern, string.Empty);

            return markupText;
        }

        private readonly ClocktowerChatAi chat;
    }
}
