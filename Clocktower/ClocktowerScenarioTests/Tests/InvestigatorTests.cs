using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Storyteller;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class InvestigatorTests
    {
        [Test]
        public async Task Investigator_SeesMinionPlusAnyOther()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Investigator,Imp,Baron,Saint,Soldier,Fisherman,Mayor");

            const Character investigatorPing = Character.Baron;
            const Character investigatorWrong = Character.Saint;
            var investigatorPingOptions = setup.Storyteller.MockGetInvestigatorPing(investigatorPing, investigatorWrong, investigatorPing);
            var receivedInvestigatorPing = setup.Agent(Character.Investigator).MockNotifyInvestigator(gameToEnd: game);

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
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Investigator,Imp,Baron,Recluse,Soldier,Fisherman,Mayor");

            const Character investigatorPing = Character.Recluse;
            const Character investigatorWrong = Character.Soldier;
            const Character recluseSeenAs = Character.Poisoner;
            var investigatorPingOptions = setup.Storyteller.MockGetInvestigatorPing(investigatorPing, investigatorWrong, recluseSeenAs);
            var receivedInvestigatorPing = setup.Agent(Character.Investigator).MockNotifyInvestigator(gameToEnd: game);

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
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Investigator,Imp,Spy,Saint,Soldier,Fisherman,Mayor");

            const Character investigatorPing = Character.Spy;
            const Character investigatorWrong = Character.Soldier;
            var investigatorPingOptions = setup.Storyteller.MockGetInvestigatorPing(investigatorPing, investigatorWrong, investigatorPing);
            var receivedInvestigatorPing = setup.Agent(Character.Investigator).MockNotifyInvestigator(gameToEnd: game);

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

        [Test]
        public async Task Investigator_IsTheDrunk()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Investigator,Imp,Baron,Saint,Soldier,Fisherman,Mayor")
                            .WithDrunk(Character.Investigator)
                            .Build();

            const Character investigatorPing = Character.Saint;
            const Character investigatorWrong = Character.Soldier;
            const Character pingCharacter = Character.Poisoner;
            var investigatorPingOptions = setup.Storyteller.MockGetInvestigatorPing(investigatorPing, investigatorWrong, pingCharacter);
            var receivedInvestigatorPing = setup.Agent(Character.Investigator).MockNotifyInvestigator(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(investigatorPingOptions, Does.Contain((investigatorPing, investigatorWrong, pingCharacter)));
                Assert.That(receivedInvestigatorPing.Value.playerA, Is.EqualTo(investigatorPing).Or.EqualTo(investigatorWrong));
                Assert.That(receivedInvestigatorPing.Value.playerB, Is.EqualTo(investigatorPing).Or.EqualTo(investigatorWrong));
                Assert.That(receivedInvestigatorPing.Value.playerA, Is.Not.EqualTo(receivedInvestigatorPing.Value.playerB));
                Assert.That(receivedInvestigatorPing.Value.seenCharacter, Is.EqualTo(pingCharacter));
            });
        }

        [Test]
        public async Task Investigator_IsTheMarionette()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Investigator,Imp,Baron,Saint,Soldier,Fisherman,Mayor")
                            .WithMarionette(Character.Investigator)
                            .Build();

            const Character investigatorPing = Character.Saint;
            const Character investigatorWrong = Character.Soldier;
            const Character pingCharacter = Character.Poisoner;
            var investigatorPingOptions = setup.Storyteller.MockGetInvestigatorPing(investigatorPing, investigatorWrong, pingCharacter);
            var receivedInvestigatorPing = setup.Agent(Character.Investigator).MockNotifyInvestigator(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(investigatorPingOptions, Does.Contain((investigatorPing, investigatorWrong, pingCharacter)));
                Assert.That(receivedInvestigatorPing.Value.playerA, Is.EqualTo(investigatorPing).Or.EqualTo(investigatorWrong));
                Assert.That(receivedInvestigatorPing.Value.playerB, Is.EqualTo(investigatorPing).Or.EqualTo(investigatorWrong));
                Assert.That(receivedInvestigatorPing.Value.playerA, Is.Not.EqualTo(receivedInvestigatorPing.Value.playerB));
                Assert.That(receivedInvestigatorPing.Value.seenCharacter, Is.EqualTo(pingCharacter));
            });
        }

        [Test]
        public async Task Investigator_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Investigator,Imp,Poisoner,Saint,Soldier,Fisherman,Mayor");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Investigator);

            const Character investigatorPing = Character.Saint;
            const Character investigatorWrong = Character.Soldier;
            const Character pingCharacter = Character.Baron;
            var investigatorPingOptions = setup.Storyteller.MockGetInvestigatorPing(investigatorPing, investigatorWrong, pingCharacter);
            var receivedInvestigatorPing = setup.Agent(Character.Investigator).MockNotifyInvestigator(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(investigatorPingOptions, Does.Contain((investigatorPing, investigatorWrong, pingCharacter)));
                Assert.That(receivedInvestigatorPing.Value.playerA, Is.EqualTo(investigatorPing).Or.EqualTo(investigatorWrong));
                Assert.That(receivedInvestigatorPing.Value.playerB, Is.EqualTo(investigatorPing).Or.EqualTo(investigatorWrong));
                Assert.That(receivedInvestigatorPing.Value.playerA, Is.Not.EqualTo(receivedInvestigatorPing.Value.playerB));
                Assert.That(receivedInvestigatorPing.Value.seenCharacter, Is.EqualTo(pingCharacter));
            });
        }

        [Test]
        public async Task Investigator_PhilosopherDrunk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Investigator,Imp,Baron,Saint,Soldier,Fisherman,Philosopher");

            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Investigator);
            setup.Storyteller.GetInvestigatorPings(Arg.Is<Player>(player => player.RealCharacter == Character.Philosopher), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetMatchingOptionFromOptionsArg((Character.Baron, Character.Saint, Character.Baron), new(), argIndex: 1));

            const Character investigatorPing = Character.Saint;
            const Character investigatorWrong = Character.Soldier;
            const Character pingCharacter = Character.Scarlet_Woman;
            setup.Storyteller.GetInvestigatorPings(Arg.Is<Player>(player => player.RealCharacter == Character.Investigator), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetMatchingOptionFromOptionsArg((investigatorPing, investigatorWrong, pingCharacter), new(), argIndex: 1));
            var receivedInvestigatorPing = setup.Agent(Character.Investigator).MockNotifyInvestigator(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(receivedInvestigatorPing.Value.playerA, Is.EqualTo(investigatorPing).Or.EqualTo(investigatorWrong));
                Assert.That(receivedInvestigatorPing.Value.playerB, Is.EqualTo(investigatorPing).Or.EqualTo(investigatorWrong));
                Assert.That(receivedInvestigatorPing.Value.playerA, Is.Not.EqualTo(receivedInvestigatorPing.Value.playerB));
                Assert.That(receivedInvestigatorPing.Value.seenCharacter, Is.EqualTo(pingCharacter));
            });
        }

        [Test]
        public async Task PhilosopherInvestigator()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Philosopher,Imp,Baron,Saint,Soldier,Fisherman,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Investigator);
            const Character investigatorPing = Character.Baron;
            const Character investigatorWrong = Character.Saint;
            var investigatorPingOptions = setup.Storyteller.MockGetInvestigatorPing(investigatorPing, investigatorWrong, investigatorPing);
            var receivedInvestigatorPing = setup.Agent(Character.Philosopher).MockNotifyInvestigator(gameToEnd: game);

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

        [Test]
        public async Task CannibalInvestigator()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Cannibal,Imp,Baron,Saint,Soldier,Investigator,Scarlet_Woman");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockGetInvestigatorPing(Character.Baron, Character.Saint, Character.Baron);
            setup.Agent(Character.Imp).MockNomination(Character.Investigator);

            await game.RunNightAndDay();

            // Night 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Storyteller.MockGetInvestigatorPing(Character.Scarlet_Woman, Character.Saint, Character.Scarlet_Woman);
            var receivedInvestigatorPing = setup.Agent(Character.Cannibal).MockNotifyInvestigator(gameToEnd: game);

            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(receivedInvestigatorPing.Value.playerA, Is.EqualTo(Character.Scarlet_Woman).Or.EqualTo(Character.Saint));
                Assert.That(receivedInvestigatorPing.Value.playerB, Is.EqualTo(Character.Scarlet_Woman).Or.EqualTo(Character.Saint));
                Assert.That(receivedInvestigatorPing.Value.playerA, Is.Not.EqualTo(receivedInvestigatorPing.Value.playerB));
                Assert.That(receivedInvestigatorPing.Value.seenCharacter, Is.EqualTo(Character.Scarlet_Woman));
            });
        }

        [Test]
        public async Task CannibalInvestigator_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Cannibal,Imp,Baron,Saint,Soldier,Fisherman,Mayor");
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Imp).MockNomination(Character.Baron);
            setup.Storyteller.MockCannibalChoice(Character.Investigator);
            setup.Storyteller.MockGetInvestigatorPing(Character.Soldier, Character.Saint, Character.Scarlet_Woman);
            var receivedInvestigatorPing = setup.Agent(Character.Cannibal).MockNotifyInvestigator(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(receivedInvestigatorPing.Value.playerA, Is.EqualTo(Character.Soldier).Or.EqualTo(Character.Saint));
                Assert.That(receivedInvestigatorPing.Value.playerB, Is.EqualTo(Character.Soldier).Or.EqualTo(Character.Saint));
                Assert.That(receivedInvestigatorPing.Value.playerA, Is.Not.EqualTo(receivedInvestigatorPing.Value.playerB));
                Assert.That(receivedInvestigatorPing.Value.seenCharacter, Is.EqualTo(Character.Scarlet_Woman));
            });
        }
    }
}
