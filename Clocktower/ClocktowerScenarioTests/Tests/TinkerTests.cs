using Clocktower.Agent;
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
            setup.Agent(Character.Imp).RequestChoiceFromImp(Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args =>
                {
                    shouldKill = true;
                    return args.GetOptionForCharacterFromArg(Character.Ravenkeeper);
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
            setup.Agent(Character.Slayer).PromptSlayerShot(Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args =>
                {
                    shouldKill = true;
                    return args.GetOptionForCharacterFromArg(Character.Tinker);
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
    }
}