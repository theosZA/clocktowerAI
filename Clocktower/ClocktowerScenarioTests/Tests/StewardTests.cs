using Clocktower.Game;
using Clocktower.Options;
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
            var receivedStewardPing = setup.Agent(Character.Steward).MockNotifySteward(gameToEnd: game);

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
            var receivedStewardPing = setup.Agent(Character.Steward).MockNotifySteward(gameToEnd: game);

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
            var receivedStewardPing = setup.Agent(Character.Steward).MockNotifySteward(gameToEnd: game);

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

        [TestCase(Character.Imp)]
        [TestCase(Character.Baron)]
        [TestCase(Character.Saint)]
        [TestCase(Character.Soldier)]
        public async Task Steward_IsTheDrunk(Character stewardPing)
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Steward,Imp,Baron,Saint,Soldier,Fisherman,Mayor")
                            .WithDrunk(Character.Steward)
                            .Build();

            setup.Storyteller.MockGetStewardPing(stewardPing);
            var receivedStewardPing = setup.Agent(Character.Steward).MockNotifySteward(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedStewardPing.Value, Is.EqualTo(stewardPing));
        }

        [TestCase(Character.Imp)]
        [TestCase(Character.Poisoner)]
        [TestCase(Character.Saint)]
        [TestCase(Character.Soldier)]
        public async Task Steward_Poisoned(Character stewardPing)
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Steward,Imp,Poisoner,Saint,Soldier,Fisherman,Mayor");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Steward);

            setup.Storyteller.MockGetStewardPing(stewardPing);
            var receivedStewardPing = setup.Agent(Character.Steward).MockNotifySteward(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedStewardPing.Value, Is.EqualTo(stewardPing));
        }

        [TestCase(Character.Imp)]
        [TestCase(Character.Baron)]
        [TestCase(Character.Saint)]
        [TestCase(Character.Soldier)]
        public async Task Steward_PhilosopherDrunk(Character stewardPing)
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Steward,Imp,Baron,Saint,Soldier,Fisherman,Philosopher");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Steward);
            setup.Storyteller.GetStewardPing(Arg.Is<Player>(player => player.RealCharacter == Character.Philosopher), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetOptionForCharacterFromArg(Character.Soldier, argIndex: 1));

            setup.Storyteller.GetStewardPing(Arg.Is<Player>(player => player.RealCharacter == Character.Steward), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetOptionForCharacterFromArg(stewardPing, argIndex: 1));
            var receivedStewardPing = setup.Agent(Character.Steward).MockNotifySteward(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedStewardPing.Value, Is.EqualTo(stewardPing));
        }

        [Test]
        public async Task PhilosopherSteward()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Philosopher,Imp,Baron,Saint,Soldier,Fisherman,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Steward);
            const Character stewardPing = Character.Saint;
            var stewardPingOptions = setup.Storyteller.MockGetStewardPing(stewardPing);
            var receivedStewardPing = setup.Agent(Character.Philosopher).MockNotifySteward(gameToEnd: game);

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
    }
}