using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class LibrarianTests
    {
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
    }
}
