namespace Clocktower.OpenAiApi
{
    public class RichTextChatLogger : IChatLogger
    {
        public RichTextChatLogger(RichTextBox messageTextBox, RichTextBox summaryTextBox)
        {
            this.messageTextBox = messageTextBox;
            this.summaryTextBox = summaryTextBox;
        }

        public void Log(Role role, string message)
        {
            string messageToDisplay = message.Trim() + "\n";

            switch (role)
            {
                case Role.System:
                    // We don't write the System message for display.
                    break;

                case Role.User:
                    messageTextBox.AppendText(messageToDisplay);
                    break;

                case Role.Assistant:
                    messageTextBox.AppendText(messageToDisplay, Color.Green);
                    break;
            }
        }

        public void LogSummary(Phase phase, int dayNumber, string summary)
        {
            summaryTextBox.AppendBoldText($"{phase} {dayNumber}\n");
            summaryTextBox.AppendText(summary.Trim() + "\n\n");
        }

        private readonly RichTextBox messageTextBox;
        private readonly RichTextBox summaryTextBox;
    }
}