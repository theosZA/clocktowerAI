using Clocktower.Agent.Notifier;
using OpenAi;

namespace Clocktower.Agent
{
    public partial class AiStorytellerForm : Form
    {
        public AiStorytellerForm()
        {
            InitializeComponent();

            display = new RichTextBoxNotifier(chatTextBox);
        }

        public void OnChatMessage(Role role, string message)
        {
            switch (role)
            {
                case Role.System:
                    // We don't write the System message for display.
                    break;

                case Role.User:
                    display.Notify(message);
                    break;

                case Role.Assistant:
                    string messageToDisplay = message.Trim() + "\n";
                    chatTextBox.AppendBoldText(messageToDisplay, Color.Green);
                    break;
            }
        }

        public void OnTokenCount(int promptTokens, int completionTokens, int totalTokens)
        {
            this.promptTokens += promptTokens;
            this.completionTokens += completionTokens;
            this.totalTokens += totalTokens;

            usageStatusLabel.Text = $"Usage: {this.totalTokens} = {this.promptTokens} + {this.completionTokens}, Latest: {totalTokens} = {promptTokens} + {completionTokens}";
        }

        private readonly IMarkupNotifier display;

        private int promptTokens;
        private int completionTokens;
        private int totalTokens;
    }
}
