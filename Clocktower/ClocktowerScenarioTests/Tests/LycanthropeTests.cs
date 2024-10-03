using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class LycanthropeTests
    {
        [Test]
        public async Task Lycanthrope_DoesNotKillEvil()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Lycanthrope,Ravenkeeper,Saint,Baron,Soldier,Mayor");
            setup.Agent(Character.Lycanthrope).MockLycanthropeChoice(Character.Baron);
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Baron).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Saint).Received().YouAreDead();
        }

        [Test]
        public async Task Lycanthrope_KillsGood()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Lycanthrope,Ravenkeeper,Saint,Baron,Soldier,Mayor");
            setup.Agent(Character.Lycanthrope).MockLycanthropeChoice(Character.Saint);
            setup.Agent(Character.Imp).MockDemonKill(Character.Baron);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Saint).Received().YouAreDead();
            await setup.Agent(Character.Baron).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Lycanthrope_KillsSelf()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Lycanthrope,Ravenkeeper,Saint,Baron,Soldier,Mayor");
            setup.Agent(Character.Lycanthrope).MockLycanthropeChoice(Character.Lycanthrope);
            setup.Agent(Character.Imp).MockDemonKill(Character.Baron);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Lycanthrope).Received().YouAreDead();
            await setup.Agent(Character.Baron).Received().YouAreDead();
        }

        [Test]
        public async Task Lycanthrope_DoesNotKillSelfIfEvil()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Lycanthrope,Ravenkeeper,Saint,Baron,Bounty_Hunter,Mayor");
            setup.Storyteller.MockGetEvilTownsfolk(Character.Lycanthrope);
            setup.Storyteller.MockGetBountyHunterPing(Character.Lycanthrope);
            setup.Agent(Character.Lycanthrope).MockLycanthropeChoice(Character.Lycanthrope);
            setup.Agent(Character.Imp).MockDemonKill(Character.Baron);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Lycanthrope).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Baron).Received().YouAreDead();
        }

        [Test]
        public async Task Lycanthrope_CanKillSoldier()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Lycanthrope,Ravenkeeper,Saint,Baron,Soldier,Mayor");
            setup.Agent(Character.Lycanthrope).MockLycanthropeChoice(Character.Soldier);
            setup.Agent(Character.Imp).MockDemonKill(Character.Baron);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Soldier).Received().YouAreDead();
            await setup.Agent(Character.Baron).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Lycanthrope_CanKillPlayerProtectedByMonk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Lycanthrope,Ravenkeeper,Saint,Baron,Monk,Mayor");
            setup.Agent(Character.Monk).MockMonkChoice(Character.Saint);
            setup.Agent(Character.Lycanthrope).MockLycanthropeChoice(Character.Saint);
            setup.Agent(Character.Imp).MockDemonKill(Character.Baron);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Saint).Received().YouAreDead();
            await setup.Agent(Character.Baron).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Lycanthrope_RecluseSuccess()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Lycanthrope,Ravenkeeper,Recluse,Baron,Soldier,Mayor");
            setup.Agent(Character.Lycanthrope).MockLycanthropeChoice(Character.Recluse);
            setup.Storyteller.MockShouldRegisterAsGoodForLycanthrope(shouldRegisterAsGood: true);
            setup.Agent(Character.Imp).MockDemonKill(Character.Baron);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Recluse).Received().YouAreDead();
            await setup.Agent(Character.Baron).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Lycanthrope_RecluseFailure()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Lycanthrope,Ravenkeeper,Recluse,Baron,Soldier,Mayor");
            setup.Agent(Character.Lycanthrope).MockLycanthropeChoice(Character.Recluse);
            setup.Storyteller.MockShouldRegisterAsGoodForLycanthrope(shouldRegisterAsGood: false);
            setup.Agent(Character.Imp).MockDemonKill(Character.Baron);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Recluse).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Baron).Received().YouAreDead();
        }

        [Test]
        public async Task Lycanthrope_SpySuccess()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Lycanthrope,Ravenkeeper,Saint,Spy,Soldier,Mayor");
            setup.Agent(Character.Lycanthrope).MockLycanthropeChoice(Character.Spy);
            setup.Storyteller.MockShouldRegisterAsGoodForLycanthrope(shouldRegisterAsGood: true);
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Spy).Received().YouAreDead();
            await setup.Agent(Character.Saint).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Lycanthrope_SpyFailure()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Lycanthrope,Ravenkeeper,Saint,Spy,Soldier,Mayor");
            setup.Agent(Character.Lycanthrope).MockLycanthropeChoice(Character.Spy);
            setup.Storyteller.MockShouldRegisterAsGoodForLycanthrope(shouldRegisterAsGood: false);
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Spy).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Saint).Received().YouAreDead();
        }

        [Test]
        public async Task Lycanthrope_MayorBounceToGood()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Lycanthrope,Ravenkeeper,Saint,Baron,Soldier,Mayor");
            setup.Agent(Character.Lycanthrope).MockLycanthropeChoice(Character.Mayor);
            setup.Storyteller.MockGetMayorBounce(Character.Saint);
            setup.Agent(Character.Imp).MockDemonKill(Character.Baron);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Mayor).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Saint).Received().YouAreDead();
            await setup.Agent(Character.Baron).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Lycanthrope_MayorBounceToEvil()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Lycanthrope,Ravenkeeper,Saint,Baron,Soldier,Mayor");
            setup.Agent(Character.Lycanthrope).MockLycanthropeChoice(Character.Mayor);
            setup.Storyteller.MockGetMayorBounce(Character.Baron);
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Mayor).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Baron).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Saint).Received().YouAreDead();
        }

        [Test]
        public async Task Lycanthrope_MayorBounceToRecluseSuccess()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Lycanthrope,Ravenkeeper,Recluse,Baron,Soldier,Mayor");
            setup.Agent(Character.Lycanthrope).MockLycanthropeChoice(Character.Mayor);
            setup.Storyteller.MockGetMayorBounce(Character.Recluse);
            setup.Storyteller.MockShouldRegisterAsGoodForLycanthrope(shouldRegisterAsGood: true);
            setup.Agent(Character.Imp).MockDemonKill(Character.Baron);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Mayor).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Recluse).Received().YouAreDead();
            await setup.Agent(Character.Baron).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Lycanthrope_MayorBounceToRecluseFailure()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Lycanthrope,Ravenkeeper,Recluse,Baron,Soldier,Mayor");
            setup.Agent(Character.Lycanthrope).MockLycanthropeChoice(Character.Mayor);
            setup.Storyteller.MockGetMayorBounce(Character.Recluse);
            setup.Storyteller.MockShouldRegisterAsGoodForLycanthrope(shouldRegisterAsGood: false);
            setup.Agent(Character.Imp).MockDemonKill(Character.Baron);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Mayor).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Recluse).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Baron).Received().YouAreDead();
        }

        [Test]
        public async Task Lycanthrope_MayorBounceToSpySuccess()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Lycanthrope,Ravenkeeper,Saint,Spy,Soldier,Mayor");
            setup.Agent(Character.Lycanthrope).MockLycanthropeChoice(Character.Mayor);
            setup.Storyteller.MockGetMayorBounce(Character.Spy);
            setup.Storyteller.MockShouldRegisterAsGoodForLycanthrope(shouldRegisterAsGood: true);
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Mayor).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Spy).Received().YouAreDead();
            await setup.Agent(Character.Saint).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Lycanthrope_MayorBounceToSpyFailure()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Lycanthrope,Ravenkeeper,Saint,Spy,Soldier,Mayor");
            setup.Agent(Character.Lycanthrope).MockLycanthropeChoice(Character.Mayor);
            setup.Storyteller.MockGetMayorBounce(Character.Spy);
            setup.Storyteller.MockShouldRegisterAsGoodForLycanthrope(shouldRegisterAsGood: false);
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Mayor).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Spy).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Saint).Received().YouAreDead();
        }

        [Test]
        public async Task Lycanthrope_MayorBounceToSelf()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Lycanthrope,Ravenkeeper,Saint,Baron,Soldier,Mayor");
            setup.Agent(Character.Lycanthrope).MockLycanthropeChoice(Character.Mayor);
            setup.Storyteller.MockGetMayorBounce(Character.Lycanthrope);
            setup.Agent(Character.Imp).MockDemonKill(Character.Baron);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Mayor).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Lycanthrope).Received().YouAreDead();
            await setup.Agent(Character.Baron).Received().YouAreDead();
        }

        [Test]
        public async Task Lycanthrope_BlocksGodfatherKill()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Lycanthrope,Ravenkeeper,Saint,Godfather,Fisherman,Recluse");
            setup.Agent(Character.Recluse).MockNomination(Character.Recluse);
            setup.Agent(Character.Lycanthrope).MockLycanthropeChoice(Character.Saint);
            setup.Agent(Character.Imp).MockDemonKill(Character.Godfather);
            setup.Agent(Character.Godfather).MockGodfather(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Saint).Received().YouAreDead();
            await setup.Agent(Character.Godfather).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Fisherman).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Lycanthrope_DoesNotBlockAssassinKill()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Lycanthrope,Ravenkeeper,Saint,Assassin,Fisherman,Mayor");
            setup.Agent(Character.Lycanthrope).MockLycanthropeChoice(Character.Saint);
            setup.Agent(Character.Imp).MockDemonKill(Character.Assassin);
            setup.Agent(Character.Assassin).MockAssassin(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Saint).Received().YouAreDead();
            await setup.Agent(Character.Assassin).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Fisherman).Received().YouAreDead();
        }

        [Test]
        public async Task Lycanthrope_TriggersRavenkeeper()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Lycanthrope,Ravenkeeper,Saint,Baron,Soldier,Mayor");
            setup.Agent(Character.Lycanthrope).MockLycanthropeChoice(Character.Ravenkeeper);
            setup.Agent(Character.Ravenkeeper).MockRavenkeeperChoice(Character.Mayor);
            setup.Agent(Character.Imp).MockDemonKill(Character.Baron);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Ravenkeeper).Received().YouAreDead();
            await setup.Agent(Character.Baron).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Ravenkeeper).Received().NotifyRavenkeeper(Arg.Is<Player>(player => player.Character == Character.Mayor), Character.Mayor);
        }
    }
}