using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class ImpTests
    {
        [Test]
        public async Task Imp_LosesWhenExecuted()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fisherman,Ravenkeeper,Saint,Baron,Soldier,Mayor");

            setup.Agent(Character.Imp).MockNomination(Character.Imp);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Good));
        }

        [Test]
        public async Task Imp_KillsAtNight()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fisherman,Ravenkeeper,Saint,Baron,Soldier,Mayor");

            setup.Agent(Character.Imp).MockImp(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).Received().YouAreDead();
        }

        [Test]
        public async Task Imp_StarPass()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fisherman,Ravenkeeper,Saint,Baron,Soldier,Mayor");

            setup.Agent(Character.Imp).MockImp(Character.Imp);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.False);
            await setup.Agent(Character.Imp).Received().YouAreDead();
            await setup.Agent(Character.Baron).Received().AssignCharacter(Character.Imp, Alignment.Evil);
        }

        [Test]
        public async Task Imp_StarPassToScarletWoman()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fisherman,Ravenkeeper,Saint,Baron,Scarlet_Woman,Soldier");

            setup.Agent(Character.Imp).MockImp(Character.Imp);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.False);
            await setup.Agent(Character.Imp).Received().YouAreDead();
            await setup.Agent(Character.Scarlet_Woman).Received().AssignCharacter(Character.Imp, Alignment.Evil);    // must go to Scarlet Woman, not Baron
        }

        [Test]
        public async Task Imp_StarPassWithMultipleMinions()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fisherman,Ravenkeeper,Saint,Baron,Spy,Soldier");

            setup.Agent(Character.Imp).MockImp(Character.Imp);
            var starPassTargets = setup.Storyteller.MockGetNewImp(Character.Spy);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.False);
            Assert.That(starPassTargets, Is.EquivalentTo(new[] { Character.Baron, Character.Spy }));
            await setup.Agent(Character.Imp).Received().YouAreDead();
            await setup.Agent(Character.Spy).Received().AssignCharacter(Character.Imp, Alignment.Evil);
        }

        [Test]
        public async Task Imp_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fisherman,Ravenkeeper,Saint,Poisoner,Soldier,Mayor");

            setup.Agent(Character.Poisoner).MockPoisoner(Character.Imp);
            setup.Agent(Character.Imp).MockImp(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Imp_SweetheartDrunk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fisherman,Ravenkeeper,Sweetheart,Baron,Soldier,Mayor");
            setup.Agent(Character.Imp).MockNomination(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Imp);
            setup.Agent(Character.Imp).MockImp(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).DidNotReceive().YouAreDead();
        }
    }
}