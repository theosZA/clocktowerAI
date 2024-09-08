using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class DamselTests
    {
        [Test]
        public async Task Damsel_PingToMinion()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Damsel,Fisherman,Baron,Mayor");

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Baron).Received().MinionInformation(Arg.Is<Player>(demon => demon.RealCharacter == Character.Imp),
                                                                            Arg.Is<IReadOnlyCollection<Player>>(fellowMinions => !fellowMinions.Any()),
                                                                            true,
                                                                            Arg.Is<IReadOnlyCollection<Character>>(notInPlayCharacters => !notInPlayCharacters.Any()));
        }

        [Test]
        public async Task PhilosopherDamsel_PingToMinion()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Philosopher,Fisherman,Baron,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Damsel);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Baron).Received().MinionInformation(Arg.Is<Player>(demon => demon.RealCharacter == Character.Imp),
                                                                            Arg.Is<IReadOnlyCollection<Player>>(fellowMinions => !fellowMinions.Any()),
                                                                            true,
                                                                            Arg.Is<IReadOnlyCollection<Character>>(notInPlayCharacters => !notInPlayCharacters.Any()));
        }

        [Test]
        public async Task Damsel_NoPingIfMarionette()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Damsel,Ravenkeeper,Soldier,Fisherman,Baron,Mayor")
                            .WithMarionette(Character.Damsel)
                            .Build();

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Baron).Received().MinionInformation(Arg.Is<Player>(demon => demon.RealCharacter == Character.Imp),
                                                                            Arg.Is<IReadOnlyCollection<Player>>(fellowMinions => !fellowMinions.Any()),
                                                                            false,
                                                                            Arg.Is<IReadOnlyCollection<Character>>(notInPlayCharacters => !notInPlayCharacters.Any()));
        }

        [Test]
        public async Task Damsel_LosesWhenGuessed()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Damsel,Fisherman,Baron,Mayor");
            setup.Agent(Character.Baron).MockMinionDamselGuess(Character.Damsel);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
        }

        [Test]
        public async Task Damsel_SecondGuessHasNoEffect()
        {
            // Night 1 & Day 1
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Damsel,Fisherman,Baron,Mayor");
            await game.StartGame();

            setup.Agent(Character.Baron).MockMinionDamselGuess(Character.Soldier);
            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Baron).MockMinionDamselGuess(Character.Damsel);

            await game.RunNightAndDay();

            Assert.That(game.Finished, Is.False);
        }

        [TestCase(Character.Imp)]
        [TestCase(Character.Damsel)]
        [TestCase(Character.Mayor)]
        public async Task Damsel_NonMinionCanNotEndGame(Character guesser)
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Damsel,Fisherman,Baron,Mayor");
            setup.Agent(guesser).MockMinionDamselGuess(Character.Damsel);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.False);
        }

        [TestCase(Character.Imp)]
        [TestCase(Character.Damsel)]
        [TestCase(Character.Mayor)]
        public async Task Damsel_NonMinionCanNotWasteGuess(Character firstGuesser)
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Damsel,Fisherman,Baron,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(firstGuesser).MockMinionDamselGuess(Character.Soldier);
            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Baron).MockMinionDamselGuess(Character.Damsel);

            await game.RunNightAndDay();

            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
        }

        [Test]
        public async Task PhilosopherDamsel_LosesWhenGuessed()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Philosopher,Fisherman,Baron,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Damsel);
            setup.Agent(Character.Baron).MockMinionDamselGuess(Character.Damsel);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
        }

        [Test]
        public async Task Damsel_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Damsel,Fisherman,Poisoner,Mayor");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Damsel);
            setup.Agent(Character.Poisoner).MockMinionDamselGuess(Character.Damsel);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.False);
        }

        [Test]
        public async Task Damsel_SweetheartDrunk()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Damsel,Fisherman,Baron,Sweetheart");
            await game.StartGame();

            // Night 1 & Day 1
            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Damsel);
            setup.Agent(Character.Baron).MockMinionDamselGuess(Character.Damsel);

            await game.RunNightAndDay();

            Assert.That(game.Finished, Is.False);
        }
    }
}