using Clocktower.Game;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class LibrarianTests
    {
        [Test]
        public async Task Librarian_NoOutsiders()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Librarian,Imp,Baron,Ravenkeeper,Soldier,Fisherman,Mayor");

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Librarian).Received().NotifyLibrarianNoOutsiders();
        }

        [Test]
        public async Task Librarian_SeesOutsiderPlusAnyOther()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Librarian,Imp,Baron,Saint,Soldier,Fisherman,Mayor");

            const Character librarianPing = Character.Saint;
            const Character librarianWrong = Character.Fisherman;
            var librarianPingOptions = setup.Storyteller.MockGetLibrarianPing(librarianPing, librarianWrong, librarianPing);
            var receivedLibrarianPing = setup.Agent(Character.Librarian).MockNotifyLibrarian(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                // Technically the Librarian could be in their own ping, but that's basically never a good idea for a storyteller so is not expected in the pings.
                Assert.That(librarianPingOptions, Is.EquivalentTo(new[] { (Character.Saint, Character.Imp, Character.Saint),
                                                                          (Character.Saint, Character.Baron, Character.Saint),
                                                                          (Character.Saint, Character.Soldier, Character.Saint),
                                                                          (Character.Saint, Character.Fisherman, Character.Saint),
                                                                          (Character.Saint, Character.Mayor, Character.Saint) }));
                Assert.That(receivedLibrarianPing.Value.playerA, Is.EqualTo(librarianPing).Or.EqualTo(librarianWrong));
                Assert.That(receivedLibrarianPing.Value.playerB, Is.EqualTo(librarianPing).Or.EqualTo(librarianWrong));
                Assert.That(receivedLibrarianPing.Value.playerA, Is.Not.EqualTo(receivedLibrarianPing.Value.playerB));
                Assert.That(receivedLibrarianPing.Value.seenCharacter, Is.EqualTo(librarianPing));
            });
        }

        [Test]
        public async Task Librarian_SeesRecluse()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Librarian,Imp,Baron,Recluse,Soldier,Fisherman,Mayor");

            const Character librarianPing = Character.Recluse;
            const Character librarianWrong = Character.Fisherman;
            var librarianPingOptions = setup.Storyteller.MockGetLibrarianPing(librarianPing, librarianWrong, librarianPing);
            var receivedLibrarianPing = setup.Agent(Character.Librarian).MockNotifyLibrarian(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(librarianPingOptions, Does.Contain((librarianPing, librarianWrong, librarianPing)));
                Assert.That(receivedLibrarianPing.Value.playerA, Is.EqualTo(librarianPing).Or.EqualTo(librarianWrong));
                Assert.That(receivedLibrarianPing.Value.playerB, Is.EqualTo(librarianPing).Or.EqualTo(librarianWrong));
                Assert.That(receivedLibrarianPing.Value.playerA, Is.Not.EqualTo(receivedLibrarianPing.Value.playerB));
                Assert.That(receivedLibrarianPing.Value.seenCharacter, Is.EqualTo(librarianPing));
            });
        }

        [Test]
        public async Task Librarian_SeesDrunk()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Librarian,Imp,Baron,Saint,Soldier,Fisherman,Mayor")
                            .WithDrunk(Character.Soldier)
                            .Build();

            const Character librarianPing = Character.Soldier;
            const Character librarianWrong = Character.Fisherman;
            var librarianPingOptions = setup.Storyteller.MockGetLibrarianPing(librarianPing, librarianWrong, Character.Drunk);
            var receivedLibrarianPing = setup.Agent(Character.Librarian).MockNotifyLibrarian(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(librarianPingOptions, Does.Contain((librarianPing, librarianWrong, Character.Drunk)));
                Assert.That(receivedLibrarianPing.Value.playerA, Is.EqualTo(librarianPing).Or.EqualTo(librarianWrong));
                Assert.That(receivedLibrarianPing.Value.playerB, Is.EqualTo(librarianPing).Or.EqualTo(librarianWrong));
                Assert.That(receivedLibrarianPing.Value.playerA, Is.Not.EqualTo(receivedLibrarianPing.Value.playerB));
                Assert.That(receivedLibrarianPing.Value.seenCharacter, Is.EqualTo(Character.Drunk));
            });
        }

        [Test]
        public async Task Librarian_SeesSpy()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Librarian,Imp,Spy,Saint,Soldier,Fisherman,Mayor");

            const Character librarianPing = Character.Spy;
            const Character librarianWrong = Character.Fisherman;
            const Character spySeenAs = Character.Drunk;
            var librarianPingOptions = setup.Storyteller.MockGetLibrarianPing(librarianPing, librarianWrong, spySeenAs);
            var receivedLibrarianPing = setup.Agent(Character.Librarian).MockNotifyLibrarian(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(librarianPingOptions, Does.Contain((librarianPing, librarianWrong, spySeenAs)));
                Assert.That(receivedLibrarianPing.Value.playerA, Is.EqualTo(librarianPing).Or.EqualTo(librarianWrong));
                Assert.That(receivedLibrarianPing.Value.playerB, Is.EqualTo(librarianPing).Or.EqualTo(librarianWrong));
                Assert.That(receivedLibrarianPing.Value.playerA, Is.Not.EqualTo(receivedLibrarianPing.Value.playerB));
                Assert.That(receivedLibrarianPing.Value.seenCharacter, Is.EqualTo(spySeenAs));
            });
        }

        [Test]
        public async Task Librarian_NoOutsidersSeeingSpy()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Librarian,Imp,Spy,Ravenkeeper,Soldier,Fisherman,Mayor");

            const Character librarianPing = Character.Spy;
            const Character librarianWrong = Character.Fisherman;
            const Character spySeenAs = Character.Drunk;
            var librarianPingOptions = setup.Storyteller.MockGetLibrarianPing(librarianPing, librarianWrong, spySeenAs);
            var receivedLibrarianPing = setup.Agent(Character.Librarian).MockNotifyLibrarian(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(librarianPingOptions, Does.Contain(((Character, Character, Character)?)null));  // must include the 0 outsiders option
                Assert.That(librarianPingOptions, Does.Contain((librarianPing, librarianWrong, spySeenAs)));
                Assert.That(receivedLibrarianPing.Value.playerA, Is.EqualTo(librarianPing).Or.EqualTo(librarianWrong));
                Assert.That(receivedLibrarianPing.Value.playerB, Is.EqualTo(librarianPing).Or.EqualTo(librarianWrong));
                Assert.That(receivedLibrarianPing.Value.playerA, Is.Not.EqualTo(receivedLibrarianPing.Value.playerB));
                Assert.That(receivedLibrarianPing.Value.seenCharacter, Is.EqualTo(spySeenAs));
            });
        }

        [Test]
        public async Task Librarian_NoOutsidersNotSeeingSpy()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Librarian,Imp,Spy,Ravenkeeper,Soldier,Fisherman,Mayor");

            setup.Storyteller.GetLibrarianPings(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetNoOutsidersOptionFromArg(argIndex: 1));

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Librarian).Received().NotifyLibrarianNoOutsiders();
        }

        [Test]
        public async Task Librarian_IsTheDrunk()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Librarian,Imp,Baron,Ravenkeeper,Soldier,Fisherman,Mayor")
                            .WithDrunk(Character.Librarian)
                            .Build();

            const Character librarianPing = Character.Imp;
            const Character librarianWrong = Character.Soldier;
            const Character pingCharacter = Character.Recluse;
            var librarianPingOptions = setup.Storyteller.MockGetLibrarianPing(librarianPing, librarianWrong, pingCharacter);
            var receivedlibrarianPing = setup.Agent(Character.Librarian).MockNotifyLibrarian(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(librarianPingOptions, Does.Contain((librarianPing, librarianWrong, pingCharacter)));
                Assert.That(receivedlibrarianPing.Value.playerA, Is.EqualTo(librarianPing).Or.EqualTo(librarianWrong));
                Assert.That(receivedlibrarianPing.Value.playerB, Is.EqualTo(librarianPing).Or.EqualTo(librarianWrong));
                Assert.That(receivedlibrarianPing.Value.playerA, Is.Not.EqualTo(receivedlibrarianPing.Value.playerB));
                Assert.That(receivedlibrarianPing.Value.seenCharacter, Is.EqualTo(pingCharacter));
            });
        }

        [Test]
        public async Task Librarian_IsTheDrunkAndSeesNoOutsiders()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Librarian,Imp,Baron,Ravenkeeper,Soldier,Fisherman,Saint")
                            .WithDrunk(Character.Librarian)
                            .Build();

            setup.Storyteller.GetLibrarianPings(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetNoOutsidersOptionFromArg(argIndex: 1));

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Librarian).Received().NotifyLibrarianNoOutsiders();
        }

        [Test]
        public async Task Librarian_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Librarian,Imp,Poisoner,Saint,Soldier,Fisherman,Mayor");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Librarian);

            const Character librarianPing = Character.Imp;
            const Character librarianWrong = Character.Soldier;
            const Character pingCharacter = Character.Recluse;
            var librarianPingOptions = setup.Storyteller.MockGetLibrarianPing(librarianPing, librarianWrong, pingCharacter);
            var receivedlibrarianPing = setup.Agent(Character.Librarian).MockNotifyLibrarian(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(librarianPingOptions, Does.Contain((librarianPing, librarianWrong, pingCharacter)));
                Assert.That(receivedlibrarianPing.Value.playerA, Is.EqualTo(librarianPing).Or.EqualTo(librarianWrong));
                Assert.That(receivedlibrarianPing.Value.playerB, Is.EqualTo(librarianPing).Or.EqualTo(librarianWrong));
                Assert.That(receivedlibrarianPing.Value.playerA, Is.Not.EqualTo(receivedlibrarianPing.Value.playerB));
                Assert.That(receivedlibrarianPing.Value.seenCharacter, Is.EqualTo(pingCharacter));
            });
        }

        [Test]
        public async Task Librarian_PoisonedAndSeesNoOutsiders()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Librarian,Imp,Poisoner,Saint,Soldier,Fisherman,Mayor");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Librarian);
            setup.Storyteller.GetLibrarianPings(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetNoOutsidersOptionFromArg(argIndex: 1));

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Librarian).Received().NotifyLibrarianNoOutsiders();
        }

        [Test]
        public async Task Librarian_PhilosopherDrunk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Librarian,Imp,Baron,Saint,Soldier,Fisherman,Philosopher");

            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Librarian);
            setup.Storyteller.GetLibrarianPings(Arg.Is<Player>(player => player.RealCharacter == Character.Philosopher), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetMatchingOptionFromOptionsArg((Character.Saint, Character.Baron, Character.Saint), new List<(Character, Character, Character)?>(), argIndex: 1));

            const Character librarianPing = Character.Imp;
            const Character librarianWrong = Character.Soldier;
            const Character pingCharacter = Character.Recluse;
            setup.Storyteller.GetLibrarianPings(Arg.Is<Player>(player => player.RealCharacter == Character.Librarian), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetMatchingOptionFromOptionsArg((librarianPing, librarianWrong, pingCharacter), new List<(Character, Character, Character)?>(), argIndex: 1));
            var receivedlibrarianPing = setup.Agent(Character.Librarian).MockNotifyLibrarian(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(receivedlibrarianPing.Value.playerA, Is.EqualTo(librarianPing).Or.EqualTo(librarianWrong));
                Assert.That(receivedlibrarianPing.Value.playerB, Is.EqualTo(librarianPing).Or.EqualTo(librarianWrong));
                Assert.That(receivedlibrarianPing.Value.playerA, Is.Not.EqualTo(receivedlibrarianPing.Value.playerB));
                Assert.That(receivedlibrarianPing.Value.seenCharacter, Is.EqualTo(pingCharacter));
            });
        }

        [Test]
        public async Task Librarian_PhilosopherDrunkAndSeesNoOutsiders()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Librarian,Imp,Baron,Saint,Soldier,Fisherman,Philosopher");

            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Librarian);
            setup.Storyteller.GetLibrarianPings(Arg.Is<Player>(player => player.RealCharacter == Character.Philosopher), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetMatchingOptionFromOptionsArg((Character.Saint, Character.Baron, Character.Saint), new List<(Character, Character, Character)?>(), argIndex: 1));

            setup.Storyteller.GetLibrarianPings(Arg.Is<Player>(player => player.RealCharacter == Character.Librarian), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetNoOutsidersOptionFromArg(argIndex: 1));

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Librarian).Received().NotifyLibrarianNoOutsiders();
        }

        [Test]
        public async Task PhilosopherLibrarian()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Philosopher,Imp,Baron,Saint,Soldier,Fisherman,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Librarian);
            const Character librarianPing = Character.Saint;
            const Character librarianWrong = Character.Fisherman;
            var librarianPingOptions = setup.Storyteller.MockGetLibrarianPing(librarianPing, librarianWrong, librarianPing);
            var receivedLibrarianPing = setup.Agent(Character.Philosopher).MockNotifyLibrarian(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(librarianPingOptions, Does.Contain((librarianPing, librarianWrong, librarianPing)));
                Assert.That(receivedLibrarianPing.Value.playerA, Is.EqualTo(librarianPing).Or.EqualTo(librarianWrong));
                Assert.That(receivedLibrarianPing.Value.playerB, Is.EqualTo(librarianPing).Or.EqualTo(librarianWrong));
                Assert.That(receivedLibrarianPing.Value.playerA, Is.Not.EqualTo(receivedLibrarianPing.Value.playerB));
                Assert.That(receivedLibrarianPing.Value.seenCharacter, Is.EqualTo(librarianPing));
            });
        }
    }
}
