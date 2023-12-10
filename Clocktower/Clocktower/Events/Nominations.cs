using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Options;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class Nominations : IGameEvent
    {
        public Player? PlayerToBeExecuted { get; private set; }

        public Nominations(IStoryteller storyteller, Grimoire grimoire, ObserverCollection observers, Random random)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.observers = observers;
            this.random = random;
        }

        public async Task RunEvent()
        {
            while (true)
            {
                var nomination = await RequestNomination();
                if (!nomination.HasValue)
                {   // No more nominations.
                    return;
                }
                await HandleNomination(nomination.Value.nominator, nomination.Value.nominee);
            }
        }

        private async Task<(Player nominator, Player nominee)?> RequestNomination()
        {
            // Only alive players who haven't yet nominated can nominate.
            var players = grimoire.Players.Where(player => player.Alive)
                                          .Except(playersWhoHaveAlreadyNominated)
                                          .ToList();
            players.Shuffle(random);

            foreach (var player in players)
            {
                var nominee = await RequestNominationFromPlayer(player);
                if (nominee != null)
                {
                    return (player, nominee);
                }
            }
            return null;
        }

        private async Task<Player?> RequestNominationFromPlayer(Player player)
        {
            var choice = await player.Agent.GetNomination(GetNominationOptions(player));
            if (choice is PlayerOption playerOption)
            {
                return playerOption.Player;
            }
            return null;
        }

        private IReadOnlyCollection<IOption> GetNominationOptions(Player player)
        {
            return grimoire.Players.Except(playersWhoHaveAlreadyBeenNominated)
                                   .ToOptions()
                                   .Prepend(new PassOption())
                                   .ToList();
        }

        private async Task HandleNomination(Player nominator, Player nominee)
        {
            playersWhoHaveAlreadyNominated.Add(nominator);
            playersWhoHaveAlreadyBeenNominated.Add(nominee);

            observers.AnnounceNomination(nominator, nominee);

            int voteCount = await RunVote(nominee);

            int minVotesRequired = (grimoire.Players.Count(player => player.Alive) + 1) / 2;
            bool beatsCurrent = voteCount >= minVotesRequired && (!highestVoteCount.HasValue || voteCount > highestVoteCount.Value);
            bool tiesCurrent = highestVoteCount.HasValue && voteCount == highestVoteCount.Value;

            observers.AnnounceVoteResult(nominee, voteCount, beatsCurrent, tiesCurrent);

            if (tiesCurrent)
            {
                PlayerToBeExecuted = null;
            }
            else if (beatsCurrent)
            {
                PlayerToBeExecuted = nominee;
                highestVoteCount = voteCount;
            }
        }

        private async Task<int> RunVote(Player nominee)
        {
            int voteCount = 0;

            var voteOptions = nominee.ToVoteOptions();

            foreach (var player in grimoire.GetAllPlayersEndingWithPlayer(nominee))
            {
                if (player.Alive || player.HasGhostVote)
                {
                    var choice = await player.Agent.GetVote(voteOptions);
                    bool votedToExecute = choice is VoteOption;
                    observers.AnnounceVote(player, nominee, votedToExecute);
                    if (votedToExecute)
                    {
                        ++voteCount;
                        if (!player.Alive)
                        {
                            player.UseGhostVote();
                        }
                    }
                }
            }

            return voteCount;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly ObserverCollection observers;
        private readonly Random random;

        private int? highestVoteCount;

        private List<Player> playersWhoHaveAlreadyNominated = new();
        private List<Player> playersWhoHaveAlreadyBeenNominated = new();
    }
}
