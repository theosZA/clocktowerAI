using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Options;

namespace ClocktowerScenarioTests.Tests
{
    [TestFixture]
    public class AiTextHandlingTests
    {
        [Test]
        public void AddJugglesFromText()
        {
            // Arrange
            var characters = new[] { Character.Juggler, Character.Monk, Character.Butler, Character.Slayer, Character.Fortune_Teller, Character.Empath, Character.Imp, Character.Baron };
            var names = new[] { "Alison", "Bernard", "Christie", "David", "Eleanor", "Franklin", "Georgina", "Harry" };
            var agents = names.Select(_ => Substitute.For<IAgent>()).ToList();
            for (int i = 0; i < names.Length; i++)
            {
                agents[i].PlayerName.Returns(names[i]);
            }

            var grimoire = new Grimoire(agents, characters);
            var players = names.Select((name, i) => new Player(grimoire, agents[i], characters[i], i > 5 ? Alignment.Evil : Alignment.Good)).ToList();

            // Act
            var jugglerOption = new JugglerOption(players, characters);
            var textJuggle = "FRANKLIN AS MONK, ELEANOR AS BUTLER, CHRISTIE AS SLAYER, DAVID AS FORTUNE TELLER, GEORGINA AS EMPATH.";
            var result = jugglerOption.AddJugglesFromText(textJuggle);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(jugglerOption.Juggles, Is.EquivalentTo(new[] { (players[5], Character.Monk),
                                                                       (players[4], Character.Butler),
                                                                       (players[2], Character.Slayer),
                                                                       (players[3], Character.Fortune_Teller),
                                                                       (players[6], Character.Empath) }));
        }
    }
}
