using Clocktower.Game;

namespace Clocktower.Agent
{
    public partial class PlayersAsCharactersDialog : Form
    {
        public PlayersAsCharactersDialog(string title, int count, IReadOnlyCollection<Player> players, IReadOnlyCollection<Character> characters,
                                         Func<IReadOnlyCollection<(Player player, Character character)>, bool> validationFunction)
        {
            this.players = players;
            this.characters = characters;
            this.validationFunction = validationFunction;

            InitializeComponent();
            Text = title;

            playersAsCharactersTable.RowCount = count;

            for (int row = 0; row < playersAsCharactersTable.RowCount; row++)
            {
                var playersComboBox = new ComboBox();
                foreach (var player in players.Select(player => player.Name))
                {
                    playersComboBox.Items.Add(player);
                    playersComboBox.SelectedIndexChanged += OnSelectionChanged;
                }
                playersAsCharactersTable.Controls.Add(playersComboBox, 0, row);

                var charactersComboBox = new ComboBox();
                foreach (var character in characters)
                {
                    charactersComboBox.Items.Add(TextUtilities.CharacterToText(character));
                    charactersComboBox.SelectedIndexChanged += OnSelectionChanged;
                }
                playersAsCharactersTable.Controls.Add(charactersComboBox, 1, row);

                comboBoxes.Add((playersComboBox, charactersComboBox));
            }

            UpdateSubmitButtonStatus();
        }

        public IEnumerable<(Player player, Character character)> GetPlayersAsCharacters()
        {
            foreach (var (playersComboBox, charactersComboBox) in comboBoxes)
            {
                var player = players.FirstOrDefault(player => player.Name == playersComboBox.Text);
                if (player != null)
                {
                    if (characters.Any(character => TextUtilities.CharacterToText(character) == charactersComboBox.Text))
                    {
                        var character = characters.First(character => TextUtilities.CharacterToText(character) == charactersComboBox.Text);
                        yield return (player, character);
                    }
                }
            }
        }

        private void OnSelectionChanged(object? sender, EventArgs e)
        {
            UpdateSubmitButtonStatus();
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void UpdateSubmitButtonStatus()
        {
            submitButton.Enabled = IsSubmissionAllowed();
        }

        private bool IsSubmissionAllowed()
        {
            return validationFunction(GetPlayersAsCharacters().ToList());
        }

        private readonly IReadOnlyCollection<Player> players;
        private readonly IReadOnlyCollection<Character> characters;
        private readonly List<(ComboBox playersComboBox, ComboBox charactersComboBox)> comboBoxes = new();

        private readonly Func<IReadOnlyCollection<(Player player, Character minionCharacter)>, bool> validationFunction;
    }
}
