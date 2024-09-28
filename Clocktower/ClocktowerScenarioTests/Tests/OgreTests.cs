using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class OgreTests
    {
        [Test]
        public async Task Ogre_PicksGood()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Ogre,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            setup.Agent(Character.Ogre).MockOgreChoice(Character.Saint);
            setup.Agent(Character.Imp).MockNomination(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.AnnounceWinner();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
            await setup.Agent(Character.Ogre).Observer.Received().AnnounceWinner(Alignment.Evil,
                                                                                 Arg.Is<IReadOnlyCollection<Player>>(players => !players.Any(player => player.RealCharacter == Character.Ogre)),
                                                                                 Arg.Is<IReadOnlyCollection<Player>>(players => players.Any(player => player.RealCharacter == Character.Ogre)));
            await setup.Agent(Character.Ogre).DidNotReceive().ChangeAlignment(Arg.Any<Alignment>());
        }

        [Test]
        public async Task Ogre_PicksEvil()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Ogre,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            setup.Agent(Character.Ogre).MockOgreChoice(Character.Baron);
            setup.Agent(Character.Imp).MockNomination(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.AnnounceWinner();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
            await setup.Agent(Character.Ogre).Observer.Received().AnnounceWinner(Alignment.Evil,
                                                                                 Arg.Is<IReadOnlyCollection<Player>>(players => players.Any(player => player.RealCharacter == Character.Ogre)),
                                                                                 Arg.Is<IReadOnlyCollection<Player>>(players => !players.Any(player => player.RealCharacter == Character.Ogre)));
            await setup.Agent(Character.Ogre).DidNotReceive().ChangeAlignment(Arg.Any<Alignment>());
        }

        [Test]
        public async Task Ogre_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Ogre,Ravenkeeper,Saint,Poisoner,Fisherman,Mayor");
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Ogre);
            setup.Agent(Character.Ogre).MockOgreChoice(Character.Poisoner);
            setup.Agent(Character.Imp).MockNomination(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.AnnounceWinner();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
            await setup.Agent(Character.Ogre).Observer.Received().AnnounceWinner(Alignment.Evil,
                                                                                 Arg.Is<IReadOnlyCollection<Player>>(players => players.Any(player => player.RealCharacter == Character.Ogre)),
                                                                                 Arg.Is<IReadOnlyCollection<Player>>(players => !players.Any(player => player.RealCharacter == Character.Ogre)));
            await setup.Agent(Character.Ogre).DidNotReceive().ChangeAlignment(Arg.Any<Alignment>());
        }
    }
}