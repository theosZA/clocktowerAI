using Clocktower.Agent.Observer;
using Clocktower.Game;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class DevilsAdvocateTests
    {
        [Test]
        public async Task DevilsAdvocate_ChosenSurvivesExecution()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Devils_Advocate,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Devils_Advocate).MockDevilsAdvocate(Character.Fisherman);
            setup.Agent(Character.Imp).MockNomination(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task DevilsAdvocate_NotChosenDoesNotSurviveExecution()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Devils_Advocate,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Devils_Advocate).MockDevilsAdvocate(Character.Fisherman);
            setup.Agent(Character.Imp).MockNomination(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Soldier).Received().YouAreDead();
        }

        [Test]
        public async Task DevilsAdvocate_MayNotPickSamePlayerTheNextNight()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Devils_Advocate,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Devils_Advocate).MockDevilsAdvocate(Character.Fisherman);
            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Devils_Advocate).ClearReceivedCalls();
            var playersThatCanBeProtected = new List<Character>();
            setup.Agent(Character.Devils_Advocate).RequestChoiceFromDevilsAdvocate(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsMatchingOptionFromOptionsArg(Character.Soldier, playersThatCanBeProtected);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Imp).MockNomination(Character.Fisherman);

            await game.RunNightAndDay();

            Assert.That(playersThatCanBeProtected, Is.EquivalentTo(new[] { Character.Imp, Character.Devils_Advocate, Character.Ravenkeeper, Character.Saint, Character.Soldier, Character.Mayor }));    // all but Fisherman
            await setup.Agent(Character.Fisherman).Received().YouAreDead();
        }

        [Test]
        public async Task DevilsAdvocate_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Devils_Advocate,Poisoner,Saint,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Devils_Advocate);
            setup.Agent(Character.Devils_Advocate).MockDevilsAdvocate(Character.Fisherman);
            setup.Agent(Character.Imp).MockNomination(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).Received().YouAreDead();
        }

        [Test]
        public async Task DevilsAdvocate_MayNotPickSamePlayerTheNextNightEvenWhenPoisoned()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Devils_Advocate,Poisoner,Saint,Fisherman,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Devils_Advocate);
            setup.Agent(Character.Devils_Advocate).MockDevilsAdvocate(Character.Fisherman);
            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Mayor);
            setup.Agent(Character.Devils_Advocate).ClearReceivedCalls();
            var playersThatCanBeProtected = new List<Character>();
            setup.Agent(Character.Devils_Advocate).RequestChoiceFromDevilsAdvocate(Arg.Any<IReadOnlyCollection<IOption>>()).ReturnsMatchingOptionFromOptionsArg(Character.Soldier, playersThatCanBeProtected);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Imp).MockNomination(Character.Soldier);

            await game.RunNightAndDay();

            Assert.That(playersThatCanBeProtected, Is.EquivalentTo(new[] { Character.Imp, Character.Devils_Advocate, Character.Poisoner, Character.Saint, Character.Soldier, Character.Mayor }));    // all but Fisherman
            await setup.Agent(Character.Soldier).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task DevilsAdvocate_DrunkedDuringNomination()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Devils_Advocate,Witch,Sweetheart,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Witch).MockWitch(Character.Sweetheart);
            setup.Agent(Character.Devils_Advocate).MockDevilsAdvocate(Character.Fisherman);
            setup.Agent(Character.Sweetheart).MockNomination(Character.Fisherman);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Devils_Advocate);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).Received().YouAreDead();
        }

        [Test]
        public async Task DevilsAdvocate_UnpoisonedDuringNomination()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Devils_Advocate,Poisoner,Sweetheart,Fisherman,Soldier,Witch");
            setup.Agent(Character.Witch).MockWitch(Character.Sweetheart);
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Devils_Advocate);
            setup.Agent(Character.Devils_Advocate).MockDevilsAdvocate(Character.Fisherman);
            setup.Agent(Character.Sweetheart).MockNomination(Character.Fisherman);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Poisoner);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).Received().YouAreDead(); // picked player was never actually protected
        }

        [Test]
        public async Task DevilsAdvocate_Tinker()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Devils_Advocate,Ravenkeeper,Tinker,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Devils_Advocate).MockDevilsAdvocate(Character.Tinker);
            setup.Agent(Character.Imp).MockNomination(Character.Tinker);

            // Only kill Tinker at end of day as they are executed.
            var shouldKillTinker = false;
            setup.Storyteller.Observer.When(observer => observer.AnnounceVoteResult(Arg.Any<Player>(), Arg.Any<int?>(), Arg.Any<VoteResult>())).Do(args => { shouldKillTinker = true; });
            setup.Storyteller.ShouldKillTinker(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetYesNoOptionFromArg(shouldKillTinker, 1));

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Imp).Observer.Received().PlayerIsExecuted(Arg.Is<Player>(player => player.Character == Character.Tinker), true);
            await setup.Agent(Character.Tinker).Received().YouAreDead();
        }
    }
}