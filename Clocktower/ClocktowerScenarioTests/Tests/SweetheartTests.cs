using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class SweetheartTests
    {
        [Test]
        public async Task Sweetheart_ShouldDrunkOnDeath()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Empath,Sweetheart,Baron,Soldier,Fisherman");
            await game.StartGame();

            // Night 1 & Day 1
            var empathNumber = setup.Agent(Character.Empath).MockNotifyEmpath();

            await game.RunNightAndDay();

            Assert.That(empathNumber.Value, Is.EqualTo(0));
            empathNumber.Value = -1;

            // Night 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Empath);
            setup.Storyteller.MockGetEmpathNumber(2);

            await game.RunNightAndDay();

            Assert.That(empathNumber.Value, Is.EqualTo(2));
        }

        [Test]
        public async Task Sweetheart_Poisoned()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Empath,Sweetheart,Poisoner,Soldier,Fisherman");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Soldier);
            var empathNumber = setup.Agent(Character.Empath).MockNotifyEmpath();

            await game.RunNightAndDay();

            Assert.That(empathNumber.Value, Is.EqualTo(0));
            empathNumber.Value = -1;

            // Night 2 & Day 2
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Sweetheart);
            setup.Agent(Character.Imp).MockDemonKill(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Empath);
            setup.Agent(Character.Imp).MockNomination(Character.Poisoner);

            await game.RunNightAndDay();

            Assert.That(empathNumber.Value, Is.EqualTo(1));   // not drunk
            empathNumber.Value = -1;

            // Night 3
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);

            await game.RunNightAndDay();

            Assert.That(empathNumber.Value, Is.EqualTo(0));   // still not drunk, even though Sweetheart is no longer poisoned
        }

        // Other Sweetheart test cases will be in the test classes for the characters that are drunked by the Sweetheart.
    }
}