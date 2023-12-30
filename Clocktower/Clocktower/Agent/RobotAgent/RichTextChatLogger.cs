using OpenAi;

namespace Clocktower.Agent.RobotAgent
{
    public class RichTextChatLogger : IChatLogger
    {
        public RichTextChatLogger(RichTextBox messageTextBox, RichTextBox summaryTextBox)
        {
            this.messageTextBox = messageTextBox;
            this.summaryTextBox = summaryTextBox;
        }

        public void Log(string subChatName, Role role, string message)
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

        public void LogSummary(string subChatName, string summary)
        {
            summaryTextBox.AppendBoldText(subChatName + "\n");
            summaryTextBox.AppendText(summary.Trim() + "\n\n");
        }

        private readonly RichTextBox messageTextBox;
        private readonly RichTextBox summaryTextBox;
    }
}