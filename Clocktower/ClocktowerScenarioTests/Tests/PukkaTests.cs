using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class PukkaTests
    {
        [Test]
        public async Task Pukka_LosesWhenExecuted()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Pukka,Fisherman,Ravenkeeper,Saint,Baron,Soldier,Mayor");
            setup.Agent(Character.Pukka).MockPukka(Character.Baron);
            setup.Agent(Character.Pukka).MockNomination(Character.Pukka);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Good));
        }

        [Test]
        public async Task Pukka_PoisonsTarget()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Pukka,Fisherman,Ravenkeeper,Saint,Baron,Soldier,Mayor");
            setup.Agent(Character.Pukka).MockPukka(Character.Saint);
            setup.Agent(Character.Pukka).MockNomination(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.False);
        }

        [Test]
        public async Task Pukka_TargetDiesTheFollowingNight()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Pukka,Empath,Ravenkeeper,Saint,Baron,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Pukka).MockPukka(Character.Empath);
            setup.Storyteller.MockGetEmpathNumber(2);
            var empathNumber = setup.Agent(Character.Empath).MockNotifyEmpath();

            await game.RunNightAndDay();

            Assert.That(empathNumber.Value, Is.EqualTo(2));

            // Night 2 & Day 2
            setup.Agent(Character.Pukka).MockPukka(Character.Baron);

            await game.RunNightAndDay();

            await setup.Agent(Character.Empath).Received().YouAreDead();
            await setup.Agent(Character.Baron).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Pukka_Soldier()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Pukka,Fisherman,Ravenkeeper,Saint,Baron,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Pukka).MockPukka(Character.Soldier);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Pukka).MockPukka(Character.Baron);

            await game.RunNightAndDay();

            await setup.Agent(Character.Soldier).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Baron).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Pukka_ProtectedByMonk()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Pukka,Fisherman,Ravenkeeper,Saint,Baron,Monk,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Pukka).MockPukka(Character.Baron);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Monk).MockMonkChoice(Character.Fisherman);
            setup.Agent(Character.Pukka).MockPukka(Character.Fisherman);

            await game.RunNightAndDay();

            await setup.Agent(Character.Baron).Received().YouAreDead();

            // Night 3 & Day 3
            setup.Agent(Character.Monk).MockMonkChoice(Character.Mayor);
            setup.Agent(Character.Pukka).MockMonkChoice(Character.Monk);

            await game.RunNightAndDay();

            await setup.Agent(Character.Fisherman).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Monk).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Pukka_Ravenkeeper()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Pukka,Fisherman,Ravenkeeper,Saint,Baron,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Pukka).MockPukka(Character.Ravenkeeper);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Pukka).MockPukka(Character.Baron);
            setup.Agent(Character.Ravenkeeper).MockRavenkeeperChoice(Character.Mayor);
            setup.Storyteller.MockGetCharacterForRavenkeeper(Character.Imp);

            await game.RunNightAndDay();

            await setup.Agent(Character.Ravenkeeper).Received().YouAreDead();
            await setup.Agent(Character.Ravenkeeper).Received().NotifyRavenkeeper(Arg.Is<Player>(player => player.RealCharacter == Character.Mayor), Character.Imp);
        }

        [Test]
        public async Task Pukka_Mayor()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Pukka,Fisherman,Ravenkeeper,Saint,Baron,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Pukka).MockPukka(Character.Mayor);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Pukka).MockPukka(Character.Baron);
            setup.Storyteller.MockGetMayorBounce(Character.Fisherman);    // should not trigger

            await game.RunNightAndDay();

            await setup.Agent(Character.Mayor).Received().YouAreDead();
            await setup.Agent(Character.Fisherman).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Pukka_SelfPoisoned()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Pukka,Fisherman,Ravenkeeper,Saint,Baron,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Pukka).MockPukka(Character.Pukka);    // they should remain poisoned forever, as they have no ability to remove the poison token

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Pukka).MockPukka(Character.Baron);

            await game.RunNightAndDay();

            Assert.That(game.Finished, Is.False);
            await setup.Agent(Character.Pukka).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Baron).DidNotReceive().YouAreDead();

            // Night 3 & Day 3
            setup.Agent(Character.Pukka).MockPukka(Character.Baron);

            await game.RunNightAndDay();

            Assert.That(game.Finished, Is.False);
            await setup.Agent(Character.Pukka).DidNotReceive().YouAreDead();
            await setup.Agent(Character.Baron).DidNotReceive().YouAreDead();
        }
    }
}