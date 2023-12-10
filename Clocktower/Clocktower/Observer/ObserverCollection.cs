using Clocktower.Game;

namespace Clocktower.Observer
{
    internal class ObserverCollection : IGameObserver
    {
        public ObserverCollection(IEnumerable<IGameObserver> observers)
        {
            this.observers = new(observers);
        }

        public void AnnounceNomination(Player nominator, Player nominee)
        {
            foreach (var observer in observers)
            {
                observer.AnnounceNomination(nominator, nominee);
            }
        }

        public void AnnounceSlayerShot(Player slayer, Player target, bool success)
        {
            foreach (var observer in observers)
            {
                observer.AnnounceSlayerShot(slayer, target, success);
            }
        }

        public void AnnounceVote(Player voter, Player nominee, bool votedToExecute)
        {
            foreach (var observer in observers)
            {
                observer.AnnounceVote(voter, nominee, votedToExecute);
            }
        }

        public void AnnounceVoteResult(Player nominee, int voteCount, bool beatsCurrent, bool tiesCurrent)
        {
            foreach (var observer in observers)
            {
                observer.AnnounceVoteResult(nominee, voteCount, beatsCurrent, tiesCurrent);
            }
        }

        public void AnnounceWinner(Alignment winner, IReadOnlyCollection<Player> winners, IReadOnlyCollection<Player> losers)
        {
            foreach (var observer in observers)
            {
                observer.AnnounceWinner(winner, winners, losers);
            }
        }

        public void Day(int dayNumber)
        {
            foreach (var observer in observers)
            {
                observer.Day(dayNumber);
            }
        }

        public void DayEndsWithNoExecution()
        {
            foreach (var observer in observers)
            {
                observer.DayEndsWithNoExecution();
            }
        }

        public void Night(int nightNumber)
        {
            foreach (var observer in observers)
            {
                observer.Night(nightNumber);
            }
        }

        public void NoOneDiedAtNight()
        {
            foreach (var observer in observers)
            {
                observer.NoOneDiedAtNight();
            }
        }

        public void PlayerDiedAtNight(Player newlyDeadPlayer)
        {
            foreach (var observer in observers)
            {
                observer.PlayerDiedAtNight(newlyDeadPlayer);
            }
        }

        public void PlayerDies(Player newlyDeadPlayer)
        {
            foreach (var observer in observers)
            {
                observer.PlayerDies(newlyDeadPlayer);
            }
        }

        public void PlayerIsExecuted(Player executedPlayer, bool playerDies)
        {
            foreach (var observer in observers)
            {
                observer.PlayerIsExecuted(executedPlayer, playerDies);
            }
        }

        private readonly List<IGameObserver> observers;
    }
}
