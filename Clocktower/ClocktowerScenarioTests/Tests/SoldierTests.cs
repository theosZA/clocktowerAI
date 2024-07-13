using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class SoldierTests
    {
        [Test]
        public async Task Soldier_SafeFromImp()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            setup.Agent(Character.Imp).MockImp(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Soldier).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Soldier_IsTheDrunk()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Soldier,Ravenkeeper,Saint,Baron,Fisherman,Mayor")
                            .WithDrunk(Character.Soldier)
                            .Build();
            setup.Agent(Character.Imp).MockImp(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Soldier).Received().YouAreDead();
        }
    }
}