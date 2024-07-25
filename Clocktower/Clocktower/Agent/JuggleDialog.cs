using Clocktower.Game;

namespace Clocktower.Agent
{
    public partial class JuggleDialog : Form
    {
        public JuggleDialog(IReadOnlyCollection<Player> players, IReadOnlyCollection<Character> characters)
        {
            this.players = players;
            this.characters = characters;

            InitializeComponent();

            for (int row = 0; row < jugglesTable.RowCount; row++)
            {
                var playersComboBox = new ComboBox();
                foreach (var player in players.Select(player => player.Name))
                {
                    playersComboBox.Items.Add(player);
                }
                jugglesTable.Controls.Add(playersComboBox, 0, row);

                var charactersComboBox = new ComboBox();
                foreach (var character in characters)
                {
                    charactersComboBox.Items.Add(TextUtilities.CharacterToText(character));
                }
                jugglesTable.Controls.Add(charactersComboBox, 1, row);

                comboBoxes.Add((playersComboBox, charactersComboBox));
            }
        }

        public IEnumerable<(Player player, Character character)> GetJuggles()
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

        private void submitButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private readonly IReadOnlyCollection<Player> players;
        private readonly IReadOnlyCollection<Character> characters;
        private readonly List<(ComboBox playersComboBox, ComboBox charactersComboBox)> comboBoxes = new();
    }
}
