using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Storyteller;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class FortuneTellerTests
    {
        [Test]
        public async Task FortuneTeller_ValidRedHerringOptions()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fortune_Teller,Ravenkeeper,Recluse,Spy,Fisherman,Baron");
            var redHerringOptions = new List<Character>();
            setup.Storyteller.GetFortuneTellerRedHerring(Arg.Is<Player>(player => player.Character == Character.Fortune_Teller), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetMatchingOptionFromOptionsArg(Character.Fortune_Teller, redHerringOptions, argIndex: 1));
            setup.Agent(Character.Fortune_Teller).MockFortuneTellerChoice(Character.Fortune_Teller, Character.Imp);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            // Options should include Recluse (a poor choice, but allowed), Spy (since they can register as good), but not any others on the evil team.
            Assert.That(redHerringOptions, Is.EquivalentTo(new[] { Character.Fortune_Teller, Character.Ravenkeeper, Character.Recluse, Character.Spy, Character.Fisherman }));
        }

        [Test]
        public async Task FortuneTeller_No()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fortune_Teller,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            setup.Storyteller.MockFortuneTellerRedHerring(Character.Mayor);
            setup.Agent(Character.Fortune_Teller).MockFortuneTellerChoice(Character.Baron, Character.Fortune_Teller);
            var fortuneTellerReading = setup.Agent(Character.Fortune_Teller).MockNotifyFortuneTeller(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(fortuneTellerReading.Value, Is.False);
        }

        [Test]
        public async Task FortuneTeller_ChoosesImp()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fortune_Teller,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            setup.Storyteller.MockFortuneTellerRedHerring(Character.Mayor);
            setup.Agent(Character.Fortune_Teller).MockFortuneTellerChoice(Character.Imp, Character.Fortune_Teller);
            var fortuneTellerReading = setup.Agent(Character.Fortune_Teller).MockNotifyFortuneTeller(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(fortuneTellerReading.Value, Is.True);
        }

        [Test]
        public async Task FortuneTeller_ChoosesRedHerring()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fortune_Teller,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            setup.Storyteller.MockFortuneTellerRedHerring(Character.Mayor);
            setup.Agent(Character.Fortune_Teller).MockFortuneTellerChoice(Character.Fisherman, Character.Mayor);
            var fortuneTellerReading = setup.Agent(Character.Fortune_Teller).MockNotifyFortuneTeller(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(fortuneTellerReading.Value, Is.True);
        }

        [Test]
        public async Task FortuneTeller_ChoosesSelfRedHerring()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fortune_Teller,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            setup.Storyteller.MockFortuneTellerRedHerring(Character.Fortune_Teller);
            setup.Agent(Character.Fortune_Teller).MockFortuneTellerChoice(Character.Fortune_Teller, Character.Mayor);
            var fortuneTellerReading = setup.Agent(Character.Fortune_Teller).MockNotifyFortuneTeller(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(fortuneTellerReading.Value, Is.True);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task FortuneTeller_ChoosesRecluse(bool reading)
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fortune_Teller,Ravenkeeper,Recluse,Baron,Fisherman,Mayor");
            setup.Storyteller.MockFortuneTellerRedHerring(Character.Fortune_Teller);
            setup.Agent(Character.Fortune_Teller).MockFortuneTellerChoice(Character.Recluse, Character.Mayor);
            setup.Storyteller.MockFortuneTellerReading(reading: reading);
            var fortuneTellerReading = setup.Agent(Character.Fortune_Teller).MockNotifyFortuneTeller(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(fortuneTellerReading.Value, Is.EqualTo(reading));
        }

        [Test]
        public async Task FortuneTeller_ChoosesPoisonedRecluse()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fortune_Teller,Ravenkeeper,Recluse,Poisoner,Fisherman,Mayor");
            setup.Storyteller.MockFortuneTellerRedHerring(Character.Fortune_Teller);
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Recluse);
            setup.Agent(Character.Fortune_Teller).MockFortuneTellerChoice(Character.Recluse, Character.Mayor);
            var fortuneTellerReading = setup.Agent(Character.Fortune_Teller).MockNotifyFortuneTeller(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(fortuneTellerReading.Value, Is.False);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task FortuneTeller_IsTheDrunk(bool reading)
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Fortune_Teller,Ravenkeeper,Saint,Baron,Fisherman,Mayor")
                            .WithDrunk(Character.Fortune_Teller)
                            .Build();

            setup.Storyteller.MockFortuneTellerRedHerring(Character.Fortune_Teller);    // Should not be called for the Drunk.
            setup.Agent(Character.Fortune_Teller).MockFortuneTellerChoice(Character.Imp, Character.Mayor);
            setup.Storyteller.MockFortuneTellerReading(reading: reading);
            var fortuneTellerReading = setup.Agent(Character.Fortune_Teller).MockNotifyFortuneTeller(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(fortuneTellerReading.Value, Is.EqualTo(reading));
            await setup.Storyteller.DidNotReceiveWithAnyArgs().GetFortuneTellerRedHerring(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task FortuneTeller_IsTheMarionette(bool reading)
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Fortune_Teller,Ravenkeeper,Saint,Baron,Fisherman,Mayor")
                            .WithMarionette(Character.Fortune_Teller)
                            .Build();

            setup.Storyteller.MockFortuneTellerRedHerring(Character.Saint);
            setup.Agent(Character.Fortune_Teller).MockFortuneTellerChoice(Character.Imp, Character.Mayor);
            setup.Storyteller.MockFortuneTellerReading(reading: reading);
            var fortuneTellerReading = setup.Agent(Character.Fortune_Teller).MockNotifyFortuneTeller(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(fortuneTellerReading.Value, Is.EqualTo(reading));
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task FortuneTeller_Poisoned(bool reading)
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fortune_Teller,Ravenkeeper,Saint,Poisoner,Fisherman,Mayor");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Fortune_Teller);
            setup.Storyteller.MockFortuneTellerRedHerring(Character.Mayor);
            setup.Agent(Character.Fortune_Teller).MockFortuneTellerChoice(Character.Imp, Character.Fortune_Teller);
            setup.Storyteller.MockFortuneTellerReading(reading: reading);
            var fortuneTellerReading = setup.Agent(Character.Fortune_Teller).MockNotifyFortuneTeller(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(fortuneTellerReading.Value, Is.EqualTo(reading));
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task FortuneTeller_SweetheartDrunk(bool reading)
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fortune_Teller,Ravenkeeper,Sweetheart,Baron,Fisherman,Mayor");
            setup.Storyteller.MockFortuneTellerRedHerring(Character.Mayor);
            setup.Storyteller.MockFortuneTellerReading(reading);
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Fortune_Teller).MockFortuneTellerChoice(Character.Imp, Character.Fortune_Teller);
            var firstReading = setup.Agent(Character.Fortune_Teller).MockNotifyFortuneTeller();

            await game.RunNightAndDay();

            Assert.That(firstReading.Value, Is.True);

            // Night 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Fortune_Teller);
            var secondReading = setup.Agent(Character.Fortune_Teller).MockNotifyFortuneTeller(gameToEnd: game);

            await game.RunNightAndDay();

            Assert.That(secondReading.Value, Is.EqualTo(reading));
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task FortuneTeller_PhilosopherDrunk(bool reading)
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fortune_Teller,Ravenkeeper,Saint,Baron,Philosopher,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Fortune_Teller);
            setup.Storyteller.GetFortuneTellerRedHerring(Arg.Is<Player>(player => player.RealCharacter == Character.Fortune_Teller), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetOptionForCharacterFromArg(Character.Saint, argIndex: 1));
            setup.Storyteller.GetFortuneTellerRedHerring(Arg.Is<Player>(player => player.RealCharacter == Character.Philosopher), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetOptionForCharacterFromArg(Character.Mayor, argIndex: 1));
            setup.Agent(Character.Fortune_Teller).MockFortuneTellerChoice(Character.Imp, Character.Fortune_Teller);
            setup.Agent(Character.Philosopher).MockFortuneTellerChoice(Character.Mayor, Character.Fortune_Teller);  // hits the Philosopher's red herring
            setup.Storyteller.MockFortuneTellerReading(reading: reading);
            var fortuneTellerReading = setup.Agent(Character.Fortune_Teller).MockNotifyFortuneTeller(gameToEnd: game);
            var philosopherFortuneTellerReading = setup.Agent(Character.Philosopher).MockNotifyFortuneTeller(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(fortuneTellerReading.Value, Is.EqualTo(reading));
            Assert.That(philosopherFortuneTellerReading.Value, Is.True);    // hit their red herring
        }

        [Test]
        public async Task PhilosopherFortuneTeller_ValidRedHerringOptions()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Philosopher,Ravenkeeper,Recluse,Spy,Fisherman,Baron");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Fortune_Teller);
            var redHerringOptions = new List<Character>();
            setup.Storyteller.GetFortuneTellerRedHerring(Arg.Is<Player>(player => player.Character == Character.Fortune_Teller), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetMatchingOptionFromOptionsArg(Character.Fortune_Teller, redHerringOptions, argIndex: 1));
            setup.Agent(Character.Philosopher).MockFortuneTellerChoice(Character.Fortune_Teller, Character.Imp);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            // Options should include Recluse (a poor choice, but allowed), Spy (since they can register as good), but not any others on the evil team.
            Assert.That(redHerringOptions, Is.EquivalentTo(new[] { Character.Fortune_Teller, Character.Ravenkeeper, Character.Recluse, Character.Spy, Character.Fisherman }));
        }

        [Test]
        public async Task PhilosopherFortuneTeller_ChoosesImp()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Philosopher,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Fortune_Teller);
            setup.Storyteller.MockFortuneTellerRedHerring(Character.Mayor);
            setup.Agent(Character.Philosopher).MockFortuneTellerChoice(Character.Imp, Character.Fortune_Teller);
            var fortuneTellerReading = setup.Agent(Character.Philosopher).MockNotifyFortuneTeller(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(fortuneTellerReading.Value, Is.True);
        }

        [Test]
        public async Task PhilosopherFortuneTeller_ChoosesRedHerring()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Philosopher,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Fortune_Teller);
            setup.Storyteller.MockFortuneTellerRedHerring(Character.Mayor);
            setup.Agent(Character.Philosopher).MockFortuneTellerChoice(Character.Fisherman, Character.Mayor);
            var fortuneTellerReading = setup.Agent(Character.Philosopher).MockNotifyFortuneTeller(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(fortuneTellerReading.Value, Is.True);
        }

        [Test]
        public async Task CannibalFortuneTeller_ValidRedHerringOptions()
        {
            // Arrange 
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Cannibal,Soldier,Recluse,Spy,Fortune_Teller,Baron");

            var redHerringOptions = new List<Character>();
            setup.Storyteller.GetFortuneTellerRedHerring(Arg.Is<Player>(player => player.Character == Character.Cannibal), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetMatchingOptionFromOptionsArg(Character.Fortune_Teller, redHerringOptions, argIndex: 1));
            setup.Storyteller.GetFortuneTellerRedHerring(Arg.Is<Player>(player => player.Character == Character.Fortune_Teller), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetMatchingOptionFromOptionsArg(Character.Fortune_Teller, argIndex: 1));

            setup.Agent(Character.Fortune_Teller).MockFortuneTellerChoice(Character.Imp, Character.Cannibal);
            setup.Agent(Character.Imp).MockNomination(Character.Fortune_Teller);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            // Options should include Recluse (a poor choice, but allowed), Spy (since they can register as good), but not any others on the evil team.
            Assert.That(redHerringOptions, Is.EquivalentTo(new[] { Character.Cannibal, Character.Soldier, Character.Recluse, Character.Spy, Character.Fortune_Teller }));
        }

        [Test]
        public async Task CannibalFortuneTeller_ChoosesImp()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Cannibal,Soldier,Recluse,Spy,Fortune_Teller,Baron");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.GetFortuneTellerRedHerring(Arg.Is<Player>(player => player.Character == Character.Fortune_Teller), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetMatchingOptionFromOptionsArg(Character.Soldier, argIndex: 1));
            setup.Agent(Character.Fortune_Teller).MockFortuneTellerChoice(Character.Imp, Character.Cannibal);
            setup.Agent(Character.Imp).MockNomination(Character.Fortune_Teller);
            setup.Storyteller.GetFortuneTellerRedHerring(Arg.Is<Player>(player => player.Character == Character.Cannibal), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetMatchingOptionFromOptionsArg(Character.Fortune_Teller, argIndex: 1));

            await game.RunNightAndDay();

            // Night 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Cannibal).MockFortuneTellerChoice(Character.Baron, Character.Imp);
            var fortuneTellerReading = setup.Agent(Character.Cannibal).MockNotifyFortuneTeller(gameToEnd: game);

            await game.RunNightAndDay();

            Assert.That(fortuneTellerReading.Value, Is.True);
        }

        [Test]
        public async Task CannibalFortuneTeller_ChoosesRedHerring()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Cannibal,Soldier,Recluse,Spy,Fortune_Teller,Baron");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.GetFortuneTellerRedHerring(Arg.Is<Player>(player => player.Character == Character.Fortune_Teller), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetMatchingOptionFromOptionsArg(Character.Soldier, argIndex: 1));
            setup.Agent(Character.Fortune_Teller).MockFortuneTellerChoice(Character.Imp, Character.Cannibal);
            setup.Agent(Character.Imp).MockNomination(Character.Fortune_Teller);
            setup.Storyteller.GetFortuneTellerRedHerring(Arg.Is<Player>(player => player.Character == Character.Cannibal), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetMatchingOptionFromOptionsArg(Character.Fortune_Teller, argIndex: 1));

            await game.RunNightAndDay();

            // Night 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Cannibal).MockFortuneTellerChoice(Character.Baron, Character.Fortune_Teller);
            var fortuneTellerReading = setup.Agent(Character.Cannibal).MockNotifyFortuneTeller(gameToEnd: game);

            await game.RunNightAndDay();

            // Assert
            Assert.That(fortuneTellerReading.Value, Is.True);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task CannibalFortuneTeller_Poisoned(bool reading)
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Cannibal,Soldier,Recluse,Spy,Fisherman,Baron");
            setup.Agent(Character.Imp).MockNomination(Character.Baron);
            setup.Storyteller.MockCannibalChoice(Character.Fortune_Teller);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Cannibal).MockFortuneTellerChoice(Character.Imp, Character.Cannibal);
            setup.Storyteller.MockFortuneTellerReading(reading: reading);
            var fortuneTellerReading = setup.Agent(Character.Cannibal).MockNotifyFortuneTeller(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(fortuneTellerReading.Value, Is.EqualTo(reading));
        }
    }
}