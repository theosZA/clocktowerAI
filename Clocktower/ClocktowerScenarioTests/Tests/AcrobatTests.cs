using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class AcrobatTests
    {
        [Test]
        public async Task Acrobat_HealthyNeighbours()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Acrobat,Fisherman,Baron,Mayor");
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Acrobat).DidNotReceive().YouAreDead();
        }

        [TestCase(Character.Ravenkeeper)]
        [TestCase(Character.Fisherman)]
        public async Task Acrobat_NeighbourIsTheDrunk(Character drunk)
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Soldier,Ravenkeeper,Acrobat,Fisherman,Baron,Mayor")
                            .WithDrunk(drunk)
                            .Build();
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Acrobat).Received().YouAreDead();
        }

        [TestCase(Character.Ravenkeeper)]
        [TestCase(Character.Fisherman)]
        public async Task Acrobat_NeighbourIsPoisoned(Character poisonTarget)
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Acrobat,Fisherman,Poisoner,Mayor");
            setup.Agent(Character.Poisoner).MockPoisoner(poisonTarget);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Acrobat).Received().YouAreDead();
        }

        [TestCase(Character.Ravenkeeper)]
        [TestCase(Character.Fisherman)]
        public async Task Acrobat_NeighbourIsSweetheartDrunk(Character drunkTarget)
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Acrobat,Fisherman,Baron,Sweetheart");
            setup.Agent(Character.Imp).MockDemonKill(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(drunkTarget);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Acrobat).Received().YouAreDead();
        }

        [Test]
        public async Task Acrobat_NeighbourIsTheMarionette()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Soldier,Acrobat,Ravenkeeper,Fisherman,Baron,Mayor")
                            .WithMarionette(Character.Soldier)
                            .Build();
            setup.Agent(Character.Imp).MockDemonKill(Character.Baron);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Acrobat).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Acrobat_NeighbourIsPoisonedCannibal()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Cannibal,Acrobat,Fisherman,Baron,Mayor");
            setup.Agent(Character.Imp).MockNomination(Character.Baron);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Storyteller.MockCannibalChoice(Character.Virgin);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Acrobat).Received().YouAreDead();
        }

        [Test]
        public async Task Acrobat_EvilNeighbour_HealthyGoodNeighbours()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Acrobat,Baron,Fisherman,Mayor");
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Acrobat).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Acrobat_EvilNeighbour_PoisonedGoodNeighbour()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Acrobat,Poisoner,Fisherman,Mayor");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Fisherman);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Acrobat).Received().YouAreDead();
        }

        [Test]
        public async Task Acrobat_DeadNeighbour_HealthyLivingNeighbours()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Fisherman,Acrobat,Ravenkeeper,Baron,Mayor");
            setup.Agent(Character.Imp).MockDemonKill(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Acrobat).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Acrobat_DeadNeighbour_PoisonedLivingNeighbour()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Fisherman,Acrobat,Ravenkeeper,Poisoner,Mayor");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Soldier);
            setup.Agent(Character.Imp).MockDemonKill(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Acrobat).Received().YouAreDead();
        }

        [Test]
        public async Task Acrobat_Poisoned()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Soldier,Ravenkeeper,Acrobat,Fisherman,Poisoner,Mayor")
                            .WithDrunk(Character.Fisherman)
                            .Build();
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Acrobat);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Acrobat).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Acrobat_SweetheartDrunk()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Soldier,Ravenkeeper,Acrobat,Fisherman,Baron,Sweetheart")
                            .WithDrunk(Character.Fisherman)
                            .Build();
            setup.Agent(Character.Imp).MockDemonKill(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Acrobat);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Acrobat).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Acrobat_IsTheMarionette()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Soldier,Ravenkeeper,Mayor,Baron,Fisherman,Acrobat")
                            .WithDrunk(Character.Fisherman)
                            .WithMarionette(Character.Acrobat)
                            .Build();
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Acrobat).DidNotReceive().YouAreDead();
        }
    }
}