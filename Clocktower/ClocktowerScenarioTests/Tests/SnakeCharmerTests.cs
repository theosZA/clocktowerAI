using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class SnakeCharmerTests
    {
        [Test]
        public async Task SnakeCharmer_Miss()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Snake_Charmer,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            var snakeCharmerOptions = setup.Agent(Character.Snake_Charmer).MockSnakeCharmerChoice(Character.Saint);
            setup.Agent(Character.Mayor).MockNomination(Character.Mayor);

            await game.RunNightAndDay();

            Assert.That(snakeCharmerOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Snake_Charmer, Character.Ravenkeeper, Character.Saint, Character.Baron, Character.Fisherman, Character.Mayor })); // all
            snakeCharmerOptions.Clear();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);

            await game.RunNightAndDay();

            Assert.That(snakeCharmerOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Snake_Charmer, Character.Ravenkeeper, Character.Saint, Character.Baron, Character.Fisherman })); // exclude dead Mayor
            await setup.Agent(Character.Snake_Charmer).DidNotReceive().AssignCharacter(Character.Imp, Alignment.Evil);
            await setup.Agent(Character.Imp).DidNotReceive().AssignCharacter(Character.Snake_Charmer, Alignment.Good);
            await setup.Agent(Character.Saint).Received().YouAreDead();
        }

        [Test]
        public async Task SnakeCharmer_FirstNightHit()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Snake_Charmer,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Snake_Charmer).MockSnakeCharmerChoice(Character.Imp);

            await game.RunNightAndDay();

            await setup.Agent(Character.Snake_Charmer).Received().AssignCharacter(Character.Imp, Alignment.Evil);
            await setup.Agent(Character.Imp).Received().AssignCharacter(Character.Snake_Charmer, Alignment.Good);
            await setup.Agent(Character.Snake_Charmer).Received().DemonInformation(Arg.Is<IReadOnlyCollection<Player>>(players => players.Count == 1 && players.ElementAt(0).Character == Character.Baron),
                                                                                   Arg.Any<IReadOnlyCollection<Character>>());

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockSnakeCharmerChoice(Character.Imp);   // New Snake Charmer is poisoned - should not swap back.
            setup.Agent(Character.Snake_Charmer).MockDemonKill(Character.Saint);

            await game.RunNightAndDay();

            await setup.Agent(Character.Saint).Received().YouAreDead();
        }

        [Test]
        public async Task SnakeCharmer_SecondNightHit()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Snake_Charmer,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Snake_Charmer).MockSnakeCharmerChoice(Character.Snake_Charmer);

            await game.RunNightAndDay();

            await setup.Agent(Character.Imp).Received().DemonInformation(Arg.Is<IReadOnlyCollection<Player>>(players => players.Count == 1 && players.ElementAt(0).Character == Character.Baron),
                                                                         Arg.Any<IReadOnlyCollection<Character>>());

            // Night 2 & Day 2
            setup.Agent(Character.Snake_Charmer).MockSnakeCharmerChoice(Character.Imp);
            setup.Agent(Character.Snake_Charmer).MockDemonKill(Character.Saint);

            await game.RunNightAndDay();

            await setup.Agent(Character.Snake_Charmer).Received().AssignCharacter(Character.Imp, Alignment.Evil);
            await setup.Agent(Character.Imp).Received().AssignCharacter(Character.Snake_Charmer, Alignment.Good);
            await setup.Agent(Character.Saint).Received().YouAreDead();
        }

        [Test]
        public async Task SnakeCharmer_HitsRecluse_RegistersAsDemon()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Snake_Charmer,Ravenkeeper,Recluse,Baron,Fisherman,Mayor");
            setup.Agent(Character.Snake_Charmer).MockSnakeCharmerChoice(Character.Recluse);
            setup.Storyteller.MockShouldRegisterForSnakeCharmer(Character.Recluse, shouldRegisterAsDemon: true);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Snake_Charmer).Received().AssignCharacter(Character.Recluse, Alignment.Good);
            await setup.Agent(Character.Recluse).Received().AssignCharacter(Character.Snake_Charmer, Alignment.Good);
        }

        [Test]
        public async Task SnakeCharmer_HitsRecluse_DoesNotRegisterAsDemon()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Snake_Charmer,Ravenkeeper,Recluse,Baron,Fisherman,Mayor");
            setup.Agent(Character.Snake_Charmer).MockSnakeCharmerChoice(Character.Recluse);
            setup.Storyteller.MockShouldRegisterForSnakeCharmer(Character.Recluse, shouldRegisterAsDemon: false);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Snake_Charmer).DidNotReceive().AssignCharacter(Character.Recluse, Alignment.Good);
            await setup.Agent(Character.Recluse).DidNotReceive().AssignCharacter(Character.Snake_Charmer, Alignment.Good);
        }

        [Test]
        public async Task SnakeCharmer_IsTheDrunk()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Snake_Charmer,Ravenkeeper,Soldier,Baron,Mayor,Saint")
                            .WithDrunk(Character.Snake_Charmer)
                            .Build();

            setup.Agent(Character.Snake_Charmer).MockSnakeCharmerChoice(Character.Imp);

            // Act
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Snake_Charmer).DidNotReceive().AssignCharacter(Character.Imp, Alignment.Evil);
            await setup.Agent(Character.Imp).DidNotReceive().AssignCharacter(Character.Snake_Charmer, Alignment.Good);
            await setup.Agent(Character.Imp).Received().DemonInformation(Arg.Is<IReadOnlyCollection<Player>>(players => players.Count == 1 && players.ElementAt(0).Character == Character.Baron),
                                                                         Arg.Any<IReadOnlyCollection<Character>>());
        }

        [Test]
        public async Task SnakeCharmer_IsTheMarionette()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Snake_Charmer,Ravenkeeper,Soldier,Fisherman,Mayor,Saint")
                            .WithMarionette(Character.Snake_Charmer)
                            .Build();

            setup.Agent(Character.Snake_Charmer).MockSnakeCharmerChoice(Character.Imp);

            // Act
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Snake_Charmer).DidNotReceive().AssignCharacter(Character.Imp, Alignment.Evil);
            await setup.Agent(Character.Imp).DidNotReceive().AssignCharacter(Character.Snake_Charmer, Alignment.Good);
            await setup.Agent(Character.Imp).Received().DemonInformation(Arg.Is<IReadOnlyCollection<Player>>(players => players.Count == 1 && players.ElementAt(0).Character == Character.Snake_Charmer),
                                                                         Arg.Any<IReadOnlyCollection<Character>>());
        }

        [Test]
        public async Task SnakeCharmer_Poisoned()
        {
            // Note that a Snake Charmer can't be poisoned on night 1, so we are testing the poisoned ability on night 2.

            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Snake_Charmer,Ravenkeeper,Saint,Poisoner,Fisherman,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Snake_Charmer).MockSnakeCharmerChoice(Character.Snake_Charmer);
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Snake_Charmer);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Snake_Charmer).MockSnakeCharmerChoice(Character.Imp);
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);

            await game.RunNightAndDay();

            await setup.Agent(Character.Snake_Charmer).DidNotReceive().AssignCharacter(Character.Imp, Alignment.Evil);
            await setup.Agent(Character.Imp).DidNotReceive().AssignCharacter(Character.Snake_Charmer, Alignment.Good);
            await setup.Agent(Character.Saint).Received().YouAreDead();
        }

        [Test]
        public async Task PhilosopherSnakeCharmer()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Philosopher,Ravenkeeper,Saint,Baron,Fisherman,Soldier");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Snake_Charmer);
            setup.Agent(Character.Philosopher).MockSnakeCharmerChoice(Character.Imp);

            await game.RunNightAndDay();

            await setup.Agent(Character.Philosopher).Received().AssignCharacter(Character.Imp, Alignment.Evil);
            await setup.Agent(Character.Imp).Received().AssignCharacter(Character.Philosopher, Alignment.Good);

            // Night 2 - the Imp turned Philosopher is poisoned
            setup.Agent(Character.Imp).MockPhilosopher(Character.Empath);
            setup.Agent(Character.Philosopher).MockDemonKill(Character.Saint);
            setup.Storyteller.MockGetEmpathNumber(2);
            var empathNumber = setup.Agent(Character.Imp).MockNotifyEmpath(gameToEnd: game);

            await game.RunNightAndDay();

            Assert.That(empathNumber.Value, Is.EqualTo(2)); // poisoned
            await setup.Agent(Character.Saint).Received().YouAreDead();
        }

        [Test]
        public async Task CannibalSnakeCharmer()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Cannibal,Ravenkeeper,Saint,Baron,Snake_Charmer,Soldier");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Snake_Charmer).MockSnakeCharmerChoice(Character.Snake_Charmer);
            setup.Agent(Character.Snake_Charmer).MockNomination(Character.Snake_Charmer);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Cannibal).MockSnakeCharmerChoice(Character.Imp);
            setup.Agent(Character.Cannibal).MockDemonKill(Character.Saint);
            setup.Agent(Character.Soldier).MockNomination(Character.Soldier);

            await game.RunNightAndDay();

            await setup.Agent(Character.Saint).Received().YouAreDead();
            await setup.Agent(Character.Cannibal).Received().AssignCharacter(Character.Imp, Alignment.Evil);
            await setup.Agent(Character.Imp).Received().AssignCharacter(Character.Cannibal, Alignment.Good);

            // Night 3 & Day 3 - the Imp turned Cannibal is poisoned, so doesn't really have the ability of the executed Soldier.
            setup.Agent(Character.Cannibal).MockDemonKill(Character.Cannibal);

            await game.RunNightAndDay();

            await setup.Agent(Character.Imp).Received().YouAreDead();
        }
    }
}