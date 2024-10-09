﻿using Clocktower.Game;
using OpenAi;
using System.Text;

namespace Clocktower.Agent.Notifier
{
    internal class RawOpenAiNotifier : IMarkupNotifier
    {
        public RawOpenAiNotifier(OpenAiChat chat)
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
            chat.AddUserMessage(markupText);
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

        private readonly OpenAiChat chat;
    }
}