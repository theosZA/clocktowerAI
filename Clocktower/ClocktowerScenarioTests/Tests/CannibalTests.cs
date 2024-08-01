using Clocktower.Game;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class CannibalTests
    {
        [Test]
        public async Task CannibalDrunk()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Baron,Ravenkeeper,Soldier,Cannibal,Mayor,Saint")
                            .WithDrunk(Character.Ravenkeeper)
                            .Build();
            setup.Agent(Character.Imp).MockNomination(Character.Ravenkeeper);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);

            List<Character> possibleCharacterAbilities = new();
            setup.Storyteller.ChooseFakeCannibalAbility(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .ReturnsMatchingOptionFromOptionsArg(Character.Empath, possibleCharacterAbilities, argIndex: 2);
            setup.Storyteller.MockGetEmpathNumber(2);
            var empathNumber = setup.Agent(Character.Cannibal).MockNotifyEmpath(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            Assert.That(empathNumber.Value, Is.EqualTo(2));
            foreach (var character in possibleCharacterAbilities)
            {   // The Cannibal ability isn't poisoned. They just gained the Drunk's ability, which means they need to think they have a Townsfolk's ability but they don't.
                Assert.That(character.CharacterType(), Is.EqualTo(CharacterType.Townsfolk), $"{character} is not a Townsfolk");
            }
        }

        [Test]
        public async Task Cannibal_Poisoned_DrunkIsNotAnOption()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Cannibal,Saint,Baron,Soldier,Fisherman");
            setup.Agent(Character.Imp).MockNomination(Character.Baron);
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);

            List<Character> possibleCharacterAbilities = new();
            setup.Storyteller.ChooseFakeCannibalAbility(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<IOption>>())
                .ReturnsMatchingOptionFromOptionsArg(Character.Empath, possibleCharacterAbilities, argIndex: 2);
            setup.Storyteller.MockGetEmpathNumber(2);
            setup.Agent(Character.Cannibal).MockNotifyEmpath(gameToEnd: game);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            foreach (var character in possibleCharacterAbilities)
            {   // The Cannibal is poisoned. It should be possible for them to appear to gain any Townsfolk or Outsider character EXCEPT the Drunk.
                Assert.That(character.CharacterType(), Is.EqualTo(CharacterType.Townsfolk).Or.EqualTo(CharacterType.Outsider), $"{character} is not a Townsfolk or Outsider");
                Assert.That(character, Is.Not.EqualTo(Character.Drunk));
            }
        }

        [Test]
        public async Task Cannibal_GainsMultipleOncePerGameAbilities()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Mayor,Cannibal,Saint,Baron,Soldier,Slayer");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Imp).MockNomination(Character.Baron);
            setup.Storyteller.MockCannibalChoice(Character.Slayer);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Cannibal).MockSlayerOption(Character.Imp);
            setup.Agent(Character.Imp).MockNomination(Character.Slayer);

            await game.RunNightAndDay();

            Assert.That(game.Finished, Is.False);   // First Slayer shot isn't real because the Cannibal didn't really gain the Slayer ability.

            // Night 3 & Day 3
            await game.RunNightAndDay();

            Assert.That(game.Finished, Is.True);   // Second Slayer shot is real because the Slayer was executed yesterday.
            Assert.That(game.Winner, Is.EqualTo(Alignment.Good));
        }

        // Other Cannibal test cases will be in the test classes for the character abilities that they gain or think they gain.
    }
}