using Clocktower.Agent.Notifier;
using Clocktower.Game;
using Clocktower.Options;

namespace Clocktower.Storyteller
{
    public partial class StorytellerForm : Form
    {
        public bool AutoAct
        {
            get => autoCheckbox.Checked;
            set => autoCheckbox.Checked = value;
        }

        public RichTextBox Output => outputText;

        public StorytellerForm(Random random)
        {
            InitializeComponent();

            this.random = random;

            notifier = new RichTextBoxNotifier(Output);
        }

        public Task<IOption> Prompt(string prompt, IReadOnlyCollection<IOption> options)
        {
            notifier.AddToTextBox(prompt);
            return PopulateOptions(options);
        }

        public Task<string> PromptForText(string prompt)
        {
            notifier.AddToTextBox(prompt);
            return GetTextResponse();
        }

        private Task<IOption> PopulateOptions(IReadOnlyCollection<IOption> options)
        {
            if (AutoAct)
            {
                var autoChosenOption = AutoChooseOption(options);
                outputText.AppendBoldText($">> {autoChosenOption.Name}\n", Color.Green);
                return Task.FromResult(autoChosenOption);
            }

            this.options = options;

            choicesComboBox.Items.Clear();
            foreach (var option in options)
            {
                choicesComboBox.Items.Add(option.Name);
            }
            choicesComboBox.Enabled = true;
            chooseButton.Enabled = true;

            var taskCompletionSource = new TaskCompletionSource<IOption>();

            void onChoiceHandler(IOption option)
            {
                taskCompletionSource.SetResult(option);
                OnChoice -= onChoiceHandler;
            }

            OnChoice += onChoiceHandler;

            return taskCompletionSource.Task;
        }

        private IOption AutoChooseOption(IReadOnlyCollection<IOption> options)
        {
            // If Pass is an option, pick it 75% of the time.
            var passOption = options.FirstOrDefault(option => option is PassOption);
            if (passOption != null && random.Next(4) < 3)
            {
                return passOption;
            }

            // For PlayerList options, limit it to just options with a single living non-Demon player.
            if (options.Any(option => option is PlayerListOption))
            {
                var autoPlayerListOptions = options.Where(option =>
                {
                    if (option is not PlayerListOption playerListOption)
                    {
                        return false;
                    }
                    var players = playerListOption.GetPlayers().ToList();
                    if (players.Count != 1)
                    {
                        return false;
                    }
                    var player = players[0];
                    return player.Alive && player.CharacterType != CharacterType.Demon;
                }).ToList();
                if (autoPlayerListOptions.Any())
                {
                    return autoPlayerListOptions.RandomPick(random);
                }
            }

            // For now, just pick an option at random.
            // Exclude dead players from our choices.
            var autoOptions = options.Where(option => option is not PassOption)
                                     .Where(option => option is not PlayerOption playerOption || playerOption.Player.Alive)
                                     .ToList();
            return autoOptions.RandomPick(random);
        }

        private Task<string> GetTextResponse()
        {
            submitButton.Enabled = true;
            responseTextBox.Enabled = true;

            var taskCompletionSource = new TaskCompletionSource<string>();

            void onTextHandler(string text)
            {
                taskCompletionSource.SetResult(text);
                OnText -= onTextHandler;
            }

            OnText += onTextHandler;

            return taskCompletionSource.Task;
        }

        private void chooseButton_Click(object sender, EventArgs e)
        {
            var option = options?.FirstOrDefault(option => option.Name == (string)choicesComboBox.SelectedItem);
            if (option == null)
            {   // No valid option has been chosen.
                return;
            }

            chooseButton.Enabled = false;
            choicesComboBox.Enabled = false;
            choicesComboBox.Items.Clear();
            choicesComboBox.Text = null;

            outputText.AppendBoldText($">> {option.Name}\n", Color.Green);

            OnChoice?.Invoke(option);
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            var text = responseTextBox.Text;
            if (string.IsNullOrEmpty(text)) 
            {   // No response provided.
                return;
            }

            submitButton.Enabled = false;
            responseTextBox.Enabled = false;
            responseTextBox.Text = null;

            outputText.AppendBoldText($">> \"{text}\"\n", Color.Green);

            OnText?.Invoke(text);
        }

        private readonly Random random;

        public delegate void ChoiceEventHandler(IOption choice);
        private event ChoiceEventHandler? OnChoice;
        private IReadOnlyCollection<IOption>? options;

        public delegate void TextEventHandler(string text);
        private event TextEventHandler? OnText;

        private readonly RichTextBoxNotifier notifier;
    }
}
