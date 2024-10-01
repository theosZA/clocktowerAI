using Clocktower.Agent.Observer;
using Clocktower.Game;
using Clocktower.Options;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class VigormortisTests
    {
        [Test]
        public async Task Vigormortis_LosesWhenExecuted()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Vigormortis,Fisherman,Ravenkeeper,Saint,Baron,Soldier,Mayor");
            setup.Agent(Character.Vigormortis).MockNomination(Character.Vigormortis);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Good));
        }

        [Test]
        public async Task Vigormortis_KillsMinion()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Vigormortis,Fisherman,Empath,Saint,Baron,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            var empathNumber = setup.Agent(Character.Empath).MockNotifyEmpath();

            await game.RunNightAndDay();

            Assert.That(empathNumber.Value, Is.EqualTo(0));
            empathNumber.Value = -1;

            // Night 2 & Day 2
            setup.Agent(Character.Vigormortis).MockDemonKill(Character.Baron);
            var townsfolkNeighbours = setup.Storyteller.MockGetTownsfolkPoisonedByVigormortis(Character.Empath);
            setup.Storyteller.MockGetEmpathNumber(2);

            await game.RunNightAndDay();

            Assert.Multiple(() =>
            {
                Assert.That(empathNumber.Value, Is.EqualTo(2));
                Assert.That(townsfolkNeighbours, Is.EquivalentTo(new[] { Character.Empath, Character.Soldier }));
            });
        }

        [Test]
        public async Task Vigormortis_KillsRecluse_RegisterAsMinion()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Vigormortis,Fisherman,Ravenkeeper,Saint,Baron,Soldier,Recluse");
            await game.StartGame();

            // Night 1 & Day 1
            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Vigormortis).MockDemonKill(Character.Recluse);
            setup.Storyteller.ShouldRegisterAsMinionForVigormortis(Arg.Is<Player>(player => player.RealCharacter == Character.Vigormortis),
                                                                   Arg.Is<Player>(player => player.RealCharacter == Character.Recluse),
                                                                   Arg.Any<IReadOnlyCollection<IOption>>())
                .ReturnsYesNoOptionFromArg(true, argIndex: 2);
            setup.Storyteller.MockGetTownsfolkPoisonedByVigormortis(Character.Soldier);

            await game.RunNightAndDay();

            // Night 3 & Day 3
            setup.Agent(Character.Vigormortis).MockDemonKill(Character.Soldier);

            await game.RunNightAndDay();

            await setup.Agent(Character.Soldier).Received().YouAreDead();
        }

        [Test]
        public async Task Vigormortis_KillsRecluse_DoesNotRegisterAsMinion()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Vigormortis,Fisherman,Ravenkeeper,Saint,Baron,Soldier,Recluse");
            await game.StartGame();

            // Night 1 & Day 1
            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Vigormortis).MockDemonKill(Character.Recluse);
            setup.Storyteller.ShouldRegisterAsMinionForVigormortis(Arg.Is<Player>(player => player.RealCharacter == Character.Vigormortis),
                                                                   Arg.Is<Player>(player => player.RealCharacter == Character.Recluse),
                                                                   Arg.Any<IReadOnlyCollection<IOption>>())
                .ReturnsYesNoOptionFromArg(false, argIndex: 2);
            setup.Storyteller.MockGetTownsfolkPoisonedByVigormortis(Character.Soldier);

            await game.RunNightAndDay();

            // Night 3 & Day 3
            setup.Agent(Character.Vigormortis).MockDemonKill(Character.Soldier);

            await game.RunNightAndDay();

            await setup.Agent(Character.Soldier).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Vigormortis_KillsGodfather()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Vigormortis,Fisherman,Ravenkeeper,Recluse,Godfather,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Vigormortis).MockDemonKill(Character.Godfather);
            setup.Storyteller.MockGetTownsfolkPoisonedByVigormortis(Character.Ravenkeeper);
            setup.Agent(Character.Recluse).MockNomination(Character.Recluse);

            await game.RunNightAndDay();

            // Night 3 & Day 3
            setup.Agent(Character.Vigormortis).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Godfather).MockGodfather(Character.Fisherman);

            await game.RunNightAndDay();

            await setup.Agent(Character.Fisherman).Received().YouAreDead();
        }

        [Test]
        public async Task Vigormortis_KillsPoisoner()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Vigormortis,Fisherman,Ravenkeeper,Recluse,Poisoner,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Poisoner).MockPoisoner(Character.Soldier);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Vigormortis).MockDemonKill(Character.Poisoner);
            setup.Storyteller.MockGetTownsfolkPoisonedByVigormortis(Character.Ravenkeeper);

            await game.RunNightAndDay();

            // Night 3 & Day 3
            setup.Agent(Character.Vigormortis).MockDemonKill(Character.Soldier);

            await game.RunNightAndDay();

            await setup.Agent(Character.Soldier).Received().YouAreDead();   // Soldier is poisoned by Poisoner.
        }

        [Test]
        public async Task Vigormortis_KillsAssassin()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Vigormortis,Fisherman,Ravenkeeper,Recluse,Assassin,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Vigormortis).MockDemonKill(Character.Assassin);
            setup.Storyteller.MockGetTownsfolkPoisonedByVigormortis(Character.Ravenkeeper);

            await game.RunNightAndDay();

            // Night 3 & Day 3
            setup.Agent(Character.Vigormortis).MockDemonKill(Character.Soldier);
            setup.Agent(Character.Assassin).MockAssassin(Character.Fisherman);

            await game.RunNightAndDay();

            await setup.Agent(Character.Fisherman).Received().YouAreDead();
        }

        [Test]
        public async Task Vigormortis_KillsScarletWoman()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Vigormortis,Fisherman,Ravenkeeper,Recluse,Scarlet_Woman,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Vigormortis).MockDemonKill(Character.Scarlet_Woman);
            setup.Storyteller.MockGetTownsfolkPoisonedByVigormortis(Character.Ravenkeeper);
            setup.Agent(Character.Vigormortis).MockNomination(Character.Vigormortis);

            await game.RunNightAndDay();

            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Good));
        }

        [Test]
        public async Task Vigormortis_KillsWitch()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Vigormortis,Fisherman,Ravenkeeper,Recluse,Witch,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Witch).MockWitch(Character.Soldier);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Vigormortis).MockDemonKill(Character.Witch);
            setup.Storyteller.MockGetTownsfolkPoisonedByVigormortis(Character.Ravenkeeper);
            setup.Agent(Character.Soldier).MockNomination(Character.Fisherman);

            await game.RunNightAndDay();

            await setup.Agent(Character.Soldier).Received().YouAreDead();
        }

        [Test]
        public async Task Vigormortis_KillsOrganGrinder()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Vigormortis,Fisherman,Ravenkeeper,Recluse,Organ_Grinder,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Vigormortis).MockDemonKill(Character.Organ_Grinder);
            setup.Storyteller.MockGetTownsfolkPoisonedByVigormortis(Character.Ravenkeeper);
            setup.Agent(Character.Soldier).MockNomination(Character.Fisherman);

            await game.RunNightAndDay();

            foreach (var agent in setup.Agents)
            {
                await agent.Observer.DidNotReceive().AnnounceVote(Arg.Any<Player>(), Arg.Any<Player>(), Arg.Any<bool>());
                await agent.Observer.Received(1).AnnounceVoteResult(Arg.Any<Player>(), null, VoteResult.UnknownResult);
            }
        }

        [Test]
        public async Task Vigormortis_KillsDevilsAdvocate()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Vigormortis,Fisherman,Ravenkeeper,Recluse,Devils_Advocate,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Devils_Advocate).MockDevilsAdvocate(Character.Devils_Advocate);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Devils_Advocate).MockDevilsAdvocate(Character.Vigormortis);
            setup.Agent(Character.Vigormortis).MockDemonKill(Character.Devils_Advocate);
            setup.Storyteller.MockGetTownsfolkPoisonedByVigormortis(Character.Ravenkeeper);
            setup.Agent(Character.Vigormortis).MockNomination(Character.Vigormortis);

            await game.RunNightAndDay();

            Assert.That(game.Finished, Is.False);
            await setup.Agent(Character.Vigormortis).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Vigormortis_KillsWidow()
        {
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Vigormortis,Fisherman,Ravenkeeper,Recluse,Widow,Soldier,Mayor");
            await game.StartGame();

            // Night 1 & Day 1
            setup.Agent(Character.Widow).MockWidow(Character.Soldier);
            setup.Storyteller.MockWidowPing(Character.Soldier);

            await game.RunNightAndDay();

            // Night 2 & Day 2
            setup.Agent(Character.Vigormortis).MockDemonKill(Character.Widow);
            setup.Storyteller.MockGetTownsfolkPoisonedByVigormortis(Character.Ravenkeeper);

            await game.RunNightAndDay();

            // Night 3 & Day 3
            setup.Agent(Character.Vigormortis).MockDemonKill(Character.Soldier);

            await game.RunNightAndDay();

            await setup.Agent(Character.Soldier).Received().YouAreDead();   // Soldier is still poisoned by Widow.
        }
    }
}