using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class InvestigatorTests
    {
        [Test]
        public async Task Investigator_SeesMinionPlusAnyOther()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Investigator,Imp,Baron,Saint,Soldier,Fisherman,Mayor,Slayer");

            const Character investigatorPing = Character.Baron;
            const Character investigatorWrong = Character.Saint;
            var investigatorPingOptions = setup.Storyteller.MockGetInvestigatorPing(investigatorPing, investigatorWrong, investigatorPing);
            var receivedInvestigatorPing = setup.Agents[0].MockNotifyInvestigator(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                // Technically the Investigator could be in their own ping, but that's basically never a good idea for a storyteller so is not expected in the pings.
                Assert.That(investigatorPingOptions, Is.EquivalentTo(new[] { (Character.Baron, Character.Saint, Character.Baron),
                                                                             (Character.Baron, Character.Soldier, Character.Baron),
                                                                             (Character.Baron, Character.Fisherman, Character.Baron),
                                                                             (Character.Baron, Character.Mayor, Character.Baron),
                                                                             (Character.Baron, Character.Slayer, Character.Baron),
                                                                             (Character.Baron, Character.Imp, Character.Baron) }));
                Assert.That(receivedInvestigatorPing.Value.playerA, Is.EqualTo(investigatorPing).Or.EqualTo(investigatorWrong));
                Assert.That(receivedInvestigatorPing.Value.playerB, Is.EqualTo(investigatorPing).Or.EqualTo(investigatorWrong));
                Assert.That(receivedInvestigatorPing.Value.playerA, Is.Not.EqualTo(receivedInvestigatorPing.Value.playerB));
                Assert.That(receivedInvestigatorPing.Value.seenCharacter, Is.EqualTo(investigatorPing));
            });
        }

        [Test]
        public async Task Investigator_SeesRecluse()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Investigator,Imp,Baron,Recluse,Soldier,Fisherman,Mayor,Slayer");

            const Character investigatorPing = Character.Recluse;
            const Character investigatorWrong = Character.Soldier;
            const Character recluseSeenAs = Character.Poisoner;
            var investigatorPingOptions = setup.Storyteller.MockGetInvestigatorPing(investigatorPing, investigatorWrong, recluseSeenAs);
            var receivedInvestigatorPing = setup.Agents[0].MockNotifyInvestigator(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(investigatorPingOptions, Does.Contain((investigatorPing, investigatorWrong, recluseSeenAs)));
                Assert.That(receivedInvestigatorPing.Value.playerA, Is.EqualTo(investigatorPing).Or.EqualTo(investigatorWrong));
                Assert.That(receivedInvestigatorPing.Value.playerB, Is.EqualTo(investigatorPing).Or.EqualTo(investigatorWrong));
                Assert.That(receivedInvestigatorPing.Value.playerA, Is.Not.EqualTo(receivedInvestigatorPing.Value.playerB));
                Assert.That(receivedInvestigatorPing.Value.seenCharacter, Is.EqualTo(recluseSeenAs));
            });
        }

        [Test]
        public async Task Investigator_SeesSpy()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Investigator,Imp,Spy,Saint,Soldier,Fisherman,Mayor,Slayer");

            const Character investigatorPing = Character.Spy;
            const Character investigatorWrong = Character.Soldier;
            var investigatorPingOptions = setup.Storyteller.MockGetInvestigatorPing(investigatorPing, investigatorWrong, investigatorPing);
            var receivedInvestigatorPing = setup.Agents[0].MockNotifyInvestigator(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(investigatorPingOptions, Does.Contain((investigatorPing, investigatorWrong, investigatorPing)));
                Assert.That(receivedInvestigatorPing.Value.playerA, Is.EqualTo(investigatorPing).Or.EqualTo(investigatorWrong));
                Assert.That(receivedInvestigatorPing.Value.playerB, Is.EqualTo(investigatorPing).Or.EqualTo(investigatorWrong));
                Assert.That(receivedInvestigatorPing.Value.playerA, Is.Not.EqualTo(receivedInvestigatorPing.Value.playerB));
                Assert.That(receivedInvestigatorPing.Value.seenCharacter, Is.EqualTo(investigatorPing));
            });
        }
    }
}
