using Clocktower.Game;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class SaintTests
    {
        [Test]
        public async Task Saint_LoseIfExecuted()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            setup.Agent(Character.Imp).MockNomination(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
        }

        [Test]
        public async Task Saint_FineIfKilledByImp()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Saint).Received().YouAreDead();
            Assert.That(game.Finished, Is.False);
        }

        [Test]
        public async Task Saint_FineIfKilledByAssassin()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Assassin,Fisherman,Mayor");
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Assassin).MockAssassin(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Saint).Received().YouAreDead();
            Assert.That(game.Finished, Is.False);
        }

        [Test]
        public async Task Saint_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Poisoner,Fisherman,Mayor");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Saint);
            setup.Agent(Character.Imp).MockNomination(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Saint).Received().YouAreDead();
            Assert.That(game.Finished, Is.False);
        }

        [Test]
        public async Task Saint_SweetheartDrunk()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Baron,Sweetheart,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Imp).MockNomination(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Saint);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Imp).MockNomination(Character.Saint);

            await game.RunNightAndDay();

            await setup.Agent(Character.Saint).Received().YouAreDead();
            Assert.That(game.Finished, Is.False);
        }

        [Test]
        public async Task Saint_PhilosopherDrunk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Baron,Fisherman,Philosopher");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Saint);
            setup.Agent(Character.Imp).GetNomination(Arg.Any<Player?>(), Arg.Any<int?>(), Arg.Any<int?>(), Arg.Any<IReadOnlyCollection<IOption>>()).Returns(args => args.GetOptionForRealCharacterFromArg(Character.Saint, argIndex: 3));

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Saint).Received().YouAreDead();
            Assert.That(game.Finished, Is.False);
        }

        [Test]
        public async Task PhilosopherSaint()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Philosopher,Baron,Fisherman,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Saint);
            setup.Agent(Character.Imp).MockNomination(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
        }
    }
}