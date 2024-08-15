using Clocktower.Game;
using System.Text;

namespace Clocktower.Agent.Notifier
{
    internal class RichTextBoxNotifier : IMarkupNotifier
    {
        public RichTextBoxNotifier(RichTextBox richTextBox)
        {
            this.richTextBox = richTextBox;
        }

        public Task Start(string name, IReadOnlyCollection<string> players, string scriptName, IReadOnlyCollection<Character> script)
        {
            // Nothing
            return Task.CompletedTask;
        }

        public Task Notify(string markupText)
        {
            var textSegments = SplitText(markupText);
            foreach ((string segment, bool bold, Color? color) in textSegments)
            {
                Append(segment, bold, color);
            }

            richTextBox.AppendText("\n");
            return Task.CompletedTask;
        }

        public async Task NotifyWithImage(string markupText, string imageFileName)
        {
            // We don't show the images in a RichTextBox.
            await Notify(markupText);
        }

        public string CreatePlayerRoll(IReadOnlyCollection<Player> players, bool storytellerView)
        {
            var sb = new StringBuilder();

            foreach (var player in players)
            {
                if (player.Alive)
                {
                    sb.AppendFormattedText("%p - ALIVE", player, storytellerView);
                    sb.AppendLine();
                }
                else
                {
                    var playerText = new StringBuilder();
                    playerText.AppendFormattedText("%p - DEAD", player, storytellerView);
                    sb.Append(playerText.Replace("**", string.Empty));   // dead players will not be bolded
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

        private void Append(string text, bool bold, Color? color)
        {
            text = text.Replace(">>>", "» ");   // Quote box can't be replicated in a RichTextBox, so just use a quote symbol.

            if (color.HasValue)
            {
                if (bold)
                {
                    richTextBox.AppendBoldText(text, color.Value);
                }
                else
                {
                    richTextBox.AppendText(text, color.Value);
                }
            }
            else
            {
                if (bold)
                {
                    richTextBox.AppendBoldText(text);
                }
                else
                {
                    richTextBox.AppendText(text);
                }
            }
        }

        private static IEnumerable<(string segment, bool bold, Color? color)> SplitText(string markupText)
        {
            bool bold = false;
            bool heading = false;
            Color? color = null;

            int currentPos = 0;
            var splitters = new[] { "**", "[color:", "[/color]", "## ", "\n"};
            while (currentPos < markupText.Length)
            {
                (int splittingPos, string? splitter) = markupText.FirstIndexOfAnyText(splitters, currentPos);
                if (splitter == null)
                {
                    yield return (markupText[currentPos..], bold || heading, color);
                    currentPos = markupText.Length;
                }
                else if (splitter == splitters[4])
                {
                    // End of line
                    int newPos = splittingPos + splitter.Length;
                    yield return (markupText[currentPos..newPos], bold || heading, color);
                    heading = false;
                    currentPos = newPos;
                }
                else
                {
                    yield return (markupText[currentPos..splittingPos], bold || heading, color);
                    currentPos = splittingPos + splitter.Length;
                    switch (Array.IndexOf(splitters, splitter))
                    {
                        case 0:
                            // Toggle bold
                            bold = !bold;
                            break;

                        case 1:
                            // Start color
                            int colorEnd = markupText.IndexOf(']', currentPos);
                            var colorText = markupText[currentPos..colorEnd];
                            color = Color.FromName(colorText);
                            currentPos = colorEnd + 1;
                            break;

                        case 2:
                            // End color
                            color = null;
                            break;

                        case 3:
                            // Heading on until end of line
                            heading = true;
                            break;
                    }
                }
            }
        }

        private readonly RichTextBox richTextBox;
    }
}
