using Clocktower.Game;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class SnitchTests
    {
        [Test]
        public async Task NoSnitch_NoMinionBluffs()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Ravenkeeper,Saint,Soldier,Slayer,Mayor");
            var minionBluffs = new List<Character>();
            setup.Agent(Character.Baron).When(agent => agent.MinionInformation(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<Player>>(), Arg.Any<IReadOnlyCollection<Character>>()))
                .Do(args => minionBluffs.AddRange(args.ArgAt<IReadOnlyCollection<Character>>(2)));

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(minionBluffs, Is.Empty);
        }

        [Test]
        public async Task Snitch_MinionBluffs()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Ravenkeeper,Snitch,Soldier,Slayer,Scarlet_Woman");
            var baronBluffs = new List<Character>();
            var scarletWomanBluffs = new List<Character>();
            setup.Agent(Character.Baron).When(agent => agent.MinionInformation(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<Player>>(), Arg.Any<IReadOnlyCollection<Character>>()))
                .Do(args => baronBluffs.AddRange(args.ArgAt<IReadOnlyCollection<Character>>(2)));
            setup.Agent(Character.Scarlet_Woman).When(agent => agent.MinionInformation(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<Player>>(), Arg.Any<IReadOnlyCollection<Character>>()))
                .Do(args => scarletWomanBluffs.AddRange(args.ArgAt<IReadOnlyCollection<Character>>(2)));
            setup.Storyteller.GetMinionBluffs(Arg.Is<Player>(minion => minion.Character == Character.Baron), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetMatchingOptionFromOptionsArg((Character.Chef, Character.Butler, Character.Monk), argIndex: 1));
            setup.Storyteller.GetMinionBluffs(Arg.Is<Player>(minion => minion.Character == Character.Scarlet_Woman), Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetMatchingOptionFromOptionsArg((Character.Librarian, Character.Investigator, Character.Washerwoman), argIndex: 1));

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(baronBluffs, Is.EquivalentTo(new[] { Character.Chef, Character.Butler, Character.Monk }));
                Assert.That(scarletWomanBluffs, Is.EquivalentTo(new[] { Character.Librarian, Character.Investigator, Character.Washerwoman }));
            });
        }
    }
}