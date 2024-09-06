using Clocktower.Game;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class WidowTests
    {
        [Test]
        public async Task Widow_SelfPoison()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Empath,Saint,Widow,Soldier,Fisherman");
            setup.Agent(Character.Widow).MockWidow(Character.Widow);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Widow).Received().ShowGrimoire(Character.Widow, Arg.Any<Grimoire>());
            foreach (var agent in setup.Agents)
            {
                await agent.DidNotReceive().LearnOfWidow();
            }
        }

        [Test]
        public async Task Widow_OnlyGoodPlayersCanLearnOfWidow()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Empath,Saint,Widow,Soldier,Fisherman");
            setup.Agent(Character.Widow).MockWidow(Character.Imp);
            var playersWhoCanLearnOfWidow = setup.Storyteller.MockWidowPing(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Soldier).Received().LearnOfWidow();
            Assert.That(playersWhoCanLearnOfWidow, Is.EquivalentTo(new[] { Character.Mayor, Character.Empath, Character.Saint, Character.Soldier, Character.Fisherman }));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public async Task Widow_ChosenPlayerIsPoisoned(int empathNumber)
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Empath,Saint,Widow,Soldier,Fisherman");
            setup.Agent(Character.Widow).MockWidow(Character.Empath);
            setup.Storyteller.MockWidowPing(Character.Soldier);
            setup.Storyteller.MockGetEmpathNumber(empathNumber);
            var receivedEmpathNumber = setup.Agent(Character.Empath).MockNotifyEmpath(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedEmpathNumber.Value, Is.EqualTo(empathNumber));
        }

        [Test]
        public async Task Widow_PoisonClearsAfterDeath()
        {
            // Night 1 & Day 1
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Empath,Saint,Widow,Soldier,Fisherman");
            await game.StartGame();

            setup.Agent(Character.Widow).MockWidow(Character.Empath);
            setup.Storyteller.MockWidowPing(Character.Soldier);
            setup.Storyteller.MockGetEmpathNumber(2);
            var receivedEmpathNumber = setup.Agent(Character.Empath).MockNotifyEmpath();

            await game.RunNightAndDay();

            Assert.That(receivedEmpathNumber.Value, Is.EqualTo(2));
            receivedEmpathNumber.Value = -1;

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Widow);

            await game.RunNightAndDay();

            Assert.That(receivedEmpathNumber.Value, Is.EqualTo(0)); // unpoisoned since Widow is dead
        }

        [Test]
        public async Task Widow_SweetheartDrunk()
        {
            // Night 1 & Day 1
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Empath,Sweetheart,Widow,Soldier,Fisherman");
            await game.StartGame();

            setup.Agent(Character.Widow).MockWidow(Character.Empath);
            setup.Storyteller.MockWidowPing(Character.Soldier);
            setup.Storyteller.MockGetEmpathNumber(2);
            var receivedEmpathNumber = setup.Agent(Character.Empath).MockNotifyEmpath();

            await game.RunNightAndDay();

            Assert.That(receivedEmpathNumber.Value, Is.EqualTo(2));
            receivedEmpathNumber.Value = -1;

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Widow);

            await game.RunNightAndDay();

            Assert.That(receivedEmpathNumber.Value, Is.EqualTo(1)); // unpoisoned since Widow is drunk; 1 because Widow is now a living neighbour of the Empath
        }

        [Test]
        public async Task Widow_Poisoned()
        {
            // Night 1 & Day 1
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Empath,Sweetheart,Widow,Soldier,Poisoner");
            await game.StartGame();

            setup.Agent(Character.Poisoner).MockPoisoner(Character.Soldier);
            setup.Agent(Character.Widow).MockWidow(Character.Empath);
            setup.Storyteller.MockWidowPing(Character.Soldier);
            setup.Storyteller.MockGetEmpathNumber(2);
            var receivedEmpathNumber = setup.Agent(Character.Empath).MockNotifyEmpath();

            await game.RunNightAndDay();

            Assert.That(receivedEmpathNumber.Value, Is.EqualTo(2));
            receivedEmpathNumber.Value = -1;

            // Night 2 & Day 2
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Widow);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);

            await game.RunNightAndDay();

            Assert.That(receivedEmpathNumber.Value, Is.EqualTo(0)); // unpoisoned since Widow is poisoned
        }
    }
}