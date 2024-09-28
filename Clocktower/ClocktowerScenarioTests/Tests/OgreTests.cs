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
            var ogreOptions = setup.Agent(Character.Ogre).MockOgreChoice(Character.Saint);
            setup.Agent(Character.Imp).MockNomination(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.AnnounceWinner();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
            Assert.That(ogreOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Ravenkeeper, Character.Saint, Character.Baron, Character.Fisherman, Character.Mayor }));  // Can't pick themself.
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
            var ogreOptions = setup.Agent(Character.Ogre).MockOgreChoice(Character.Baron);
            setup.Agent(Character.Imp).MockNomination(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.AnnounceWinner();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
            Assert.That(ogreOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Ravenkeeper, Character.Saint, Character.Baron, Character.Fisherman, Character.Mayor }));  // Can't pick themself.
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

        [Test]
        public async Task PhilosopherOgre_NightOne()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Philosopher,Ravenkeeper,Saint,Baron,Fisherman,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Ogre);
            setup.Agent(Character.Philosopher).MockOgreChoice(Character.Baron);
            setup.Agent(Character.Imp).MockNomination(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.AnnounceWinner();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
            await setup.Agent(Character.Philosopher).Observer.Received().AnnounceWinner(Alignment.Evil,
                                                                                        Arg.Is<IReadOnlyCollection<Player>>(players => players.Any(player => player.RealCharacter == Character.Philosopher)),
                                                                                        Arg.Is<IReadOnlyCollection<Player>>(players => !players.Any(player => player.RealCharacter == Character.Philosopher)));
            await setup.Agent(Character.Philosopher).DidNotReceive().ChangeAlignment(Arg.Any<Alignment>());
        }

        [Test]
        public async Task PhilosopherOgre_NightTwo()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Philosopher,Ravenkeeper,Saint,Baron,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Ogre);
            setup.Agent(Character.Philosopher).MockOgreChoice(Character.Baron);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Imp).MockNomination(Character.Saint);

            await game.RunNightAndDay();
            await game.AnnounceWinner();

            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
            await setup.Agent(Character.Philosopher).Observer.Received().AnnounceWinner(Alignment.Evil,
                                                                                        Arg.Is<IReadOnlyCollection<Player>>(players => players.Any(player => player.RealCharacter == Character.Philosopher)),
                                                                                        Arg.Is<IReadOnlyCollection<Player>>(players => !players.Any(player => player.RealCharacter == Character.Philosopher)));
            await setup.Agent(Character.Philosopher).DidNotReceive().ChangeAlignment(Arg.Any<Alignment>());
        }

        [Test]
        public async Task Ogre_PhilosopherDrunk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Ogre,Philosopher,Saint,Baron,Fisherman,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Ogre);
            setup.Agent(Character.Philosopher).MockOgreChoice(Character.Baron);
            setup.Agent(Character.Ogre).MockOgreChoice(Character.Baron);
            setup.Agent(Character.Imp).MockNomination(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.AnnounceWinner();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
            // Both Philosopher-Ogre and Philosopher-drunked Ogre should successfully trigger their alignment swap.
            await setup.Agent(Character.Ogre).Observer.Received().AnnounceWinner(Alignment.Evil,
                                                                                 Arg.Is<IReadOnlyCollection<Player>>(players => players.Any(player => player.RealCharacter == Character.Ogre)),
                                                                                 Arg.Is<IReadOnlyCollection<Player>>(players => !players.Any(player => player.RealCharacter == Character.Ogre)));
            await setup.Agent(Character.Philosopher).Observer.Received().AnnounceWinner(Alignment.Evil,
                                                                                        Arg.Is<IReadOnlyCollection<Player>>(players => players.Any(player => player.RealCharacter == Character.Philosopher)),
                                                                                        Arg.Is<IReadOnlyCollection<Player>>(players => !players.Any(player => player.RealCharacter == Character.Philosopher)));
            await setup.Agent(Character.Ogre).DidNotReceive().ChangeAlignment(Arg.Any<Alignment>());
            await setup.Agent(Character.Philosopher).DidNotReceive().ChangeAlignment(Arg.Any<Alignment>());
        }

        [Test]
        public async Task CannibalOgre_GoodOgre()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Ogre,Cannibal,Saint,Baron,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Ogre).MockOgreChoice(Character.Cannibal);
            setup.Agent(Character.Imp).MockNomination(Character.Ogre);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Cannibal).MockOgreChoice(Character.Baron);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Imp).MockNomination(Character.Saint);

            await game.RunNightAndDay();
            await game.AnnounceWinner();

            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
            // The original Ogre picked good, and the Cannibal-Ogre picked evil.
            await setup.Agent(Character.Ogre).Observer.Received().AnnounceWinner(Alignment.Evil,
                                                                                 Arg.Is<IReadOnlyCollection<Player>>(players => !players.Any(player => player.RealCharacter == Character.Ogre)),
                                                                                 Arg.Is<IReadOnlyCollection<Player>>(players => players.Any(player => player.RealCharacter == Character.Ogre)));
            await setup.Agent(Character.Cannibal).Observer.Received().AnnounceWinner(Alignment.Evil,
                                                                                     Arg.Is<IReadOnlyCollection<Player>>(players => players.Any(player => player.RealCharacter == Character.Cannibal)),
                                                                                     Arg.Is<IReadOnlyCollection<Player>>(players => !players.Any(player => player.RealCharacter == Character.Cannibal)));
            await setup.Agent(Character.Ogre).DidNotReceive().ChangeAlignment(Arg.Any<Alignment>());
            await setup.Agent(Character.Cannibal).DidNotReceive().ChangeAlignment(Arg.Any<Alignment>());
        }

        [Test]
        public async Task CannibalOgre_EvilOgre()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Ogre,Cannibal,Saint,Baron,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Ogre).MockOgreChoice(Character.Baron);
            setup.Agent(Character.Imp).MockNomination(Character.Ogre);
            setup.Storyteller.MockCannibalChoice(Character.Ogre);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Cannibal).MockOgreChoice(Character.Baron);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Imp).MockNomination(Character.Saint);

            await game.RunNightAndDay();
            await game.AnnounceWinner();

            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
            // The original Ogre picked evil, and so the Cannibal never gained the Ogre ability and so remains good.
            await setup.Agent(Character.Ogre).Observer.Received().AnnounceWinner(Alignment.Evil,
                                                                                 Arg.Is<IReadOnlyCollection<Player>>(players => players.Any(player => player.RealCharacter == Character.Ogre)),
                                                                                 Arg.Is<IReadOnlyCollection<Player>>(players => !players.Any(player => player.RealCharacter == Character.Ogre)));
            await setup.Agent(Character.Cannibal).Observer.Received().AnnounceWinner(Alignment.Evil,
                                                                                     Arg.Is<IReadOnlyCollection<Player>>(players => !players.Any(player => player.RealCharacter == Character.Cannibal)),
                                                                                     Arg.Is<IReadOnlyCollection<Player>>(players => players.Any(player => player.RealCharacter == Character.Cannibal)));
            await setup.Agent(Character.Ogre).DidNotReceive().ChangeAlignment(Arg.Any<Alignment>());
            await setup.Agent(Character.Cannibal).DidNotReceive().ChangeAlignment(Arg.Any<Alignment>());
        }

        [Test]
        public async Task CannibalOgre_FromMinion()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Cannibal,Ravenkeeper,Saint,Baron,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Imp).MockNomination(Character.Baron);
            setup.Storyteller.MockCannibalChoice(Character.Ogre);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Cannibal).MockOgreChoice(Character.Imp);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Imp).MockNomination(Character.Saint);

            await game.RunNightAndDay();
            await game.AnnounceWinner();

            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
            await setup.Agent(Character.Cannibal).Observer.Received().AnnounceWinner(Alignment.Evil,
                                                                                     Arg.Is<IReadOnlyCollection<Player>>(players => !players.Any(player => player.RealCharacter == Character.Cannibal)),
                                                                                     Arg.Is<IReadOnlyCollection<Player>>(players => players.Any(player => player.RealCharacter == Character.Cannibal)));
            await setup.Agent(Character.Cannibal).DidNotReceive().ChangeAlignment(Arg.Any<Alignment>());
        }

        [Test]
        public async Task DrunkPhilosopherOgre()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Philosopher,Ravenkeeper,Saint,Baron,Fisherman,Mayor")
                            .WithDrunk(Character.Philosopher)
                            .Build();
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Ogre);
            setup.Agent(Character.Philosopher).MockOgreChoice(Character.Baron);
            setup.Agent(Character.Imp).MockNomination(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.AnnounceWinner();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
            // Loses as good, since the Philosopher was really the Drunk and never gained the Ogre ability.
            await setup.Agent(Character.Philosopher).Observer.Received().AnnounceWinner(Alignment.Evil,
                                                                                        Arg.Is<IReadOnlyCollection<Player>>(players => !players.Any(player => player.RealCharacter == Character.Drunk)),
                                                                                        Arg.Is<IReadOnlyCollection<Player>>(players => players.Any(player => player.RealCharacter == Character.Drunk)));
            await setup.Agent(Character.Philosopher).DidNotReceive().ChangeAlignment(Arg.Any<Alignment>());
        }

        [Test]
        public async Task DrunkCannibalOgre()
        {
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Ogre,Cannibal,Saint,Baron,Soldier,Mayor")
                            .WithDrunk(Character.Cannibal)
                            .Build();
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Ogre).MockOgreChoice(Character.Saint);
            setup.Agent(Character.Imp).MockNomination(Character.Ogre);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Cannibal).MockOgreChoice(Character.Baron);
            setup.Agent(Character.Imp).MockNomination(Character.Saint);

            await game.RunNightAndDay();
            await game.AnnounceWinner();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
            // Loses as good, since the Cannibal was really the Drunk and never gained the Ogre ability.
            await setup.Agent(Character.Cannibal).Observer.Received().AnnounceWinner(Alignment.Evil,
                                                                                     Arg.Is<IReadOnlyCollection<Player>>(players => !players.Any(player => player.RealCharacter == Character.Drunk)),
                                                                                     Arg.Is<IReadOnlyCollection<Player>>(players => players.Any(player => player.RealCharacter == Character.Drunk)));
            await setup.Agent(Character.Cannibal).DidNotReceive().ChangeAlignment(Arg.Any<Alignment>());
        }

        [Test]
        public async Task MarionetteOgre()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Ogre,Ravenkeeper,Saint,Baron,Soldier,Mayor")
                            .WithMarionette(Character.Ogre)
                            .Build();
            setup.Agent(Character.Ogre).MockOgreChoice(Character.Saint);
            setup.Agent(Character.Imp).MockNomination(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.AnnounceWinner();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
            // Wins as evil, since they never had the Ogre ability.
            await setup.Agent(Character.Ogre).Observer.Received().AnnounceWinner(Alignment.Evil,
                                                                                 Arg.Is<IReadOnlyCollection<Player>>(players => players.Any(player => player.RealCharacter == Character.Marionette)),
                                                                                 Arg.Is<IReadOnlyCollection<Player>>(players => !players.Any(player => player.RealCharacter == Character.Marionette)));
            await setup.Agent(Character.Ogre).DidNotReceive().ChangeAlignment(Arg.Any<Alignment>());
        }

        [Test]
        public async Task MarionettePhilosopherOgre()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Philosopher,Ravenkeeper,Saint,Baron,Fisherman,Mayor")
                            .WithMarionette(Character.Philosopher)
                            .Build();
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Ogre);
            setup.Agent(Character.Philosopher).MockOgreChoice(Character.Saint);
            setup.Agent(Character.Imp).MockNomination(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.AnnounceWinner();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
            // Wins as evil, since the Philosopher was really the Marionette and never gained the Ogre ability.
            await setup.Agent(Character.Philosopher).Observer.Received().AnnounceWinner(Alignment.Evil,
                                                                                        Arg.Is<IReadOnlyCollection<Player>>(players => players.Any(player => player.RealCharacter == Character.Marionette)),
                                                                                        Arg.Is<IReadOnlyCollection<Player>>(players => !players.Any(player => player.RealCharacter == Character.Marionette)));
            await setup.Agent(Character.Philosopher).DidNotReceive().ChangeAlignment(Arg.Any<Alignment>());
        }

        [Test]
        public async Task MarionetteCannibalOgre()
        {
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Cannibal,Ogre,Saint,Baron,Soldier,Mayor")
                            .WithMarionette(Character.Cannibal)
                            .Build();
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Ogre).MockOgreChoice(Character.Saint);
            setup.Agent(Character.Imp).MockNomination(Character.Ogre);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Cannibal).MockOgreChoice(Character.Saint);
            setup.Agent(Character.Imp).MockNomination(Character.Saint);

            await game.RunNightAndDay();
            await game.AnnounceWinner();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
            // Wins as evil, since the Cannibal was really the Marionette and never gained the Ogre ability.
            await setup.Agent(Character.Cannibal).Observer.Received().AnnounceWinner(Alignment.Evil,
                                                                                     Arg.Is<IReadOnlyCollection<Player>>(players => players.Any(player => player.RealCharacter == Character.Marionette)),
                                                                                     Arg.Is<IReadOnlyCollection<Player>>(players => !players.Any(player => player.RealCharacter == Character.Marionette)));
            await setup.Agent(Character.Cannibal).DidNotReceive().ChangeAlignment(Arg.Any<Alignment>());
        }

        [Test]
        public async Task EvilPhilosopherOgre_PicksGood()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Philosopher,Bounty_Hunter,Saint,Baron,Fisherman,Mayor");
            setup.Storyteller.MockGetEvilTownsfolk(Character.Philosopher);
            setup.Storyteller.MockGetBountyHunterPing(Character.Imp);
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Ogre);
            setup.Agent(Character.Philosopher).MockOgreChoice(Character.Saint);
            setup.Agent(Character.Imp).MockNomination(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.AnnounceWinner();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
            await setup.Agent(Character.Philosopher).Observer.Received().AnnounceWinner(Alignment.Evil,
                                                                                        Arg.Is<IReadOnlyCollection<Player>>(players => !players.Any(player => player.RealCharacter == Character.Philosopher)),
                                                                                        Arg.Is<IReadOnlyCollection<Player>>(players => players.Any(player => player.RealCharacter == Character.Philosopher)));
            await setup.Agent(Character.Philosopher).Received().ChangeAlignment(Alignment.Evil);
        }

        [Test]
        public async Task EvilPhilosopherOgre_PicksEvil()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Philosopher,Bounty_Hunter,Saint,Baron,Fisherman,Mayor");
            setup.Storyteller.MockGetEvilTownsfolk(Character.Philosopher);
            setup.Storyteller.MockGetBountyHunterPing(Character.Imp);
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Ogre);
            setup.Agent(Character.Philosopher).MockOgreChoice(Character.Baron);
            setup.Agent(Character.Imp).MockNomination(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.AnnounceWinner();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
            await setup.Agent(Character.Philosopher).Observer.Received().AnnounceWinner(Alignment.Evil,
                                                                                        Arg.Is<IReadOnlyCollection<Player>>(players => players.Any(player => player.RealCharacter == Character.Philosopher)),
                                                                                        Arg.Is<IReadOnlyCollection<Player>>(players => !players.Any(player => player.RealCharacter == Character.Philosopher)));
            await setup.Agent(Character.Philosopher).Received().ChangeAlignment(Alignment.Evil);
        }

        [Test]
        public async Task EvilCannibalOgre_PicksGood()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Ogre,Cannibal,Saint,Baron,Soldier,Bounty_Hunter");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockGetEvilTownsfolk(Character.Cannibal);
            setup.Storyteller.MockGetBountyHunterPing(Character.Imp);
            setup.Agent(Character.Ogre).MockOgreChoice(Character.Saint);
            setup.Agent(Character.Imp).MockNomination(Character.Ogre);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Cannibal).MockOgreChoice(Character.Saint);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Imp).MockNomination(Character.Saint);

            await game.RunNightAndDay();
            await game.AnnounceWinner();

            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
            await setup.Agent(Character.Cannibal).Observer.Received().AnnounceWinner(Alignment.Evil,
                                                                                     Arg.Is<IReadOnlyCollection<Player>>(players => !players.Any(player => player.RealCharacter == Character.Cannibal)),
                                                                                     Arg.Is<IReadOnlyCollection<Player>>(players => players.Any(player => player.RealCharacter == Character.Cannibal)));
            await setup.Agent(Character.Cannibal).Received().ChangeAlignment(Alignment.Evil);
        }

        [Test]
        public async Task EvilCannibalOgre_PicksEvil()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Ogre,Cannibal,Saint,Baron,Soldier,Bounty_Hunter");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockGetEvilTownsfolk(Character.Cannibal);
            setup.Storyteller.MockGetBountyHunterPing(Character.Imp);
            setup.Agent(Character.Ogre).MockOgreChoice(Character.Saint);
            setup.Agent(Character.Imp).MockNomination(Character.Ogre);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Cannibal).MockOgreChoice(Character.Baron);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Imp).MockNomination(Character.Saint);

            await game.RunNightAndDay();
            await game.AnnounceWinner();

            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Evil));
            await setup.Agent(Character.Cannibal).Observer.Received().AnnounceWinner(Alignment.Evil,
                                                                                     Arg.Is<IReadOnlyCollection<Player>>(players => players.Any(player => player.RealCharacter == Character.Cannibal)),
                                                                                     Arg.Is<IReadOnlyCollection<Player>>(players => !players.Any(player => player.RealCharacter == Character.Cannibal)));
            await setup.Agent(Character.Cannibal).Received().ChangeAlignment(Alignment.Evil);
        }

        [Test]
        public async Task Ogre_PicksSpy()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Ogre,Ravenkeeper,Saint,Spy,Fisherman,Mayor");
            setup.Agent(Character.Ogre).MockOgreChoice(Character.Spy);
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