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
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Librarian,Imp,Baron,Ravenkeeper,Soldier,Fisherman,Mayor,Slayer");

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agents[0].Received().NotifyLibrarianNoOutsiders();
        }

        [Test]
        public async Task Librarian_SeesOutsiderPlusAnyOther()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Librarian,Imp,Baron,Saint,Soldier,Fisherman,Mayor,Slayer");

            const Character librarianPing = Character.Saint;
            const Character librarianWrong = Character.Slayer;
            var librarianPingOptions = setup.Storyteller.MockGetLibrarianPing(librarianPing, librarianWrong, librarianPing);
            var receivedLibrarianPing = setup.Agents[0].MockNotifyLibrarian(gameToEnd: game);

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
                                                                          (Character.Saint, Character.Mayor, Character.Saint),
                                                                          (Character.Saint, Character.Slayer, Character.Saint) }));
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
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Librarian,Imp,Baron,Recluse,Soldier,Fisherman,Mayor,Slayer");

            const Character librarianPing = Character.Recluse;
            const Character librarianWrong = Character.Slayer;
            var librarianPingOptions = setup.Storyteller.MockGetLibrarianPing(librarianPing, librarianWrong, librarianPing);
            var receivedLibrarianPing = setup.Agents[0].MockNotifyLibrarian(gameToEnd: game);

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
        public async Task Librarian_SeesSpy()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Librarian,Imp,Spy,Saint,Soldier,Fisherman,Mayor,Slayer");

            const Character librarianPing = Character.Spy;
            const Character librarianWrong = Character.Slayer;
            const Character spySeenAs = Character.Drunk;
            var librarianPingOptions = setup.Storyteller.MockGetLibrarianPing(librarianPing, librarianWrong, spySeenAs);
            var receivedLibrarianPing = setup.Agents[0].MockNotifyLibrarian(gameToEnd: game);

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
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Librarian,Imp,Spy,Ravenkeeper,Soldier,Fisherman,Mayor,Slayer");

            const Character librarianPing = Character.Spy;
            const Character librarianWrong = Character.Slayer;
            const Character spySeenAs = Character.Drunk;
            var librarianPingOptions = setup.Storyteller.MockGetLibrarianPing(librarianPing, librarianWrong, spySeenAs);
            var receivedLibrarianPing = setup.Agents[0].MockNotifyLibrarian(gameToEnd: game);

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
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Librarian,Imp,Spy,Ravenkeeper,Soldier,Fisherman,Mayor,Slayer");

            setup.Storyteller.GetLibrarianPings(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(new NoOutsiders());

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agents[0].Received().NotifyLibrarianNoOutsiders();
        }
    }
}
