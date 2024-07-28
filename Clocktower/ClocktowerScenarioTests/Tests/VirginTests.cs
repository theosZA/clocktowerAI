using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class VirginTests
    {
        [Test]
        public async Task Virgin_NominatedByTownsfolk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Virgin,Soldier,Ravenkeeper,Fisherman,Saint");
            setup.Agent(Character.Soldier).MockNomination(Character.Virgin);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Storyteller.Observer.Received().PlayerIsExecuted(Arg.Is<Player>(player => player.Character == Character.Soldier), true);
        }

        [Test]
        public async Task Virgin_NominatedByOutsider()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Virgin,Soldier,Ravenkeeper,Fisherman,Saint");
            setup.Agent(Character.Saint).MockNomination(Character.Virgin);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Storyteller.Observer.DidNotReceive().PlayerIsExecuted(Arg.Is<Player>(player => player.Character == Character.Saint), Arg.Any<bool>());
        }

        [Test]
        public async Task Virgin_NominatedByMinion()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Virgin,Soldier,Ravenkeeper,Fisherman,Saint");
            setup.Agent(Character.Baron).MockNomination(Character.Virgin);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Storyteller.Observer.DidNotReceive().PlayerIsExecuted(Arg.Is<Player>(player => player.Character == Character.Baron), Arg.Any<bool>());
        }

        [Test]
        public async Task Virgin_NominatedByDemon()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Virgin,Soldier,Ravenkeeper,Fisherman,Saint");
            setup.Agent(Character.Imp).MockNomination(Character.Virgin);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Storyteller.Observer.DidNotReceive().PlayerIsExecuted(Arg.Is<Player>(player => player.Character == Character.Imp), Arg.Any<bool>());
        }

        [Test]
        public async Task Virgin_NominatedByDrunk()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Baron,Virgin,Soldier,Ravenkeeper,Fisherman,Saint")
                            .WithDrunk(Character.Soldier)
                            .Build();
            setup.Agent(Character.Soldier).MockNomination(Character.Virgin);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Storyteller.Observer.DidNotReceive().PlayerIsExecuted(Arg.Is<Player>(player => player.Character == Character.Saint), Arg.Any<bool>());
        }

        [Test]
        public async Task Virgin_NominatedBySpyRegisteringAsTownsfolk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Spy,Virgin,Soldier,Ravenkeeper,Fisherman,Saint");
            setup.Agent(Character.Spy).MockNomination(Character.Virgin);
            setup.Storyteller.MockShouldExecuteWithVirgin(shouldExecute: true);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Storyteller.Observer.Received().PlayerIsExecuted(Arg.Is<Player>(player => player.Character == Character.Spy), true);
        }

        [Test]
        public async Task Virgin_NominatedBySpyNotRegisteringAsTownsfolk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Spy,Virgin,Soldier,Ravenkeeper,Fisherman,Saint");
            setup.Agent(Character.Spy).MockNomination(Character.Virgin);
            setup.Storyteller.MockShouldExecuteWithVirgin(shouldExecute: false);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Storyteller.Observer.DidNotReceive().PlayerIsExecuted(Arg.Is<Player>(player => player.Character == Character.Spy), Arg.Any<bool>());
        }

        [Test]
        public async Task Virgin_IsTheDrunk()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Baron,Virgin,Soldier,Ravenkeeper,Fisherman,Saint")
                            .WithDrunk(Character.Virgin)
                            .Build();
            setup.Agent(Character.Soldier).MockNomination(Character.Virgin);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Storyteller.Observer.DidNotReceive().PlayerIsExecuted(Arg.Is<Player>(player => player.Character == Character.Soldier), Arg.Any<bool>());
        }

        [Test]
        public async Task Virgin_IsTheMarionette()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Virgin,Baron,Soldier,Ravenkeeper,Fisherman,Saint")
                            .WithMarionette(Character.Virgin)
                            .Build();
            setup.Agent(Character.Soldier).MockNomination(Character.Virgin);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Storyteller.Observer.DidNotReceive().PlayerIsExecuted(Arg.Is<Player>(player => player.Character == Character.Soldier), Arg.Any<bool>());
        }

        [Test]
        public async Task Virgin_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Poisoner,Virgin,Soldier,Ravenkeeper,Fisherman,Saint");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Virgin);
            setup.Agent(Character.Soldier).MockNomination(Character.Virgin);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Storyteller.Observer.DidNotReceive().PlayerIsExecuted(Arg.Is<Player>(player => player.Character == Character.Soldier), Arg.Any<bool>());
        }

        [Test]
        public async Task Virgin_SweetheartDrunk()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Virgin,Soldier,Ravenkeeper,Fisherman,Sweetheart");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Imp).MockNomination(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Virgin);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Fisherman).MockNomination(Character.Virgin);

            await game.RunNightAndDay();

            await setup.Storyteller.Observer.DidNotReceive().PlayerIsExecuted(Arg.Is<Player>(player => player.Character == Character.Fisherman), Arg.Any<bool>());
        }

        [Test]
        public async Task Virgin_PhilosopherDrunk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Virgin,Soldier,Ravenkeeper,Fisherman,Philosopher");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Virgin);
            setup.Agent(Character.Soldier).GetNomination(Arg.Any<IReadOnlyCollection<IOption>>()).Returns(args => args.GetOptionForRealCharacterFromArg(Character.Virgin));

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Storyteller.Observer.DidNotReceive().PlayerIsExecuted(Arg.Is<Player>(player => player.Character == Character.Soldier), Arg.Any<bool>());
        }

        [Test]
        public async Task PhilosopherVirgin()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Philosopher,Soldier,Ravenkeeper,Fisherman,Saint");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Virgin);
            setup.Agent(Character.Soldier).MockNomination(Character.Virgin);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Storyteller.Observer.Received().PlayerIsExecuted(Arg.Is<Player>(player => player.Character == Character.Soldier), true);
        }
    }
}