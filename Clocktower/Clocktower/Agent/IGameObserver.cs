using Clocktower.Game;

namespace Clocktower.Agent
{
    /// <summary>
    /// An observer sees public information such as public changes to the game state.
    /// </summary>
    internal interface IGameObserver
    {
        void Night(int nightNumber);
        void Day(int dayNumber);
        void PlayerDiedAtNight(Player newlyDeadPlayer);
        void PlayerIsExecuted(Player executedPlayer, bool playerDies);
        void DayEndsWithNoExecution();

        void AnnounceNomination(Player nominator, Player nominee);
        void AnnounceVote(Player voter, Player nominee, bool votedToExecute);
        void AnnounceVoteResult(Player nominee, int voteCount, bool beatsCurrent, bool tiesCurrent);
    }
}
