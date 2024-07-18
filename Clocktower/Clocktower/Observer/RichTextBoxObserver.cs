using Clocktower.Game;
using System.Text;

namespace Clocktower.Observer
{
    internal class RichTextBoxObserver : IGameObserver
    {
        public bool StorytellerView { get; set; } = false;

        public RichTextBoxObserver(RichTextBox outputText)
        {
            this.outputText = outputText;
        }

        public Task AnnounceWinner(Alignment winner, IReadOnlyCollection<Player> winners, IReadOnlyCollection<Player> losers)
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

            return Task.CompletedTask;
        }

        public Task Night(int nightNumber)
        {
            outputText.AppendBoldText($"\nNight {nightNumber}\n\n");
            return Task.CompletedTask;
        }

        public Task Day(int dayNumber)
        {
            outputText.AppendBoldText($"\nDay {dayNumber}\n\n");
            return Task.CompletedTask;
        }

        public Task AnnounceLivingPlayers(IReadOnlyCollection<Player> players)
        {
            outputText.AppendText($"There are {players.Count(player => player.Alive)} players still alive: ");
            bool firstPlayer = true;
            foreach (var player in players)
            {
                if (!firstPlayer)
                {
                    outputText.AppendText(", ");
                }
                outputText.AppendFormattedText($"%p - {(player.Alive ? "ALIVE" : "DEAD")}", player, StorytellerView);
                firstPlayer = false;
            }
            return Task.CompletedTask;
        }

        public Task NoOneDiedAtNight()
        {
            outputText.AppendText("No one died in the night.\n");
            return Task.CompletedTask;
        }

        public Task PlayerDiedAtNight(Player newlyDeadPlayer)
        {
            outputText.AppendFormattedText("%p died in the night.\n", newlyDeadPlayer, StorytellerView);
            return Task.CompletedTask;
        }

        public Task PlayerDies(Player newlyDeadPlayer)
        {
            outputText.AppendFormattedText("%p dies.\n", newlyDeadPlayer, StorytellerView);
            return Task.CompletedTask;
        }

        public Task PlayerIsExecuted(Player executedPlayer, bool playerDies)
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
            return Task.CompletedTask;
        }

        public Task DayEndsWithNoExecution()
        {
            outputText.AppendText("There is no execution and the day ends.\n");
            return Task.CompletedTask;
        }

        public Task StartNominations(int numberOfLivingPlayers, int votesToPutOnBlock)
        {
            outputText.AppendFormattedText("\nNominations for who will be executed are now open. There are %b players currently still alive, so we'll require %b votes to put a nominee on the block.\n", numberOfLivingPlayers, votesToPutOnBlock);
            return Task.CompletedTask;
        }

        public Task AnnounceNomination(Player nominator, Player nominee, int? votesToTie, int? votesToPutOnBlock)
        {
            outputText.AppendFormattedText("%p nominates %p.", nominator, nominee, StorytellerView);
            if (votesToTie.HasValue && votesToPutOnBlock.HasValue)
            {
                outputText.AppendFormattedText(" %b votes to tie, %b votes to put them on the block.", votesToTie.Value, votesToPutOnBlock.Value);
            }
            else if (votesToTie.HasValue)
            {
                outputText.AppendFormattedText(" %b votes to tie.", votesToTie.Value);
            }
            else if (votesToPutOnBlock.HasValue)
            {
                outputText.AppendFormattedText(" %b votes to put them on the block.", votesToPutOnBlock.Value);
            }
            outputText.AppendText("\n");

            return Task.CompletedTask;
        }

        public Task AnnounceVote(Player voter, Player nominee, bool votedToExecute)
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

            return Task.CompletedTask;
        }

        public Task AnnounceVoteResult(Player nominee, int voteCount, bool beatsCurrent, bool tiesCurrent)
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

            return Task.CompletedTask;
        }

        public Task AnnounceSlayerShot(Player slayer, Player target, bool success)
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

            return Task.CompletedTask;
        }

        public Task PublicStatement(Player player, string statement)
        {
            outputText.AppendFormattedText("%p: %n", player, statement, StorytellerView);
            if (!statement.EndsWith("\n"))
            {
                outputText.AppendText("\n");
            }

            return Task.CompletedTask;
        }

        public Task PrivateChatStarts(Player playerA, Player playerB)
        {
            outputText.AppendFormattedText("%p goes for a private chat with %p.\n", playerA, playerB, StorytellerView);
            return Task.CompletedTask;
        }

        public Task StartRollCall(int playersAlive)
        {
            outputText.AppendFormattedText("Since there are only %b players still alive, we will hold an optional roll call. Everyone will have a chance to claim their character and elaborate on what they learned or how they used their character's ability.\n",
                                           playersAlive);
            return Task.CompletedTask;
        }

        private readonly RichTextBox outputText;
    }
}
