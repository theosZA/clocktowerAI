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
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Slayer,Empath,Saint,Baron,Soldier,Fisherman,Mayor");

            var receivedEmpathNumber = setup.Agents[2].MockNotifyEmpath(gameToEnd: game);

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
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Slayer,Empath,Baron,Saint,Soldier,Fisherman,Mayor");

            var receivedEmpathNumber = setup.Agents[2].MockNotifyEmpath(gameToEnd: game);

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
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Slayer,Imp,Empath,Baron,Saint,Soldier,Fisherman,Mayor");

            var receivedEmpathNumber = setup.Agents[2].MockNotifyEmpath(gameToEnd: game);

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
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Slayer,Empath,Recluse,Baron,Soldier,Fisherman,Mayor");

            var empathNumbers = setup.Storyteller.MockGetEmpathNumbers(1);
            var receivedEmpathNumber = setup.Agents[2].MockNotifyEmpath(gameToEnd: game);

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
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Slayer,Empath,Spy,Saint,Soldier,Fisherman,Mayor");

            var empathNumbers = setup.Storyteller.MockGetEmpathNumbers(1);
            var receivedEmpathNumber = setup.Agents[2].MockNotifyEmpath(gameToEnd: game);

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
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Recluse,Empath,Spy,Slayer,Soldier,Fisherman,Mayor");

            var empathNumbers = setup.Storyteller.MockGetEmpathNumbers(1);
            var receivedEmpathNumber = setup.Agents[2].MockNotifyEmpath(gameToEnd: game);

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
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Slayer,Empath,Saint,Baron,Soldier,Fisherman,Mayor");

            var receivedEmpathNumber = setup.Agents[2].MockNotifyEmpath();
            setup.Agents[0].MockImp(Character.Slayer);

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
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Slayer,Empath,Saint,Baron,Soldier,Fisherman,Mayor");

            var receivedEmpathNumber = setup.Agents[2].MockNotifyEmpath();
            setup.Agents[0].MockImp(new[] { Character.Slayer, Character.Saint });

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedEmpathNumber.Value, Is.EqualTo(2));
        }
    }
}