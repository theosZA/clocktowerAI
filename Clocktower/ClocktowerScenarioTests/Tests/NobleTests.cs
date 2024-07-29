using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class NobleTests
    {
        [Test]
        public async Task Noble_SeesTwoGoodOneEvil()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Noble,Imp,Baron,Saint,Soldier,Fisherman,Mayor");
            setup.Storyteller.MockGetNobleInformation(Character.Baron, Character.Saint, Character.Soldier);
            var receivedNoblePings = setup.Agent(Character.Noble).MockNotifyNoble(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedNoblePings, Is.EquivalentTo(new[] { Character.Baron, Character.Saint, Character.Soldier }));
        }

        [Test]
        public async Task Noble_SeesSpyAsGood()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Noble,Imp,Spy,Saint,Soldier,Fisherman,Mayor");
            setup.Storyteller.MockGetNobleInformation(Character.Imp, Character.Saint, Character.Spy);
            var receivedNoblePings = setup.Agent(Character.Noble).MockNotifyNoble(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedNoblePings, Is.EquivalentTo(new[] { Character.Imp, Character.Saint, Character.Spy }));
        }

        [Test]
        public async Task Noble_SeesRecluseAsEvil()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Noble,Imp,Baron,Recluse,Soldier,Fisherman,Mayor");
            setup.Storyteller.MockGetNobleInformation(Character.Recluse, Character.Soldier, Character.Fisherman);
            var receivedNoblePings = setup.Agent(Character.Noble).MockNotifyNoble(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedNoblePings, Is.EquivalentTo(new[] { Character.Recluse, Character.Soldier, Character.Fisherman }));
        }

        [Test]
        public async Task Noble_IsTheDrunk()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Noble,Imp,Baron,Saint,Soldier,Fisherman,Mayor")
                            .WithDrunk(Character.Noble)
                            .Build();
            setup.Storyteller.MockGetNobleInformation(Character.Mayor, Character.Saint, Character.Soldier);
            var receivedNoblePings = setup.Agent(Character.Noble).MockNotifyNoble(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedNoblePings, Is.EquivalentTo(new[] { Character.Mayor, Character.Saint, Character.Soldier }));
        }

        [Test]
        public async Task Noble_IsTheMarionette()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Noble,Imp,Baron,Saint,Soldier,Fisherman,Mayor")
                            .WithMarionette(Character.Noble)
                            .Build();
            setup.Storyteller.MockGetNobleInformation(Character.Mayor, Character.Saint, Character.Soldier);
            var receivedNoblePings = setup.Agent(Character.Noble).MockNotifyNoble(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedNoblePings, Is.EquivalentTo(new[] { Character.Mayor, Character.Saint, Character.Soldier }));
        }

        [Test]
        public async Task Noble_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Noble,Imp,Poisoner,Saint,Soldier,Fisherman,Mayor");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Noble);
            setup.Storyteller.MockGetNobleInformation(Character.Mayor, Character.Saint, Character.Soldier);
            var receivedNoblePings = setup.Agent(Character.Noble).MockNotifyNoble(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedNoblePings, Is.EquivalentTo(new[] { Character.Mayor, Character.Saint, Character.Soldier }));
        }

        [Test]
        public async Task PhilosopherNoble()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Philosopher,Imp,Baron,Saint,Soldier,Fisherman,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Noble);
            setup.Storyteller.MockGetNobleInformation(Character.Baron, Character.Saint, Character.Soldier);
            var receivedNoblePings = setup.Agent(Character.Philosopher).MockNotifyNoble(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedNoblePings, Is.EquivalentTo(new[] { Character.Baron, Character.Saint, Character.Soldier }));
        }
    }
}