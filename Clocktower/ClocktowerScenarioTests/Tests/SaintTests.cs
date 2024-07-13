using Clocktower.Game;
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
            setup.Agent(Character.Imp).MockImp(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.False);
        }

        [Test]
        public async Task Saint_FineIfKilledByAssassin()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Assassin,Fisherman,Mayor");
            setup.Agent(Character.Imp).MockImp(Character.Soldier);
            setup.Agent(Character.Assassin).MockAssassin(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
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
            Assert.That(game.Finished, Is.False);
        }
    }
}