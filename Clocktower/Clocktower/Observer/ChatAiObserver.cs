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

        public Task AnnounceWinner(Alignment winner, IReadOnlyCollection<Player> winners, IReadOnlyCollection<Player> losers)
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

            return Task.CompletedTask;
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

        public Task AnnounceLivingPlayers(IReadOnlyCollection<Player> players)
        {
            StringBuilder sb = new();

            sb.AppendLine($"There are {players.Count(player => player.Alive)} players still alive. Our players are...");
            bool firstPlayer = true;
            foreach (var player in players)
            {
                if (!firstPlayer)
                {
                    sb.Append(", ");
                }
                sb.AppendFormattedText($"%p - {(player.Alive ? "ALIVE" : "DEAD")}", player);
                firstPlayer = false;
            }

            clocktowerChat.AddMessage(sb.ToString());
            return Task.CompletedTask;
        }

        public Task NoOneDiedAtNight()
        {
            clocktowerChat.AddMessage("No one died in the night.");
            return Task.CompletedTask;
        }

        public Task PlayerDiedAtNight(Player newlyDeadPlayer)
        {
            clocktowerChat.AddFormattedMessage("%p died in the night.", newlyDeadPlayer);
            return Task.CompletedTask;
        }

        public Task PlayerDies(Player newlyDeadPlayer)
        {
            clocktowerChat.AddFormattedMessage("%p dies.", newlyDeadPlayer);
            return Task.CompletedTask;
        }

        public Task PlayerIsExecuted(Player executedPlayer, bool playerDies)
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

            return Task.CompletedTask;
        }

        public Task DayEndsWithNoExecution()
        {
            clocktowerChat.AddMessage("There is no execution and the day ends.");
            return Task.CompletedTask;
        }

        public async Task StartNominations(int numberOfLivingPlayers, int votesToPutOnBlock)
        {
            clocktowerChat.AddMessage($"Nominations for who will be executed are now open. There are {numberOfLivingPlayers} players currently still alive, so we'll require {votesToPutOnBlock} votes to put a nominee on the block.");
            await robotAgent.PromptForOverview();
            if (numberOfLivingPlayers <= 4)
            {   // Down to what may well be the final round of nominations, we want our AI to express who it thinks is the demon, and so encourage it to try sway the rest of town and hopefully vote that way if they still have a vote left.
                await robotAgent.PromptForDemonGuess();
            }
        }

        public Task AnnounceNomination(Player nominator, Player nominee, int? votesToTie, int? votesToPutOnBlock)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("%p nominates %p. ", nominator, nominee);
            if (votesToTie.HasValue && votesToPutOnBlock.HasValue)
            {
                sb.AppendFormattedText("%b votes to tie, %b votes to put them on the block.", votesToTie.Value, votesToPutOnBlock.Value);
            }
            else if (votesToTie.HasValue)
            {
                sb.AppendFormattedText("%b votes to tie.", votesToTie.Value);
            }
            else if (votesToPutOnBlock.HasValue)
            {
                sb.AppendFormattedText("%b votes to put them on the block.", votesToPutOnBlock.Value);
            }
            clocktowerChat.AddMessage(sb.ToString());

            return Task.CompletedTask;
        }

        public Task AnnounceVote(Player voter, Player nominee, bool votedToExecute)
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

            return Task.CompletedTask;
        }

        public Task AnnounceVoteResult(Player nominee, int voteCount, bool beatsCurrent, bool tiesCurrent)
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

            return Task.CompletedTask;
        }

        public Task AnnounceSlayerShot(Player slayer, Player target, bool success)
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

            return Task.CompletedTask;
        }

        public Task PublicStatement(Player player, string statement)
        {
            clocktowerChat.AddFormattedMessage("%p: %n", player, statement);
            return Task.CompletedTask;
        }

        public Task PrivateChatStarts(Player playerA, Player playerB)
        {
            clocktowerChat.AddFormattedMessage("%p goes for a private chat with %p.", playerA, playerB);
            return Task.CompletedTask;
        }

        public Task StartRollCall(int playersAlive)
        {
            clocktowerChat.AddFormattedMessage("Since there are only %b players still alive, we will hold an optional roll call. Everyone will have a chance to claim their character and elaborate on what they learned or how they used their character's ability.",
                                               playersAlive);
            return Task.CompletedTask;
        }

        private readonly ClocktowerChatAi clocktowerChat;
        private readonly RobotAgent robotAgent;
    }
}
