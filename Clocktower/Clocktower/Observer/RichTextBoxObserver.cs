using Clocktower.Game;

namespace Clocktower.Observer
{
    internal class RichTextBoxObserver : IGameObserver
    {
        public bool StorytellerView { get; set; } = false;

        public RichTextBoxObserver(RichTextBox outputText)
        {
            this.outputText = outputText;
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
            outputText.AppendFormattedText("%p died in the night.\n", newlyDeadPlayer, StorytellerView);
        }

        public void PlayerIsExecuted(Player executedPlayer, bool playerDies)
        {
            if (playerDies)
            {
                outputText.AppendFormattedText("%p is executed and dies.\n", executedPlayer, StorytellerView);
            }
            else if (executedPlayer.Alive)
            {
                outputText.AppendFormattedText("%p is executed but does not die.\n", executedPlayer, StorytellerView);
            }
            else
            {
                outputText.AppendFormattedText("%p's corpse is executed.\n", executedPlayer, StorytellerView);
            }
        }

        public void DayEndsWithNoExecution()
        {
            outputText.AppendText("There is no execution and the day ends.\n");
        }

        public void AnnounceNomination(Player nominator, Player nominee)
        {
            outputText.AppendFormattedText("%p nominates %p.\n", nominator, nominee, StorytellerView);
        }

        public void AnnounceVote(Player voter, Player nominee, bool votedToExecute)
        {
            if (votedToExecute)
            {
                outputText.AppendFormattedText("%p votes to execute %p.\n", voter, nominee, StorytellerView);
            }
            else
            {
                outputText.AppendFormattedText("%p does not vote.\n", voter, nominee, StorytellerView);
            }
        }

        public void AnnounceVoteResult(Player nominee, int voteCount, bool beatsCurrent, bool tiesCurrent)
        {
            if (beatsCurrent)
            {
                outputText.AppendFormattedText("%p received %b votes. That is enough to put them on the block.\n", nominee, voteCount, StorytellerView);
            }
            else if (tiesCurrent)
            {
                outputText.AppendFormattedText("%p received %b votes which is a tie. No one is on the block.\n", nominee, voteCount, StorytellerView);
            }
            else
            {
                outputText.AppendFormattedText("%p received %b votes which is not enough.\n", nominee, voteCount, StorytellerView);
            }
        }

        private readonly RichTextBox outputText;
    }
}
