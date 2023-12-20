using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.OpenAiApi;

namespace Clocktower.Agent
{
    public partial class RobotAgentForm : Form, ITokenCounter
    {
        public string PlayerName => robot.PlayerName;

        public IGameObserver Observer => robot.Observer;
        public IAgent Agent => robot;

        public RobotAgentForm(string playerName, IReadOnlyCollection<string> playerNames, IReadOnlyCollection<Character> script)
        {
            InitializeComponent();

            robot = new(playerName, playerNames, script, onStart: Show, tokenCounter: this);
            UpdateWithLatest();
            refreshTimer.Enabled = true;
        }

        public void NewTokenUsage(int promptTokens, int completionTokens, int totalTokens)
        {
            this.promptTokens += promptTokens;
            this.completionTokens += completionTokens;
            this.totalTokens += totalTokens;

            usageStatusLabel.Text = $"Usage: {this.totalTokens} = {this.promptTokens} + {this.completionTokens}, Latest: {totalTokens} = {promptTokens} + {completionTokens}";
        }

        private void UpdateWithLatest()
        {
            // Form title.
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

            // Text display.
            summaryTextBox.Text = string.Empty;
            foreach ((Role role, string message) in robot.Messages)
            {
                summaryTextBox.AppendText(message, role switch
                {
                    Role.System => Color.Purple,
                    Role.User => Color.Blue,
                    Role.Assistant => Color.Green,
                    _ => Color.Black,
                });
                if (!message.EndsWith('\n'))
                {
                    summaryTextBox.AppendText("\n");
                }
            }
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            UpdateWithLatest();
        }

        private readonly RobotAgent robot;

        private int promptTokens;
        private int completionTokens;
        private int totalTokens;
    }
}
