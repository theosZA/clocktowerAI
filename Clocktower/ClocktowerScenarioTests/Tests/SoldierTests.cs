using Clocktower.Game;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class SoldierTests
    {
        [Test]
        public async Task Soldier_SafeFromImp()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Soldier).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Soldier_IsTheDrunk()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Soldier,Ravenkeeper,Saint,Baron,Fisherman,Mayor")
                            .WithDrunk(Character.Soldier)
                            .Build();
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Soldier).Received().YouAreDead();
        }

        [Test]
        public async Task Soldier_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Poisoner,Fisherman,Mayor");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Soldier);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Soldier).Received().YouAreDead();
        }

        [Test]
        public async Task Soldier_SweetheartDrunk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Sweetheart,Baron,Fisherman,Mayor");
            setup.Agent(Character.Imp).MockNomination(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Soldier);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Soldier).Received().YouAreDead();
        }

        [Test]
        public async Task Soldier_PhilosopherDrunk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Baron,Fisherman,Philosopher");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Soldier);
            setup.Agent(Character.Imp).RequestChoiceFromDemon(Arg.Any<Character>(), Arg.Any<IReadOnlyCollection<IOption>>()).Returns(args => args.GetOptionForRealCharacterFromArg(Character.Soldier, argIndex: 1));

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Soldier).Received().YouAreDead();
        }

        [Test]
        public async Task PhilosopherSoldier()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Philosopher,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Soldier);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Philosopher).DidNotReceive().YouAreDead();
        }
    }
}