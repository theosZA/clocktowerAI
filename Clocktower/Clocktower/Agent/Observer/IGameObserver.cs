using Clocktower.Game;

namespace Clocktower.Agent.Observer
{
    /// <summary>
    /// An observer sees public information such as public changes to the game state.
    /// </summary>
    public interface IGameObserver
    {
        Task AnnounceWinner(Alignment winner, IReadOnlyCollection<Player> winners, IReadOnlyCollection<Player> losers);

        Task Night(int nightNumber);
        Task Day(int dayNumber);

        Task AnnounceLivingPlayers(IReadOnlyCollection<Player> players);
        Task NoOneDiedAtNight();
        Task PlayerDiedAtNight(Player newlyDeadPlayer);
        Task PlayerDies(Player newlyDeadPlayer);
        Task PlayerIsExecuted(Player executedPlayer, bool playerDies);
        Task DayEndsWithNoExecution();

        Task StartNominations(int numberOfLivingPlayers, int votesToPutOnBlock);
        Task AnnounceNomination(Player nominator, Player nominee, int? votesToTie, int? votesToPutOnBlock);
        Task AnnounceSecretVote(Player nominee);
        Task AnnounceVote(Player voter, Player nominee, bool votedToExecute);
        Task AnnounceVoteResult(Player nominee, int? voteCount, VoteResult voteResult);

        Task AnnounceSlayerShot(Player slayer, Player target, bool success);
        Task AnnounceJuggles(Player juggler, IEnumerable<(Player player, Character character)> juggles);

        Task PublicStatement(Player player, string statement);
        Task PrivateChatStarts(Player playerA, Player playerB);
        Task StartRollCall(int playersAlive);
    }
}
