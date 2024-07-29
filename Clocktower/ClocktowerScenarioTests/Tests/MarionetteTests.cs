using Clocktower.Game;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class MarionetteTests
    {
        [Test]
        public async Task Marionette_MustNeighbourDemon()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            setup.WithDefaultAgents()
                 .WithCharacters("Imp,Slayer,Ravenkeeper,Soldier,Fisherman,Mayor,Saint");
            setup.Setup.IsCharacterSelected(Character.Marionette).Returns(true);
            var marionetteCandidates = new List<Character>();
            setup.Storyteller.GetMarionette(Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetMatchingOptionFromOptionsArg(Character.Slayer, marionetteCandidates));
            var game = setup.Build();

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(marionetteCandidates, Is.EquivalentTo(new[] { Character.Slayer, Character.Saint }));
        }

        [Test]
        public async Task Marionette_CanNeighbourRecluse()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            setup.WithDefaultAgents()
                 .WithCharacters("Imp,Slayer,Ravenkeeper,Soldier,Recluse,Mayor,Saint");
            setup.Setup.IsCharacterSelected(Character.Marionette).Returns(true);
            var marionetteCandidates = new List<Character>();
            setup.Storyteller.GetMarionette(Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetMatchingOptionFromOptionsArg(Character.Slayer, marionetteCandidates));
            var game = setup.Build();

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(marionetteCandidates, Is.EquivalentTo(new[] { Character.Slayer, Character.Saint, Character.Soldier, Character.Mayor }));
        }

        [Test]
        public async Task Marionette_MustReplaceGood()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            setup.WithDefaultAgents()
                 .WithCharacters("Imp,Slayer,Ravenkeeper,Soldier,Fisherman,Mayor,Baron");
            setup.Setup.IsCharacterSelected(Character.Marionette).Returns(true);
            var game = setup.Build();

            var seenMinions = new List<Character>();
            setup.Agent(Character.Imp).When(agent => agent.DemonInformation(Arg.Any<IReadOnlyCollection<Player>>(), Arg.Any<IReadOnlyCollection<Character>>()))
                .Do(args =>
                {
                    seenMinions.AddRange(args.ArgAt<IReadOnlyCollection<Player>>(0).Select(player => player.Character));
                });

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(seenMinions, Is.EquivalentTo(new[] { Character.Baron, Character.Slayer }));
        }

        [Test]
        public async Task Marionette_ShouldNotKnowTheyAreTheMarionette()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Slayer,Ravenkeeper,Soldier,Baron,Mayor,Saint")
                            .WithMarionette(Character.Slayer)
                            .Build();

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Slayer).Received().AssignCharacter(Character.Slayer, Alignment.Good);
            await setup.Agent(Character.Slayer).DidNotReceive().AssignCharacter(Character.Marionette, Arg.Any<Alignment>());
            await setup.Agent(Character.Slayer).DidNotReceive().MinionInformation(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<Player>>(), Arg.Any<IReadOnlyCollection<Character>>());
        }

        [Test]
        public async Task Marionette_ShouldOnlyBeSeenByDemon()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Slayer,Ravenkeeper,Scarlet_Woman,Baron,Mayor,Saint")
                            .WithMarionette(Character.Slayer)
                            .Build();

            var seenMinions = new List<Character>();
            setup.Agent(Character.Imp).When(agent => agent.DemonInformation(Arg.Any<IReadOnlyCollection<Player>>(), Arg.Any<IReadOnlyCollection<Character>>()))
                .Do(args =>
                {
                    seenMinions.AddRange(args.ArgAt<IReadOnlyCollection<Player>>(0).Select(player => player.Character));
                });
            var fellowMinions = new List<Character>();
            setup.Agent(Character.Baron).When(agent => agent.MinionInformation(Arg.Is<Player>(player => player.Character == Character.Imp), Arg.Any<IReadOnlyCollection<Player>>(), Arg.Any<IReadOnlyCollection<Character>>()))
                .Do(args =>
                {
                    fellowMinions.AddRange(args.ArgAt<IReadOnlyCollection<Player>>(1).Select(player => player.Character));
                });

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(seenMinions, Is.EquivalentTo(new[] { Character.Scarlet_Woman, Character.Baron, Character.Slayer }));    // Demon should see all minions.
            Assert.That(fellowMinions, Is.EquivalentTo(new[] { Character.Scarlet_Woman }));  // Baron should see the Scarlet Woman but NOT the Marionette
        }

        [Test]
        public async Task Marionette_Snitch_Jinx_DemonGetsSixBluffs()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Slayer,Ravenkeeper,Soldier,Baron,Mayor,Snitch")
                            .WithMarionette(Character.Slayer)
                            .Build();
            setup.Storyteller.GetMinionBluffs(Arg.Is<Player>(minion => minion.Character == Character.Baron), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetMatchingOptionFromOptionsArg((Character.Chef, Character.Butler, Character.Monk), argIndex: 1));
            setup.Storyteller.GetDemonBluffs(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>()).
                Returns(args => args.GetMatchingOptionFromOptionsArg((Character.Chef, Character.Butler, Character.Monk), argIndex: 1));
            setup.Storyteller.GetAdditionalDemonBluffs(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>()).
                Returns(args => args.GetMatchingOptionFromOptionsArg((Character.Librarian, Character.Washerwoman, Character.Investigator), argIndex: 2));
            var bluffs = new List<Character>();
            setup.Agent(Character.Imp).When(agent => agent.DemonInformation(Arg.Any<IReadOnlyCollection<Player>>(), Arg.Any<IReadOnlyCollection<Character>>()))
                .Do(args =>
                {
                    bluffs.AddRange(args.ArgAt<IReadOnlyCollection<Character>>(1));
                });

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(bluffs, Is.EquivalentTo(new[] { Character.Chef, Character.Butler, Character.Monk, Character.Librarian, Character.Washerwoman, Character.Investigator }));
        }
    }
}