using Clocktower.Game;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class WasherwomanTests
    {
        [Test]
        public async Task Washerwoman_SeesTownsfolkPlusAnyOther()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Washerwoman,Imp,Baron,Saint,Soldier,Fisherman,Mayor");

            const Character washerwomanPing = Character.Fisherman;
            const Character washerwomanWrong = Character.Imp;
            var washerwomanPingOptions = setup.Storyteller.MockGetWasherwomanPing(washerwomanPing, washerwomanWrong, washerwomanPing);
            var receivedWasherwomanPing = setup.Agent(Character.Washerwoman).MockNotifyWasherwoman(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                // Spot check that we can do all combinations of Townsfolk + Townsfolk, Townsfolk + Outsider, Townsfolk + Minion, Townsfolk + Demon.
                Assert.That(washerwomanPingOptions, Does.Contain((Character.Soldier, Character.Fisherman, Character.Soldier)));
                Assert.That(washerwomanPingOptions, Does.Contain((Character.Mayor, Character.Saint, Character.Mayor)));
                Assert.That(washerwomanPingOptions, Does.Contain((Character.Fisherman, Character.Baron, Character.Fisherman)));
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
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Washerwoman,Imp,Spy,Saint,Soldier,Fisherman,Mayor");

            const Character washerwomanPing = Character.Spy;
            const Character washerwomanWrong = Character.Soldier;
            const Character spySeenAs = Character.Chef;
            var washerwomanPingOptions = setup.Storyteller.MockGetWasherwomanPing(washerwomanPing, washerwomanWrong, spySeenAs);
            var receivedWasherwomanPing = setup.Agent(Character.Washerwoman).MockNotifyWasherwoman(gameToEnd: game);

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

        [Test]
        public async Task Washerwoman_CanNotSeeDrunk()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Washerwoman,Imp,Baron,Saint,Soldier,Fisherman,Mayor")
                            .WithDrunk(Character.Soldier)
                            .Build();

            var washerwomanPingOptions = setup.Storyteller.MockGetWasherwomanPing(Character.Fisherman, Character.Mayor, Character.Fisherman);
            setup.Agent(Character.Washerwoman).MockNotifyWasherwoman(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            foreach (var (washerwomanPing, _, _) in washerwomanPingOptions)
            {
                Assert.That(washerwomanPing, Is.Not.EqualTo(Character.Soldier));
            }
        }

        [Test]
        public async Task Washerwoman_IsTheDrunk()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Washerwoman,Imp,Baron,Saint,Soldier,Fisherman,Mayor")
                            .WithDrunk(Character.Washerwoman)
                            .Build();

            const Character washerwomanPing = Character.Imp;
            const Character washerwomanWrong = Character.Soldier;
            const Character pingCharacter = Character.Empath;
            setup.Storyteller.MockGetWasherwomanPing(washerwomanPing, washerwomanWrong, pingCharacter);
            var receivedWasherwomanPing = setup.Agent(Character.Washerwoman).MockNotifyWasherwoman(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(receivedWasherwomanPing.Value.playerA, Is.EqualTo(washerwomanPing).Or.EqualTo(washerwomanWrong));
                Assert.That(receivedWasherwomanPing.Value.playerB, Is.EqualTo(washerwomanPing).Or.EqualTo(washerwomanWrong));
                Assert.That(receivedWasherwomanPing.Value.playerA, Is.Not.EqualTo(receivedWasherwomanPing.Value.playerB));
                Assert.That(receivedWasherwomanPing.Value.seenCharacter, Is.EqualTo(pingCharacter));
            });
        }

        [Test]
        public async Task Washerwoman_IsTheMarionette()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Washerwoman,Imp,Baron,Saint,Soldier,Fisherman,Mayor")
                            .WithMarionette(Character.Washerwoman)
                            .Build();

            const Character washerwomanPing = Character.Imp;
            const Character washerwomanWrong = Character.Soldier;
            const Character pingCharacter = Character.Empath;
            setup.Storyteller.MockGetWasherwomanPing(washerwomanPing, washerwomanWrong, pingCharacter);
            var receivedWasherwomanPing = setup.Agent(Character.Washerwoman).MockNotifyWasherwoman(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(receivedWasherwomanPing.Value.playerA, Is.EqualTo(washerwomanPing).Or.EqualTo(washerwomanWrong));
                Assert.That(receivedWasherwomanPing.Value.playerB, Is.EqualTo(washerwomanPing).Or.EqualTo(washerwomanWrong));
                Assert.That(receivedWasherwomanPing.Value.playerA, Is.Not.EqualTo(receivedWasherwomanPing.Value.playerB));
                Assert.That(receivedWasherwomanPing.Value.seenCharacter, Is.EqualTo(pingCharacter));
            });
        }

        [Test]
        public async Task Washerwoman_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Washerwoman,Imp,Poisoner,Saint,Soldier,Fisherman,Mayor");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Washerwoman);

            const Character washerwomanPing = Character.Imp;
            const Character washerwomanWrong = Character.Soldier;
            const Character pingCharacter = Character.Empath;
            setup.Storyteller.MockGetWasherwomanPing(washerwomanPing, washerwomanWrong, pingCharacter);
            var receivedWasherwomanPing = setup.Agent(Character.Washerwoman).MockNotifyWasherwoman(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(receivedWasherwomanPing.Value.playerA, Is.EqualTo(washerwomanPing).Or.EqualTo(washerwomanWrong));
                Assert.That(receivedWasherwomanPing.Value.playerB, Is.EqualTo(washerwomanPing).Or.EqualTo(washerwomanWrong));
                Assert.That(receivedWasherwomanPing.Value.playerA, Is.Not.EqualTo(receivedWasherwomanPing.Value.playerB));
                Assert.That(receivedWasherwomanPing.Value.seenCharacter, Is.EqualTo(pingCharacter));
            });
        }

        [Test]
        public async Task Washerwoman_PhilosopherDrunk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Washerwoman,Imp,Baron,Saint,Soldier,Fisherman,Philosopher");

            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Washerwoman);
            setup.Storyteller.GetWasherwomanPings(Arg.Is<Player>(player => player.RealCharacter == Character.Philosopher), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetMatchingOptionFromOptionsArg((Character.Soldier, Character.Fisherman, Character.Soldier), new(), argIndex: 1));

            const Character washerwomanPing = Character.Imp;
            const Character washerwomanWrong = Character.Soldier;
            const Character pingCharacter = Character.Empath;
            setup.Storyteller.GetWasherwomanPings(Arg.Is<Player>(player => player.RealCharacter == Character.Washerwoman), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetMatchingOptionFromOptionsArg((washerwomanPing, washerwomanWrong, pingCharacter), new(), argIndex: 1));
            var receivedWasherwomanPing = setup.Agent(Character.Washerwoman).MockNotifyWasherwoman(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(receivedWasherwomanPing.Value.playerA, Is.EqualTo(washerwomanPing).Or.EqualTo(washerwomanWrong));
                Assert.That(receivedWasherwomanPing.Value.playerB, Is.EqualTo(washerwomanPing).Or.EqualTo(washerwomanWrong));
                Assert.That(receivedWasherwomanPing.Value.playerA, Is.Not.EqualTo(receivedWasherwomanPing.Value.playerB));
                Assert.That(receivedWasherwomanPing.Value.seenCharacter, Is.EqualTo(pingCharacter));
            });
        }

        [Test]
        public async Task PhilosopherWasherwoman()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Philosopher,Imp,Baron,Saint,Soldier,Fisherman,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Washerwoman);
            const Character washerwomanPing = Character.Fisherman;
            const Character washerwomanWrong = Character.Imp;
            setup.Storyteller.MockGetWasherwomanPing(washerwomanPing, washerwomanWrong, washerwomanPing);
            var receivedWasherwomanPing = setup.Agent(Character.Philosopher).MockNotifyWasherwoman(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(receivedWasherwomanPing.Value.playerA, Is.EqualTo(washerwomanPing).Or.EqualTo(washerwomanWrong));
                Assert.That(receivedWasherwomanPing.Value.playerB, Is.EqualTo(washerwomanPing).Or.EqualTo(washerwomanWrong));
                Assert.That(receivedWasherwomanPing.Value.playerA, Is.Not.EqualTo(receivedWasherwomanPing.Value.playerB));
                Assert.That(receivedWasherwomanPing.Value.seenCharacter, Is.EqualTo(washerwomanPing));
            });
        }

        [Test]
        public async Task CannibalWasherwoman()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Cannibal,Imp,Baron,Saint,Soldier,Fisherman,Washerwoman");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockGetWasherwomanPing(Character.Cannibal, Character.Imp, Character.Cannibal);
            setup.Agent(Character.Imp).MockNomination(Character.Washerwoman);

            await game.RunNightAndDay();

            // Night 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Storyteller.MockGetWasherwomanPing(Character.Soldier, Character.Baron, Character.Soldier);
            var receivedWasherwomanPing = setup.Agent(Character.Cannibal).MockNotifyWasherwoman(gameToEnd: game);

            await game.RunNightAndDay();

            Assert.Multiple(() =>
            {
                Assert.That(receivedWasherwomanPing.Value.playerA, Is.EqualTo(Character.Soldier).Or.EqualTo(Character.Baron));
                Assert.That(receivedWasherwomanPing.Value.playerB, Is.EqualTo(Character.Soldier).Or.EqualTo(Character.Baron));
                Assert.That(receivedWasherwomanPing.Value.playerA, Is.Not.EqualTo(receivedWasherwomanPing.Value.playerB));
                Assert.That(receivedWasherwomanPing.Value.seenCharacter, Is.EqualTo(Character.Soldier));
            });
        }

        [Test]
        public async Task CannibalWasherwoman_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Cannibal,Imp,Baron,Saint,Soldier,Fisherman,Scarlet_Woman");
            setup.Agent(Character.Imp).MockNomination(Character.Scarlet_Woman);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Storyteller.MockCannibalChoice(Character.Washerwoman);
            setup.Storyteller.MockGetWasherwomanPing(Character.Imp, Character.Saint, Character.Mayor);
            var receivedWasherwomanPing = setup.Agent(Character.Cannibal).MockNotifyWasherwoman(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(receivedWasherwomanPing.Value.playerA, Is.EqualTo(Character.Imp).Or.EqualTo(Character.Saint));
                Assert.That(receivedWasherwomanPing.Value.playerB, Is.EqualTo(Character.Imp).Or.EqualTo(Character.Saint));
                Assert.That(receivedWasherwomanPing.Value.playerA, Is.Not.EqualTo(receivedWasherwomanPing.Value.playerB));
                Assert.That(receivedWasherwomanPing.Value.seenCharacter, Is.EqualTo(Character.Mayor));
            });
        }
    }
}
