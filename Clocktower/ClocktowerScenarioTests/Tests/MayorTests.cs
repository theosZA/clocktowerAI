using Clocktower.Game;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class MayorTests
    {
        [Test]
        public async Task Mayor_SafeFromImp()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            setup.Agent(Character.Imp).MockImp(Character.Mayor);
            setup.Storyteller.MockGetMayorBounce(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Mayor).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Fisherman).Received().YouAreDead();
        }

        [Test]
        public async Task Mayor_IsTheDrunk()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Soldier,Ravenkeeper,Saint,Baron,Fisherman,Mayor")
                            .WithDrunk(Character.Mayor)
                            .Build();
            setup.Agent(Character.Imp).MockImp(Character.Mayor);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Mayor).Received().YouAreDead();
        }

        [Test]
        public async Task Mayor_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Poisoner,Fisherman,Mayor");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Mayor);
            setup.Agent(Character.Imp).MockImp(Character.Mayor);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Mayor).Received().YouAreDead();
        }

        [Test]
        public async Task Mayor_SweetheartDrunk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Sweetheart,Baron,Fisherman,Mayor");
            setup.Agent(Character.Imp).MockNomination(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Mayor);
            setup.Agent(Character.Imp).MockImp(Character.Mayor);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Mayor).Received().YouAreDead();
        }

        [Test]
        public async Task Mayor_PhilosopherDrunk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Ravenkeeper,Saint,Baron,Fisherman,Philosopher");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Mayor);
            setup.Agent(Character.Imp).RequestChoiceFromImp(Arg.Any<IReadOnlyCollection<IOption>>()).Returns(args => args.GetOptionForRealCharacterFromArg(Character.Mayor));

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Mayor).Received().YouAreDead();
        }

        [Test]
        public async Task Mayor_BounceToSoldier()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            setup.Agent(Character.Imp).MockImp(Character.Mayor);
            setup.Storyteller.MockGetMayorBounce(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Mayor).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Soldier).DidNotReceive().YouAreDead();  // Soldier should stay protected because the kill is still from the Imp.
        }

        [Test]
        public async Task Mayor_ProtectedByMonk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Monk,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            setup.Agent(Character.Monk).MockMonkChoice(Character.Mayor);
            setup.Agent(Character.Imp).MockImp(Character.Mayor);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Mayor).DidNotReceive().YouAreDead();
            await setup.Storyteller.DidNotReceive().GetMayorBounce(Arg.Any<Player>(), Arg.Any<Player?>(), Arg.Any<IReadOnlyCollection<IOption>>()); // Monk protection means Mayor bounce shouldn't trigger.
        }

        [Test]
        public async Task PhilosopherMayor()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Philosopher,Ravenkeeper,Saint,Baron,Fisherman,Soldier");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Mayor);
            setup.Agent(Character.Imp).MockImp(Character.Mayor);
            setup.Storyteller.MockGetMayorBounce(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Philosopher).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Fisherman).Received().YouAreDead();
        }
    }
}