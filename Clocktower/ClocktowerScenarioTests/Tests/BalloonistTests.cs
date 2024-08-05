using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class BalloonistTests
    {
        [Test]
        public async Task Balloonist_CanStartWithAnyone()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Balloonist,Saint,Baron,Soldier,Fisherman");

            var possiblePlayers = setup.Storyteller.MockGetPlayerForBalloonist(Character.Balloonist);
            var receivedPlayer = setup.Agent(Character.Balloonist).MockNotifyBalloonist(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(possiblePlayers, Is.EquivalentTo(new[] { Character.Imp, Character.Mayor, Character.Balloonist, Character.Saint, Character.Baron, Character.Soldier, Character.Fisherman }));
                Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Balloonist));
            });
        }

        [Test]
        public async Task Balloonist_StartingWithTownsfolk()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Balloonist,Saint,Baron,Soldier,Fisherman");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Fisherman);
            var receivedPlayer = setup.Agent(Character.Balloonist).MockNotifyBalloonist();

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Fisherman));

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            var possiblePlayers = setup.Storyteller.MockGetPlayerForBalloonist(Character.Saint);

            await game.RunNightAndDay();

            Assert.Multiple(() =>
            {
                Assert.That(possiblePlayers, Is.EquivalentTo(new[] { Character.Imp, Character.Saint, Character.Baron }));   // Demons, Minions, Outsiders
                Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Saint));
            });
        }

        [Test]
        public async Task Balloonist_StartingWithOutsider()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Balloonist,Saint,Baron,Soldier,Sweetheart");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Sweetheart);
            var receivedPlayer = setup.Agent(Character.Balloonist).MockNotifyBalloonist();

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Sweetheart));

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            var possiblePlayers = setup.Storyteller.MockGetPlayerForBalloonist(Character.Balloonist);

            await game.RunNightAndDay();

            Assert.Multiple(() =>
            {
                Assert.That(possiblePlayers, Is.EquivalentTo(new[] { Character.Imp, Character.Mayor, Character.Balloonist, Character.Baron, Character.Soldier }));   // Demons, Minions, Townsfolk
                Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Balloonist));
            });
        }

        [Test]
        public async Task Balloonist_StartingWithMinion()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Balloonist,Saint,Baron,Soldier,Scarlet_Woman");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Scarlet_Woman);
            var receivedPlayer = setup.Agent(Character.Balloonist).MockNotifyBalloonist();

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Scarlet_Woman));

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            var possiblePlayers = setup.Storyteller.MockGetPlayerForBalloonist(Character.Balloonist);

            await game.RunNightAndDay();

            Assert.Multiple(() =>
            {
                Assert.That(possiblePlayers, Is.EquivalentTo(new[] { Character.Imp, Character.Mayor, Character.Balloonist, Character.Saint, Character.Soldier }));   // Demons, Outsiders, Townsfolk
                Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Balloonist));
            });
        }

        [Test]
        public async Task Balloonist_StartingWithDemon()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Balloonist,Saint,Baron,Soldier,Ojo");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Ojo);
            var receivedPlayer = setup.Agent(Character.Balloonist).MockNotifyBalloonist();

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Ojo));

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Ojo).MockOjo(Character.Soldier);
            var possiblePlayers = setup.Storyteller.MockGetPlayerForBalloonist(Character.Balloonist);

            await game.RunNightAndDay();

            Assert.Multiple(() =>
            {
                Assert.That(possiblePlayers, Is.EquivalentTo(new[] { Character.Mayor, Character.Balloonist, Character.Saint, Character.Baron, Character.Soldier }));   // Minions, Outsiders, Townsfolk
                Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Balloonist));
            });
        }

        [Test]
        public async Task Balloonist_StartingWithSpy()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Balloonist,Saint,Baron,Soldier,Spy");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Spy);
            var receivedPlayer = setup.Agent(Character.Balloonist).MockNotifyBalloonist();

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Spy));

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            var possiblePlayers = setup.Storyteller.MockGetPlayerForBalloonist(Character.Balloonist);

            await game.RunNightAndDay();

            Assert.Multiple(() =>
            {
                Assert.That(possiblePlayers, Is.EquivalentTo(new[] { Character.Imp, Character.Mayor, Character.Balloonist, Character.Saint, Character.Baron, Character.Soldier }));   // any character type
                Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Balloonist));
            });
        }

        [Test]
        public async Task Balloonist_StartingWithRecluse()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Balloonist,Saint,Baron,Soldier,Recluse");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Recluse);
            var receivedPlayer = setup.Agent(Character.Balloonist).MockNotifyBalloonist();

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Recluse));

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            var possiblePlayers = setup.Storyteller.MockGetPlayerForBalloonist(Character.Balloonist);

            await game.RunNightAndDay();

            Assert.Multiple(() =>
            {
                Assert.That(possiblePlayers, Is.EquivalentTo(new[] { Character.Imp, Character.Mayor, Character.Balloonist, Character.Saint, Character.Baron, Character.Soldier }));   // any character type
                Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Balloonist));
            });
        }

        [Test]
        public async Task Balloonist_SpyCanFollowMinion()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Balloonist,Saint,Baron,Soldier,Spy");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Baron);
            var receivedPlayer = setup.Agent(Character.Balloonist).MockNotifyBalloonist();

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Baron));

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Spy);

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Spy));
        }

        [Test]
        public async Task Balloonist_RecluseCanFollowOutsider()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Balloonist,Saint,Baron,Soldier,Recluse");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Saint);
            var receivedPlayer = setup.Agent(Character.Balloonist).MockNotifyBalloonist();

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Saint));

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Recluse);

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Recluse));
        }

        [Test]
        public async Task Balloonist_IsTheDrunk_TwoTownsfolk()
        {
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Mayor,Balloonist,Saint,Baron,Soldier,Fisherman")
                            .WithDrunk(Character.Balloonist)
                            .Build();
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Fisherman);
            var receivedPlayer = setup.Agent(Character.Balloonist).MockNotifyBalloonist();

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Fisherman));

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Mayor);

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Mayor));
        }

        [Test]
        public async Task Balloonist_IsTheDrunk_TwoOutsiders()
        {
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Mayor,Balloonist,Saint,Baron,Soldier,Sweetheart")
                            .WithDrunk(Character.Balloonist)
                            .Build();
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Sweetheart);
            var receivedPlayer = setup.Agent(Character.Balloonist).MockNotifyBalloonist();

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Sweetheart));

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Saint);

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Saint));
        }

        [Test]
        public async Task Balloonist_IsTheDrunk_TwoMinions()
        {
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Mayor,Balloonist,Saint,Baron,Soldier,Scarlet_Woman")
                            .WithDrunk(Character.Balloonist)
                            .Build();
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Scarlet_Woman);
            var receivedPlayer = setup.Agent(Character.Balloonist).MockNotifyBalloonist();

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Scarlet_Woman));

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Baron);

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Baron));
        }

        [Test]
        public async Task Balloonist_IsTheDrunk_TwoDemons()
        {
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Mayor,Balloonist,Saint,Baron,Soldier,Ojo")
                            .WithDrunk(Character.Balloonist)
                            .Build();
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Ojo);
            var receivedPlayer = setup.Agent(Character.Balloonist).MockNotifyBalloonist();

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Ojo));

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Ojo).MockOjo(Character.Soldier);
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Imp);

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Imp));
        }

        [Test]
        public async Task Balloonist_IsTheMarionette_TwoTownsfolk()
        {
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Balloonist,Mayor,Saint,Baron,Soldier,Fisherman")
                            .WithMarionette(Character.Balloonist)
                            .Build();
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Fisherman);
            var receivedPlayer = setup.Agent(Character.Balloonist).MockNotifyBalloonist();

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Fisherman));

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Mayor);

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Mayor));
        }

        [Test]
        public async Task Balloonist_IsTheMarionette_TwoOutsiders()
        {
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Balloonist,Mayor,Saint,Baron,Soldier,Sweetheart")
                            .WithMarionette(Character.Balloonist)
                            .Build();
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Sweetheart);
            var receivedPlayer = setup.Agent(Character.Balloonist).MockNotifyBalloonist();

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Sweetheart));

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Saint);

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Saint));
        }

        [Test]
        public async Task Balloonist_IsTheMarionette_TwoMinions()
        {
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Balloonist,Mayor,Saint,Baron,Soldier,Scarlet_Woman")
                            .WithMarionette(Character.Balloonist)
                            .Build();
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Scarlet_Woman);
            var receivedPlayer = setup.Agent(Character.Balloonist).MockNotifyBalloonist();

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Scarlet_Woman));

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Baron);

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Baron));
        }

        [Test]
        public async Task Balloonist_IsTheMarionette_TwoDemons()
        {
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Balloonist,Mayor,Saint,Baron,Soldier,Ojo")
                            .WithMarionette(Character.Balloonist)
                            .Build();
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Ojo);
            var receivedPlayer = setup.Agent(Character.Balloonist).MockNotifyBalloonist();

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Ojo));

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Ojo).MockOjo(Character.Soldier);
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Imp);

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Imp));
        }

        [Test]
        public async Task Balloonist_PoisonedNightOne()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Balloonist,Saint,Poisoner,Soldier,Fisherman");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Balloonist);
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Fisherman);
            var receivedPlayer = setup.Agent(Character.Balloonist).MockNotifyBalloonist();

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Fisherman));

            // Night 2 & Day 2
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Poisoner);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            var possiblePlayers = setup.Storyteller.MockGetPlayerForBalloonist(Character.Saint);

            await game.RunNightAndDay();

            Assert.Multiple(() =>
            {
                Assert.That(possiblePlayers, Is.EquivalentTo(new[] { Character.Imp, Character.Saint, Character.Poisoner }));   // Demons, Minions, Outsiders - not Townsfolk as Fisherman seen on night 1
                Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Saint));
            });
        }

        [Test]
        public async Task Balloonist_PoisonedNightTwo()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Balloonist,Saint,Poisoner,Soldier,Fisherman");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Poisoner);
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Fisherman);
            var receivedPlayer = setup.Agent(Character.Balloonist).MockNotifyBalloonist();

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Fisherman));

            // Night 2 & Day 2
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Balloonist);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            var possiblePlayers = setup.Storyteller.MockGetPlayerForBalloonist(Character.Soldier);

            await game.RunNightAndDay();

            Assert.Multiple(() =>
            {
                Assert.That(possiblePlayers, Is.EquivalentTo(new[] { Character.Imp, Character.Mayor, Character.Balloonist, Character.Saint, Character.Poisoner, Character.Soldier }));   // any new player since currently poisoned
                Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Soldier));
            });
        }

        [Test]
        public async Task PhilosopherBalloonist()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Philosopher,Saint,Baron,Soldier,Fisherman");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Balloonist);
            setup.Storyteller.MockGetPlayerForBalloonist(Character.Fisherman);
            var receivedPlayer = setup.Agent(Character.Philosopher).MockNotifyBalloonist();

            await game.RunNightAndDay();

            Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Fisherman));

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            var possiblePlayers = setup.Storyteller.MockGetPlayerForBalloonist(Character.Saint);

            await game.RunNightAndDay();

            Assert.Multiple(() =>
            {
                Assert.That(possiblePlayers, Is.EquivalentTo(new[] { Character.Imp, Character.Saint, Character.Baron }));   // Demons, Minions, Outsiders
                Assert.That(receivedPlayer.Value, Is.EqualTo(Character.Saint));
            });
        }
    }
}