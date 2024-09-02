using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class NightwatchmanTests
    {
        [Test]
        public async Task Nightwatchman_UseOnFirstNight()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Baron,Nightwatchman,Mayor");
            setup.Agent(Character.Nightwatchman).MockNightwatchmanOption(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            setup.Storyteller.Received().ShowNightwatchman(Arg.Is<Player>(player => player.Character == Character.Nightwatchman), Arg.Is<Player>(player => player.Character == Character.Saint), true);
            await setup.Agent(Character.Saint).Received().ShowNightwatchman(Arg.Is<Player>(player => player.Character == Character.Nightwatchman));
        }

        [Test]
        public async Task Nightwatchman_UseOnSecondNight()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Baron,Nightwatchman,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            await game.RunNightAndDay();

            setup.Storyteller.DidNotReceiveWithAnyArgs().ShowNightwatchman(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<bool>());
            await setup.Agent(Character.Saint).DidNotReceiveWithAnyArgs().ShowNightwatchman(Arg.Any<Player>());

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Nightwatchman).MockNightwatchmanOption(Character.Saint);

            await game.RunNightAndDay();

            setup.Storyteller.Received().ShowNightwatchman(Arg.Is<Player>(player => player.Character == Character.Nightwatchman), Arg.Is<Player>(player => player.Character == Character.Saint), true);
            await setup.Agent(Character.Saint).Received().ShowNightwatchman(Arg.Is<Player>(player => player.Character == Character.Nightwatchman));
        }

        [Test]
        public async Task Nightwatchman_CanOnlyBeUsedOnce()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Baron,Nightwatchman,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Nightwatchman).MockNightwatchmanOption(Character.Saint);

            await game.RunNightAndDay();

            setup.Storyteller.ClearReceivedCalls();
            setup.Agent(Character.Saint).ClearReceivedCalls();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);

            await game.RunNightAndDay();

            setup.Storyteller.DidNotReceiveWithAnyArgs().ShowNightwatchman(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<bool>());
            await setup.Agent(Character.Saint).DidNotReceiveWithAnyArgs().ShowNightwatchman(Arg.Any<Player>());
        }

        [Test]
        public async Task Nightwatchman_IsTheDrunk()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Mayor,Nightwatchman,Saint,Baron,Soldier,Fisherman")
                            .WithDrunk(Character.Nightwatchman)
                            .Build();
            setup.Agent(Character.Nightwatchman).MockNightwatchmanOption(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            setup.Storyteller.Received().ShowNightwatchman(Arg.Is<Player>(player => player.Character == Character.Nightwatchman), Arg.Is<Player>(player => player.Character == Character.Saint), false);
            await setup.Agent(Character.Saint).DidNotReceiveWithAnyArgs().ShowNightwatchman(Arg.Any<Player>());
        }

        [Test]
        public async Task Nightwatchman_IsTheMarionette()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Nightwatchman,Mayor,Saint,Baron,Soldier,Fisherman")
                            .WithMarionette(Character.Nightwatchman)
                            .Build();
            setup.Agent(Character.Nightwatchman).MockNightwatchmanOption(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            setup.Storyteller.Received().ShowNightwatchman(Arg.Is<Player>(player => player.Character == Character.Nightwatchman), Arg.Is<Player>(player => player.Character == Character.Saint), false);
            await setup.Agent(Character.Saint).DidNotReceiveWithAnyArgs().ShowNightwatchman(Arg.Any<Player>());
        }

        [Test]
        public async Task Nightwatchman_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Nightwatchman,Saint,Poisoner,Soldier,Fisherman");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Nightwatchman);
            setup.Agent(Character.Nightwatchman).MockNightwatchmanOption(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            setup.Storyteller.Received().ShowNightwatchman(Arg.Is<Player>(player => player.Character == Character.Nightwatchman), Arg.Is<Player>(player => player.Character == Character.Saint), false);
            await setup.Agent(Character.Saint).DidNotReceiveWithAnyArgs().ShowNightwatchman(Arg.Any<Player>());
        }

        [Test]
        public async Task Nightwatchman_SweetheartDrunk()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Nightwatchman,Sweetheart,Baron,Soldier,Fisherman");
            await game.StartGame();

            // Night 1 & Day 1
            await game.RunNightAndDay();

            setup.Storyteller.DidNotReceiveWithAnyArgs().ShowNightwatchman(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<bool>());
            await setup.Agent(Character.Soldier).DidNotReceiveWithAnyArgs().ShowNightwatchman(Arg.Any<Player>());

            // Night 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Nightwatchman);
            setup.Agent(Character.Nightwatchman).MockNightwatchmanOption(Character.Soldier);

            await game.RunNightAndDay();

            setup.Storyteller.Received().ShowNightwatchman(Arg.Is<Player>(player => player.Character == Character.Nightwatchman), Arg.Is<Player>(player => player.Character == Character.Soldier), false);
            await setup.Agent(Character.Soldier).DidNotReceiveWithAnyArgs().ShowNightwatchman(Arg.Any<Player>());
        }
        
        [Test]
        public async Task Nightwatchman_PhilosopherDrunk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Nightwatchman,Saint,Baron,Soldier,Philosopher");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Nightwatchman);
            setup.Agent(Character.Nightwatchman).MockNightwatchmanOption(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            setup.Storyteller.Received().ShowNightwatchman(Arg.Is<Player>(player => player.Character == Character.Nightwatchman), Arg.Is<Player>(player => player.Character == Character.Saint), false);
            await setup.Agent(Character.Saint).DidNotReceiveWithAnyArgs().ShowNightwatchman(Arg.Any<Player>());
        }

        [Test]
        public async Task PhilosopherNightwatchman()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Baron,Philosopher,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Nightwatchman);
            setup.Agent(Character.Philosopher).MockNightwatchmanOption(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            setup.Storyteller.Received().ShowNightwatchman(Arg.Is<Player>(player => player.RealCharacter == Character.Philosopher), Arg.Is<Player>(player => player.Character == Character.Saint), true);
            await setup.Agent(Character.Saint).Received().ShowNightwatchman(Arg.Is<Player>(player => player.RealCharacter == Character.Philosopher));
        }

        [Test]
        public async Task CannibalNightwatchman()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Baron,Nightwatchman,Cannibal");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Imp).MockNomination(Character.Nightwatchman);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Cannibal).MockNightwatchmanOption(Character.Saint);

            await game.RunNightAndDay();

            setup.Storyteller.Received().ShowNightwatchman(Arg.Is<Player>(player => player.Character == Character.Cannibal), Arg.Is<Player>(player => player.Character == Character.Saint), true);
            await setup.Agent(Character.Saint).Received().ShowNightwatchman(Arg.Is<Player>(player => player.Character == Character.Cannibal));
        }

        [Test]
        public async Task CannibalNightwatchman_Poisoned()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Baron,Mayor,Cannibal");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Imp).MockNomination(Character.Baron);
            setup.Storyteller.MockCannibalChoice(Character.Nightwatchman);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Cannibal).MockNightwatchmanOption(Character.Saint);

            await game.RunNightAndDay();

            setup.Storyteller.Received().ShowNightwatchman(Arg.Is<Player>(player => player.Character == Character.Cannibal), Arg.Is<Player>(player => player.Character == Character.Saint), false);
            await setup.Agent(Character.Saint).DidNotReceiveWithAnyArgs().ShowNightwatchman(Arg.Any<Player>());
        }
    }
}