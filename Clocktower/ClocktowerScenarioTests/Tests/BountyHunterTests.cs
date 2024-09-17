using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class BountyHunterTests
    {
        [Test]
        public async Task BountyHunter_EvilTownsfolkRegistersAsEvil()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Bounty_Hunter,Imp,Baron,Saint,Soldier,Empath,Mayor");
            setup.Storyteller.MockGetEvilTownsfolk(Character.Mayor);
            setup.Storyteller.MockGetBountyHunterPing(Character.Baron);
            var empathNumber = setup.Agent(Character.Empath).MockNotifyEmpath(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(empathNumber.Value, Is.EqualTo(1));
        }

        [Test]
        public async Task BountyHunter_SeesEvil()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Bounty_Hunter,Imp,Baron,Saint,Soldier,Fisherman,Mayor");
            setup.Storyteller.MockGetEvilTownsfolk(Character.Mayor);
            var bountyHunterPingOptions = setup.Storyteller.MockGetBountyHunterPing(Character.Baron);
            var bountyHunterPing = setup.Agent(Character.Bounty_Hunter).MockNotifyBountyHunter(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(bountyHunterPingOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Baron, Character.Mayor }));
                Assert.That(bountyHunterPing.Value, Is.EqualTo(Character.Baron));
            });
        }

        [Test]
        public async Task BountyHunter_SeesRecluse()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Bounty_Hunter,Imp,Baron,Recluse,Soldier,Fisherman,Mayor");
            setup.Storyteller.MockGetEvilTownsfolk(Character.Mayor);
            var bountyHunterPingOptions = setup.Storyteller.MockGetBountyHunterPing(Character.Recluse);
            var bountyHunterPing = setup.Agent(Character.Bounty_Hunter).MockNotifyBountyHunter(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(bountyHunterPingOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Baron, Character.Recluse, Character.Mayor }));
                Assert.That(bountyHunterPing.Value, Is.EqualTo(Character.Recluse));
            });
        }

        [Test]
        public async Task BountyHunter_EvilPingDiesFromExecution()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Bounty_Hunter,Imp,Baron,Saint,Soldier,Fisherman,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockGetEvilTownsfolk(Character.Mayor);
            setup.Storyteller.MockGetBountyHunterPing(Character.Baron);
            var bountyHunterPing = setup.Agent(Character.Bounty_Hunter).MockNotifyBountyHunter();
            setup.Agent(Character.Baron).MockNomination(Character.Baron);

            await game.RunNightAndDay();

            Assert.That(bountyHunterPing.Value, Is.EqualTo(Character.Baron));

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Storyteller.MockGetBountyHunterPing(Character.Imp);

            await game.RunNightAndDay();

            Assert.That(bountyHunterPing.Value, Is.EqualTo(Character.Imp));
        }

        [Test]
        public async Task BountyHunter_EvilPingDiesAtNight()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Bounty_Hunter,Imp,Baron,Saint,Soldier,Fisherman,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockGetEvilTownsfolk(Character.Mayor);
            setup.Storyteller.MockGetBountyHunterPing(Character.Baron);
            var bountyHunterPing = setup.Agent(Character.Bounty_Hunter).MockNotifyBountyHunter();

            await game.RunNightAndDay();

            Assert.That(bountyHunterPing.Value, Is.EqualTo(Character.Baron));

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Baron);
            setup.Storyteller.MockGetBountyHunterPing(Character.Imp);

            await game.RunNightAndDay();

            Assert.That(bountyHunterPing.Value, Is.EqualTo(Character.Imp));
        }

        [Test]
        public async Task BountyHunter_IsTheDrunk()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Baron,Bounty_Hunter,Soldier,Fisherman,Mayor,Saint")
                            .WithDrunk(Character.Bounty_Hunter)
                            .Build();
            var bountyHunterPingOptions = setup.Storyteller.MockGetBountyHunterPing(Character.Saint);
            var bountyHunterPing = setup.Agent(Character.Bounty_Hunter).MockNotifyBountyHunter(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(bountyHunterPingOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Baron, Character.Bounty_Hunter, Character.Soldier, Character.Fisherman, Character.Mayor, Character.Saint })); // all
                Assert.That(bountyHunterPing.Value, Is.EqualTo(Character.Saint));
            });
        }

        [Test]
        public async Task BountyHunter_IsTheMarionette()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Bounty_Hunter,Ravenkeeper,Soldier,Fisherman,Mayor,Saint")
                            .WithMarionette(Character.Bounty_Hunter)
                            .Build();
            var bountyHunterPingOptions = setup.Storyteller.MockGetBountyHunterPing(Character.Saint);
            var bountyHunterPing = setup.Agent(Character.Bounty_Hunter).MockNotifyBountyHunter(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(bountyHunterPingOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Bounty_Hunter, Character.Ravenkeeper, Character.Soldier, Character.Fisherman, Character.Mayor, Character.Saint })); // all
                Assert.That(bountyHunterPing.Value, Is.EqualTo(Character.Saint));
            });
        }

        [Test]
        public async Task BountyHunter_Poisoned()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Bounty_Hunter,Imp,Poisoner,Saint,Soldier,Fisherman,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockGetEvilTownsfolk(Character.Mayor);
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Bounty_Hunter);
            setup.Storyteller.MockGetBountyHunterPing(Character.Fisherman);
            var bountyHunterPing = setup.Agent(Character.Bounty_Hunter).MockNotifyBountyHunter();
            setup.Agent(Character.Fisherman).MockNomination(Character.Fisherman);

            await game.RunNightAndDay();

            Assert.That(bountyHunterPing.Value, Is.EqualTo(Character.Fisherman));

            // Night 2 & Day 2
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Poisoner);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            var bountyHunterPingOptions = setup.Storyteller.MockGetBountyHunterPing(Character.Poisoner);

            await game.RunNightAndDay();

            Assert.Multiple(() =>
            {
                Assert.That(bountyHunterPingOptions, Is.EquivalentTo(new[] { Character.Imp, Character.Poisoner, Character.Mayor })); // unpoisoned
                Assert.That(bountyHunterPing.Value, Is.EqualTo(Character.Poisoner));
            });
        }

        [Test]
        public async Task BountyHunter_SweetheartDrunk()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Bounty_Hunter,Imp,Baron,Sweetheart,Soldier,Fisherman,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Storyteller.MockGetEvilTownsfolk(Character.Mayor);
            setup.Storyteller.MockGetBountyHunterPing(Character.Baron);
            var bountyHunterPing = setup.Agent(Character.Bounty_Hunter).MockNotifyBountyHunter();
            setup.Agent(Character.Baron).MockNomination(Character.Baron);

            await game.RunNightAndDay();

            Assert.That(bountyHunterPing.Value, Is.EqualTo(Character.Baron));

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Bounty_Hunter);
            var bountyHunterPingOptions = setup.Storyteller.MockGetBountyHunterPing(Character.Fisherman);

            await game.RunNightAndDay();

            Assert.Multiple(() =>
            {
                Assert.That(bountyHunterPingOptions, Is.EquivalentTo(new[] { Character.Bounty_Hunter, Character.Imp, Character.Baron, Character.Sweetheart, Character.Soldier, Character.Fisherman, Character.Mayor })); // drunk
                Assert.That(bountyHunterPing.Value, Is.EqualTo(Character.Fisherman));
            });
        }
    }
}