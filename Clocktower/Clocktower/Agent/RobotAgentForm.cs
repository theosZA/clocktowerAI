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

            var chatLoggers = new IChatLogger[]
            {
                new RichTextChatLogger(chatTextBox, summaryTextBox),
                new FileChatLogger($"{playerName}-{DateTime.UtcNow:yyyyMMddTHHmmss}.log")
            };

            robot = new(playerName, playerNames, script, onStart: Show, onCharacterChange: SetTitle, ProxyCollection<IChatLogger>.CreateProxy(chatLoggers), tokenCounter: this);

            SetTitle();
        }

        public void NewTokenUsage(int promptTokens, int completionTokens, int totalTokens)
        {
            this.promptTokens += promptTokens;
            this.completionTokens += completionTokens;
            this.totalTokens += totalTokens;

            usageStatusLabel.Text = $"Usage: {this.totalTokens} = {this.promptTokens} + {this.completionTokens}, Latest: {totalTokens} = {promptTokens} + {completionTokens}";
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

        private readonly RobotAgent robot;

        private int promptTokens;
        private int completionTokens;
        private int totalTokens;
    }
}
