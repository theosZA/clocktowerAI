using Clocktower.Game;

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

        private IEnumerable<(string segment, bool bold, Color? color)> SplitText(string markupText)
        {
            bool bold = false;
            Color? color = null;

            int currentPos = 0;
            var splitters = new[] { "**", "[color:", "[/color]" };
            while (currentPos < markupText.Length)
            {
                int splittingPos = markupText.FirstIndexOfAnyText(splitters, currentPos);
                if (splittingPos == -1)
                {
                    yield return (markupText[currentPos..], bold, color);
                    currentPos = markupText.Length;
                }
                else
                {
                    yield return (markupText[currentPos..splittingPos], bold, color);
                    if (markupText[splittingPos..(splittingPos + splitters[0].Length)] == splitters[0])
                    {   // Toggle bold
                        bold = !bold;
                        currentPos = splittingPos + splitters[0].Length;
                    }
                    else if (markupText[splittingPos..(splittingPos + splitters[1].Length)] == splitters[1])
                    {   // Start color
                        int colorStart = splittingPos + splitters[1].Length;
                        int colorEnd = markupText.IndexOf(']', colorStart);
                        var colorText = markupText[colorStart..colorEnd];
                        color = Color.FromName(colorText);
                        currentPos = colorEnd + 1;
                    }
                    else if (markupText[splittingPos..(splittingPos + splitters[2].Length)] == splitters[2])
                    {   // End color
                        color = null;
                        currentPos = splittingPos + splitters[2].Length;
                    }
                }
            }
        }

        private readonly RichTextBox richTextBox;
    }
}
