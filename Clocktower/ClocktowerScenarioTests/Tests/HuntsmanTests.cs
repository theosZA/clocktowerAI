using Clocktower.Game;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class HuntsmanTests
    {
        [Test]
        public async Task Huntsman_SuccessfulGuessOnFirstNight()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Damsel,Baron,Fisherman,Huntsman");
            setup.Agent(Character.Huntsman).MockHuntsmanOption(Character.Damsel);
            setup.Storyteller.MockNewDamselCharacter(Character.Empath);
            var empathNumber = setup.Agent(Character.Damsel).MockNotifyEmpath(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(empathNumber.Value, Is.EqualTo(1));
        }

        [Test]
        public async Task Huntsman_SuccessfulGuessOnLaterNight()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Damsel,Baron,Fisherman,Huntsman");
            await game.StartGame();

            // Night 1 & Day 1
            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Huntsman).MockHuntsmanOption(Character.Damsel);
            setup.Storyteller.MockNewDamselCharacter(Character.Empath);
            var empathNumber = setup.Agent(Character.Damsel).MockNotifyEmpath(gameToEnd: game);

            await game.RunNightAndDay();

            Assert.That(empathNumber.Value, Is.EqualTo(1));
        }

        [Test]
        public async Task Huntsman_FailedGuess()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Damsel,Baron,Fisherman,Huntsman");
            setup.Agent(Character.Huntsman).MockHuntsmanOption(Character.Imp);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Storyteller.DidNotReceive().ChooseDamselCharacter(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>());
        }

        [Test]
        public async Task Huntsman_IsTheDrunk()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Soldier,Ravenkeeper,Damsel,Baron,Fisherman,Huntsman")
                            .WithDrunk(Character.Huntsman)
                            .Build();
            setup.Agent(Character.Huntsman).MockHuntsmanOption(Character.Damsel);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Storyteller.DidNotReceive().ChooseDamselCharacter(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>());
        }

        [Test]
        public async Task Huntsman_IsTheMarionette()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Soldier,Ravenkeeper,Damsel,Baron,Fisherman,Huntsman")
                            .WithMarionette(Character.Huntsman)
                            .Build();
            setup.Agent(Character.Huntsman).MockHuntsmanOption(Character.Damsel);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Storyteller.DidNotReceive().ChooseDamselCharacter(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>());
        }

        [Test]
        public async Task Huntsman_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Damsel,Poisoner,Fisherman,Huntsman");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Huntsman);
            setup.Agent(Character.Huntsman).MockHuntsmanOption(Character.Damsel);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Storyteller.DidNotReceive().ChooseDamselCharacter(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>());
        }

        [Test]
        public async Task Huntsman_DamselIsPoisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Damsel,Poisoner,Fisherman,Huntsman");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Damsel);
            setup.Agent(Character.Huntsman).MockHuntsmanOption(Character.Damsel);
            setup.Storyteller.MockNewDamselCharacter(Character.Empath);
            setup.Storyteller.MockGetEmpathNumber(2);
            var empathNumber = setup.Agent(Character.Damsel).MockNotifyEmpath(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(empathNumber.Value, Is.EqualTo(2)); // poisoned
        }

        [Test]
        public async Task PhilosopherHuntsman()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Damsel,Baron,Fisherman,Philosopher");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Huntsman);
            setup.Agent(Character.Philosopher).MockHuntsmanOption(Character.Damsel);
            setup.Storyteller.MockNewDamselCharacter(Character.Empath);
            var empathNumber = setup.Agent(Character.Damsel).MockNotifyEmpath(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(empathNumber.Value, Is.EqualTo(1));
        }

        [Test]
        public async Task CannibalHuntsman()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Damsel,Baron,Cannibal,Huntsman");
            setup.Agent(Character.Huntsman).MockHuntsmanOption(Character.Imp);
            setup.Agent(Character.Imp).MockNomination(Character.Huntsman);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Cannibal).MockHuntsmanOption(Character.Damsel);
            setup.Storyteller.MockNewDamselCharacter(Character.Empath);
            var empathNumber = setup.Agent(Character.Damsel).MockNotifyEmpath(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(empathNumber.Value, Is.EqualTo(1));
        }
    }
}