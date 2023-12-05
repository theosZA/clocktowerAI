using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Options;

namespace Clocktower.Events
{
    internal class Nominations : IGameEvent
    {
        public Nominations(IStoryteller storyteller, Grimoire grimoire, ObserverCollection observers, Random random)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.observers = observers;
            this.random = random;
        }

        public void RunEvent(Action onEventFinished)
        {
            RunNominations(() =>
            {
                EndDay();
                onEventFinished();
            });
        }

        private void RunNominations(Action endDay)
        {
            RequestNomination((nominator, nominee) => HandleNomination(nominator, nominee, endDay), endDay);
        }

        private void RequestNomination(Action<Player, Player> onNomination, Action endDay)
        {
            // Only alive players who haven't yet nominated can nominate.
            var players = grimoire.Players.Where(player => player.Alive)
                                          .Except(playersWhoHaveAlreadyNominated)
                                          .ToList();
            players.Shuffle(random);
            var remainingPlayers = new Queue<Player>(players);
            
            if (remainingPlayers.TryDequeue(out var nextPlayer))
            {
                RequestNomination(nextPlayer, remainingPlayers, onNomination, endDay);
            }
            else
            {   // There are no more players who can nominate.
                endDay();
            }
        }

        private void RequestNomination(Player currentPlayer, Queue<Player> remainingPlayers, Action<Player, Player> onNomination, Action endDay)
        {
            var nominationOptions = grimoire.Players.Except(playersWhoHaveAlreadyBeenNominated)
                                                    .Select(player => (IOption)new PlayerOption(player))
                                                    .Prepend(new PassOption())
                                                    .ToList();

            currentPlayer.Agent.GetNomination(nominationOptions, option =>
            {
                if (option is PlayerOption playerOption)
                {
                    onNomination(currentPlayer, playerOption.Player);
                }
                else if (remainingPlayers.TryDequeue(out var nextPlayer))
                {
                    RequestNomination(nextPlayer, remainingPlayers, onNomination, endDay);
                }
                else
                {   // Everyone has had the chance to nominate but no-one has.
                    endDay();
                }
            });
        }

        private void HandleNomination(Player nominator, Player nominee, Action endDay)
        {
            playersWhoHaveAlreadyNominated.Add(nominator);
            playersWhoHaveAlreadyBeenNominated.Add(nominee);

            observers.AnnounceNomination(nominator, nominee);

            HandleVote(nominee, voteCount =>
            {
                int minVotesRequired = (grimoire.Players.Count(player => player.Alive) + 1) / 2;
                bool beatsCurrent = voteCount >= minVotesRequired && (!highestVoteCount.HasValue || voteCount > highestVoteCount.Value);
                bool tiesCurrent = highestVoteCount.HasValue && voteCount == highestVoteCount.Value;

                observers.AnnounceVoteResult(nominee, voteCount, beatsCurrent, tiesCurrent);

                if (tiesCurrent)
                {
                    playerOnTheBlock = null;
                }
                else if (beatsCurrent)
                {
                    playerOnTheBlock = nominee;
                    highestVoteCount = voteCount;
                }

                RunNominations(endDay);
            });
        }

        private void HandleVote(Player nominee, Action<int> voteResult)
        {
            var playersToVote = new Queue<Player>(grimoire.GetAllPlayersEndingWithPlayer(nominee));
            var firstPlayerToVote = playersToVote.Dequeue();
            HandleVote(firstPlayerToVote, nominee, playersToVote, currentVoteCount: 0, voteResult);
        }

        private void HandleVote(Player currentPlayer, Player nominee, Queue<Player> remainingVoters, int currentVoteCount, Action<int> voteResult)
        {
            currentPlayer.Agent.GetVote(new IOption[] 
            { 
                new PassOption(),
                new VoteOption(nominee) 
            }, choice => 
            {
                bool votedToExecute = choice is VoteOption;
                observers.AnnounceVote(currentPlayer, nominee, votedToExecute);
                if (votedToExecute)
                {
                    ++currentVoteCount;
                }

                if (remainingVoters.TryDequeue(out var nextVoter))
                {
                    HandleVote(nextVoter, nominee, remainingVoters, currentVoteCount, voteResult);
                }
                else
                {   // Everyone has had the chance to vote.
                    voteResult(currentVoteCount);
                }
            });
        }

        private void EndDay()
        {
            if (playerOnTheBlock == null)
            {
                observers.DayEndsWithNoExecution();
                return;
            }

            bool playerDies = playerOnTheBlock.Alive;
            observers.PlayerIsExecuted(playerOnTheBlock, playerDies);
            if (playerDies)
            {
                playerOnTheBlock.Kill();
            }
        }

        private IStoryteller storyteller;
        private Grimoire grimoire;
        private ObserverCollection observers;
        private Random random;

        private Player? playerOnTheBlock;
        private int? highestVoteCount;

        private List<Player> playersWhoHaveAlreadyNominated = new();
        private List<Player> playersWhoHaveAlreadyBeenNominated = new();
    }
}
