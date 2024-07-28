using Clocktower.Game;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class SlayerTests
    {
        [Test]
        public async Task Slayer_ShootsNonDemon()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Ravenkeeper,Saint,Soldier,Slayer,Mayor");
            setup.Agent(Character.Slayer).MockSlayerOption(Character.Baron);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Baron).DidNotReceive().YouAreDead();
            Assert.That(game.Finished, Is.False);
        }

        [Test]
        public async Task Slayer_ShootsDemon()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Ravenkeeper,Saint,Soldier,Slayer,Mayor");
            setup.Agent(Character.Slayer).MockSlayerOption(Character.Imp);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Imp).Received().YouAreDead();
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Good));
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Slayer_ShootsRecluse(bool shouldKill)
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Ravenkeeper,Recluse,Soldier,Slayer,Mayor");
            setup.Agent(Character.Slayer).MockSlayerOption(Character.Recluse);
            setup.Storyteller.MockShouldKillWithSlayer(shouldKill);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            if (shouldKill)
            {
                await setup.Agent(Character.Recluse).Received().YouAreDead();
            }
            else
            {
                await setup.Agent(Character.Recluse).DidNotReceive().YouAreDead();
            }
            Assert.That(game.Finished, Is.False);
        }

        [Test]
        public async Task Slayer_ShootsDemonOnDayTwo()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Ravenkeeper,Saint,Soldier,Slayer,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            await game.RunNightAndDay();

            Assert.That(game.Finished, Is.False);

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Slayer).MockSlayerOption(Character.Imp);

            await game.RunNightAndDay();

            await setup.Agent(Character.Imp).Received().YouAreDead();
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Good));
        }

        [Test]
        public async Task Slayer_ShootsDemonWithScarletWomanBackup()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Scarlet_Woman,Ravenkeeper,Saint,Soldier,Slayer,Mayor");
            var imp = setup.Agent(Character.Imp);
            var scarletWoman = setup.Agent(Character.Scarlet_Woman);
            setup.Agent(Character.Slayer).MockSlayerOption(Character.Imp);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await imp.Received().YouAreDead();
            await scarletWoman.Received().AssignCharacter(Character.Imp, Alignment.Evil);
            Assert.That(game.Finished, Is.False);
        }

        [Test]
        public async Task Slayer_IsTheDrunk()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Baron,Ravenkeeper,Saint,Soldier,Slayer,Mayor")
                            .WithDrunk(Character.Slayer)
                            .Build();

            setup.Agent(Character.Slayer).MockSlayerOption(Character.Imp);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Imp).DidNotReceive().YouAreDead();
            Assert.That(game.Finished, Is.False);
        }

        [Test]
        public async Task Slayer_IsTheMarionette()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Slayer,Baron,Ravenkeeper,Saint,Soldier,Mayor")
                            .WithMarionette(Character.Slayer)
                            .Build();

            setup.Agent(Character.Slayer).MockSlayerOption(Character.Imp);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Imp).DidNotReceive().YouAreDead();
            Assert.That(game.Finished, Is.False);
        }

        [Test]
        public async Task Slayer_PoisonedTargetingImp()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Poisoner,Ravenkeeper,Saint,Soldier,Slayer,Mayor");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Slayer);
            setup.Agent(Character.Slayer).MockSlayerOption(Character.Imp);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Imp).DidNotReceive().YouAreDead();
            Assert.That(game.Finished, Is.False);
        }

        [Test]
        public async Task Slayer_PoisonedTargetingRecluse()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Poisoner,Ravenkeeper,Recluse,Soldier,Slayer,Mayor");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Slayer);
            setup.Agent(Character.Slayer).MockSlayerOption(Character.Recluse);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Storyteller.DidNotReceive().ShouldKillWithSlayer(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>());
            await setup.Agent(Character.Recluse).DidNotReceive().YouAreDead();
            Assert.That(game.Finished, Is.False);
        }

        [Test]
        public async Task Slayer_SweetheartDrunk()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Ravenkeeper,Sweetheart,Soldier,Slayer,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Imp).MockNomination(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Slayer);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Slayer).MockSlayerOption(Character.Imp);

            await game.RunNightAndDay();

            await setup.Agent(Character.Imp).DidNotReceive().YouAreDead();
            Assert.That(game.Finished, Is.False);
        }

        [Test]
        public async Task Slayer_SweetheartDrunkTargetingRecluse()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Recluse,Sweetheart,Soldier,Slayer,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Imp).MockNomination(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Slayer);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Slayer).MockSlayerOption(Character.Recluse);

            await game.RunNightAndDay();

            await setup.Storyteller.DidNotReceive().ShouldKillWithSlayer(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>());
            await setup.Agent(Character.Recluse).DidNotReceive().YouAreDead();
            Assert.That(game.Finished, Is.False);
        }

        [Test]
        public async Task Slayer_PhilosopherDrunkTargetingImp()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Ravenkeeper,Saint,Soldier,Slayer,Philosopher");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Slayer);
            setup.Agent(Character.Slayer).MockSlayerOption(Character.Imp);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Imp).DidNotReceive().YouAreDead();
            Assert.That(game.Finished, Is.False);
        }

        [Test]
        public async Task Slayer_PhilosopherDrunkTargetingRecluse()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Ravenkeeper,Recluse,Soldier,Slayer,Philosopher");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Slayer);
            setup.Agent(Character.Slayer).MockSlayerOption(Character.Recluse);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Storyteller.DidNotReceive().ShouldKillWithSlayer(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>());
            await setup.Agent(Character.Recluse).DidNotReceive().YouAreDead();
            Assert.That(game.Finished, Is.False);
        }

        [Test]
        public async Task PhilosopherSlayer()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Ravenkeeper,Saint,Soldier,Philosopher,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Slayer);
            setup.Agent(Character.Philosopher).MockSlayerOption(Character.Imp);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Imp).Received().YouAreDead();
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Good));
        }
    }
}