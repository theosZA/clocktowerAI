using Clocktower.Agent.RobotAgent;
using Clocktower.Game;
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

        private static string CleanMarkupText(string markupText)
        {
            // Remove coloured text as the AI won't necessarily understand how to interpret the codes.
            string pattern = @"(\[color:[^\]]+\])|(\[\/color\])";
            return Regex.Replace(markupText, pattern, string.Empty);
        }

        private readonly ClocktowerChatAi chat;
    }
}
