using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class NoDashiiTests
    {
        [Test]
        public async Task NoDashii_LosesWhenExecuted()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("No_Dashii,Fisherman,Ravenkeeper,Saint,Baron,Soldier,Mayor");

            setup.Agent(Character.No_Dashii).MockNomination(Character.No_Dashii);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Good));
        }

        [Test]
        public async Task NoDashii_KillsAtNight()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("No_Dashii,Fisherman,Ravenkeeper,Saint,Baron,Soldier,Mayor");

            setup.Agent(Character.No_Dashii).MockDemonKill(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).Received().YouAreDead();
        }

        [Test]
        public async Task NoDashii_PoisonAdjacentClockwise()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("No_Dashii,Slayer,Ravenkeeper,Saint,Baron,Soldier,Mayor");

            setup.Agent(Character.Slayer).MockSlayerOption(Character.No_Dashii);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.False);
            await setup.Agent(Character.No_Dashii).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task NoDashii_PoisonAdjacentCounterclockwise()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("No_Dashii,Mayor,Ravenkeeper,Saint,Baron,Soldier,Slayer");

            setup.Agent(Character.Slayer).MockSlayerOption(Character.No_Dashii);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.False);
            await setup.Agent(Character.No_Dashii).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task NoDashii_PoisonMultipleStepsAwayClockwise()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("No_Dashii,Baron,Saint,Slayer,Ravenkeeper,Soldier,Mayor");

            setup.Agent(Character.Slayer).MockSlayerOption(Character.No_Dashii);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.False);
            await setup.Agent(Character.No_Dashii).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task NoDashii_PoisonMultipleStepsAwayCounterclockwise()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("No_Dashii,Ravenkeeper,Soldier,Mayor,Slayer,Baron,Saint");

            setup.Agent(Character.Slayer).MockSlayerOption(Character.No_Dashii);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.False);
            await setup.Agent(Character.No_Dashii).DidNotReceive().YouAreDead();
        }


        [Test]
        public async Task NoDashii_NotPoisonedIfInterveningTownsfolk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("No_Dashii,Ravenkeeper,Slayer,Saint,Baron,Soldier,Mayor");

            setup.Agent(Character.Slayer).MockSlayerOption(Character.No_Dashii);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Good));
            await setup.Agent(Character.No_Dashii).Received().YouAreDead();
        }

        [Test]
        public async Task NoDashii_Poisoned_NoKill()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("No_Dashii,Fisherman,Ravenkeeper,Saint,Poisoner,Soldier,Mayor");

            setup.Agent(Character.Poisoner).MockPoisoner(Character.No_Dashii);
            setup.Agent(Character.No_Dashii).MockDemonKill(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task NoDashii_Poisoned_UnpoisonTownsfolkNeighbour()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("No_Dashii,Slayer,Ravenkeeper,Saint,Poisoner,Soldier,Mayor");

            setup.Agent(Character.Poisoner).MockPoisoner(Character.No_Dashii);
            setup.Agent(Character.Slayer).MockSlayerOption(Character.No_Dashii);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Good));
            await setup.Agent(Character.No_Dashii).Received().YouAreDead();
        }

        [Test]
        public async Task NoDashii_SweetheartDrunk_NoKill()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("No_Dashii,Fisherman,Ravenkeeper,Sweetheart,Baron,Soldier,Mayor");
            setup.Agent(Character.No_Dashii).MockNomination(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.No_Dashii);
            setup.Agent(Character.No_Dashii).MockDemonKill(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task NoDashii_SweetheartDrunk_UnpoisonTownsfolkNeighbour()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("No_Dashii,Slayer,Ravenkeeper,Sweetheart,Baron,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.No_Dashii).MockNomination(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.No_Dashii);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.No_Dashii).MockDemonKill(Character.Slayer);
            setup.Agent(Character.Slayer).MockSlayerOption(Character.No_Dashii);

            await game.RunNightAndDay();

            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Good));
            await setup.Agent(Character.No_Dashii).Received().YouAreDead();
        }
    }
}