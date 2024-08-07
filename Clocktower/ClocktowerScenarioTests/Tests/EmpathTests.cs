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

            var empathNumbers = setup.Storyteller.MockGetEmpathNumber(1);
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

            var empathNumbers = setup.Storyteller.MockGetEmpathNumber(1);
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

            var empathNumbers = setup.Storyteller.MockGetEmpathNumber(1);
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
            setup.Agents[0].MockDemonKill(Character.Fisherman);

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
            setup.Agents[0].MockDemonKill(new[] { Character.Fisherman, Character.Saint });

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

            var empathNumbers = setup.Storyteller.MockGetEmpathNumber(empathNumber);
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

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public async Task Empath_IsTheMarionette(int empathNumber)
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Empath,Mayor,Saint,Baron,Soldier,Fisherman")
                            .WithMarionette(Character.Empath)
                            .Build();

            var empathNumbers = setup.Storyteller.MockGetEmpathNumber(empathNumber);
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

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public async Task Empath_Poisoned(int empathNumber)
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Empath,Saint,Poisoner,Soldier,Fisherman");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Empath);
            var empathNumbers = setup.Storyteller.MockGetEmpathNumber(empathNumber);
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

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public async Task Empath_SweetheartDrunk(int empathNumber)
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Empath,Sweetheart,Baron,Soldier,Fisherman");
            setup.Storyteller.MockGetEmpathNumber(empathNumber);
            await game.StartGame();

            // Night 1 & Day 1
            var firstEmpathNumber = setup.Agent(Character.Empath).MockNotifyEmpath();

            await game.RunNightAndDay();

            Assert.That(firstEmpathNumber.Value, Is.EqualTo(0));

            // Night 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Empath);
            var secondEmpathNumber = setup.Agent(Character.Empath).MockNotifyEmpath(gameToEnd: game);

            await game.RunNightAndDay();

            Assert.That(secondEmpathNumber.Value, Is.EqualTo(empathNumber));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public async Task Empath_PhilosopherDrunk(int empathNumber)
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Empath,Saint,Baron,Soldier,Philosopher");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Empath);
            var empathNumbers = setup.Storyteller.MockGetEmpathNumber(empathNumber);
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

        [Test]
        public async Task PhilosopherEmpath()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Philosopher,Baron,Saint,Soldier,Fisherman");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Empath);
            var receivedEmpathNumber = setup.Agent(Character.Philosopher).MockNotifyEmpath(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedEmpathNumber.Value, Is.EqualTo(1));
        }

        [Test]
        public async Task CannibalEmpath()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Cannibal,Baron,Saint,Soldier,Empath");
            setup.Agent(Character.Imp).MockNomination(Character.Empath);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            var receivedEmpathNumber = setup.Agent(Character.Cannibal).MockNotifyEmpath(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedEmpathNumber.Value, Is.EqualTo(1));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public async Task CannibalEmpath_Poisoned(int empathNumber)
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Cannibal,Baron,Saint,Soldier,Fisherman");
            setup.Agent(Character.Imp).MockNomination(Character.Baron);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Storyteller.MockCannibalChoice(Character.Empath);
            var empathNumbers = setup.Storyteller.MockGetEmpathNumber(empathNumber);
            var receivedEmpathNumber = setup.Agent(Character.Cannibal).MockNotifyEmpath(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
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