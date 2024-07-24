using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class OrganGrinderTests
    {
        [Test]
        public async Task OrganGrinder_VotesHidden()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Organ_Grinder,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Imp).MockNomination(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            foreach (var agent in setup.Agents)
            {
                await agent.Observer.DidNotReceive().AnnounceVote(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<bool>());
                await agent.Observer.Received(1).AnnounceVoteResult(Arg.Any<Player>(), null, VoteResult.UnknownResult);
            }
        }

        [Test]
        public async Task OrganGrinder_VotesPublicWhenDead()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Organ_Grinder,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Organ_Grinder);
            setup.Agent(Character.Imp).MockNomination(Character.Soldier);

            await game.RunNightAndDay();

            foreach (var agent in setup.Agents)
            {
                await agent.Observer.Received(7).AnnounceVote(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<bool>());
                await agent.Observer.Received(1).AnnounceVoteResult(Arg.Any<Player>(), 7, VoteResult.OnTheBlock);
            }
        }

        [Test]
        public async Task OrganGrinder_VotesPublicWhenPoisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Organ_Grinder,Ravenkeeper,Saint,Poisoner,Soldier,Mayor");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Organ_Grinder);
            setup.Agent(Character.Imp).MockNomination(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            foreach (var agent in setup.Agents)
            {
                await agent.Observer.Received(7).AnnounceVote(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<bool>());
                await agent.Observer.Received(1).AnnounceVoteResult(Arg.Any<Player>(), 7, VoteResult.OnTheBlock);
            }
        }

        [Test]
        public async Task OrganGrinder_DoesNotDieIfDoesNotSelfVote()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Organ_Grinder,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Imp).MockNomination(Character.Organ_Grinder);
            setup.Agent(Character.Organ_Grinder).GetVote(Arg.Any<IReadOnlyCollection<IOption>>(), Arg.Any<bool>()).Returns(args => args.GetPassOptionFromArg());

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Organ_Grinder).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task OrganGrinder_DiesIfSelfVotes()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Organ_Grinder,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Imp).MockNomination(Character.Organ_Grinder);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Organ_Grinder).Received().YouAreDead();
        }

        [Test]
        public async Task OrganGrinder_DiesIfPoisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Organ_Grinder,Ravenkeeper,Saint,Poisoner,Soldier,Mayor");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Organ_Grinder);
            setup.Agent(Character.Imp).MockNomination(Character.Organ_Grinder);
            setup.Agent(Character.Organ_Grinder).GetVote(Arg.Any<IReadOnlyCollection<IOption>>(), Arg.Any<bool>()).Returns(args => args.GetPassOptionFromArg());

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Organ_Grinder).Received().YouAreDead();
        }

        [Test]
        public async Task OrganGrinder_WitchCursed()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Organ_Grinder,Ravenkeeper,Saint,Witch,Soldier,Mayor");
            setup.Agent(Character.Witch).MockWitch(Character.Organ_Grinder);
            setup.Agent(Character.Organ_Grinder).MockNomination(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            foreach (var agent in setup.Agents)
            {
                await agent.Observer.Received(7).AnnounceVote(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<bool>());
                await agent.Observer.Received(1).AnnounceVoteResult(Arg.Any<Player>(), 7, VoteResult.OnTheBlock);
            }
        }
    }
}