using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class EmpathTests
    {
        [Test]
        public async Task Empath_NoAdjacentEvils()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Empath,Saint,Baron,Soldier,Fisherman");

            var receivedEmpathNumber = setup.Agent(Character.Empath).MockNotifyEmpath(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedEmpathNumber.Value, Is.EqualTo(0));
        }

        [Test]
        public async Task Empath_OneAdjacentEvil()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Empath,Baron,Saint,Soldier,Fisherman");

            var receivedEmpathNumber = setup.Agent(Character.Empath).MockNotifyEmpath(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedEmpathNumber.Value, Is.EqualTo(1));
        }

        [Test]
        public async Task Empath_TwoAdjacentEvils()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Mayor,Imp,Empath,Baron,Saint,Soldier,Fisherman");

            var receivedEmpathNumber = setup.Agent(Character.Empath).MockNotifyEmpath(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedEmpathNumber.Value, Is.EqualTo(2));
        }

        [Test]
        public async Task Empath_AdjacentToRecluse()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Empath,Recluse,Baron,Soldier,Fisherman");

            var empathNumbers = setup.Storyteller.MockGetEmpathNumbers(1);
            var receivedEmpathNumber = setup.Agent(Character.Empath).MockNotifyEmpath(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(empathNumbers, Is.EquivalentTo(new[] { 0, 1 }));
                Assert.That(receivedEmpathNumber.Value, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task Empath_AdjacentToSpy()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Empath,Spy,Saint,Soldier,Fisherman");

            var empathNumbers = setup.Storyteller.MockGetEmpathNumbers(1);
            var receivedEmpathNumber = setup.Agent(Character.Empath).MockNotifyEmpath(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(empathNumbers, Is.EquivalentTo(new[] { 0, 1 }));
                Assert.That(receivedEmpathNumber.Value, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task Empath_AdjacentToRecluseAndSpy()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Recluse,Empath,Spy,Mayor,Soldier,Fisherman");

            var empathNumbers = setup.Storyteller.MockGetEmpathNumbers(1);
            var receivedEmpathNumber = setup.Agent(Character.Empath).MockNotifyEmpath(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(empathNumbers, Is.EquivalentTo(new[] { 0, 1, 2 }));
                Assert.That(receivedEmpathNumber.Value, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task Empath_OneDeadNeighbour()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fisherman,Empath,Saint,Baron,Soldier,Mayor");

            var receivedEmpathNumber = setup.Agent(Character.Empath).MockNotifyEmpath();
            setup.Agents[0].MockImp(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedEmpathNumber.Value, Is.EqualTo(1));
        }

        [Test]
        public async Task Empath_TwoDeadNeighbours()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fisherman,Empath,Saint,Baron,Soldier,Mayor");

            var receivedEmpathNumber = setup.Agent(Character.Empath).MockNotifyEmpath();
            setup.Agents[0].MockImp(new[] { Character.Fisherman, Character.Saint });

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedEmpathNumber.Value, Is.EqualTo(2));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public async Task Empath_IsTheDrunk(int empathNumber)
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Mayor,Empath,Saint,Baron,Soldier,Fisherman")
                            .WithDrunk(Character.Empath)
                            .Build();

            var empathNumbers = setup.Storyteller.MockGetEmpathNumbers(empathNumber);
            var receivedEmpathNumber = setup.Agent(Character.Empath).MockNotifyEmpath(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(empathNumbers, Is.EquivalentTo(new[] { 0, 1, 2 }));
                Assert.That(receivedEmpathNumber.Value, Is.EqualTo(empathNumber));
            });
        }
    }
}