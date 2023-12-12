using Clocktower.Game;

namespace Clocktower.Observer
{
    /// <summary>
    /// An observer sees public information such as public changes to the game state.
    /// </summary>
    public interface IGameObserver
    {
        void AnnounceWinner(Alignment winner, IReadOnlyCollection<Player> winners, IReadOnlyCollection<Player> losers);

        void Night(int nightNumber);
        void Day(int dayNumber);

        void NoOneDiedAtNight();
        void PlayerDiedAtNight(Player newlyDeadPlayer);
        void PlayerDies(Player newlyDeadPlayer);
        void PlayerIsExecuted(Player executedPlayer, bool playerDies);
        void DayEndsWithNoExecution();

        void AnnounceNomination(Player nominator, Player nominee);
        void AnnounceVote(Player voter, Player nominee, bool votedToExecute);
        void AnnounceVoteResult(Player nominee, int voteCount, bool beatsCurrent, bool tiesCurrent);

        void AnnounceSlayerShot(Player slayer, Player target, bool success);

        void PublicStatement(Player player, string statement);
        void StartRollCall(int playersAlive);
    }
}
