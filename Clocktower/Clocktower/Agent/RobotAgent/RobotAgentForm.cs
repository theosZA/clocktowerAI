using Clocktower.Agent.Notifier;
using Clocktower.Agent.RobotAgent;
using OpenAi;

namespace Clocktower.Agent
{
    public partial class RobotAgentForm : Form
    {
        public string PlayerName => robotTriggers.PlayerName;

        public RobotAgentForm(RobotTriggers robotTriggers)
        {
            InitializeComponent();

            this.robotTriggers = robotTriggers;
            this.robotTriggers.OnStatusChange += SetTitle;

            display = new RichTextBoxNotifier(chatTextBox);

            SetTitle();
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

        public void OnDaySummary(int dayNumber, string summary)
        {
            summaryTextBox.AppendBoldText($"Day {dayNumber}\n");
            summaryTextBox.AppendText(summary.Trim() + "\n\n");
        }

        public void OnTokenCount(int promptTokens, int completionTokens, int totalTokens)
        {
            this.promptTokens += promptTokens;
            this.completionTokens += completionTokens;
            this.totalTokens += totalTokens;

            usageStatusLabel.Text = $"Usage: {this.totalTokens} = {this.promptTokens} + {this.completionTokens}, Latest: {totalTokens} = {promptTokens} + {completionTokens}";
        }

        private void SetTitle()
        {
            Text = PlayerName;
            if (robotTriggers.Character != null)
            {
                Text += " (";
                if (robotTriggers.OriginalCharacter != null)
                {
                    Text += $"{TextUtilities.CharacterToText(robotTriggers.OriginalCharacter.Value)}-";
                }
                Text += $"{TextUtilities.CharacterToText(robotTriggers.Character.Value)})";
            }
            if (!robotTriggers.Alive)
            {
                Text += " GHOST";
            }
        }

        private readonly RobotTriggers robotTriggers;
        private readonly IMarkupNotifier display;

        private int promptTokens;
        private int completionTokens;
        private int totalTokens;
    }
}
