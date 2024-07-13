using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class PoisonerTests
    {
        [Test]
        public async Task Poisoner_RemovePoisonOnDeath()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Empath,Saint,Poisoner,Soldier,Fisherman");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Empath);
            setup.Storyteller.MockGetEmpathNumbers(2);
            var firstEmpathNumber = setup.Agent(Character.Empath).MockNotifyEmpath();

            await game.RunNightAndDay();

            Assert.That(firstEmpathNumber.Value, Is.EqualTo(2)); // poisoned

            // Night 2 & Day 2
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Empath);
            setup.Agent(Character.Imp).MockImp(Character.Poisoner);
            var secondEmpathNumber = setup.Agent(Character.Empath).MockNotifyEmpath();

            await game.RunNightAndDay();

            Assert.That(secondEmpathNumber.Value, Is.EqualTo(0)); // unpoisoned since Poisoner is dead
        }

        // Other Poisoner test cases will be in the test classes for the characters that are poisoned.
    }
}