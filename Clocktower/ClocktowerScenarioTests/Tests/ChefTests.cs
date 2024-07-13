using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class ChefTests
    {
        [Test]
        public async Task Chef_NoEvilPairs()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Chef,Saint,Baron,Soldier,Fisherman");

            var receivedChefNumber = setup.Agent(Character.Chef).MockNotifyChef(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedChefNumber.Value, Is.EqualTo(0));
        }

        [Test]
        public async Task Chef_OneEvilPairs()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Chef,Saint,Mayor,Soldier,Fisherman");

            var receivedChefNumber = setup.Agent(Character.Chef).MockNotifyChef(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedChefNumber.Value, Is.EqualTo(1));
        }

        [Test]
        public async Task Chef_TwoSeparateEvilPairs()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Chef,Saint,Scarlet_Woman,Assassin,Fisherman");

            var receivedChefNumber = setup.Agent(Character.Chef).MockNotifyChef(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedChefNumber.Value, Is.EqualTo(2));
        }

        [Test]
        public async Task Chef_EvilGroupOfThree()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Chef,Saint,Mayor,Soldier,Scarlet_Woman");

            var receivedChefNumber = setup.Agent(Character.Chef).MockNotifyChef(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedChefNumber.Value, Is.EqualTo(2));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public async Task Chef_Spy(int chefNumber)
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Spy,Baron,Chef,Mayor,Soldier,Fisherman");

            var chefNumbers = setup.Storyteller.MockGetChefNumber(chefNumber);
            var receivedChefNumber = setup.Agent(Character.Chef).MockNotifyChef(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(chefNumbers, Is.EquivalentTo(new[] { 0, 1, 2 }));
                Assert.That(receivedChefNumber.Value, Is.EqualTo(chefNumber));
            });
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public async Task Chef_Recluse(int chefNumber)
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Recluse,Baron,Chef,Mayor,Soldier,Fisherman");

            var chefNumbers = setup.Storyteller.MockGetChefNumber(chefNumber);
            var receivedChefNumber = setup.Agent(Character.Chef).MockNotifyChef(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(chefNumbers, Is.EquivalentTo(new[] { 0, 1, 2 }));
                Assert.That(receivedChefNumber.Value, Is.EqualTo(chefNumber));
            });
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(7)]
        public async Task Chef_IsTheDrunk(int chefNumber)
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Mayor,Chef,Saint,Baron,Soldier,Fisherman")
                            .WithDrunk(Character.Chef)
                            .Build();

            var chefNumbers = setup.Storyteller.MockGetChefNumber(chefNumber);
            var receivedChefNumber = setup.Agent(Character.Chef).MockNotifyChef(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(chefNumbers, Is.EquivalentTo(new[] { 0, 1, 2, 3, 4, 5, 6, 7 }));    // 7 is the largest possible value, even if patently absurd.
                Assert.That(receivedChefNumber.Value, Is.EqualTo(chefNumber));
            });
        }
    }
}