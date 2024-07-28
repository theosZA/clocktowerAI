using Clocktower.Game;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class FishermanTests
    {
        [Test]
        public async Task Fisherman_GetAdviceOnFirstDay()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            setup.Agent(Character.Fisherman).MockFishermanOption(getAdvice: true);
            const string expectedAdvice = "Sample advice";
            setup.Storyteller.MockFishermanAdvice(expectedAdvice);
            var actualAdvice = setup.Agent(Character.Fisherman).MockFishermanAdvice(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(actualAdvice.Value, Is.EqualTo(expectedAdvice));
        }

        [Test]
        public async Task Fisherman_GetAdviceOnSecondDay()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            await game.StartGame();
            const string expectedAdvice = "Sample advice";
            setup.Storyteller.MockFishermanAdvice(expectedAdvice);
            var actualAdvice = setup.Agent(Character.Fisherman).MockFishermanAdvice(gameToEnd: game);

            // Night 1 & Day 1
            setup.Agent(Character.Fisherman).MockFishermanOption(getAdvice: false);

            await game.RunNightAndDay();

            Assert.That(actualAdvice.Value, Is.Null);
            setup.Agent(Character.Fisherman).ClearReceivedCalls();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Fisherman).MockFishermanOption(getAdvice: true);

            await game.RunNightAndDay();

            Assert.That(actualAdvice.Value, Is.EqualTo(expectedAdvice));
        }


        [Test]
        public async Task Fisherman_NoAdviceWhenDead()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Fisherman).MockFishermanOption(getAdvice: false);

            await game.RunNightAndDay();

            setup.Agent(Character.Fisherman).ClearReceivedCalls();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Fisherman);

            await game.RunNightAndDay();

            await setup.Agent(Character.Fisherman).DidNotReceive().PromptFishermanAdvice(Arg.Any<IReadOnlyCollection<IOption>>());
        }

        [Test]
        public async Task Fisherman_IsTheDrunk()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Soldier,Ravenkeeper,Saint,Baron,Fisherman,Mayor")
                            .WithDrunk(Character.Fisherman)
                            .Build();

            setup.Agent(Character.Fisherman).MockFishermanOption(getAdvice: true);
            const string expectedAdvice = "Sample advice (which is drunk)";
            setup.Storyteller.MockFishermanAdvice(expectedAdvice);
            var actualAdvice = setup.Agent(Character.Fisherman).MockFishermanAdvice(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(actualAdvice.Value, Is.EqualTo(expectedAdvice));
        }

        [Test]
        public async Task Fisherman_IsTheMarionette()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Fisherman,Ravenkeeper,Saint,Baron,Soldier,Mayor")
                            .WithMarionette(Character.Fisherman)
                            .Build();

            setup.Agent(Character.Fisherman).MockFishermanOption(getAdvice: true);
            const string expectedAdvice = "Sample advice (which is drunk)";
            setup.Storyteller.MockFishermanAdvice(expectedAdvice);
            var actualAdvice = setup.Agent(Character.Fisherman).MockFishermanAdvice(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(actualAdvice.Value, Is.EqualTo(expectedAdvice));
        }

        [Test]
        public async Task Fisherman_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Poisoner,Fisherman,Mayor");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Fisherman);
            setup.Agent(Character.Fisherman).MockFishermanOption(getAdvice: true);
            const string expectedAdvice = "Sample advice (which is poisoned)";
            setup.Storyteller.MockFishermanAdvice(expectedAdvice);
            var actualAdvice = setup.Agent(Character.Fisherman).MockFishermanAdvice(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(actualAdvice.Value, Is.EqualTo(expectedAdvice));
        }

        [Test]
        public async Task PhilosopherFisherman()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Baron,Philosopher,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Fisherman);
            setup.Agent(Character.Philosopher).MockFishermanOption(getAdvice: true);
            const string expectedAdvice = "Sample advice";
            setup.Storyteller.MockFishermanAdvice(expectedAdvice);
            var actualAdvice = setup.Agent(Character.Philosopher).MockFishermanAdvice(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(actualAdvice.Value, Is.EqualTo(expectedAdvice));
        }
    }
}