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
            setup.Agent(Character.Imp).MockImp(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Godfather).DidNotReceive().RequestChoiceFromGodfather(Arg.Any<IReadOnlyCollection<IOption>>());
        }

        [Test]
        public async Task Godfather_TinkerKilledByStorytellerDuringDay()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Godfather,Ravenkeeper,Tinker,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Imp).MockImp(Character.Soldier);
            setup.Agent(Character.Godfather).MockGodfather(Character.Fisherman);
            setup.Storyteller.MockShouldKillTinker(shouldKill: true);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).Received().YouAreDead();
        }

        [Test]
        public async Task Godfather_OutsiderKilledByExecution()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Godfather,Ravenkeeper,Recluse,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Imp).MockNomination(Character.Recluse);
            setup.Agent(Character.Imp).MockImp(Character.Soldier);
            setup.Agent(Character.Godfather).MockGodfather(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).Received().YouAreDead();
        }

        [Test]
        public async Task Godfather_OutsiderKilledAtNight()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Godfather,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Imp).MockImp(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Godfather).DidNotReceive().RequestChoiceFromGodfather(Arg.Any<IReadOnlyCollection<IOption>>());
        }

        [Test]
        public async Task Godfather_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Godfather,Poisoner,Recluse,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Imp).MockNomination(Character.Recluse);
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Godfather);
            setup.Agent(Character.Imp).MockImp(Character.Soldier);
            setup.Agent(Character.Godfather).MockGodfather(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).DidNotReceive().YouAreDead();
        }
    }
}