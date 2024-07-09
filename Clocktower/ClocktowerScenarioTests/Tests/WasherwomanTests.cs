using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class WasherwomanTests
    {
        [Test]
        public async Task Washerwoman_SeesTownsfolkPlusAnyOther()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Washerwoman,Imp,Baron,Saint,Soldier,Fisherman,Mayor,Slayer");

            const Character washerwomanPing = Character.Slayer;
            const Character washerwomanWrong = Character.Imp;
            var washerwomanPingOptions = setup.Storyteller.MockGetWasherwomanPing(washerwomanPing, washerwomanWrong, washerwomanPing);
            var receivedWasherwomanPing = setup.Agents[0].MockNotifyWasherwoman(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                // Spot check that we can do all combinations of Townsfolk + Townsfolk, Townsfolk + Outsider, Townsfolk + Minion, Townsfolk + Demon.
                Assert.That(washerwomanPingOptions, Does.Contain((Character.Soldier, Character.Fisherman, Character.Soldier)));
                Assert.That(washerwomanPingOptions, Does.Contain((Character.Mayor, Character.Saint, Character.Mayor)));
                Assert.That(washerwomanPingOptions, Does.Contain((Character.Slayer, Character.Baron, Character.Slayer)));
                Assert.That(washerwomanPingOptions, Does.Contain((Character.Fisherman, Character.Imp, Character.Fisherman)));

                Assert.That(receivedWasherwomanPing.Value.playerA, Is.EqualTo(washerwomanPing).Or.EqualTo(washerwomanWrong));
                Assert.That(receivedWasherwomanPing.Value.playerB, Is.EqualTo(washerwomanPing).Or.EqualTo(washerwomanWrong));
                Assert.That(receivedWasherwomanPing.Value.playerA, Is.Not.EqualTo(receivedWasherwomanPing.Value.playerB));
                Assert.That(receivedWasherwomanPing.Value.seenCharacter, Is.EqualTo(washerwomanPing));
            });
        }

        [Test]
        public async Task Washerwoman_SeesSpy()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Washerwoman,Imp,Spy,Saint,Soldier,Fisherman,Mayor,Slayer");

            const Character washerwomanPing = Character.Spy;
            const Character washerwomanWrong = Character.Soldier;
            const Character spySeenAs = Character.Chef;
            var washerwomanPingOptions = setup.Storyteller.MockGetWasherwomanPing(washerwomanPing, washerwomanWrong, spySeenAs);
            var receivedWasherwomanPing = setup.Agents[0].MockNotifyWasherwoman(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(washerwomanPingOptions, Does.Contain((washerwomanPing, washerwomanWrong, spySeenAs)));
                Assert.That(receivedWasherwomanPing.Value.playerA, Is.EqualTo(washerwomanPing).Or.EqualTo(washerwomanWrong));
                Assert.That(receivedWasherwomanPing.Value.playerB, Is.EqualTo(washerwomanPing).Or.EqualTo(washerwomanWrong));
                Assert.That(receivedWasherwomanPing.Value.playerA, Is.Not.EqualTo(receivedWasherwomanPing.Value.playerB));
                Assert.That(receivedWasherwomanPing.Value.seenCharacter, Is.EqualTo(spySeenAs));
            });
        }
    }
}
