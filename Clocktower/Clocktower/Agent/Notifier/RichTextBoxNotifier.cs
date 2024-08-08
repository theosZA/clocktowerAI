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
            bool bold = false;
            var textSegments = markupText.Split("**");
            foreach (var segment in textSegments)
            {
                Append(segment, bold);
                bold = !bold;
            }

            richTextBox.AppendText("\n");
            return Task.CompletedTask;
        }

        public async Task NotifyWithImage(string markupText, string imageFileName)
        {
            // We don't show the images in a RichTextBox.
            await Notify(markupText);
        }

        private void Append(string text, bool bold)
        {
            text = text.Replace(">>>", "» ");   // Quote box can't be replicated in a RichTextBox, so just use a quote symbol.

            if (bold)
            {
                richTextBox.AppendBoldText(text);
            }
            else
            {
                richTextBox.AppendText(text);
            }
        }

        private readonly RichTextBox richTextBox;
    }
}
