using Clocktower.Agent;
using Clocktower.Agent.Observer;
using Clocktower.Game;
using Clocktower.Storyteller;
using Clocktower.Triggers;

namespace Clocktower.Events
{
    internal class Nominations : IGameEvent
    {
        public Nominations(IStoryteller storyteller, Grimoire grimoire, Deaths deaths, IGameObserver observers, IReadOnlyCollection<Character> scriptCharacters, Random random)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.deaths = deaths;
            this.observers = observers;
            this.scriptCharacters = scriptCharacters;
            this.random = random;
        }

        public async Task RunEvent()
        {
            await StartNominations();

            while (true)
            {
                // Is it even possible to change the player on the block?
                var (votesToTie, votesToPutOnBlock) = GetVotesRequired();
                if (!votesToTie.HasValue && !votesToPutOnBlock.HasValue && !anyVotesConductedInSecret)
                {   // Not enough votes available to remove the player on the block or add a new player. End nominations.
                    return;
                }

                var nomination = await RequestNomination();
                if (!nomination.HasValue)
                {   // No more nominations.
                    return;
                }
                if (await RunNominationTriggersBeforeAnnouncement(nomination.Value.nominator, nomination.Value.nominee))
                {   // Nomination phase has been ended by a trigger.
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
            var (playerOnTheBlock, votesToTie, votesToPutOnBlock) = GetKnownVotesRequired();

            // Only alive players who haven't yet nominated can nominate.
            var players = grimoire.Players.Where(player => player.Alive)
                                          .Except(playersWhoHaveAlreadyNominated)
                                          .ToList();
            players.Shuffle(random);

            foreach (var player in players)
            {
                var nominee = await player.Agent.GetNomination(playerOnTheBlock, votesToTie, votesToPutOnBlock, grimoire.Players.Except(playersWhoHaveAlreadyBeenNominated));
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

            await AnnounceNomination(nominator, nominee);
            await GetPublicStatements(nominator, nominee);

            int voteCount = await RunVote(nominator, nominee);

            await HandleVoteResult(nominee, voteCount);
        }

        private async Task AnnounceNomination(Player nominator, Player nominee)
        {
            if (anyVotesConductedInSecret)
            {
                await observers.AnnounceNomination(nominator, nominee, votesToTie: null, votesToPutOnBlock: null);
            }
            else
            {
                (int? votesToTie, int? votesToPutOnBlock) = GetVotesRequired();
                await observers.AnnounceNomination(nominator, nominee, votesToTie, votesToPutOnBlock);
            }
            await RunNominationTriggersOnAnnouncement(nominator, nominee);
        }

        private async Task GetPublicStatements(Player nominator, Player nominee)
        {
            if (nominator == nominee)
            {
                var statement = await nominator.Agent.GetReasonForSelfNomination();
                if (!string.IsNullOrEmpty(statement))
                {
                    await observers.PublicStatement(nominator, statement);
                }
            }
            else
            {
                var prosecution = await nominator.Agent.GetProsecution(nominee);
                if (!string.IsNullOrEmpty(prosecution))
                {
                    await observers.PublicStatement(nominator, prosecution);
                }
                var defence = await nominee.Agent.GetDefence(nominator);
                if (!string.IsNullOrEmpty(defence))
                {
                    await observers.PublicStatement(nominee, defence);
                }
            }
        }

        private async Task HandleVoteResult(Player nominee, int voteCount)
        {
            (int? votesToTie, int? votesToPutOnBlock) = GetVotesRequired();
            var voteResult = voteCount >= votesToPutOnBlock ? VoteResult.OnTheBlock
                                  : voteCount == votesToTie ? VoteResult.Tied
                                                            : VoteResult.InsufficientVotes;

            if (SecretVoting())
            {
                anyVotesConductedInSecret = true;
                await observers.AnnounceVoteResult(nominee, voteCount: null, VoteResult.UnknownResult);
                await storyteller.Observer.AnnounceVoteResult(nominee, voteCount, voteResult);
            }
            else if (anyVotesConductedInSecret)
            {   // This is the case when an Organ Grinder has died. This vote was public, but the players don't know
                // if this vote changed anything.
                await observers.AnnounceVoteResult(nominee, voteCount, VoteResult.UnknownResult);
                await storyteller.Observer.AnnounceVoteResult(nominee, voteCount, voteResult);
            }
            else
            {
                await observers.AnnounceVoteResult(nominee, voteCount, voteResult);
            }

            if (voteResult == VoteResult.Tied)
            {
                grimoire.PlayerToBeExecuted = null;
            }
            else if (voteResult == VoteResult.OnTheBlock)
            {
                grimoire.PlayerToBeExecuted = nominee;
                highestVoteCount = voteCount;
            }
        }

        private (Player? playerOnTheBlock, int? votesToTie, int? votesToPutOnBlock) GetKnownVotesRequired()
        {
            if (anyVotesConductedInSecret)
            {
                return (null, null, null);
            }

            var (votesToTie, votesToPutOnBlock) = GetVotesRequired();
            return (grimoire.PlayerToBeExecuted, votesToTie, votesToPutOnBlock);
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

        private async Task<int> RunVote(Player nominator, Player nominee)
        {
            List<Player> votesInFavourOfExecution = new();
            bool secretVoting = SecretVoting();

            if (secretVoting)
            {
                await observers.AnnounceSecretVote(nominee);
            }

            foreach (var player in grimoire.GetAllPlayersEndingWithPlayer(nominee))
            {
                if (player.Alive || player.HasGhostVote)
                {
                    bool votedToExecute = await GetVote(player, nominee, votesInFavourOfExecution, secretVoting);
                    if (secretVoting)
                    {
                        await storyteller.Observer.AnnounceVote(player, nominee, votedToExecute);
                    }
                    else
                    {
                        await observers.AnnounceVote(player, nominee, votedToExecute);
                    }
                    if (votedToExecute)
                    {
                        votesInFavourOfExecution.Add(player);
                        if (!player.Alive)
                        {
                            player.UseGhostVote();
                        }
                    }
                }
            }

            if (nominee.HasHealthyAbility(Character.Organ_Grinder) && !votesInFavourOfExecution.Contains(nominee))
            {   // Organ Grinder did not vote for themself. By their character ability this counts as a vote tally of 0.
                return 0;
            }

            if (secretVoting)
            {
                // Organ Grinder - Butler jinx: "the Butler may raise their hand to vote but their vote is only counted if their master voted too"
                // Check for any (healthy) Butlers and see if their vote should be excluded.
                votesInFavourOfExecution = votesInFavourOfExecution.Where(player =>
                {
                    if (!player.HasHealthyAbility(Character.Butler))
                    {
                        return true;
                    }

                    var master = GetMaster(player);
                    if (master == null)
                    {
                        return false;
                    }
                    return votesInFavourOfExecution.Contains(master);
                }).ToList();
            }

            return votesInFavourOfExecution.Count;
        }

        private async Task<bool> GetVote(Player voter, Player nominee, IReadOnlyCollection<Player> playersWhoHaveVotedForNomination, bool secretVoting)
        {
            if (voter.ShouldRunAbility(Character.Butler) && !secretVoting)    
            {   
                var master = GetMaster(voter);
                if (master != null && !playersWhoHaveVotedForNomination.Contains(master))
                {   // Butler may not vote unless their master already voted (even if drunk or poisoned).
                    // Note that this is slightly different to how it would be run in a real game, being a bit more restrictive,
                    // but is the best we can do in the current way of processing the votes.
                    return false;
                }
            }

            return await voter.Agent.GetVote(nominee, ghostVote: !voter.Alive);
        }

        private Player? GetMaster(Player butler)
        {
            return grimoire.Players.FirstOrDefault(player => player.Tokens.HasTokenForPlayer(Token.ChosenByButler, butler));
        }

        /// <summary>
        /// Checks to see if anything should happen when the nominator nominates the nominee.
        /// This version is for triggers that can conclude the nomination phase immediately and so will use their own appropriate announcement.
        /// </summary>
        /// <param name="nominator">The player making the nomination.</param>
        /// <param name="nominee">The player who is the target of the nomination.</param>
        /// <returns>True if this concludes the Nominations event. Otherwise false is return and the vote should be run as normal.</returns>
        private async Task<bool> RunNominationTriggersBeforeAnnouncement(Player nominator, Player nominee)
        {
            return await VirginCheck(nominator, nominee);
        }

        /// <summary>
        /// Checks to see if anything should happen when the nominator nominates the nominee.
        /// This version is for triggers that will not stop the running of the voting, such as placing tokens or triggering a Witch's curse.
        /// </summary>
        /// <param name="nominator">The player making the nomination.</param>
        /// <param name="nominee">The player who is the target of the nomination.</param>
        private async Task RunNominationTriggersOnAnnouncement(Player nominator, Player nominee)
        {
            await WitchCheck(nominator);
        }

        private async Task<bool> VirginCheck(Player nominator, Player nominee)
        {
            if (!nominee.ShouldRunAbility(Character.Virgin))
            {
                return false;
            }

            nominee.Tokens.Add(Token.UsedOncePerGameAbility, nominee);
            if (nominee.DrunkOrPoisoned)
            {
                return false;
            }
            
            if (!nominator.CanRegisterAsTownsfolk)
            {
                return false;
            }

            if (nominator.CharacterType != CharacterType.Townsfolk && !await storyteller.ShouldExecuteWithVirgin(nominee, nominator))
            {   // Storyteller decided the nominator should not register as a Townsfolk.
                return false;
            }

            await observers.AnnounceNomination(nominator, nominee, votesToTie: null, votesToPutOnBlock: null);
            var earlyEndDay = new EndDay(storyteller, grimoire, deaths, observers);
            grimoire.PlayerToBeExecuted = nominator;
            await earlyEndDay.RunEvent();
            grimoire.PhaseShouldEndImmediately = true;

            return true;
        }

        private async Task WitchCheck(Player nominator)
        {
            if (grimoire.Players.Count(player => player.Alive) <= 3)
            {   // Witch can't trigger on 3 alive players.
                return;
            }

            if (nominator.Tokens.HasHealthyToken(Token.CursedByWitch))
            {
                var witch = nominator.Tokens.GetAssigningPlayerForToken(Token.CursedByWitch);
                await observers.PlayerDies(nominator);
                await deaths.DayKill(nominator, witch);
            }
            else if (scriptCharacters.Contains(Character.Witch) && nominator.HasHealthyAbility(Character.Tinker))
            {   // The Tinker can die at any time, so doesn't need to actually be cursed by the Witch.
                if (await storyteller.ShouldKillTinker(nominator))
                {
                    await observers.PlayerDies(nominator);
                    await deaths.DayKill(nominator, killer: null);
                }
            }
        }

        private bool SecretVoting()
        {
            return grimoire.PlayersWithHealthyAbility(Character.Organ_Grinder).Any();
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly Deaths deaths;
        private readonly IGameObserver observers;
        private readonly IReadOnlyCollection<Character> scriptCharacters;
        private readonly Random random;

        private int? highestVoteCount;
        private bool anyVotesConductedInSecret = false;

        private readonly List<Player> playersWhoHaveAlreadyNominated = new();
        private readonly List<Player> playersWhoHaveAlreadyBeenNominated = new();
    }
}
