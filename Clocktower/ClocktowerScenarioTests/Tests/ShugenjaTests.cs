using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class ShugenjaTests
    {
        [Test]
        public async Task Shugenja_UniqueEvilClockwise()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Slayer,Shugenja,Baron,Saint,Soldier,Fisherman,Mayor");

            var receivedShugenjaDirection = setup.Agents[2].MockNotifyShugenja(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedShugenjaDirection.Value, Is.EqualTo(Direction.Clockwise));
        }

        [Test]
        public async Task Shugenja_UniqueEvilCounterclockwise()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Shugenja,Slayer,Baron,Saint,Soldier,Fisherman,Mayor");

            var receivedShugenjaDirection = setup.Agents[1].MockNotifyShugenja(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedShugenjaDirection.Value, Is.EqualTo(Direction.Counterclockwise));
        }

        [Test]
        public async Task Shugenja_TwoEvilsEquidistant()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Slayer,Shugenja,Saint,Baron,Soldier,Fisherman,Mayor");

            var shugenjaOptions = setup.Storyteller.MockGetShugenjaDirection(Direction.Counterclockwise);
            var receivedShugenjaDirection = setup.Agents[2].MockNotifyShugenja(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(shugenjaOptions, Is.EquivalentTo(new[] { Direction.Clockwise, Direction.Counterclockwise }));
                Assert.That(receivedShugenjaDirection.Value, Is.EqualTo(Direction.Counterclockwise));
            });
        }

        [Test]
        public async Task Shugenja_WithSpy()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Slayer,Shugenja,Spy,Saint,Soldier,Fisherman,Mayor");

            var shugenjaOptions = setup.Storyteller.MockGetShugenjaDirection(Direction.Counterclockwise);
            var receivedShugenjaDirection = setup.Agents[2].MockNotifyShugenja(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(shugenjaOptions, Is.EquivalentTo(new[] { Direction.Clockwise, Direction.Counterclockwise }));
                Assert.That(receivedShugenjaDirection.Value, Is.EqualTo(Direction.Counterclockwise));
            });
        }

        [Test]
        public async Task Shugenja_WithRecluse()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Soldier,Recluse,Shugenja,Slayer,Baron,Imp,Fisherman,Mayor");

            var shugenjaOptions = setup.Storyteller.MockGetShugenjaDirection(Direction.Counterclockwise);
            var receivedShugenjaDirection = setup.Agents[2].MockNotifyShugenja(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(shugenjaOptions, Is.EquivalentTo(new[] { Direction.Clockwise, Direction.Counterclockwise }));
                Assert.That(receivedShugenjaDirection.Value, Is.EqualTo(Direction.Counterclockwise));
            });
        }
    }
}