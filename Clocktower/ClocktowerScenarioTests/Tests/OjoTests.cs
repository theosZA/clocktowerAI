using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;
using Clocktower.Options;

namespace ClocktowerScenarioTests.Tests
{
    public class OjoTests
    {
        [Test]
        public async Task Ojo_LosesWhenExecuted()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Ojo,Fisherman,Ravenkeeper,Saint,Baron,Soldier,Mayor");

            setup.Agent(Character.Ojo).MockNomination(Character.Ojo);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Good));
        }

        [Test]
        public async Task Ojo_TargetIsInPlay()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Ojo,Fisherman,Ravenkeeper,Saint,Baron,Soldier,Mayor");

            setup.Agent(Character.Ojo).MockOjo(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).Received().YouAreDead();
        }

        [Test]
        public async Task Ojo_TargetIsNotInPlay()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Ojo,Fisherman,Ravenkeeper,Saint,Baron,Soldier,Mayor");

            setup.Agent(Character.Ojo).MockOjo(Character.Butler);
            setup.Storyteller.MockGetOjoVictims(Character.Butler, new[] { Character.Fisherman });

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).Received().YouAreDead();
        }

        [Test]
        public async Task Ojo_NoKillOnMiss()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Ojo,Fisherman,Ravenkeeper,Saint,Baron,Soldier,Mayor");

            setup.Agent(Character.Ojo).MockOjo(Character.Butler);
            setup.Storyteller.MockGetOjoVictims(Character.Butler, Array.Empty<Character>());

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            foreach (var agent in setup.Agents)
            {
                await agent.DidNotReceive().YouAreDead();
            }
        }

        [Test]
        public async Task Ojo_MultiKillOnMiss()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Ojo,Fisherman,Ravenkeeper,Saint,Baron,Soldier,Mayor");

            setup.Agent(Character.Ojo).MockOjo(Character.Butler);
            setup.Storyteller.MockGetOjoVictims(Character.Butler, new[] { Character.Fisherman, Character.Baron });

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Ojo).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Fisherman).Received().YouAreDead();
            await setup.Agent(Character.Ravenkeeper).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Saint).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Baron).Received().YouAreDead();
            await setup.Agent(Character.Soldier).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Mayor).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Ojo_KillEveryoneOnMiss()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Ojo,Fisherman,Slayer,Saint,Baron,Virgin,Tinker");

            setup.Agent(Character.Ojo).MockOjo(Character.Butler);
            setup.Storyteller.MockGetOjoVictims(Character.Butler, new[] { Character.Ojo, Character.Fisherman, Character.Slayer, Character.Saint, Character.Baron, Character.Virgin, Character.Tinker });

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            foreach (var agent in setup.Agents)
            {
                await agent.Received().YouAreDead();
            }
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Good));   // Good win takes priority over the evil win.
        }

        [TestCase(Character.Spy)]
        [TestCase(Character.Fisherman)]
        public async Task Ojo_Spy(Character victim)
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Ojo,Fisherman,Ravenkeeper,Saint,Spy,Soldier,Mayor");

            setup.Agent(Character.Ojo).MockOjo(Character.Fisherman);
            setup.Storyteller.MockGetOjoVictims(Character.Fisherman, new[] { victim });

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(victim).Received().YouAreDead();
        }

        [TestCase(Character.Recluse)]
        [TestCase(Character.Baron)]
        public async Task Ojo_Recluse(Character victim)
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Ojo,Fisherman,Ravenkeeper,Recluse,Baron,Soldier,Mayor");

            setup.Agent(Character.Ojo).MockOjo(Character.Baron);
            setup.Storyteller.MockGetOjoVictims(Character.Baron, new[] { victim });

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(victim).Received().YouAreDead();
        }

        [Test]
        public async Task Ojo_Soldier()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Ojo,Fisherman,Ravenkeeper,Saint,Baron,Soldier,Mayor");

            setup.Agent(Character.Ojo).MockOjo(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Soldier).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Ojo_TargetSelf()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Ojo,Fisherman,Ravenkeeper,Saint,Baron,Soldier,Mayor");

            setup.Agent(Character.Ojo).MockOjo(Character.Ojo);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Ojo).Received().YouAreDead();
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Good));
        }

        [Test]
        public async Task Ojo_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Ojo,Fisherman,Ravenkeeper,Saint,Poisoner,Soldier,Mayor");

            setup.Agent(Character.Poisoner).MockPoisoner(Character.Ojo);
            setup.Agent(Character.Ojo).MockOjo(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Ojo_SweetheartDrunk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Ojo,Fisherman,Ravenkeeper,Sweetheart,Baron,Soldier,Mayor");
            setup.Agent(Character.Ojo).MockNomination(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Ojo);
            setup.Agent(Character.Ojo).MockOjo(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).DidNotReceive().YouAreDead();
        }
    }
}