using Clocktower.Game;

namespace ClocktowerScenarioTests.Tests
{
    public class DrunkTests
    {
        [Test]
        public async Task Drunk_ShouldNotKnowTheyAreDrunk()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Baron,Ravenkeeper,Soldier,Fisherman,Mayor,Saint")
                            .WithDrunk(Character.Ravenkeeper)
                            .Build();

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Ravenkeeper).Received().AssignCharacter(Character.Ravenkeeper, Alignment.Good);
            await setup.Agent(Character.Ravenkeeper).DidNotReceive().AssignCharacter(Character.Drunk, Arg.Any<Alignment>());
        }

        // Other Drunk test cases will be in the test classes for the characters that are drunked.
    }
}