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
            setup.Agent(Character.Imp).GetNomination(Arg.Any<Player?>(), Arg.Any<int?>(), Arg.Any<int?>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetOptionForRealCharacterFromArg(Character.Philosopher, argIndex: 3));

            await game.RunNightAndDay();
            Assert.Multiple(() =>
            {
                Assert.That(empathReading.Value, Is.EqualTo(0));
                Assert.That(philoEmpathReading.Value, Is.EqualTo(1));
            });

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);

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
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);

            await game.RunNightAndDay();

            Assert.That(empathReading.Value, Is.EqualTo(2));    // Empath should be sober if Philosopher is drunk.
        }

        [Test]
        public async Task Philosopher_Undrunked()
        {
            // A Philosopher who is drunk on choosing to gain an ability, doesn't really gain that ability, and so as soon as they
            // are undrunk, they will no longer appear to have that ability.

            // This is quite a tricky scenario to set up with the initial characters. The Sweetheart needs to drunk the Philosopher
            // for when they gain their ability. 
            // Then the Poisoner needs to poison the dead Sweetheart on a future night.

            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fisherman,Poisoner,Philosopher,Sweetheart,Ravenkeeper,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Mayor);
            setup.Agent(Character.Philosopher).RequestChoiceFromPhilosopher(Arg.Any<IReadOnlyCollection<IOption>>()).Returns(args => args.GetPassOptionFromArg());
            setup.Agent(Character.Imp).MockNomination(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Philosopher);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Empath); // Note that they now think they have the Empath ability but they do not.
            setup.Storyteller.MockGetEmpathNumber(empathNumber: 2); // It would be 1 if they were the real Empath.
            var empathReading = setup.Agent(Character.Philosopher).MockNotifyEmpath();

            await game.RunNightAndDay();

            Assert.That(empathReading.Value, Is.EqualTo(2));    // Non-real Empath value.

            // Night 3 & Day 3
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Sweetheart); // This poisons the Sweetheart, turning off the Sweetheart's drunking ability.
            setup.Agent(Character.Philosopher).ClearReceivedCalls();

            await game.RunNightAndDay();

            await setup.Agent(Character.Philosopher).DidNotReceive().NotifyEmpath(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<int>()); // And so the Philosopher should not wake tonight.
        }

        [Test]
        public async Task CannibalPhilosopher()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fisherman,Philosopher,Ravenkeeper,Baron,Soldier,Cannibal");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Mayor);
            setup.Agent(Character.Imp).MockNomination(Character.Mayor);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Cannibal).MockPhilosopher(Character.Empath);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            var empathReading = setup.Agent(Character.Cannibal).MockNotifyEmpath(gameToEnd: game);
            setup.Agent(Character.Imp).MockNomination(Character.Soldier);

            await game.RunNightAndDay();

            Assert.That(empathReading.Value, Is.EqualTo(1));
        }

        [Test]
        public async Task CannibalPhilosopher_DrunksExistingCharacter()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fisherman,Philosopher,Empath,Baron,Soldier,Cannibal");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Mayor);
            var realEmpathReading = setup.Agent(Character.Empath).MockNotifyEmpath();
            setup.Agent(Character.Imp).MockNomination(Character.Mayor);

            await game.RunNightAndDay();

            Assert.That(realEmpathReading.Value, Is.EqualTo(1), "Real Empath should get a reading of 1 on night 1");

            // Night 2 & Day 2
            setup.Agent(Character.Cannibal).MockPhilosopher(Character.Empath);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Storyteller.MockGetEmpathNumber(2);   // for the philo-drunk Empath
            var cannibalPhilosopherEmpathReading = setup.Agent(Character.Cannibal).MockNotifyEmpath();
            setup.Agent(Character.Imp).MockNomination(Character.Soldier);

            await game.RunNightAndDay();

            Assert.Multiple(() =>
            {
                Assert.That(realEmpathReading.Value, Is.EqualTo(2), "Real Empath should get a reading of 2 on night 2 (philo-drunk)");
                Assert.That(cannibalPhilosopherEmpathReading.Value, Is.EqualTo(1), "Cannibal-Philosopher-Empath should get a reading of 1 on night 2");
            });

            // Night 3 & Day 3 - since the Soldier was executed, the Cannibal no longer has the Philosopher ability, undrunking the real Empath.
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            cannibalPhilosopherEmpathReading.Value = -1;

            await game.RunNightAndDay();

            Assert.Multiple(() =>
            {
                Assert.That(realEmpathReading.Value, Is.EqualTo(1), "Real Empath should get a reading of 1 on night 3");
                Assert.That(cannibalPhilosopherEmpathReading.Value, Is.EqualTo(-1), "Cannibal should not get a reading on night 3");
            });
        }

        [Test]
        public async Task CannibalPhilosopher_Poisoned()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Fisherman,Mayor,Empath,Baron,Soldier,Cannibal");
            await game.StartGame();

            // Night 1 & Day 1
            var realEmpathReading = setup.Agent(Character.Empath).MockNotifyEmpath();
            setup.Agent(Character.Imp).MockNomination(Character.Baron);
            setup.Storyteller.MockCannibalChoice(Character.Philosopher);

            await game.RunNightAndDay();

            Assert.That(realEmpathReading.Value, Is.EqualTo(1), "Real Empath should get a reading of 1 on night 1");

            // Night 2 & Day 2
            setup.Agent(Character.Cannibal).MockPhilosopher(Character.Empath);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Storyteller.MockGetEmpathNumber(2);   // for the poisoned Cannibal-Philo-Empath
            var cannibalPhilosopherEmpathReading = setup.Agent(Character.Cannibal).MockNotifyEmpath();

            await game.RunNightAndDay();

            Assert.Multiple(() =>
            {
                Assert.That(realEmpathReading.Value, Is.EqualTo(0), "Real Empath should get a reading of 0 on night 2 (Baron is dead)");
                Assert.That(cannibalPhilosopherEmpathReading.Value, Is.EqualTo(2), "Poisoned Cannibal-Philosopher-Empath should get a reading of 2 on night 2");
            });
        }

        [Test]
        public async Task PhilosopherCannibal()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Philosopher,Baron,Saint,Soldier,Empath");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Cannibal);
            setup.Agent(Character.Imp).MockNomination(Character.Empath);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            var receivedEmpathNumber = setup.Agent(Character.Philosopher).MockNotifyEmpath(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedEmpathNumber.Value, Is.EqualTo(1));
        }

        [Test]
        public async Task PhilosopherCannibal_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Philosopher,Baron,Saint,Soldier,Fisherman");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Cannibal);
            setup.Agent(Character.Imp).MockNomination(Character.Baron);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Storyteller.MockCannibalChoice(Character.Empath);
            setup.Storyteller.MockGetEmpathNumber(2);
            var receivedEmpathNumber = setup.Agent(Character.Philosopher).MockNotifyEmpath(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(receivedEmpathNumber.Value, Is.EqualTo(2));
        }

        [Test]
        public async Task PhilosopherCannibal_GainAbilityAfterExecution()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Philosopher,Baron,Saint,Soldier,Empath");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Imp).MockNomination(Character.Empath);

            await game.RunNightAndDay();

            // Night 2
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Cannibal);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            var receivedEmpathNumber = setup.Agent(Character.Philosopher).MockNotifyEmpath(gameToEnd: game);

            await game.RunNightAndDay();

            Assert.That(receivedEmpathNumber.Value, Is.EqualTo(1));
        }

        // Other Philosopher test cases will be in the test classes for the character ability that they choose.
    }
}