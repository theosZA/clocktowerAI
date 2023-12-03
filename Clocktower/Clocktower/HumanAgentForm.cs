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

        public void AssignCharacter(Character character, Alignment alignment)
        {
            Text = $"{playerName} ({TextUtilities.CharacterToText(character)})";

            outputText.AppendText("You are the ");
            outputText.AppendText(TextUtilities.CharacterToText(character), TextUtilities.AlignmentToColor(alignment));
            outputText.AppendText(".\n");
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
            outputText.AppendBoldText(newlyDeadPlayer.Name);
            outputText.AppendText(" died in the night.\n");
        }

        public void MinionInformation(Player demon, IReadOnlyCollection<Player> fellowMinions)
        {
            outputText.AppendText("As a minion, you learn that ");
            outputText.AppendBoldText(demon.Name, Color.Red);
            outputText.AppendText(" is your demon");

            if (fellowMinions.Count == 1)
            {
                outputText.AppendText(" and your fellow minion is ");
                outputText.AppendBoldText(fellowMinions.First().Name, Color.Red);
            }
            else if (fellowMinions.Count == 2)
            {
                outputText.AppendText(" and your fellow minions are ");
                outputText.AppendBoldText(fellowMinions.First().Name, Color.Red);
                outputText.AppendText(" and ");
                outputText.AppendBoldText(fellowMinions.Last().Name, Color.Red);
            }

            outputText.AppendText(".\n");
        }

        public void DemonInformation(IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            outputText.AppendText("As a demon, you learn that ");
            if (minions.Count == 1)
            {
                outputText.AppendBoldText(minions.First().Name, Color.Red);
                outputText.AppendText(" is your minion");
            }
            else if (minions.Count == 2)
            {
                outputText.AppendBoldText(minions.First().Name, Color.Red);
                outputText.AppendText(" and ");
                outputText.AppendBoldText(minions.Last().Name, Color.Red);
                outputText.AppendText(" are your minions");
            }
            else if (minions.Count == 3)
            {
                outputText.AppendBoldText(minions.First().Name, Color.Red);
                outputText.AppendText(", ");
                outputText.AppendBoldText(minions.Skip(1).First().Name, Color.Red);
                outputText.AppendText(" and ");
                outputText.AppendBoldText(minions.Last().Name, Color.Red);
                outputText.AppendText(" are your minions");
            }
            else
            {
                throw new ArgumentException("There must be exactly 1-3 minions");
            }

            outputText.AppendText(", and that the following characters are not in play: ");
            AppendCharacters(notInPlayCharacters);
            outputText.AppendText(".\n");
        }

        public void NotifyGodfather(IReadOnlyCollection<Character> outsiders)
        {
            if (outsiders.Count == 0)
            {
                outputText.AppendText("You learn that there are no outsiders in play.");
                return;
            }
            outputText.AppendText("You learn that the following outsiders are in play: ");
            AppendCharacters(outsiders);
            outputText.AppendText(".\n");
        }

        public void NotifySteward(Player goodPlayer)
        {
            outputText.AppendText("You learn that ");
            outputText.AppendBoldText(goodPlayer.Name);
            outputText.AppendText(" is a good player.\n");
        }

        public void NotifyLibrarian(Player playerA, Player playerB, Character character)
        {
            outputText.AppendText("You learn that either ");
            outputText.AppendBoldText(playerA.Name);
            outputText.AppendText(" or ");
            outputText.AppendBoldText(playerB.Name);
            outputText.AppendText(" is the ");
            AppendCharacterText(character);
            outputText.AppendText(".\n");
        }

        public void NotifyEmpath(Player neighbourA, Player neighbourB, int evilCount)
        {
            outputText.AppendText("You learn that ");
            outputText.AppendBoldText(evilCount.ToString());
            outputText.AppendText(" of your living neighbours (");
            outputText.AppendBoldText(neighbourA.Name);
            outputText.AppendText(" and ");
            outputText.AppendBoldText(neighbourB.Name);
            if (evilCount == 1)
            {
                outputText.AppendText(") is evil.\n");
            }
            else
            {
                outputText.AppendText(") are evil.\n");
            }
        }

        public void NotifyRavenkeeper(Player target, Character character)
        {
            outputText.AppendText("You learn that ");
            outputText.AppendBoldText(target.Name);
            outputText.AppendText(" is the ");
            AppendCharacterText(character);
            outputText.AppendText(".\n");
        }

        public void RequestChoiceFromImp(IReadOnlyCollection<Player> players, Action<Player> onChoice)
        {
            outputText.AppendText("As the ");
            AppendCharacterText(Character.Imp);
            outputText.AppendText(" please choose a player to kill...\n");

            PopulateChoices(players, onChoice);
        }

        public void RequestChoiceFromRavenkeeper(IReadOnlyCollection<Player> players, Action<Player> onChoice)
        {
            outputText.AppendText("As the ");
            AppendCharacterText(Character.Ravenkeeper);
            outputText.AppendText(" please choose a player whose character you wish to learn...\n");

            PopulateChoices(players, onChoice);
        }

        private void AppendCharacterText(Character character)
        {
            outputText.AppendText(TextUtilities.CharacterToText(character), TextUtilities.CharacterToColor(character));
        }

        private void AppendCharacters(IReadOnlyCollection<Character> characters)
        {
            bool first = true;
            foreach (var character in characters)
            {
                if (!first)
                {
                    outputText.AppendText(", ");
                }
                AppendCharacterText(character);
                first = false;
            }
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
