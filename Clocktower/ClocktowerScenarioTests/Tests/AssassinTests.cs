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
            setup.Agents[0].MockImp(new[] { Character.Soldier, Character.Soldier });
            setup.Agents[1].MockAssassin(Character.Saint);

            await game.RunNightAndDay();

            await setup.Agents[3].Received().YouAreDead();

            // Night 3 and Day 3 - ensure the Assassin doesn't kill again.
            setup.Agents[1].ClearReceivedCalls();

            await game.RunNightAndDay();

            await setup.Agents[1].DidNotReceive().RequestChoiceFromAssassin(Arg.Any<IReadOnlyCollection<IOption>>());
        }

        [Test]
        public async Task Assassin_KillOnNight3()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Assassin,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");

            // Night 1 and Day 1
            await game.StartGame();
            await game.RunNightAndDay();

            // Night 2 and Day 2 - ensure no one dies
            setup.Agents[0].MockImp(new[] { Character.Soldier, Character.Soldier });
            setup.Agents[1].MockAssassin(target: null);

            await game.RunNightAndDay();

            foreach (var agent in setup.Agents)
            {
                await agent.DidNotReceive().YouAreDead();
            }

            // Night 3 and Day 3
            setup.Agents[1].MockAssassin(Character.Saint);

            await game.RunNightAndDay();

            await setup.Agents[3].Received().YouAreDead();
        }

        [Test]
        public async Task Assassin_CanKillSoldier()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Assassin,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");
            setup.Agents[0].MockImp(new[] { Character.Fisherman, Character.Saint });
            setup.Agents[1].MockAssassin(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agents[5].Received().YouAreDead();
        }

        [Test]
        public async Task Assassin_CanKillMayor()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Assassin,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");
            setup.Agents[0].MockImp(new[] { Character.Fisherman, Character.Saint });
            setup.Agents[1].MockAssassin(Character.Mayor);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agents[6].Received().YouAreDead();
            await setup.Storyteller.DidNotReceive().GetMayorBounce(Arg.Any<Player>(), Arg.Any<Player?>(), Arg.Any<IReadOnlyCollection<IOption>>());
        }

        [Test]
        public async Task Assassin_CanKillImpToEndGame()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Assassin,Ravenkeeper,Saint,Fisherman,Soldier,Mayor");
            setup.Agents[0].MockImp(new[] { Character.Fisherman, Character.Saint });
            setup.Agents[1].MockAssassin(Character.Imp);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agents[0].Received().YouAreDead();
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Good));
        }
    }
}