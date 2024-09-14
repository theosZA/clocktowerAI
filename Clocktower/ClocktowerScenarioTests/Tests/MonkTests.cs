using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class MonkTests
    {
        [Test]
        public async Task Monk_ProtectFromImp()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Monk,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            var monkOptions = setup.Agent(Character.Monk).MockMonkChoice(Character.Saint);
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(monkOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Ravenkeeper, Character.Saint, Character.Baron, Character.Fisherman, Character.Mayor }));  // not Monk
            await setup.Agent(Character.Saint).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Monk_CanNotProtectFromAssassin()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Monk,Ravenkeeper,Saint,Assassin,Fisherman,Mayor");
            var monkOptions = setup.Agent(Character.Monk).MockMonkChoice(Character.Saint);
            setup.Agent(Character.Imp).MockDemonKill(Character.Fisherman);
            setup.Agent(Character.Assassin).MockAssassin(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(monkOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Ravenkeeper, Character.Saint, Character.Assassin, Character.Fisherman, Character.Mayor }));  // not Monk
            await setup.Agent(Character.Fisherman).Received().YouAreDead();
            await setup.Agent(Character.Saint).Received().YouAreDead();
        }

        [Test]
        public async Task Monk_ProtectEachNight()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Monk,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Monk).MockMonkChoice(Character.Saint);
            setup.Agent(Character.Imp).MockDemonKill(Character.Fisherman);

            await game.RunNightAndDay();

            await setup.Agent(Character.Fisherman).Received().YouAreDead();
            setup.Agent(Character.Monk).ClearReceivedCalls();
            setup.Agent(Character.Imp).ClearReceivedCalls();

            // Night 3 & Day 3
            var monkOptions = setup.Agent(Character.Monk).MockMonkChoice(Character.Saint);
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);

            await game.RunNightAndDay();

            Assert.That(monkOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Ravenkeeper, Character.Saint, Character.Baron, Character.Fisherman, Character.Mayor }));  // not Monk
            await setup.Agent(Character.Saint).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Monk_IsTheDrunk()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Monk,Ravenkeeper,Saint,Baron,Fisherman,Mayor")
                            .WithDrunk(Character.Monk)
                            .Build();

            setup.Agent(Character.Monk).MockMonkChoice(Character.Saint);
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Saint).Received().YouAreDead();
        }

        [Test]
        public async Task Monk_IsTheMarionette()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Monk,Ravenkeeper,Saint,Baron,Fisherman,Mayor")
                            .WithMarionette(Character.Monk)
                            .Build();

            setup.Agent(Character.Monk).MockMonkChoice(Character.Saint);
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Saint).Received().YouAreDead();
        }

        [Test]
        public async Task Monk_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Monk,Ravenkeeper,Saint,Poisoner,Fisherman,Mayor");

            setup.Agent(Character.Poisoner).MockPoisoner(Character.Monk);
            setup.Agent(Character.Monk).MockMonkChoice(Character.Saint);
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Saint).Received().YouAreDead();
        }

        [Test]
        public async Task Monk_SweetheartDrunk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Monk,Ravenkeeper,Sweetheart,Baron,Fisherman,Mayor");
            setup.Agent(Character.Imp).MockNomination(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Monk);
            setup.Agent(Character.Monk).MockMonkChoice(Character.Fisherman);
            setup.Agent(Character.Imp).MockDemonKill(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).Received().YouAreDead();
        }

        [Test]
        public async Task Monk_PhilosopherDrunk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Monk,Ravenkeeper,Saint,Baron,Fisherman,Philosopher");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Monk);
            setup.Agent(Character.Philosopher).MockMonkChoice(Character.Imp);
            setup.Agent(Character.Monk).MockMonkChoice(Character.Saint);
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Saint).Received().YouAreDead();
        }

        [Test]
        public async Task PhilosopherMonk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Philosopher,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Monk);
            var monkOptions = setup.Agent(Character.Philosopher).MockMonkChoice(Character.Saint);
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(monkOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Ravenkeeper, Character.Saint, Character.Baron, Character.Fisherman, Character.Mayor }));  // not Philosopher or Monk
            await setup.Agent(Character.Saint).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task CannibalMonk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Cannibal,Ravenkeeper,Saint,Baron,Fisherman,Monk");
            setup.Agent(Character.Imp).MockNomination(Character.Monk);
            var monkOptions = setup.Agent(Character.Cannibal).MockMonkChoice(Character.Saint);
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(monkOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Ravenkeeper, Character.Saint, Character.Baron, Character.Fisherman, Character.Monk }));  // not Cannibal
            await setup.Agent(Character.Saint).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task CannibalMonk_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Cannibal,Ravenkeeper,Saint,Baron,Fisherman,Scarlet_Woman");
            setup.Agent(Character.Imp).MockNomination(Character.Scarlet_Woman);
            setup.Storyteller.MockCannibalChoice(Character.Monk);
            setup.Agent(Character.Cannibal).MockMonkChoice(Character.Saint);
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Saint).Received().YouAreDead();
        }

        [Test]
        public async Task Monk_ProtectFromNoDashiiPoisonDuringNight()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("No_Dashii,Empath,Ravenkeeper,Saint,Baron,Monk,Mayor");
            setup.Storyteller.MockGetEmpathNumber(2);
            var empathNumber = setup.Agent(Character.Empath).MockNotifyEmpath();
            setup.Agent(Character.Monk).MockMonkChoice(Character.Empath);
            setup.Agent(Character.No_Dashii).MockDemonKill(Character.Empath);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(empathNumber.Value, Is.EqualTo(1)); // unpoisoned
        }

        [Test]
        public async Task Monk_CanNotProtectFromNoDashiiPoisonDuringDay()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("No_Dashii,Slayer,Ravenkeeper,Saint,Baron,Monk,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Monk).MockMonkChoice(Character.Slayer);
            setup.Agent(Character.No_Dashii).MockDemonKill(Character.Saint);
            setup.Agent(Character.Slayer).MockSlayerOption(Character.No_Dashii);

            await game.RunNightAndDay();

            Assert.That(game.Finished, Is.False);   // Slayer is poisoned
        }
    }
}