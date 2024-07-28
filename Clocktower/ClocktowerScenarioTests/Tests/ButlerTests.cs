using Clocktower.Game;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class ButlerTests
    {
        [Test]
        public async Task Butler_MasterHasVotedForExecution()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Butler,Baron,Fisherman,Mayor");
            var masterOptions = setup.Agent(Character.Butler).MockButlerChoice(Character.Ravenkeeper);
            setup.Agent(Character.Soldier).MockNomination(Character.Soldier);
            // - by default all players will vote

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(masterOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Soldier, Character.Ravenkeeper, Character.Baron, Character.Fisherman, Character.Mayor }));  // excludes Butler
            await setup.Agent(Character.Butler).Received().GetVote(Arg.Any<IReadOnlyCollection<IOption>>(), false);
        }

        [Test]
        public async Task Butler_MasterHasVotedAgainstExecution()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Butler,Baron,Fisherman,Mayor");
            var masterOptions = setup.Agent(Character.Butler).MockButlerChoice(Character.Ravenkeeper);
            setup.Agent(Character.Soldier).MockNomination(Character.Soldier);
            setup.Agent(Character.Ravenkeeper).GetVote(Arg.Any<IReadOnlyCollection<IOption>>(), false).Returns(args => args.GetPassOptionFromArg());

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(masterOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Soldier, Character.Ravenkeeper, Character.Baron, Character.Fisherman, Character.Mayor }));  // excludes Butler
            await setup.Agent(Character.Butler).DidNotReceive().GetVote(Arg.Any<IReadOnlyCollection<IOption>>(), Arg.Any<bool>());
        }

        [Test]
        public async Task Butler_MasterHasNominatedButNotYetVoted()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Butler,Baron,Fisherman,Mayor");
            var masterOptions = setup.Agent(Character.Butler).MockButlerChoice(Character.Baron);
            setup.Agent(Character.Baron).MockNomination(Character.Ravenkeeper);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(masterOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Soldier, Character.Ravenkeeper, Character.Baron, Character.Fisherman, Character.Mayor }));  // excludes Butler
            await setup.Agent(Character.Butler).DidNotReceive().GetVote(Arg.Any<IReadOnlyCollection<IOption>>(), Arg.Any<bool>());
        }

        [Test]
        public async Task Butler_MasterHasNominatedButVotedAgainstExecution()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Butler,Baron,Fisherman,Mayor");
            var masterOptions = setup.Agent(Character.Butler).MockButlerChoice(Character.Ravenkeeper);
            setup.Agent(Character.Ravenkeeper).MockNomination(Character.Soldier);
            setup.Agent(Character.Ravenkeeper).GetVote(Arg.Any<IReadOnlyCollection<IOption>>(), false).Returns(args => args.GetPassOptionFromArg());

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(masterOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Soldier, Character.Ravenkeeper, Character.Baron, Character.Fisherman, Character.Mayor }));  // excludes Butler
            await setup.Agent(Character.Butler).DidNotReceive().GetVote(Arg.Any<IReadOnlyCollection<IOption>>(), Arg.Any<bool>());
        }

        [Test]
        public async Task Butler_SweetheartDrunk()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Sweetheart,Baron,Butler,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Butler).MockButlerChoice(Character.Mayor);
            setup.Agent(Character.Imp).MockNomination(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Butler);

            await game.RunNightAndDay();

            await setup.Agent(Character.Butler).DidNotReceive().GetVote(Arg.Any<IReadOnlyCollection<IOption>>(), false);

            // Night 2 & Day 2
            setup.Agent(Character.Butler).ClearReceivedCalls();
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Imp).MockNomination(Character.Soldier);

            await game.RunNightAndDay();

            await setup.Agent(Character.Butler).DidNotReceive().GetVote(Arg.Any<IReadOnlyCollection<IOption>>(), false);    // Butler restriction doesn't turn off if drunked
        }

        [Test]
        public async Task PhilosopherButler()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Philosopher,Baron,Fisherman,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Butler);
            var masterOptions = setup.Agent(Character.Philosopher).MockButlerChoice(Character.Ravenkeeper);
            setup.Agent(Character.Soldier).MockNomination(Character.Soldier);
            setup.Agent(Character.Ravenkeeper).GetVote(Arg.Any<IReadOnlyCollection<IOption>>(), false).Returns(args => args.GetPassOptionFromArg());

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(masterOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Soldier, Character.Ravenkeeper, Character.Baron, Character.Fisherman, Character.Mayor }));  // excludes Philo-Butler
            await setup.Agent(Character.Philosopher).DidNotReceive().GetVote(Arg.Any<IReadOnlyCollection<IOption>>(), Arg.Any<bool>());
        }

        [Test]
        public async Task Butler_IsTheMarionette()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Butler,Mayor,Saint,Baron,Soldier,Fisherman")
                            .WithMarionette(Character.Butler)
                            .Build();
            setup.Agent(Character.Butler).MockButlerChoice(Character.Mayor);
            setup.Agent(Character.Imp).MockNomination(Character.Mayor);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Butler).DidNotReceive().GetVote(Arg.Any<IReadOnlyCollection<IOption>>(), false);    // Butler restriction doesn't turn off if the Marionette
        }
    }
}