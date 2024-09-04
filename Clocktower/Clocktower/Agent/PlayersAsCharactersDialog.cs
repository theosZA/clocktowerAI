using Clocktower.Game;

namespace Clocktower.Agent
{
    public partial class PlayersAsCharactersDialog : Form
    {
        public PlayersAsCharactersDialog(string title, int count, IReadOnlyCollection<Player> players, IReadOnlyCollection<Character> characters,
                                         bool allowEmptyChoices = true, bool allowDuplicatePlayers = true, bool allowDuplicateCharacters = true)
        {
            this.players = players;
            this.characters = characters;
            this.allowEmptyChoices = allowEmptyChoices;
            this.allowDuplicatePlayers = allowDuplicatePlayers;
            this.allowDuplicateCharacters = allowDuplicateCharacters;

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
            // Check missing players.
            if (!allowEmptyChoices && comboBoxes.Any(row => string.IsNullOrEmpty(row.playersComboBox.Text)))
            {
                return false;
            }
            // Check missing characters.
            if (!allowEmptyChoices && comboBoxes.Any(row => string.IsNullOrEmpty(row.charactersComboBox.Text)))
            {
                return false;
            }
            // Check duplicate players.
            if (!allowDuplicatePlayers && comboBoxes.DistinctBy(row => row.playersComboBox.Text).Count() != comboBoxes.Count)
            {
                return false;
            }
            // Check duplicate characters.
            if (!allowDuplicateCharacters && comboBoxes.DistinctBy(row => row.charactersComboBox.Text).Count() != comboBoxes.Count)
            {
                return false;
            }

            return true;
        }

        private readonly IReadOnlyCollection<Player> players;
        private readonly IReadOnlyCollection<Character> characters;
        private readonly List<(ComboBox playersComboBox, ComboBox charactersComboBox)> comboBoxes = new();

        private readonly bool allowEmptyChoices;
        private readonly bool allowDuplicatePlayers;
        public readonly bool allowDuplicateCharacters;
    }
}
