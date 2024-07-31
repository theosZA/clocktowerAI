using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class PacifistTests
    {
        [Test]
        public async Task Pacifist_CanSaveGood()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Pacifist,Saint,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Imp).MockNomination(Character.Soldier);
            setup.Storyteller.MockShouldSaveWithPacifist(shouldSave: true);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Soldier).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Pacifist_CanNotSaveEvil()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Pacifist,Saint,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Imp).MockNomination(Character.Baron);
            setup.Storyteller.MockShouldSaveWithPacifist(shouldSave: true);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Baron).Received().YouAreDead();
        }

        [Test]
        public async Task Pacifist_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Poisoner,Pacifist,Saint,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Pacifist);
            setup.Agent(Character.Imp).MockNomination(Character.Soldier);
            setup.Storyteller.MockShouldSaveWithPacifist(shouldSave: true);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Soldier).Received().YouAreDead();
        }

        [Test]
        public async Task Pacifist_Dead()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Pacifist,Saint,Fisherman,Soldier,Mayor");
            setup.Storyteller.MockShouldSaveWithPacifist(shouldSave: true);
            await game.StartGame();

            // Night 1 & Day 1
            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Pacifist);
            setup.Agent(Character.Imp).MockNomination(Character.Soldier);

            await game.RunNightAndDay();

            await setup.Agent(Character.Soldier).Received().YouAreDead();
        }

        [Test]
        public async Task PhilosopherPacifist()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Philosopher,Saint,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Pacifist);
            setup.Agent(Character.Imp).MockNomination(Character.Soldier);
            setup.Storyteller.MockShouldSaveWithPacifist(shouldSave: true);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Soldier).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task CannibalPacifist()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Cannibal,Saint,Fisherman,Soldier,Pacifist");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Imp).MockNomination(Character.Pacifist);
            setup.Storyteller.MockShouldSaveWithPacifist(shouldSave: false);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Imp).MockNomination(Character.Soldier);
            setup.Storyteller.MockShouldSaveWithPacifist(shouldSave: true);

            await game.RunNightAndDay();

            await setup.Agent(Character.Soldier).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task CannibalPacifist_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Cannibal,Saint,Fisherman,Soldier,Scarlet_Woman");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Imp).MockNomination(Character.Scarlet_Woman);
            setup.Storyteller.MockCannibalChoice(Character.Pacifist);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Scarlet_Woman);
            setup.Agent(Character.Imp).MockNomination(Character.Soldier);
            setup.Storyteller.MockShouldSaveWithPacifist(shouldSave: true);

            await game.RunNightAndDay();

            await setup.Agent(Character.Soldier).Received().YouAreDead();
        }
    }
}