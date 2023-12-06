using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Options;

namespace Clocktower
{
    public partial class HumanAgentForm : Form
    {
        public IGameObserver Observer { get; private set; }

        public bool AutoAct
        {
            get => autoCheckbox.Checked;
            set => autoCheckbox.Checked = value;
        }

        public HumanAgentForm(string playerName, Random random)
        {
            InitializeComponent();

            this.random = random;

            this.playerName = playerName;
            Text = playerName;

            Observer = new RichTextBoxObserver(outputText);

            AutoAct = true; // for testing
        }

        public void AssignCharacter(Character character, Alignment _)
        {
            this.character = character;

            SetTitleText();

            outputText.AppendFormattedText("You are the %c.\n", character);
        }

        public void YouAreDead()
        {
            alive = false;

            SetTitleText();

            outputText.AppendBoldText("You are dead and are now a ghost. You may only vote one more time.\n");
        }

        public void MinionInformation(Player demon, IReadOnlyCollection<Player> fellowMinions)
        {
            if (fellowMinions.Any())
            {
                outputText.AppendFormattedText($"As a minion, you learn that %p is your demon and your fellow {(fellowMinions.Count > 1 ? "minions are" : "minion is")} %P.\n", demon, fellowMinions);
            }
            else
            {
                outputText.AppendFormattedText($"As a minion, you learn that %p is your demon.\n", demon);
            }
        }

        public void DemonInformation(IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            outputText.AppendFormattedText($"As a demon, you learn that %P {(minions.Count > 1 ? "are your minions" : "is your minion")}, and that the following characters are not in play: %C.\n", minions, notInPlayCharacters);
        }

        public void NotifyGodfather(IReadOnlyCollection<Character> outsiders)
        {
            if (outsiders.Count == 0)
            {
                outputText.AppendFormattedText("You learn that there are no outsiders in play.\n");
                return;
            }
            outputText.AppendFormattedText("You learn that the following outsiders are in play: %C.\n", outsiders);
        }

        public void NotifySteward(Player goodPlayer)
        {
            outputText.AppendFormattedText("You learn that %p is a good player.\n", goodPlayer);
        }

        public void NotifyLibrarian(Player playerA, Player playerB, Character character)
        {
            outputText.AppendFormattedText("You learn that either %p or %p is the %c.\n", playerA, playerB, character);
        }

        public void NotifyInvestigator(Player playerA, Player playerB, Character character)
        {
            outputText.AppendFormattedText("You learn that either %p or %p is the %c.\n", playerA, playerB, character);
        }

        public void NotifyEmpath(Player neighbourA, Player neighbourB, int evilCount)
        {
            outputText.AppendFormattedText($"You learn that %b of your living neighbours (%p and %p) {(evilCount == 1 ? "is" : "are")} evil.\n", evilCount, neighbourA, neighbourB);
        }

        public void NotifyRavenkeeper(Player target, Character character)
        {
            outputText.AppendFormattedText("You learn that %p is the %c.\n", target, character);
        }

        public async Task<IOption> RequestChoiceFromImp(IReadOnlyCollection<IOption> options)
        {
            outputText.AppendFormattedText("As the %c please choose a player to kill...\n", Character.Imp);
            return await PopulateOptions(options);
        }

        public async Task<IOption> RequestChoiceFromGodfather(IReadOnlyCollection<IOption> options)
        {
            outputText.AppendFormattedText("As the %c please choose a player to kill...\n", Character.Godfather);
            return await PopulateOptions(options);
        }

        public async Task<IOption> RequestChoiceFromRavenkeeper(IReadOnlyCollection<IOption> options)
        {
            outputText.AppendFormattedText("As the %c please choose a player whose character you wish to learn...\n", Character.Ravenkeeper);
            return await PopulateOptions(options);
        }

        public async Task<IOption> GetNomination(IReadOnlyCollection<IOption> options)
        {
            outputText.AppendText("Please nominate a player or pass...\n");
            return await PopulateOptions(options);
        }

        public async Task<IOption> GetVote(IReadOnlyCollection<IOption> options)
        {
            var voteOption = (VoteOption)(options.First(option => option is VoteOption));
            outputText.AppendFormattedText("If you wish, you may vote for executing %p or pass...\n", voteOption.Nominee);
            return await PopulateOptions(options);
        }

        private Task<IOption> PopulateOptions(IReadOnlyCollection<IOption> options)
        {
            if (AutoAct)
            {
                // If Pass is an option, pick it 40% of the time.
                var passOption = options.FirstOrDefault(option => option is PassOption);
                if (passOption != null && random.Next(5) < 2)
                {
                    return Task.FromResult(passOption);
                }

                // For now, just pick an option at random.
                // Exclude dead players and ourself from our choices.
                var autoOptions = options.Where(option => option is not PassOption)
                                         .Where(option => option is not PlayerOption playerOption || (playerOption.Player.Alive && playerOption.Player.Name != playerName))
                                         .ToList();
                return Task.FromResult(autoOptions.RandomPick(random));
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
                this.OnChoice -= onChoiceHandler;
            }

            this.OnChoice += onChoiceHandler;

            return taskCompletionSource.Task;
        }

        private void SetTitleText()
        {
            Text = $"{playerName} ({TextUtilities.CharacterToText(character)})";
            if (!alive)
            {
                Text += " GHOST";
            }
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

        private Random random;

        private string playerName;
        private Character character;
        private bool alive = true;

        private IReadOnlyCollection<IOption>? options;

        public delegate void ChoiceEventHandler(IOption choice);
        private event ChoiceEventHandler? OnChoice;
    }
}
