using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class RavenkeeperTests
    {
        [Test]
        public async Task Ravenkeeper_KilledByImp()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Saint,Ravenkeeper,Soldier,Undertaker,Mayor");
            setup.Agent(Character.Imp).MockImp(Character.Ravenkeeper);
            var ravenkeeperOptions = setup.Agent(Character.Ravenkeeper).MockRavenkeeperChoice(Character.Mayor);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(ravenkeeperOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Baron, Character.Saint, Character.Ravenkeeper, Character.Soldier, Character.Undertaker, Character.Mayor }));
            await setup.Agent(Character.Ravenkeeper).Received().YouAreDead();
            await setup.Agent(Character.Ravenkeeper).Received().NotifyRavenkeeper(Arg.Is<Player>(player => player.Character == Character.Mayor), Character.Mayor);
        }

        [Test]
        public async Task Ravenkeeper_KilledByAssassin()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Assassin,Saint,Ravenkeeper,Soldier,Undertaker,Mayor");
            setup.Agent(Character.Imp).MockImp(Character.Soldier);
            setup.Agent(Character.Assassin).MockAssassin(Character.Ravenkeeper);
            var ravenkeeperOptions = setup.Agent(Character.Ravenkeeper).MockRavenkeeperChoice(Character.Mayor);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(ravenkeeperOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Assassin, Character.Saint, Character.Ravenkeeper, Character.Soldier, Character.Undertaker, Character.Mayor }));
            await setup.Agent(Character.Ravenkeeper).Received().YouAreDead();
            await setup.Agent(Character.Ravenkeeper).Received().NotifyRavenkeeper(Arg.Is<Player>(player => player.Character == Character.Mayor), Character.Mayor);
        }

        [Test]
        public async Task Ravenkeeper_AlreadyDead()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Saint,Ravenkeeper,Soldier,Undertaker,Mayor");
            setup.Agent(Character.Imp).MockNomination(Character.Ravenkeeper);
            setup.Agent(Character.Imp).MockImp(Character.Ravenkeeper);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Ravenkeeper).DidNotReceive().RequestChoiceFromRavenkeeper(Arg.Any<IReadOnlyCollection<IOption>>());
            await setup.Agent(Character.Ravenkeeper).DidNotReceive().NotifyRavenkeeper(Arg.Any<Player>(), Arg.Any<Character>());
        }

        [Test]
        public async Task Ravenkeeper_SeesRecluse()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Recluse,Ravenkeeper,Soldier,Undertaker,Mayor");
            setup.Agent(Character.Imp).MockImp(Character.Ravenkeeper);
            var ravenkeeperOptions = setup.Agent(Character.Ravenkeeper).MockRavenkeeperChoice(Character.Recluse);
            setup.Storyteller.MockGetCharacterForRavenkeeper(Character.Assassin);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(ravenkeeperOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Baron, Character.Recluse, Character.Ravenkeeper, Character.Soldier, Character.Undertaker, Character.Mayor }));
            await setup.Agent(Character.Ravenkeeper).Received().YouAreDead();
            await setup.Agent(Character.Ravenkeeper).Received().NotifyRavenkeeper(Arg.Is<Player>(player => player.Character == Character.Recluse), Character.Assassin);
        }

        [Test]
        public async Task Ravenkeeper_SeesSpy()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Spy,Saint,Ravenkeeper,Soldier,Undertaker,Mayor");
            setup.Agent(Character.Imp).MockImp(Character.Ravenkeeper);
            var ravenkeeperOptions = setup.Agent(Character.Ravenkeeper).MockRavenkeeperChoice(Character.Spy);
            setup.Storyteller.MockGetCharacterForRavenkeeper(Character.Butler);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(ravenkeeperOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Spy, Character.Saint, Character.Ravenkeeper, Character.Soldier, Character.Undertaker, Character.Mayor }));
            await setup.Agent(Character.Ravenkeeper).Received().YouAreDead();
            await setup.Agent(Character.Ravenkeeper).Received().NotifyRavenkeeper(Arg.Is<Player>(player => player.Character == Character.Spy), Character.Butler);
        }
    }
}