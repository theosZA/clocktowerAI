using Clocktower.Game;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class PhilosopherTests
    {
        [Test]
        public async Task Philosopher_UnavailableOptions()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Soldier,Ravenkeeper,Saint,Baron,Philosopher,Mayor");
            var philosopherOptions = new List<Character?>();
            setup.Agent(Character.Philosopher).RequestChoiceFromPhilosopher(Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetMatchingOptionFromOptionsArg(Character.Mayor, philosopherOptions));

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(philosopherOptions, Does.Not.Contain(Character.Imp));
                Assert.That(philosopherOptions, Does.Not.Contain(Character.Scarlet_Woman));

                // We don't allow the Philosopher to choose the Drunk since the rules for handling this case are not well defined.
                Assert.That(philosopherOptions, Does.Not.Contain(Character.Drunk));

                Assert.That(philosopherOptions, Does.Not.Contain(Character.Philosopher));
            });
        }

        [Test]
        public async Task Philosopher_Dies()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Empath,Baron,Philosopher,Saint,Ravenkeeper,Soldier,Mayor");
            await game.StartGame();

            setup.Storyteller.MockGetEmpathNumber(0);   // for the Empath it's supposed to be 2 and for the Philo-Empath it's supposed to be 1
            var empathReading = setup.Agent(Character.Empath).MockNotifyEmpath();
            var philoEmpathReading = setup.Agent(Character.Philosopher).MockNotifyEmpath();

            // Night 1 & Day 1
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Empath);
            setup.Agent(Character.Imp).GetNomination(Arg.Any<IReadOnlyCollection<IOption>>()).Returns(args => args.GetOptionForRealCharacterFromArg(Character.Philosopher));

            await game.RunNightAndDay();
            Assert.Multiple(() =>
            {
                Assert.That(empathReading.Value, Is.EqualTo(0));
                Assert.That(philoEmpathReading.Value, Is.EqualTo(1));
            });

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockImp(Character.Soldier);

            await game.RunNightAndDay();

            Assert.That(empathReading.Value, Is.EqualTo(2));    // Empath should now be sober.
        }


        [Test]
        public async Task Philosopher_SweetheartDrunk()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Empath,Baron,Philosopher,Sweetheart,Ravenkeeper,Soldier,Mayor");
            await game.StartGame();

            setup.Storyteller.MockGetEmpathNumber(0);   // for the Empath it's supposed to be 2 and for the Philo-Empath it's supposed to be 1
            var empathReading = setup.Agent(Character.Empath).MockNotifyEmpath();
            var philoEmpathReading = setup.Agent(Character.Philosopher).MockNotifyEmpath();

            // Night 1 & Day 1
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Empath);
            setup.Agent(Character.Imp).MockNomination(Character.Sweetheart);
            setup.Storyteller.GetSweetheartDrunk(Arg.Any<IReadOnlyCollection<IOption>>()).Returns(args => args.GetOptionForRealCharacterFromArg(Character.Philosopher));

            await game.RunNightAndDay();
            Assert.Multiple(() =>
            {
                Assert.That(empathReading.Value, Is.EqualTo(0));
                Assert.That(philoEmpathReading.Value, Is.EqualTo(1));
            });

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockImp(Character.Soldier);

            await game.RunNightAndDay();

            Assert.That(empathReading.Value, Is.EqualTo(2));    // Empath should sober if Philosopher is drunk.
        }

        // Other Philosopher test cases will be in the test classes for the character ability that they choose.
    }
}