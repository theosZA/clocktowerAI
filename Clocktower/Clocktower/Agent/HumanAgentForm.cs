using Clocktower.Game;
using Clocktower.Options;

namespace Clocktower.Agent
{
    public partial class HumanAgentForm : Form
    {
        public string PlayerName { get; private set; }

        public Character? Character => character;
        public Character? AutoClaim => autoClaim;

        public bool AutoAct
        {
            get => autoCheckbox.Checked;
            set => autoCheckbox.Checked = value;
        }

        public RichTextBox Output => outputText;

        public HumanAgentForm(string playerName, IReadOnlyCollection<Character> script, Random random)
        {
            InitializeComponent();

            this.script = script.ToList();
            this.random = random;

            PlayerName = playerName;
            Text = playerName;
        }

        public Task<IOption> RequestOptionChoice(IReadOnlyCollection<IOption> options)
        {
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

        public Task<string> RequestText()
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

        public async Task AssignCharacter(Character character, Alignment alignment)
        {
            this.character = character;

            await SetTitleText();

            autoClaim = character;
            if (autoClaim.Value.Alignment() == Alignment.Evil)
            {
                autoClaim = script.OfAlignment(Alignment.Good).ToList().RandomPick(random);
            }
        }

        public async Task OnGainCharacterAbility(Character character)
        {
            originalCharacter = this.character;
            this.character = character;

            await SetTitleText();
        }

        public async Task YouAreDead()
        {
            alive = false;

            await SetTitleText();
        }

        private Task SetTitleText()
        {
            Text = PlayerName;
            if (character != null)
            {
                Text += " (";
                if (originalCharacter != null)
                {
                    Text += $"{TextUtilities.CharacterToText(originalCharacter.Value)}-";
                }
                Text += $"{TextUtilities.CharacterToText(character.Value)})";
            }
            if (!alive)
            {
                Text += " GHOST";
            }

            return Task.CompletedTask;
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

            submitButton.Enabled = false;
            responseTextBox.Enabled = false;
            responseTextBox.Text = null;

            OnText?.Invoke(text);
        }

        private readonly List<Character> script;
        private readonly Random random;

        private Character? originalCharacter;
        private Character? character;
        private Character? autoClaim;
        private bool alive = true;

        public delegate void ChoiceEventHandler(IOption choice);
        private event ChoiceEventHandler? OnChoice;
        private IReadOnlyCollection<IOption>? options;

        public delegate void TextEventHandler(string text);
        private event TextEventHandler? OnText;
    }
}
