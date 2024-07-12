using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class MonkTests
    {
        [Test]
        public async Task Monk_ProtectFromImp()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Monk,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            var monkOptions = setup.Agent(Character.Monk).MockMonkChoice(Character.Saint);
            setup.Agent(Character.Imp).MockImp(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(monkOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Ravenkeeper, Character.Saint, Character.Baron, Character.Fisherman, Character.Mayor }));  // not Monk
            await setup.Agent(Character.Saint).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Monk_CanNotProtectFromAssassin()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Monk,Ravenkeeper,Saint,Assassin,Fisherman,Mayor");
            var monkOptions = setup.Agent(Character.Monk).MockMonkChoice(Character.Saint);
            setup.Agent(Character.Imp).MockImp(Character.Fisherman);
            setup.Agent(Character.Assassin).MockAssassin(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(monkOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Ravenkeeper, Character.Saint, Character.Assassin, Character.Fisherman, Character.Mayor }));  // not Monk
            await setup.Agent(Character.Fisherman).Received().YouAreDead();
            await setup.Agent(Character.Saint).Received().YouAreDead();
        }

        [Test]
        public async Task Monk_ProtectEachNight()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Monk,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Monk).MockMonkChoice(Character.Saint);
            setup.Agent(Character.Imp).MockImp(Character.Fisherman);

            await game.RunNightAndDay();

            await setup.Agent(Character.Fisherman).Received().YouAreDead();
            setup.Agent(Character.Monk).ClearReceivedCalls();
            setup.Agent(Character.Imp).ClearReceivedCalls();

            // Night 3 & Day 3
            var monkOptions = setup.Agent(Character.Monk).MockMonkChoice(Character.Saint);
            setup.Agent(Character.Imp).MockImp(Character.Saint);

            await game.RunNightAndDay();

            Assert.That(monkOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Ravenkeeper, Character.Saint, Character.Baron, Character.Fisherman, Character.Mayor }));  // not Monk
            await setup.Agent(Character.Saint).DidNotReceive().YouAreDead();
        }
    }
}