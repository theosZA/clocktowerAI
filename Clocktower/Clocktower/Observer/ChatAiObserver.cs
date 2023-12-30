using Clocktower.Agent.RobotAgent;
using Clocktower.Game;
using System.Text;

namespace Clocktower.Observer
{
    internal class ChatAiObserver : IGameObserver
    {
        public ChatAiObserver(ClocktowerChatAi clocktowerChat, RobotAgent robotAgent)
        {
            this.clocktowerChat = clocktowerChat;
            this.robotAgent = robotAgent;
        }

        public void AnnounceWinner(Alignment winner, IReadOnlyCollection<Player> winners, IReadOnlyCollection<Player> losers)
        {
            const bool forceStorytellerView = true;
            if (winner == Alignment.Good)
            {
                clocktowerChat.AddFormattedMessage("\nThe GOOD team has won!\nWinning with the good team are: %P.\nLosing with the evil team are: %P.", winners, losers, forceStorytellerView);
            }
            else
            {
                clocktowerChat.AddFormattedMessage("\nThe EVIL team has won!\nWinning with the evil team are: %P.\nLosing with the good team are: %P.", winners, losers, forceStorytellerView);
            }
        }

        public async Task Night(int nightNumber)
        {
            await clocktowerChat.Night(nightNumber);
        }

        public async Task Day(int dayNumber)
        {
            if (dayNumber == 1)
            {
                await robotAgent.PromptForBluff();
            }

            await clocktowerChat.Day(dayNumber);
        }

        public void NoOneDiedAtNight()
        {
            clocktowerChat.AddMessage("No one died in the night.");
        }

        public void PlayerDiedAtNight(Player newlyDeadPlayer)
        {
            clocktowerChat.AddFormattedMessage("%p died in the night.", newlyDeadPlayer);
        }

        public void PlayerDies(Player newlyDeadPlayer)
        {
            clocktowerChat.AddFormattedMessage("%p dies.", newlyDeadPlayer);
        }

        public void PlayerIsExecuted(Player executedPlayer, bool playerDies)
        {
            if (playerDies)
            {
                clocktowerChat.AddFormattedMessage("%p is executed and dies.", executedPlayer);
            }
            else if (executedPlayer.Alive)
            {
                clocktowerChat.AddFormattedMessage("%p is executed but does not die.", executedPlayer);
            }
            else
            {
                clocktowerChat.AddFormattedMessage("%p's corpse is executed.", executedPlayer);
            }
        }

        public void DayEndsWithNoExecution()
        {
            clocktowerChat.AddMessage("There is no execution and the day ends.");
        }

        public void AnnounceNomination(Player nominator, Player nominee, int? votesToTie, int votesToPutOnBlock)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("%p nominates %p.", nominator, nominee);
            if (votesToTie.HasValue)
            {
                sb.AppendFormattedText(" %b votes to tie,", votesToTie.Value);
            }
            sb.AppendFormattedText(" %b votes to put them on the block.", votesToPutOnBlock);
            clocktowerChat.AddMessage(sb.ToString());
        }

        public void AnnounceVote(Player voter, Player nominee, bool votedToExecute)
        {
            if (votedToExecute)
            {
                if (voter.Alive)
                {
                    clocktowerChat.AddFormattedMessage("%p votes to execute %p.", voter, nominee);
                }
                else
                {
                    clocktowerChat.AddFormattedMessage("%p uses their ghost vote to execute %p.", voter, nominee);
                }
            }
            else
            {
                clocktowerChat.AddFormattedMessage("%p does not vote.", voter, nominee);
            }
        }

        public void AnnounceVoteResult(Player nominee, int voteCount, bool beatsCurrent, bool tiesCurrent)
        {
            if (beatsCurrent)
            {
                clocktowerChat.AddFormattedMessage("%p received %b votes. That is enough to put them on the block.", nominee, voteCount);
            }
            else if (tiesCurrent)
            {
                clocktowerChat.AddFormattedMessage("%p received %b votes which is a tie. No one is on the block.", nominee, voteCount);
            }
            else
            {
                clocktowerChat.AddFormattedMessage("%p received %b votes which is not enough.", nominee, voteCount);
            }
        }

        public void AnnounceSlayerShot(Player slayer, Player target, bool success)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("%p claims %c and takes a shot at %p. ", slayer, Character.Slayer, target);
            if (success)
            {
                sb.AppendFormattedText("%p dies.", target);
            }
            else
            {
                sb.Append("Nothing happens.");
            }
            clocktowerChat.AddMessage(sb.ToString());
        }

        public void PublicStatement(Player player, string statement)
        {
            clocktowerChat.AddFormattedMessage("%p: %n", player, statement);
        }

        public void PrivateChatStarts(Player playerA, Player playerB)
        {
            clocktowerChat.AddFormattedMessage("%p goes for a private chat with %p.", playerA, playerB);
        }

        public void StartRollCall(int playersAlive)
        {
            clocktowerChat.AddFormattedMessage("Since there are only %b players still alive, we will hold an optional roll call. Everyone will have a chance to claim their character and elaborate on what they learned or how they used their character's ability.",
                                               playersAlive);
        }

        private readonly ClocktowerChatAi clocktowerChat;
        private readonly RobotAgent robotAgent;
    }
}
