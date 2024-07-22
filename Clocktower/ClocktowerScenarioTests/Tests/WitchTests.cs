using Clocktower.Game;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class WitchTests
    {
        [Test]
        public async Task Witch_NoKillOnUncursedPlayer()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Witch,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Witch).MockWitch(Character.Fisherman);
            setup.Agent(Character.Ravenkeeper).MockNomination(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Soldier).Received().YouAreDead();
        }

        [Test]
        public async Task Witch_KillCursedPlayer()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Witch,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Witch).MockWitch(Character.Fisherman);
            setup.Agent(Character.Fisherman).MockNomination(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).Received().YouAreDead();
            await setup.Agent(Character.Soldier).Received().YouAreDead();
        }

        [Test]
        public async Task Witch_KillSoldier()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Witch,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Witch).MockWitch(Character.Soldier);
            setup.Agent(Character.Soldier).MockNomination(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Soldier).Received().YouAreDead();
            await setup.Agent(Character.Fisherman).Received().YouAreDead();
        }

        [Test]
        public async Task Witch_KillDemon()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Witch,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Witch).MockWitch(Character.Imp);
            setup.Agent(Character.Imp).MockNomination(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Imp).Received().YouAreDead();
            await setup.Agent(Character.Fisherman).DidNotReceive().YouAreDead();
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Good));
        }

        [Test]
        public async Task Witch_SelfKill()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Witch,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Witch).MockWitch(Character.Witch);
            setup.Agent(Character.Witch).MockNomination(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Witch).Received().YouAreDead();
            await setup.Agent(Character.Soldier).Received().YouAreDead();
        }

        [Test]
        public async Task Witch_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Witch,Poisoner,Saint,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Witch).MockWitch(Character.Fisherman);
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Witch);
            setup.Agent(Character.Fisherman).MockNomination(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Soldier).Received().YouAreDead();
        }

        [Test]
        public async Task Witch_NoKillWhenDead()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Witch,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Witch).MockWitch(Character.Witch);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Witch).MockWitch(Character.Fisherman);
            setup.Agent(Character.Imp).MockDemonKill(Character.Witch);
            setup.Agent(Character.Fisherman).MockNomination(Character.Soldier);
            
            await game.RunNightAndDay();

            await setup.Agent(Character.Fisherman).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Soldier).Received().YouAreDead();
        }

        [Test]
        public async Task Witch_NoKillWhenThreeAlive()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Witch,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Witch).MockWitch(Character.Witch);
            setup.Agent(Character.Imp).MockNomination(Character.Ravenkeeper);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);
            setup.Agent(Character.Imp).MockNomination(Character.Soldier);

            await game.RunNightAndDay();

            // Night 3 & Day 3
            setup.Agent(Character.Witch).MockWitch(Character.Imp);  // 4 players are alive at this point, so this will be called.
            setup.Agent(Character.Imp).MockDemonKill(Character.Fisherman);
            setup.Agent(Character.Imp).MockNomination(Character.Mayor);

            await game.RunNightAndDay();

            await setup.Agent(Character.Imp).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Mayor).Received().YouAreDead();
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
        }

        [Test]
        public async Task Witch_NoAbilityWhenThreeAlive()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Witch,Ravenkeeper,Saint,Fisherman,Soldier,Slayer");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Witch).MockWitch(Character.Witch);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Slayer);
            setup.Agent(Character.Imp).MockNomination(Character.Soldier);

            await game.RunNightAndDay();

            // Night 3 & Day 3
            setup.Agent(Character.Imp).MockDemonKill(Character.Fisherman);
            setup.Agent(Character.Imp).MockNomination(Character.Ravenkeeper);

            await game.RunNightAndDay();

            // Night 4
            setup.Agent(Character.Witch).ClearReceivedCalls();
            setup.Agent(Character.Witch).MockWitch(Character.Imp);  // 3 players are alive at this point, so this should not be called.
            setup.Agent(Character.Imp).MockDemonKill(Character.Saint);

            await game.RunNightAndDay();

            await setup.Agent(Character.Witch).DidNotReceive().RequestChoiceFromWitch(Arg.Any<IReadOnlyCollection<IOption>>());
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
        }
    }
}