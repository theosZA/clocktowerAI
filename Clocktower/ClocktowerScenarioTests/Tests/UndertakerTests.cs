using Clocktower.Game;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class UndertakerTests
    {
        [Test]
        public async Task Undertaker_SeesExecutedPlayer()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Saint,Ravenkeeper,Soldier,Undertaker,Mayor");
            setup.Agent(Character.Imp).MockNomination(Character.Mayor);
            setup.Agent(Character.Imp).MockImp(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Undertaker).Received().NotifyUndertaker(Arg.Is<Player>(player => player.Character == Character.Mayor), Character.Mayor);
        }

        [Test]
        public async Task Undertaker_SeesExecutedImp()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Scarlet_Woman,Saint,Ravenkeeper,Soldier,Undertaker,Mayor");
            var scarletWoman = setup.Agent(Character.Scarlet_Woman);
            scarletWoman.MockNomination(Character.Imp);
            scarletWoman.MockImp(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Undertaker).Received().NotifyUndertaker(Arg.Is<Player>(player => player.Character == Character.Imp), Character.Imp);
        }

        [Test]
        public async Task Undertaker_SeesExecutedDrunk()
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Baron,Fisherman,Ravenkeeper,Soldier,Undertaker,Mayor")
                            .WithDrunk(Character.Mayor)
                            .Build();

            setup.Agent(Character.Imp).MockNomination(Character.Mayor);
            setup.Agent(Character.Imp).MockImp(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Undertaker).Received().NotifyUndertaker(Arg.Is<Player>(player => player.Character == Character.Mayor), Character.Drunk);
        }

        [Test]
        public async Task Undertaker_SeesExecutedRecluse()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Recluse,Ravenkeeper,Soldier,Undertaker,Mayor");
            setup.Agent(Character.Imp).MockNomination(Character.Recluse);
            setup.Agent(Character.Imp).MockImp(Character.Soldier);
            setup.Storyteller.MockGetCharacterForUndertaker(Character.Assassin);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Undertaker).Received().NotifyUndertaker(Arg.Is<Player>(player => player.Character == Character.Recluse), Character.Assassin);
        }

        [Test]
        public async Task Undertaker_SeesExecutedSpy()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Spy,Saint,Ravenkeeper,Soldier,Undertaker,Mayor");
            setup.Agent(Character.Imp).MockNomination(Character.Spy);
            setup.Agent(Character.Imp).MockImp(Character.Soldier);
            setup.Storyteller.MockGetCharacterForUndertaker(Character.Butler);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Undertaker).Received().NotifyUndertaker(Arg.Is<Player>(player => player.Character == Character.Spy), Character.Butler);
        }

        [Test]
        public async Task Undertaker_SeesPlayerExecutedByVirgin()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Saint,Virgin,Soldier,Undertaker,Mayor");
            setup.Agent(Character.Mayor).MockNomination(Character.Virgin);
            setup.Agent(Character.Imp).MockImp(Character.Soldier);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Undertaker).Received().NotifyUndertaker(Arg.Is<Player>(player => player.Character == Character.Mayor), Character.Mayor);
        }

        [Test]
        public async Task Undertaker_SeesExecutedPlayerEachDay()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Saint,Ravenkeeper,Soldier,Undertaker,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Imp).MockNomination(Character.Mayor);
            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockImp(Character.Soldier);
            setup.Agent(Character.Imp).MockNomination(Character.Ravenkeeper);
            await game.RunNightAndDay();

            await setup.Agent(Character.Undertaker).Received().NotifyUndertaker(Arg.Is<Player>(player => player.Character == Character.Mayor), Character.Mayor);
            setup.Agent(Character.Undertaker).ClearReceivedCalls();

            // Night 3 & Day 3
            setup.Agent(Character.Imp).MockImp(Character.Soldier);
            await game.RunNightAndDay();

            await setup.Agent(Character.Undertaker).Received().NotifyUndertaker(Arg.Is<Player>(player => player.Character == Character.Ravenkeeper), Character.Ravenkeeper);
        }

        [Test]
        public async Task Undertaker_DoesNotSeeExecutedDeadPlayers()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Imp,Baron,Saint,Ravenkeeper,Soldier,Undertaker,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Imp).MockNomination(Character.Mayor);
            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Imp).MockImp(Character.Baron);
            setup.Agent(Character.Imp).MockNomination(Character.Baron);
            await game.RunNightAndDay();

            await setup.Agent(Character.Undertaker).Received().NotifyUndertaker(Arg.Is<Player>(player => player.Character == Character.Mayor), Character.Mayor);
            setup.Agent(Character.Undertaker).ClearReceivedCalls();

            // Night 3 & Day 3
            setup.Agent(Character.Imp).MockImp(Character.Soldier);
            await game.RunNightAndDay();

            await setup.Agent(Character.Undertaker).DidNotReceive().NotifyUndertaker(Arg.Any<Player>(), Arg.Any<Character>());
        }

        [TestCase(Character.Imp)]
        [TestCase(Character.Poisoner)]
        [TestCase(Character.Butler)]
        [TestCase(Character.Ravenkeeper)]
        public async Task Undertaker_IsTheDrunk(Character characterToSee)
        {
            // Arrange
            var setup = new ClocktowerGameBuilder(playerCount: 7);
            var game = setup.WithDefaultAgents()
                            .WithCharacters("Imp,Baron,Saint,Ravenkeeper,Soldier,Undertaker,Mayor")
                            .WithDrunk(Character.Undertaker)
                            .Build();

            setup.Agent(Character.Imp).MockNomination(Character.Mayor);
            setup.Agent(Character.Imp).MockImp(Character.Soldier);
            setup.Storyteller.MockGetCharacterForUndertaker(characterToSee);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Undertaker).Received().NotifyUndertaker(Arg.Is<Player>(player => player.Character == Character.Mayor), characterToSee);
        }
    }
}