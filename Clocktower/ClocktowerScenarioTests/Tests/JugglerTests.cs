using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class JugglerTests
    {
        [Test]
        public async Task Juggler_NoJuggleMade()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Saint,Ravenkeeper,Soldier,Juggler,Mayor");
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Juggler).Received().NotifyJuggler(0);
        }

        [Test]
        public async Task Juggler_ZeroJuggles()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Saint,Ravenkeeper,Soldier,Juggler,Mayor");
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Juggler).MockJugglerOption(Array.Empty<(Character player, Character character)>());

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Juggler).Received().NotifyJuggler(0);
        }

        [Test]
        public async Task Juggler_JuggleZero()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Saint,Ravenkeeper,Soldier,Juggler,Mayor");
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Juggler).MockJugglerOption(new[] { (Character.Imp, Character.Tinker) });

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Juggler).Received().NotifyJuggler(0);
        }

        [Test]
        public async Task Juggler_JuggleOne()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Saint,Ravenkeeper,Soldier,Juggler,Mayor");
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Juggler).MockJugglerOption(new[] { (Character.Imp, Character.Imp) });

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Juggler).Received().NotifyJuggler(1);
        }

        [Test]
        public async Task Juggler_JuggleFive()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Saint,Ravenkeeper,Soldier,Juggler,Mayor");
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Juggler).MockJugglerOption(new[] { (Character.Imp, Character.Imp),
                                                                     (Character.Baron, Character.Baron),
                                                                     (Character.Saint, Character.Saint),
                                                                     (Character.Ravenkeeper, Character.Ravenkeeper),
                                                                     (Character.Soldier, Character.Soldier) });

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Juggler).Received().NotifyJuggler(5);
        }

        [Test]
        public async Task Juggler_RepeatJuggles()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Saint,Ravenkeeper,Soldier,Juggler,Mayor");
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Juggler).MockJugglerOption(new[] { (Character.Imp, Character.Imp),
                                                                     (Character.Imp, Character.Imp),
                                                                     (Character.Imp, Character.Imp) });

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Juggler).Received().NotifyJuggler(3);
        }

        [TestCase(Character.Fisherman, 0)]
        [TestCase(Character.Drunk, 1)]
        public async Task Juggler_Drunk(Character asCharacter, int expectedJuggleCount)
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Juggler,Ravenkeeper,Saint,Baron,Fisherman,Mayor")
                            .WithDrunk(Character.Fisherman)
                            .Build();
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);
            setup.Agent(Character.Juggler).MockJugglerOption(new[] { (Character.Fisherman, asCharacter) });

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Juggler).Received().NotifyJuggler(expectedJuggleCount);
        }

        [TestCase(Character.Mayor, 0)]
        [TestCase(Character.Philosopher, 1)]
        public async Task Juggler_Philosopher(Character asCharacter, int expectedJuggleCount)
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Saint,Ravenkeeper,Soldier,Juggler,Philosopher");
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Mayor);
            setup.Agent(Character.Juggler).MockJugglerOption(new[] { (Character.Mayor, asCharacter) });

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Juggler).Received().NotifyJuggler(expectedJuggleCount);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Juggler_Recluse(bool shouldRegisterForJuggle)
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Recluse,Ravenkeeper,Soldier,Juggler,Mayor");
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Juggler).MockJugglerOption(new[] { (Character.Recluse, Character.Assassin) });
            setup.Storyteller.MockShouldRegisterForJuggle(Character.Recluse, Character.Assassin, shouldRegisterForJuggle);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Juggler).Received().NotifyJuggler(shouldRegisterForJuggle ? 1 : 0);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Juggler_Spy(bool shouldRegisterForJuggle)
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Spy,Saint,Ravenkeeper,Soldier,Juggler,Mayor");
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Juggler).MockJugglerOption(new[] { (Character.Spy, Character.Juggler) });
            setup.Storyteller.MockShouldRegisterForJuggle(Character.Spy, Character.Juggler, shouldRegisterForJuggle);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Juggler).Received().NotifyJuggler(shouldRegisterForJuggle ? 1 : 0);
        }

        [Test]
        public async Task Juggler_IsTheDrunk()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Juggler,Baron,Saint,Soldier,Fisherman,Mayor")
                            .WithDrunk(Character.Juggler)
                            .Build();
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            var jugglerNumbers = setup.Storyteller.MockGetJugglerNumber(3);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(jugglerNumbers, Is.EquivalentTo(new[] { 0, 1, 2, 3, 4, 5 }));
            await setup.Agent(Character.Juggler).Received().NotifyJuggler(3);
        }

        [Test]
        public async Task Juggler_IsTheMarionette()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Juggler,Baron,Saint,Soldier,Fisherman,Mayor")
                            .WithMarionette(Character.Juggler)
                            .Build();
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            var jugglerNumbers = setup.Storyteller.MockGetJugglerNumber(3);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(jugglerNumbers, Is.EquivalentTo(new[] { 0, 1, 2, 3, 4, 5 }));
            await setup.Agent(Character.Juggler).Received().NotifyJuggler(3);
        }

        [Test]
        public async Task Juggler_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Poisoner,Saint,Ravenkeeper,Soldier,Juggler,Mayor");
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Juggler);
            var jugglerNumbers = setup.Storyteller.MockGetJugglerNumber(3);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(jugglerNumbers, Is.EquivalentTo(new[] { 0, 1, 2, 3, 4, 5 }));
            await setup.Agent(Character.Juggler).Received().NotifyJuggler(3);
        }

        [Test]
        public async Task Juggler_Unpoisoned()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Poisoner,Saint,Ravenkeeper,Soldier,Juggler,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Juggler);
            setup.Agent(Character.Juggler).MockJugglerOption(new[] { (Character.Juggler, Character.Juggler) });

            await game.RunNightAndDay();


            // Night 2 & Day 2
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Imp);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);

            await game.RunNightAndDay();

            await setup.Agent(Character.Juggler).Received().NotifyJuggler(1);
        }

        [Test]
        public async Task Juggler_DayTwoJugglesYieldNoInformation()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Saint,Ravenkeeper,Soldier,Juggler,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            await game.RunNightAndDay();


            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Juggler).MockJugglerOption(new[] { (Character.Juggler, Character.Juggler) });

            await game.RunNightAndDay();

            // Night 3 & Day 3
            setup.Agent(Character.Juggler).ClearReceivedCalls();

            await game.RunNightAndDay();

            await setup.Agent(Character.Juggler).DidNotReceive().NotifyJuggler(Arg.Any<int>());
        }

        [Test]
        public async Task PhilosopherJuggler_DayOne()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Saint,Ravenkeeper,Soldier,Philosopher,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Juggler);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Philosopher).MockJugglerOption(new[] { (Character.Imp, Character.Imp),
                                                                         (Character.Baron, Character.Baron),
                                                                         (Character.Saint, Character.Saint),
                                                                         (Character.Ravenkeeper, Character.Ravenkeeper),
                                                                         (Character.Juggler, Character.Philosopher) });

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Philosopher).Received().NotifyJuggler(5);
        }

        [Test]
        public async Task PhilosopherJuggler_DayTwo()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Saint,Ravenkeeper,Soldier,Philosopher,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Juggler);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Philosopher).MockJugglerOption(new[] { (Character.Imp, Character.Imp),
                                                                         (Character.Baron, Character.Baron),
                                                                         (Character.Saint, Character.Saint),
                                                                         (Character.Ravenkeeper, Character.Ravenkeeper),
                                                                         (Character.Juggler, Character.Philosopher) });

            await game.RunNightAndDay();

            await setup.Agent(Character.Philosopher).DidNotReceive().NotifyJuggler(Arg.Any<int>());

            // Night 3 & Day 3
            setup.Agent(Character.Philosopher).ClearReceivedCalls();

            await game.RunNightAndDay();

            await setup.Agent(Character.Philosopher).Received().NotifyJuggler(5);
        }

        [Test]
        public async Task PhilosopherJuggler_Juggler()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Saint,Juggler,Soldier,Philosopher,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Juggler);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Philosopher).MockJugglerOption(new[] { (Character.Imp, Character.Imp) });
            setup.Agent(Character.Juggler).MockJugglerOption(new[] { (Character.Imp, Character.Imp) });
            setup.Storyteller.MockGetJugglerNumber(3);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Philosopher).Received().NotifyJuggler(1);
            await setup.Agent(Character.Juggler).Received().NotifyJuggler(3);
        }
    }
}