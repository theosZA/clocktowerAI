using Clocktower.Game;
using DiscordChatBot;

namespace Clocktower.Observer
{
    internal class DiscordChatObserver : IGameObserver
    {
        public void Start(Chat chat)
        {
            this.chat = chat;
        }

        public void AnnounceNomination(Player nominator, Player nominee, int? votesToTie, int? votesToPutOnBlock)
        {
            throw new NotImplementedException();
        }

        public void AnnounceSlayerShot(Player slayer, Player target, bool success)
        {
            throw new NotImplementedException();
        }

        public void AnnounceVote(Player voter, Player nominee, bool votedToExecute)
        {
            throw new NotImplementedException();
        }

        public void AnnounceVoteResult(Player nominee, int voteCount, bool beatsCurrent, bool tiesCurrent)
        {
            throw new NotImplementedException();
        }

        public void AnnounceWinner(Alignment winner, IReadOnlyCollection<Player> winners, IReadOnlyCollection<Player> losers)
        {
            throw new NotImplementedException();
        }

        public Task Day(int dayNumber)
        {
            throw new NotImplementedException();
        }

        public void DayEndsWithNoExecution()
        {
            throw new NotImplementedException();
        }

        public void LivingPlayerCount(int numberOfLivingPlayers)
        {
            throw new NotImplementedException();
        }

        public Task Night(int nightNumber)
        {
            throw new NotImplementedException();
        }

        public void NoOneDiedAtNight()
        {
            throw new NotImplementedException();
        }

        public void PlayerDiedAtNight(Player newlyDeadPlayer)
        {
            throw new NotImplementedException();
        }

        public void PlayerDies(Player newlyDeadPlayer)
        {
            throw new NotImplementedException();
        }

        public void PlayerIsExecuted(Player executedPlayer, bool playerDies)
        {
            throw new NotImplementedException();
        }

        public void PrivateChatStarts(Player playerA, Player playerB)
        {
            throw new NotImplementedException();
        }

        public void PublicStatement(Player player, string statement)
        {
            throw new NotImplementedException();
        }

        public Task StartNominations(int numberOfLivingPlayers, int votesToPutOnBlock)
        {
            throw new NotImplementedException();
        }

        public void StartRollCall(int playersAlive)
        {
            throw new NotImplementedException();
        }

        private Chat? chat;
    }
}
