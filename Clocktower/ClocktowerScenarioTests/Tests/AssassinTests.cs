using Clocktower.Game;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class AssassinTests
    {
        [Test]
        public async Task Assassin_KillOnNight2()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Assassin,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");

            // Night 1 and Day 1
            await game.StartGame();
            await game.RunNightAndDay();

            // Night 2 and Day 2
            setup.Agent(Character.Imp).MockDemonKill(new[] { Character.Soldier, Character.Soldier });
            setup.Agent(Character.Assassin).MockAssassin(Character.Saint);

            await game.RunNightAndDay();

            await setup.Agent(Character.Saint).Received().YouAreDead();

            // Night 3 and Day 3 - ensure the Assassin doesn't kill again.
            setup.Agent(Character.Assassin).ClearReceivedCalls();

            await game.RunNightAndDay();

            await setup.Agent(Character.Assassin).DidNotReceive().RequestChoiceFromAssassin(Arg.Any<IReadOnlyCollection<IOption>>());
        }

        [Test]
        public async Task Assassin_KillOnNight3()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Assassin,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");

            // Night 1 and Day 1
            await game.StartGame();
            await game.RunNightAndDay();

            // Night 2 and Day 2 - ensure no one dies
            setup.Agent(Character.Imp).MockDemonKill(new[] { Character.Soldier, Character.Soldier });
            setup.Agent(Character.Assassin).MockAssassin(target: null);

            await game.RunNightAndDay();

            foreach (var agent in setup.Agents)
            {
                await agent.DidNotReceive().YouAreDead();
            }

            // Night 3 and Day 3
            setup.Agent(Character.Assassin).MockAssassin(Character.Saint);

            await game.RunNightAndDay();

            await setup.Agent(Character.Saint).Received().YouAreDead();
        }

        [Test]
        public async Task Assassin_CanKillSoldier()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Assassin,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Imp).MockDemonKill(Character.Fisherman);
            setup.Agent(Character.Assassin).MockAssassin(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Soldier).Received().YouAreDead();
        }

        [Test]
        public async Task Assassin_CanKillMayor()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Assassin,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Imp).MockDemonKill(Character.Fisherman);
            setup.Agent(Character.Assassin).MockAssassin(Character.Mayor);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Mayor).Received().YouAreDead();
            await setup.Storyteller.DidNotReceive().GetMayorBounce(Arg.Any<Player>(), Arg.Any<Player?>(), Arg.Any<IReadOnlyCollection<IOption>>());
        }

        [Test]
        public async Task Assassin_CanKillImpToEndGame()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Assassin,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Imp).MockDemonKill(Character.Fisherman);
            setup.Agent(Character.Assassin).MockAssassin(Character.Imp);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Imp).Received().YouAreDead();
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Good));
        }

        [Test]
        public async Task Assassin_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Assassin,Poisoner,Saint,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Assassin);
            setup.Agent(Character.Imp).MockDemonKill(Character.Fisherman);
            setup.Agent(Character.Assassin).MockAssassin(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Saint).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Assassin_SweetheartDrunk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Assassin,Ravenkeeper,Sweetheart,Fisherman,Soldier,Mayor");
            setup.Agent(Character.Imp).MockNomination(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Assassin);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Assassin).MockAssassin(Character.Fisherman);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).DidNotReceive().YouAreDead();
        }
    }
}