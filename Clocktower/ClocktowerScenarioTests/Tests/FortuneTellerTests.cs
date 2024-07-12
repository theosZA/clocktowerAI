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
    }
}