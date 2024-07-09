using Clocktower.Game;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class ImpTests
    {
        [Test]
        public async Task Imp_LosesWhenExecuted()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fisherman,Ravenkeeper,Saint,Baron,Slayer,Soldier,Mayor");

            setup.Agents[0].MockNomination(Character.Imp);

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
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fisherman,Ravenkeeper,Saint,Baron,Slayer,Soldier,Mayor");

            setup.Agents[0].MockImp(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agents[1].Received().YouAreDead();
        }

        [Test]
        public async Task Imp_StarPass()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fisherman,Ravenkeeper,Saint,Baron,Slayer,Soldier,Mayor");

            setup.Agents[0].MockImp(Character.Imp);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.False);
            await setup.Agents[0].Received().YouAreDead();
            await setup.Agents[4].Received().AssignCharacter(Character.Imp, Alignment.Evil);
        }

        [Test]
        public async Task Imp_StarPassToScarletWoman()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fisherman,Ravenkeeper,Saint,Baron,Scarlet_Woman,Soldier,Mayor");

            setup.Agents[0].MockImp(Character.Imp);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.False);
            await setup.Agents[0].Received().YouAreDead();
            await setup.Agents[5].Received().AssignCharacter(Character.Imp, Alignment.Evil);    // must go to Scarlet Woman, not Baron
        }

        [Test]
        public async Task Imp_StarPassWithMultipleMinions()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fisherman,Ravenkeeper,Saint,Baron,Spy,Soldier,Mayor");

            setup.Agents[0].MockImp(Character.Imp);
            var starPassTargets = setup.Storyteller.MockGetNewImp(Character.Spy);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.False);
            Assert.That(starPassTargets, Is.EquivalentTo(new[] { Character.Baron, Character.Spy }));
            await setup.Agents[0].Received().YouAreDead();
            await setup.Agents[5].Received().AssignCharacter(Character.Imp, Alignment.Evil);    // must go to Scarlet Woman, not Baron
        }
    }
}