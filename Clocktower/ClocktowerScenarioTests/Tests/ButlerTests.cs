using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class ButlerTests
    {
        [Test]
        public async Task Butler_MasterHasVotedForExecution()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Butler,Baron,Fisherman,Mayor");
            var masterOptions = setup.Agent(Character.Butler).MockButlerChoice(Character.Ravenkeeper);
            setup.Agent(Character.Soldier).MockNomination(Character.Soldier);
            // - by default all players will vote

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(masterOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Soldier, Character.Ravenkeeper, Character.Baron, Character.Fisherman, Character.Mayor }));  // excludes Butler
            await setup.Agent(Character.Butler).Received().GetVote(Arg.Any<IReadOnlyCollection<IOption>>(), false);
        }

        [Test]
        public async Task Butler_MasterHasVotedAgainstExecution()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Butler,Baron,Fisherman,Mayor");
            var masterOptions = setup.Agent(Character.Butler).MockButlerChoice(Character.Ravenkeeper);
            setup.Agent(Character.Soldier).MockNomination(Character.Soldier);
            setup.Agent(Character.Ravenkeeper).GetVote(Arg.Any<IReadOnlyCollection<IOption>>(), false).Returns(args => args.GetPassOptionFromArg());

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(masterOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Soldier, Character.Ravenkeeper, Character.Baron, Character.Fisherman, Character.Mayor }));  // excludes Butler
            await setup.Agent(Character.Butler).DidNotReceive().GetVote(Arg.Any<IReadOnlyCollection<IOption>>(), Arg.Any<bool>());
        }

        [Test]
        public async Task Butler_MasterHasNominatedButNotYetVoted()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Butler,Baron,Fisherman,Mayor");
            var masterOptions = setup.Agent(Character.Butler).MockButlerChoice(Character.Baron);
            setup.Agent(Character.Baron).MockNomination(Character.Ravenkeeper);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(masterOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Soldier, Character.Ravenkeeper, Character.Baron, Character.Fisherman, Character.Mayor }));  // excludes Butler
            await setup.Agent(Character.Butler).Received().GetVote(Arg.Any<IReadOnlyCollection<IOption>>(), false);
        }

        // This is the most dubious case as it runs counter to how the Butler is run in a real game of Blood on the Clocktower.
        // I've chosen to allow the Butler vote in this case to keep the text of the adjusted Butler ability to a reasonable length, and thus simpler to understand.
        [Test]
        public async Task Butler_MasterHasNominatedButVotedAgainstExecution()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Butler,Baron,Fisherman,Mayor");
            var masterOptions = setup.Agent(Character.Butler).MockButlerChoice(Character.Ravenkeeper);
            setup.Agent(Character.Ravenkeeper).MockNomination(Character.Soldier);
            setup.Agent(Character.Ravenkeeper).GetVote(Arg.Any<IReadOnlyCollection<IOption>>(), false).Returns(args => args.GetPassOptionFromArg());

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(masterOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Soldier, Character.Ravenkeeper, Character.Baron, Character.Fisherman, Character.Mayor }));  // excludes Butler
            await setup.Agent(Character.Butler).Received().GetVote(Arg.Any<IReadOnlyCollection<IOption>>(), false);
        }
    }
}