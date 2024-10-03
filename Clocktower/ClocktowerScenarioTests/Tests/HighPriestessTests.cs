using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class HighPriestessTests
    {
        [Test]
        public async Task HighPriestess_LearnsAnyPlayerEachNight()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,High_Priestess,Saint,Baron,Soldier,Fisherman");
            await game.StartGame();

            // Night 1 & Day 1
            var night1Options = setup.Storyteller.MockGetPlayerForHighPriestess(Character.Imp);
            var receivedPlayer = setup.Agent(Character.High_Priestess).MockNotifyHighPriestess();

            await game.RunNightAndDay();

            Assert.Multiple(() =>
            {
                Assert.That(night1Options, Is.EquivalentTo(new[] { Character.Imp, Character.Mayor, Character.High_Priestess, Character.Saint, Character.Baron, Character.Soldier, Character.Fisherman }));
                Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Imp));
            });

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            var night2Options = setup.Storyteller.MockGetPlayerForHighPriestess(Character.Mayor);

            await game.RunNightAndDay();

            Assert.Multiple(() =>
            {
                Assert.That(night2Options, Is.EquivalentTo(night1Options));
                Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Mayor));
            });
        }
    }
}