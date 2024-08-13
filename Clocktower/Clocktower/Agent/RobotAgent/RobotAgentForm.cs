using Clocktower.Game;
using Clocktower.Observer;
using OpenAi;

namespace Clocktower.Agent
{
    public partial class RobotAgentForm : Form
    {
        public string PlayerName => robot.PlayerName;

        public IGameObserver Observer => robot.Observer;
        public IAgent Agent => robot;

        public RobotAgentForm(string model, string playerName, string personality, IReadOnlyCollection<string> players, string scriptName, IReadOnlyCollection<Character> script)
        {
            InitializeComponent();

            robot = new(model, playerName, personality, players, scriptName, script, onStart: Show, onStatusChange: SetTitle);
            robot.OnChatMessage += OnChatMessage;
            robot.OnDaySummary += OnDaySummary;
            robot.OnTokenCount += OnTokenCount;

            SetTitle();
        }

        private void SetTitle()
        {
            Text = PlayerName;
            if (robot.Character != null)
            {
                Text += " (";
                if (robot.OriginalCharacter != null)
                {
                    Text += $"{TextUtilities.CharacterToText(robot.OriginalCharacter.Value)}-";
                }
                Text += $"{TextUtilities.CharacterToText(robot.Character.Value)})";
            }
            if (!robot.Alive)
            {
                Text += " GHOST";
            }
        }

        private void OnChatMessage(Role role, string message)
        {
            string messageToDisplay = message.Trim() + "\n";

            switch (role)
            {
                case Role.System:
                    // We don't write the System message for display.
                    break;

                case Role.User:
                    chatTextBox.AppendText(messageToDisplay);
                    break;

                case Role.Assistant:
                    chatTextBox.AppendBoldText(messageToDisplay, Color.Green);
                    break;
            }
        }

        private void OnDaySummary(int dayNumber, string summary)
        {
            summaryTextBox.AppendBoldText($"Day {dayNumber}\n");
            summaryTextBox.AppendText(summary.Trim() + "\n\n");
        }

        private void OnTokenCount(int promptTokens, int completionTokens, int totalTokens)
        {
            this.promptTokens += promptTokens;
            this.completionTokens += completionTokens;
            this.totalTokens += totalTokens;

            usageStatusLabel.Text = $"Usage: {this.totalTokens} = {this.promptTokens} + {this.completionTokens}, Latest: {totalTokens} = {promptTokens} + {completionTokens}";
        }

        private readonly RobotAgent.RobotAgent robot;

        private int promptTokens;
        private int completionTokens;
        private int totalTokens;
    }
}
