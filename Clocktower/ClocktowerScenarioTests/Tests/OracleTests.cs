using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class OracleTests
    {
        [Test]
        public async Task Oracle_NoDeadPlayers()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Oracle,Saint,Baron,Soldier,Fisherman");

            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            var receivedOracleNumber = setup.Agent(Character.Oracle).MockNotifyOracle(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedOracleNumber.Value, Is.EqualTo(0));
        }

        [Test]
        public async Task Oracle_OnePlayerDeadWhoIsGood()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Oracle,Saint,Baron,Soldier,Fisherman");

            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);
            var receivedOracleNumber = setup.Agent(Character.Oracle).MockNotifyOracle(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedOracleNumber.Value, Is.EqualTo(0));
        }

        [Test]
        public async Task Oracle_OnePlayerDeadWhoIsEvil()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Oracle,Saint,Baron,Soldier,Fisherman");

            setup.Agent(Character.Imp).MockDemonKill(Character.Baron);
            var receivedOracleNumber = setup.Agent(Character.Oracle).MockNotifyOracle(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedOracleNumber.Value, Is.EqualTo(1));
        }

        [Test]
        public async Task Oracle_MultipleDeadPlayers()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Oracle,Saint,Baron,Soldier,Fisherman");

            setup.Agent(Character.Imp).MockNomination(Character.Baron);
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);
            var receivedOracleNumber = setup.Agent(Character.Oracle).MockNotifyOracle(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedOracleNumber.Value, Is.EqualTo(1));
        }

        [TestCase(0)]
        [TestCase(1)]
        public async Task Oracle_DeadRecluse(int oracleNumber)
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Oracle,Recluse,Baron,Soldier,Fisherman");

            setup.Agent(Character.Imp).MockDemonKill(Character.Recluse);
            var possibleOracleNumbers = setup.Storyteller.MockGetOracleNumber(oracleNumber);
            var receivedOracleNumber = setup.Agent(Character.Oracle).MockNotifyOracle(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(receivedOracleNumber.Value, Is.EqualTo(oracleNumber));
                Assert.That(possibleOracleNumbers, Is.EquivalentTo(new[] { 0, 1 }));
            });
        }

        [TestCase(0)]
        [TestCase(1)]
        public async Task Oracle_DeadSpy(int oracleNumber)
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Oracle,Saint,Spy,Soldier,Fisherman");

            setup.Agent(Character.Imp).MockDemonKill(Character.Spy);
            var possibleOracleNumbers = setup.Storyteller.MockGetOracleNumber(oracleNumber);
            var receivedOracleNumber = setup.Agent(Character.Oracle).MockNotifyOracle(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(receivedOracleNumber.Value, Is.EqualTo(oracleNumber));
                Assert.That(possibleOracleNumbers, Is.EquivalentTo(new[] { 0, 1 }));
            });
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public async Task Oracle_IsTheDrunk(int oracleNumber)
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Mayor,Oracle,Saint,Baron,Soldier,Fisherman")
                            .WithDrunk(Character.Oracle)
                            .Build();

            setup.Agent(Character.Imp).MockNomination(Character.Baron);
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);
            var possibleOracleNumbers = setup.Storyteller.MockGetOracleNumber(oracleNumber);
            var receivedOracleNumber = setup.Agent(Character.Oracle).MockNotifyOracle(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(receivedOracleNumber.Value, Is.EqualTo(oracleNumber));
                Assert.That(possibleOracleNumbers, Is.EquivalentTo(new[] { 0, 1, 2 }));
            });
        }

        [Test]
        public async Task Oracle_IsTheMarionette()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Oracle,Mayor,Saint,Baron,Soldier,Fisherman")
                            .WithMarionette(Character.Oracle)
                            .Build();

            setup.Agent(Character.Imp).MockNomination(Character.Baron);
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);
            var possibleOracleNumbers = setup.Storyteller.MockGetOracleNumber(2);
            var receivedOracleNumber = setup.Agent(Character.Oracle).MockNotifyOracle(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(receivedOracleNumber.Value, Is.EqualTo(2));
                Assert.That(possibleOracleNumbers, Is.EquivalentTo(new[] { 0, 1, 2 }));
            });
        }

        [Test]
        public async Task Oracle_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Oracle,Saint,Poisoner,Soldier,Fisherman");

            setup.Agent(Character.Poisoner).MockPoisoner(Character.Oracle);
            setup.Agent(Character.Imp).MockNomination(Character.Soldier);
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);
            var possibleOracleNumbers = setup.Storyteller.MockGetOracleNumber(2);
            var receivedOracleNumber = setup.Agent(Character.Oracle).MockNotifyOracle(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(receivedOracleNumber.Value, Is.EqualTo(2));
                Assert.That(possibleOracleNumbers, Is.EquivalentTo(new[] { 0, 1, 2 }));
            });
        }

        [Test]
        public async Task PhilosopherOracle()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Philosopher,Baron,Saint,Soldier,Fisherman");

            setup.Agent(Character.Imp).MockNomination(Character.Baron);
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Oracle);
            var receivedOracleNumber = setup.Agent(Character.Philosopher).MockNotifyOracle(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedOracleNumber.Value, Is.EqualTo(1));
        }

        [Test]
        public async Task CannibalOracle()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Cannibal,Baron,Saint,Soldier,Oracle");
            
            setup.Agent(Character.Imp).MockNomination(Character.Oracle);
            setup.Agent(Character.Imp).MockDemonKill(Character.Baron);
            var receivedOracleNumber = setup.Agent(Character.Cannibal).MockNotifyOracle(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedOracleNumber.Value, Is.EqualTo(1));
        }

        public async Task CannibalOracle_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Cannibal,Baron,Saint,Soldier,Fisherman");
            setup.Agent(Character.Imp).MockNomination(Character.Baron);
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);
            setup.Storyteller.MockCannibalChoice(Character.Oracle);
            var possibleOracleNumbers = setup.Storyteller.MockGetOracleNumber(2);
            var receivedOracleNumber = setup.Agent(Character.Cannibal).MockNotifyOracle(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(possibleOracleNumbers, Is.EquivalentTo(new[] { 0, 1, 2 }));
                Assert.That(receivedOracleNumber.Value, Is.EqualTo(2));
            });
        }
    }
}