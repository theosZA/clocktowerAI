using Clocktower.Agent;
using Clocktower.Events;
using Clocktower.Observer;
using Clocktower.Options;

namespace Clocktower.Game
{
    internal class Nominations
    {
        public Nominations(IStoryteller storyteller, Grimoire grimoire, ObserverCollection observers, Random random)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.observers = observers;
            this.random = random;
        }

        public async Task RunNominations()
        {
            while (true)
            {
                var nomination = await RequestNomination();
                if (!nomination.HasValue)
                {   // No more nominations.
                    await EndDay();
                    return;
                }
                await HandleNomination(nomination.Value.nominator, nomination.Value.nominee);
            }
        }

        private async Task<(Player nominator, Player nominee)?> RequestNomination()
        {
            var nominationOptions = grimoire.Players.Except(playersWhoHaveAlreadyBeenNominated)
                                                    .Select(player => (IOption)new PlayerOption(player))
                                                    .Prepend(new PassOption())
                                                    .ToList();

            // Only alive players who haven't yet nominated can nominate.
            var players = grimoire.Players.Where(player => player.Alive)
                                          .Except(playersWhoHaveAlreadyNominated)
                                          .ToList();
            players.Shuffle(random);

            foreach (var player in players)
            {
                var choice = await player.Agent.GetNomination(nominationOptions);
                if (choice is PlayerOption playerOption)
                {
                    return (player, playerOption.Player);
                }
            }
            return null;
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
                playerOnTheBlock = null;
            }
            else if (beatsCurrent)
            {
                playerOnTheBlock = nominee;
                highestVoteCount = voteCount;
            }
        }

        private async Task<int> RunVote(Player nominee)
        {
            int voteCount = 0;

            var voteOptions = new IOption[]
            {
                new PassOption(),
                new VoteOption(nominee)
            };

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

        private async Task EndDay()
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

                if (playerOnTheBlock.CharacterType == CharacterType.Outsider)
                {
                    foreach (var player in grimoire.Players.Where(player => player.Character == Character.Godfather))
                    {
                        player.Tokens.Add(Token.GodfatherKillsTonight);
                    }
                }
                if (playerOnTheBlock.Character == Character.Sweetheart && !playerOnTheBlock.DrunkOrPoisoned)
                {
                    await new SweetheartDrunk(storyteller, grimoire).RunEvent();
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly ObserverCollection observers;
        private readonly Random random;

        private Player? playerOnTheBlock;
        private int? highestVoteCount;

        private List<Player> playersWhoHaveAlreadyNominated = new();
        private List<Player> playersWhoHaveAlreadyBeenNominated = new();
    }
}
