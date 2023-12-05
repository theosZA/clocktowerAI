using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Options;

namespace Clocktower
{
    public partial class HumanAgentForm : Form, IGameObserver
    {
        public HumanAgentForm(string playerName)
        {
            InitializeComponent();

            this.playerName = playerName;
            Text = playerName;
        }

        public void AssignCharacter(Character character, Alignment _)
        {
            this.character = character;

            SetTitleText();

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
            CheckForDeath(newlyDeadPlayer);
        }

        public void PlayerIsExecuted(Player executedPlayer, bool playerDies)
        {
            if (playerDies)
            {
                outputText.AppendFormattedText("%p is executed and dies.\n", executedPlayer);
                CheckForDeath(executedPlayer);
            }
            else if (executedPlayer.Alive)
            {
                outputText.AppendFormattedText("%p is executed but does not die.\n", executedPlayer);
            }
            else
            {
                outputText.AppendFormattedText("%p's corpse is executed.\n", executedPlayer);
            }
        }

        public void DayEndsWithNoExecution()
        {
            outputText.AppendText("There is no execution and the day ends.\n");
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

        public void RequestChoiceFromImp(IReadOnlyCollection<IOption> options, Action<IOption> onChoice)
        {
            outputText.AppendFormattedText("As the %c please choose a player to kill...\n", Character.Imp);
            PopulateOptions(options, onChoice);
        }

        public void RequestChoiceFromRavenkeeper(IReadOnlyCollection<IOption> options, Action<IOption> onChoice)
        {
            outputText.AppendFormattedText("As the %c please choose a player whose character you wish to learn...\n", Character.Ravenkeeper);
            PopulateOptions(options, onChoice);
        }

        public void GetNomination(IReadOnlyCollection<IOption> options, Action<IOption> onChoice)
        {
            outputText.AppendText("Please nominate a player or pass...\n");
            PopulateOptions(options, onChoice);
        }

        public void AnnounceNomination(Player nominator, Player nominee)
        {
            outputText.AppendFormattedText("%p nominates %p.\n", nominator, nominee);
        }

        public void GetVote(IReadOnlyCollection<IOption> options, Action<IOption> onChoice)
        {
            var voteOption = (VoteOption)(options.First(option => option is VoteOption));
            outputText.AppendFormattedText("If you wish, you may vote for executing %p or pass...\n", voteOption.Nominee);
            PopulateOptions(options, onChoice);
        }

        public void AnnounceVote(Player voter, Player nominee, bool votedToExecute)
        {
            if (votedToExecute)
            {
                outputText.AppendFormattedText("%p votes to execute %p.\n", voter, nominee);
            }
            else
            {
                outputText.AppendFormattedText("%p does not vote.\n", voter, nominee);
            }
        }

        public void AnnounceVoteResult(Player nominee, int voteCount, bool beatsCurrent, bool tiesCurrent)
        {
            if (beatsCurrent)
            {
                outputText.AppendFormattedText("%p received %b votes. That is enough to put them on the block.\n", nominee, voteCount);
            }
            else if (tiesCurrent)
            {
                outputText.AppendFormattedText("%p received %b votes which is a tie. No one is on the block.\n", nominee, voteCount);
            }
            else
            {
                outputText.AppendFormattedText("%p received %b votes which is not enough.\n", nominee, voteCount);
            }
        }

        private void PopulateOptions(IReadOnlyCollection<IOption> options, Action<IOption> onChoice)
        {
            this.options = options;
            this.onChoice = onChoice;

            choicesComboBox.Items.Clear();
            foreach (var option in options)
            {
                choicesComboBox.Items.Add(option.Name);
            }
            choicesComboBox.Enabled = true;
            chooseButton.Enabled = true;
        }

        private void SetTitleText()
        {
            Text = $"{playerName} ({TextUtilities.CharacterToText(character)})";
            if (!alive)
            {
                Text += " GHOST";
            }
        }

        private void CheckForDeath(Player playerWhoDied)
        {
            if (alive && playerWhoDied.Name == playerName)
            {
                alive = false;
                SetTitleText();
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

            onChoice?.Invoke(option);
        }

        private string playerName;
        private Character character;
        private bool alive = true;

        private IReadOnlyCollection<IOption>? options;
        private Action<IOption>? onChoice;
    }
}
