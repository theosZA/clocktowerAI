using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class GodfatherTests
    {
        [Test]
        public async Task Godfather_NoOutsidersKilledDuringDay()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Godfather,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");
            setup.Agents[0].MockImp(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agents[1].DidNotReceive().RequestChoiceFromGodfather(Arg.Any<IReadOnlyCollection<IOption>>());
        }

        [Test]
        public async Task Godfather_TinkerKilledByStorytellerDuringDay()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Godfather,Ravenkeeper,Tinker,Fisherman,Soldier,Mayor");
            setup.Agents[0].MockImp(Character.Soldier);
            setup.Agents[1].MockGodfather(Character.Fisherman);
            setup.Storyteller.MockShouldKillTinker(shouldKill: true);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agents[4].Received().YouAreDead();
        }

        [Test]
        public async Task Godfather_OutsiderKilledByExecution()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Godfather,Ravenkeeper,Recluse,Fisherman,Soldier,Mayor");
            setup.Agents[0].MockNomination(Character.Recluse);
            setup.Agents[0].MockImp(Character.Soldier);
            setup.Agents[1].MockGodfather(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agents[4].Received().YouAreDead();
        }

        [Test]
        public async Task Godfather_OutsiderKilledAtNight()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Godfather,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");
            setup.Agents[0].MockImp(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agents[1].DidNotReceive().RequestChoiceFromGodfather(Arg.Any<IReadOnlyCollection<IOption>>());
        }
    }
}