using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class StewardTests
    {
        [Test]
        public async Task Steward_SeesGood()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Steward,Imp,Baron,Saint,Soldier,Fisherman,Mayor");

            const Character stewardPing = Character.Saint;
            var stewardPingOptions = setup.Storyteller.MockGetStewardPing(stewardPing);
            var receivedStewardPing = setup.Agents[0].MockNotifySteward(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(stewardPingOptions, Is.EquivalentTo(new[] { Character.Steward, Character.Saint, Character.Soldier, Character.Fisherman, Character.Mayor }));
                Assert.That(receivedStewardPing.Value, Is.EqualTo(stewardPing));
            });
        }

        [Test]
        public async Task Steward_SeesSpy()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Steward,Imp,Spy,Saint,Soldier,Fisherman,Mayor");

            const Character stewardPing = Character.Spy;
            var stewardPingOptions = setup.Storyteller.MockGetStewardPing(stewardPing);
            var receivedStewardPing = setup.Agents[0].MockNotifySteward(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(stewardPingOptions, Does.Contain(stewardPing));
                Assert.That(receivedStewardPing.Value, Is.EqualTo(stewardPing));
            });
        }


        [Test]
        public async Task Steward_SeesRecluse()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Steward,Imp,Baron,Recluse,Soldier,Fisherman,Mayor");

            const Character stewardPing = Character.Recluse;
            var stewardPingOptions = setup.Storyteller.MockGetStewardPing(stewardPing);
            var receivedStewardPing = setup.Agents[0].MockNotifySteward(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(stewardPingOptions, Does.Contain(stewardPing));
                Assert.That(receivedStewardPing.Value, Is.EqualTo(stewardPing));
            });
        }
    }
}