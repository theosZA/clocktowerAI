using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class Nominations : IGameEvent
    {
        public Nominations(IStoryteller storyteller, Grimoire grimoire, IGameObserver observers, Random random)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.observers = observers;
            this.random = random;
        }

        public async Task RunEvent()
        {
            await StartNominations();

            while (true)
            {
                // Is it even possible to change the player on the block?
                var (votesToTie, votesToPutOnBlock) = GetVotesRequired();
                if (!votesToTie.HasValue && !votesToPutOnBlock.HasValue)
                {   // Not enough votes available to remove the player on the block or add a new player. End nominations.
                    return;
                }

                var nomination = await RequestNomination();
                if (!nomination.HasValue)
                {   // No more nominations.
                    return;
                }
                await HandleNomination(nomination.Value.nominator, nomination.Value.nominee);
            }
        }

        private async Task StartNominations()
        {
            var (_, votesToPutOnBlock) = GetVotesRequired();
            if (votesToPutOnBlock.HasValue)
            {
                await observers.StartNominations(grimoire.Players.Alive(), votesToPutOnBlock.Value);
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
                var nominee = await player.Agent.GetNomination(grimoire.Players.Except(playersWhoHaveAlreadyBeenNominated));
                if (nominee != null)
                {
                    return (player, nominee);
                }
            }
            return null;
        }

        private async Task HandleNomination(Player nominator, Player nominee)
        {
            playersWhoHaveAlreadyNominated.Add(nominator);
            playersWhoHaveAlreadyBeenNominated.Add(nominee);

            (int? votesToTie, int? votesToPutOnBlock) = GetVotesRequired();

            observers.AnnounceNomination(nominator, nominee, votesToTie, votesToPutOnBlock);
            if (nominator == nominee)
            {
                var statement = await nominator.Agent.GetReasonForSelfNomination();
                if (!string.IsNullOrEmpty(statement))
                {
                    observers.PublicStatement(nominator, statement);
                }
            }
            else
            {
                var prosecution = await nominator.Agent.GetProsecution(nominee);
                if (!string.IsNullOrEmpty(prosecution))
                {
                    observers.PublicStatement(nominator, prosecution);
                }
                var defence = await nominee.Agent.GetDefence(nominator);
                if (!string.IsNullOrEmpty(defence))
                {
                    observers.PublicStatement(nominee, defence);
                }
            }

            int voteCount = await RunVote(nominee);
            bool beatsCurrent = voteCount >= votesToPutOnBlock;
            bool tiesCurrent = voteCount == votesToTie;

            observers.AnnounceVoteResult(nominee, voteCount, beatsCurrent, tiesCurrent);

            if (tiesCurrent)
            {
                grimoire.PlayerToBeExecuted = null;
            }
            else if (beatsCurrent)
            {
                grimoire.PlayerToBeExecuted = nominee;
                highestVoteCount = voteCount;
            }
        }

        private (int? votesToTie, int? votesToPutOnBlock) GetVotesRequired()
        {
            int? votesToTie = null;
            int? votesToPutOnBlock = null;

            // Has there already been someone put on the block this nomination phase?
            if (highestVoteCount.HasValue)
            {
                votesToPutOnBlock = highestVoteCount.Value + 1;
                // Is someone still on the block? If so, their vote total can be tied.
                if (grimoire.PlayerToBeExecuted != null)
                {
                    votesToTie = highestVoteCount.Value;
                }
            }
            else
            {   // No one has previously been on the block, so voting threshold is determined by alive players.
                votesToPutOnBlock = (grimoire.Players.Count(player => player.Alive) + 1) / 2;
            }

            // Ensure the required votes aren't exceeding the available votes.
            int votesAvailable = grimoire.Players.Count(player => player.Alive || player.HasGhostVote);
            if (votesAvailable < votesToTie)
            {
                votesToTie = null;
            }
            if (votesAvailable < votesToPutOnBlock)
            {
                votesToPutOnBlock = null;
            }

            return (votesToTie, votesToPutOnBlock);
        }

        private (int? votesToTie, int votesToPutOnBlock) GetVotesRequiredBase()
        {
            if (highestVoteCount.HasValue)
            {
                if (grimoire.PlayerToBeExecuted == null)
                {
                    return (null, highestVoteCount.Value + 1);
                }
                return (highestVoteCount.Value, highestVoteCount.Value + 1);
            }
            int minVotesRequired = (grimoire.Players.Count(player => player.Alive) + 1) / 2;
            return (null, minVotesRequired);
        }

        private async Task<int> RunVote(Player nominee)
        {
            int voteCount = 0;

            foreach (var player in grimoire.GetAllPlayersEndingWithPlayer(nominee))
            {
                if (player.Alive || player.HasGhostVote)
                {
                    bool votedToExecute = await player.Agent.GetVote(nominee, ghostVote: !player.Alive);
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
        private readonly IGameObserver observers;
        private readonly Random random;

        private int? highestVoteCount;

        private readonly List<Player> playersWhoHaveAlreadyNominated = new();
        private readonly List<Player> playersWhoHaveAlreadyBeenNominated = new();
    }
}
