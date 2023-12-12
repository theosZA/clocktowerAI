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

        public void AnnounceWinner(Alignment winner, IReadOnlyCollection<Player> winners, IReadOnlyCollection<Player> losers)
        {
            bool forceStorytellerView = true;   // Game over - everyone can know who was what.
            if (winner == Alignment.Good)
            {
                outputText.AppendBoldText("\nThe GOOD team has won!\n\n", Color.Green);
                outputText.AppendFormattedText("Winning with the good team are: %P.\n", winners, forceStorytellerView);
                outputText.AppendFormattedText("Losing with the evil team are: %P.\n", losers, forceStorytellerView);
            }
            else
            {
                outputText.AppendBoldText("\nThe EVIL team has won!\n\n", Color.Red);
                outputText.AppendFormattedText("Winning with the evil team are: %P.\n", winners, forceStorytellerView);
                outputText.AppendFormattedText("Losing with the good team are: %P.\n", losers, forceStorytellerView);
            }
        }

        public void Night(int nightNumber)
        {
            outputText.AppendBoldText($"\nNight {nightNumber}\n\n");
        }

        public void Day(int dayNumber)
        {
            outputText.AppendBoldText($"\nDay {dayNumber}\n\n");
        }

        public void NoOneDiedAtNight()
        {
            outputText.AppendText("No one died in the night.\n");
        }

        public void PlayerDiedAtNight(Player newlyDeadPlayer)
        {
            outputText.AppendFormattedText("%p died in the night.\n", newlyDeadPlayer, StorytellerView);
        }

        public void PlayerDies(Player newlyDeadPlayer)
        {
            outputText.AppendFormattedText("%p dies.\n", newlyDeadPlayer, StorytellerView);
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

        public void AnnounceNomination(Player nominator, Player nominee, int? votesToTie, int votesToPutOnBlock)
        {
            outputText.AppendFormattedText("%p nominates %p.", nominator, nominee, StorytellerView);
            if (votesToTie.HasValue)
            {
                outputText.AppendFormattedText(" %b votes to tie,", votesToTie.Value);
            }
            outputText.AppendFormattedText(" %b votes to put them on the block.\n", votesToPutOnBlock);
        }

        public void AnnounceVote(Player voter, Player nominee, bool votedToExecute)
        {
            if (votedToExecute)
            {
                if (voter.Alive)
                {
                    outputText.AppendFormattedText("%p votes to execute %p.\n", voter, nominee, StorytellerView);
                }
                else
                {
                    outputText.AppendFormattedText("%p uses their ghost vote to execute %p.\n", voter, nominee, StorytellerView);
                }
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

        public void AnnounceSlayerShot(Player slayer, Player target, bool success)
        {
            outputText.AppendFormattedText("%p claims %c and takes a shot at %p. ", slayer, Character.Slayer, target, StorytellerView);
            if (success)
            {
                outputText.AppendFormattedText("%p dies.\n", target, StorytellerView);
            }
            else
            {
                outputText.AppendText("Nothing happens.\n");
            }
        }

        public void PublicStatement(Player player, string statement)
        {
            outputText.AppendFormattedText("%p: %n", player, statement, StorytellerView);
            if (!statement.EndsWith("\n"))
            {
                outputText.AppendText("\n");
            }
        }

        public void PrivateChatStarts(Player playerA, Player playerB)
        {
            outputText.AppendFormattedText("%p goes for a private chat with %p.\n", playerA, playerB, StorytellerView);
        }

        public void StartRollCall(int playersAlive)
        {
            outputText.AppendFormattedText("Since there are only %b players still alive, we will hold an optional roll call. Everyone will have a chance to claim their character and elaborate on what they learned or how they used their character's ability\n",
                                           playersAlive);
        }

        private readonly RichTextBox outputText;
    }
}
