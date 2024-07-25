using Clocktower.Game;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class TinkerTests
    {
        [Test]
        public async Task Tinker_CanDieDayOne()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Ravenkeeper,Tinker,Soldier,Slayer,Mayor");
            setup.Storyteller.MockShouldKillTinker(shouldKill: true);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Tinker).Received().YouAreDead();
        }

        [Test]
        public async Task Tinker_CanDieNightTwo()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Ravenkeeper,Tinker,Soldier,Slayer,Mayor");

            bool shouldKill = false;
            setup.Storyteller.ShouldKillTinker(Arg.Is<Player>(player => player.Character == Character.Tinker), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args =>
                {
                    return args.GetYesNoOptionFromArg(shouldKill, argIndex: 1);
                });

            // - Tinker should trigger between Imp kill and Ravenkeeper choice.
            setup.Agent(Character.Imp).RequestChoiceFromDemon(Arg.Any<Character>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args =>
                {
                    shouldKill = true;
                    return args.GetOptionForCharacterFromArg(Character.Ravenkeeper, argIndex: 1);
                });
            setup.Agent(Character.Ravenkeeper).RequestChoiceFromRavenkeeper(Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args =>
                {
                    shouldKill = false;
                    return args.GetOptionForCharacterFromArg(Character.Ravenkeeper);
                });

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Tinker).Received().YouAreDead();
        }

        [Test]
        public async Task Tinker_CanDieToSlayerShot()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Ravenkeeper,Tinker,Soldier,Slayer,Mayor");

            bool shouldKill = false;
            setup.Storyteller.ShouldKillTinker(Arg.Is<Player>(player => player.Character == Character.Tinker), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args =>
                {
                    return args.GetYesNoOptionFromArg(shouldKill, argIndex: 1);
                });

            // - Tinker should trigger between Slayer choice and nominations.
            setup.Agent(Character.Slayer).PromptShenanigans(Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args =>
                {
                    shouldKill = true;
                    var slayerOption = (SlayerShotOption)args.ArgAt<IReadOnlyCollection<IOption>>(0).First(option => option is SlayerShotOption);
                    slayerOption.SetTarget(slayerOption.PossiblePlayers.First(player => player.Character == Character.Tinker));
                    return slayerOption;
                });
            setup.Agent(Character.Imp).GetNomination(Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args =>
                {
                    shouldKill = false;
                    return args.GetPassOptionFromArg();
                });

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Tinker).Received().YouAreDead();
        }

        [Test]
        public async Task Tinker_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Poisoner,Ravenkeeper,Tinker,Soldier,Slayer,Mayor");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Tinker);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Storyteller.DidNotReceive().ShouldKillTinker(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>());
        }

        [Test]
        public async Task Tinker_SweetheartDrunk()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Ravenkeeper,Tinker,Soldier,Sweetheart,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockShouldKillTinker(false);
            setup.Agent(Character.Imp).MockNomination(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Tinker);

            await game.RunNightAndDay();

            await setup.Storyteller.Received().ShouldKillTinker(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>());
            setup.Storyteller.ClearReceivedCalls();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);

            await game.RunNightAndDay();

            await setup.Storyteller.DidNotReceive().ShouldKillTinker(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>());
        }

        [Test]
        public async Task Tinker_PhilosopherDrunk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Ravenkeeper,Tinker,Soldier,Slayer,Philosopher");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Tinker);
            setup.Storyteller.MockShouldKillTinker(shouldKill: false);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Storyteller.DidNotReceive().ShouldKillTinker(Arg.Is<Player>(player => player.RealCharacter == Character.Tinker), Arg.Any<IReadOnlyCollection<IOption>>());
        }

        [Test]
        public async Task PhilosopherTinker()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Ravenkeeper,Philosopher,Soldier,Slayer,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Tinker);
            setup.Storyteller.MockShouldKillTinker(shouldKill: true);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Philosopher).Received().YouAreDead();
        }
    }
}