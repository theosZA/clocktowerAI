using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class FortuneTellerTests
    {
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

            setup.Storyteller.MockFortuneTellerRedHerring(Character.Fortune_Teller);
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
            setup.Agent(Character.Imp).MockImp(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Fortune_Teller);
            var secondReading = setup.Agent(Character.Fortune_Teller).MockNotifyFortuneTeller(gameToEnd: game);

            await game.RunNightAndDay();

            Assert.That(secondReading.Value, Is.EqualTo(reading));
        }
    }
}