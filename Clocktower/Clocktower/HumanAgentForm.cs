using Clocktower.Game;

namespace Clocktower
{
    public partial class HumanAgentForm : Form
    {
        public HumanAgentForm(string playerName)
        {
            InitializeComponent();

            this.playerName = playerName;
            Text = playerName;
        }

        public void AssignCharacter(Character character, Alignment _)
        {
            Text = $"{playerName} ({TextUtilities.CharacterToText(character)})";

            outputText.AppendFormattedText("You are the %c.\n", character);
        }

        public void Night(int nightNumber)
        {
            outputText.AppendBoldText($"\nNight {nightNumber}\n\n");
        }

        public void Day(int dayNumber)
        {
            outputText.AppendBoldText($"\nDay {dayNumber}\n\n");
        }

        public void PlayerDiedAtNight(Player newlyDeadPlayer)
        {
            outputText.AppendFormattedText("%p died in the night.\n", newlyDeadPlayer);
        }

        public void MinionInformation(Player demon, IReadOnlyCollection<Player> fellowMinions)
        {
            outputText.AppendFormattedText($"As a minion, you learn that %p is your demon and your fellow {(fellowMinions.Count > 1 ? "minions are" : "minion is")} %P.\n", demon, fellowMinions);
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

        public void RequestChoiceFromImp(IReadOnlyCollection<Player> players, Action<Player> onChoice)
        {
            outputText.AppendFormattedText("As the %c please choose a player to kill...\n", Character.Imp);
            PopulateChoices(players, onChoice);
        }

        public void RequestChoiceFromRavenkeeper(IReadOnlyCollection<Player> players, Action<Player> onChoice)
        {
            outputText.AppendFormattedText("As the %c please choose a player whose character you wish to learn...\n", Character.Ravenkeeper);
            PopulateChoices(players, onChoice);
        }

        private void PopulateChoices(IReadOnlyCollection<Player> players, Action<Player> onChoice)
        {
            choicesComboBox.Items.Clear();
            foreach (var player in players)
            {
                choicesComboBox.Items.Add(player.Name);
            }
            choicesComboBox.Enabled = true;
            chooseButton.Enabled = true;

            this.players = players;
            this.onChoice = onChoice;
        }

        private void chooseButton_Click(object sender, EventArgs e)
        {
            var player = players?.FirstOrDefault(player => player.Name == (string)choicesComboBox.SelectedItem);
            if (player != null)
            {
                chooseButton.Enabled = false;
                choicesComboBox.Enabled = false;
                choicesComboBox.Items.Clear();

                outputText.AppendBoldText($">> {player.Name}\n", Color.Green);

                onChoice?.Invoke(player);
            }
        }

        private string playerName;

        private IReadOnlyCollection<Player>? players;
        private Action<Player>? onChoice;
    }
}
