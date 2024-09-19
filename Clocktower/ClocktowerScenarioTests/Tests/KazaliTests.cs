using Clocktower;
using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Selection;
using ClocktowerScenarioTests.Mocks;

namespace ClocktowerScenarioTests.Tests
{
    public class KazaliTests
    {
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(9)]
        public async Task Kazali_ChooseOneMinion(int playerCount)
        {
            // Arrange
            var characters = new[] { Character.Kazali, Character.Fisherman, Character.Ravenkeeper, Character.Saint, Character.Sweetheart, Character.Monk,
                                     Character.Mayor, Character.Virgin, Character.Recluse };
            var (setup, game) = ClocktowerGameBuilder.BuildDefault(characters.Take(playerCount).ToList());

            var minionCount = setup.Agent(Character.Kazali).MockKazaliMinionChoice(new[] { (Character.Fisherman, Character.Baron) });

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(minionCount.Value, Is.EqualTo(1));
            Received.InOrder(async () =>
            {
                await setup.Agent(Character.Fisherman).AssignCharacter(Character.Fisherman, Alignment.Good);
                await setup.Agent(Character.Fisherman).AssignCharacter(Character.Baron, Alignment.Evil);
                await setup.Agent(Character.Fisherman).MinionInformation(Arg.Is<Player>(player => player.RealCharacter == Character.Kazali),
                                                                         Arg.Is<IReadOnlyCollection<Player>>(fellowMinions => fellowMinions.Count() == 0),
                                                                         false,
                                                                         Arg.Is<IReadOnlyCollection<Character>>(bluffs => bluffs.Count() == 0));
                await setup.Agent(Character.Kazali).DemonInformation(Arg.Is<IReadOnlyCollection<Player>>(minions => minions.Count() == 1 && 
                                                                                                                    minions.ElementAt(0).CharacterHistory.ElementAt(0)[0] == Character.Fisherman),
                                                                     Arg.Is<IReadOnlyCollection<Character>>(bluffs => bluffs.Count() == 3));
            });
        }

        [TestCase(10)]
        [TestCase(11)]
        [TestCase(12)]
        public async Task Kazali_ChooseTwoMinions(int playerCount)
        {
            // Arrange
            var characters = new[] { Character.Kazali, Character.Fisherman, Character.Ravenkeeper, Character.Saint, Character.Sweetheart, Character.Monk,
                                     Character.Mayor, Character.Virgin, Character.Recluse, Character.Cannibal, Character.Undertaker, Character.Pacifist };
            var (setup, game) = ClocktowerGameBuilder.BuildDefault(characters.Take(playerCount).ToList());

            var minionCount = setup.Agent(Character.Kazali).MockKazaliMinionChoice(new[] 
            { 
                (Character.Fisherman, Character.Baron),
                (Character.Virgin, Character.Assassin)
            });

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(minionCount.Value, Is.EqualTo(2));
            Received.InOrder(async () =>
            {
                await setup.Agent(Character.Fisherman).AssignCharacter(Character.Fisherman, Alignment.Good);
                await setup.Agent(Character.Virgin).AssignCharacter(Character.Virgin, Alignment.Good);
                await setup.Agent(Character.Fisherman).AssignCharacter(Character.Baron, Alignment.Evil);
                await setup.Agent(Character.Virgin).AssignCharacter(Character.Assassin, Alignment.Evil);
                await setup.Agent(Character.Fisherman).MinionInformation(Arg.Is<Player>(player => player.RealCharacter == Character.Kazali),
                                                                         Arg.Is<IReadOnlyCollection<Player>>(fellowMinions => fellowMinions.Count() == 1 &&
                                                                                                                              fellowMinions.ElementAt(0).CharacterHistory.ElementAt(0)[0] == Character.Virgin),
                                                                         false,
                                                                         Arg.Is<IReadOnlyCollection<Character>>(bluffs => bluffs.Count() == 0));
                await setup.Agent(Character.Virgin).MinionInformation(Arg.Is<Player>(player => player.RealCharacter == Character.Kazali),
                                                                      Arg.Is<IReadOnlyCollection<Player>>(fellowMinions => fellowMinions.Count() == 1 &&
                                                                                                                           fellowMinions.ElementAt(0).CharacterHistory.ElementAt(0)[0] == Character.Fisherman),
                                                                      false,
                                                                      Arg.Is<IReadOnlyCollection<Character>>(bluffs => bluffs.Count() == 0));
                await setup.Agent(Character.Kazali).DemonInformation(Arg.Is<IReadOnlyCollection<Player>>(minions => minions.Count() == 2 &&
                                                                                                                    minions.ElementAt(0).CharacterHistory.ElementAt(0)[0] == Character.Fisherman &&
                                                                                                                    minions.ElementAt(1).CharacterHistory.ElementAt(0)[0] == Character.Virgin),
                                                                     Arg.Is<IReadOnlyCollection<Character>>(bluffs => bluffs.Count() == 3));
            });
        }

        [TestCase(13)]
        [TestCase(14)]
        [TestCase(15)]
        public async Task Kazali_ChooseThreeMinions(int playerCount)
        {
            // Arrange
            var characters = new[] { Character.Kazali, Character.Fisherman, Character.Ravenkeeper, Character.Saint, Character.Sweetheart, Character.Monk,
                                     Character.Mayor, Character.Virgin, Character.Recluse, Character.Cannibal, Character.Undertaker, Character.Pacifist,
                                     Character.Tinker, Character.Juggler, Character.Soldier };
            var (setup, game) = ClocktowerGameBuilder.BuildDefault(characters.Take(playerCount).ToList());

            var minionCount = setup.Agent(Character.Kazali).MockKazaliMinionChoice(new[]
            {
                (Character.Fisherman, Character.Baron),
                (Character.Virgin, Character.Assassin),
                (Character.Tinker, Character.Scarlet_Woman)
            });

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(minionCount.Value, Is.EqualTo(3));
            Received.InOrder(async () =>
            {
                await setup.Agent(Character.Fisherman).AssignCharacter(Character.Fisherman, Alignment.Good);
                await setup.Agent(Character.Virgin).AssignCharacter(Character.Virgin, Alignment.Good);
                await setup.Agent(Character.Tinker).AssignCharacter(Character.Tinker, Alignment.Good);
                await setup.Agent(Character.Fisherman).AssignCharacter(Character.Baron, Alignment.Evil);
                await setup.Agent(Character.Virgin).AssignCharacter(Character.Assassin, Alignment.Evil);
                await setup.Agent(Character.Tinker).AssignCharacter(Character.Scarlet_Woman, Alignment.Evil);
                await setup.Agent(Character.Fisherman).MinionInformation(Arg.Is<Player>(player => player.RealCharacter == Character.Kazali),
                                                                         Arg.Is<IReadOnlyCollection<Player>>(fellowMinions => fellowMinions.Count() == 2 &&
                                                                                                                              fellowMinions.ElementAt(0).CharacterHistory.ElementAt(0)[0] == Character.Virgin &&
                                                                                                                              fellowMinions.ElementAt(1).CharacterHistory.ElementAt(0)[0] == Character.Tinker),
                                                                         false,
                                                                         Arg.Is<IReadOnlyCollection<Character>>(bluffs => bluffs.Count() == 0));
                await setup.Agent(Character.Virgin).MinionInformation(Arg.Is<Player>(player => player.RealCharacter == Character.Kazali),
                                                                      Arg.Is<IReadOnlyCollection<Player>>(fellowMinions => fellowMinions.Count() == 2 &&
                                                                                                                           fellowMinions.ElementAt(0).CharacterHistory.ElementAt(0)[0] == Character.Fisherman &&
                                                                                                                           fellowMinions.ElementAt(1).CharacterHistory.ElementAt(0)[0] == Character.Tinker),
                                                                      false,
                                                                      Arg.Is<IReadOnlyCollection<Character>>(bluffs => bluffs.Count() == 0));
                await setup.Agent(Character.Tinker).MinionInformation(Arg.Is<Player>(player => player.RealCharacter == Character.Kazali),
                                                                      Arg.Is<IReadOnlyCollection<Player>>(fellowMinions => fellowMinions.Count() == 2 &&
                                                                                                                           fellowMinions.ElementAt(0).CharacterHistory.ElementAt(0)[0] == Character.Fisherman &&
                                                                                                                           fellowMinions.ElementAt(1).CharacterHistory.ElementAt(0)[0] == Character.Virgin),
                                                                      false,
                                                                      Arg.Is<IReadOnlyCollection<Character>>(bluffs => bluffs.Count() == 0));
                await setup.Agent(Character.Kazali).DemonInformation(Arg.Is<IReadOnlyCollection<Player>>(minions => minions.Count() == 3 &&
                                                                                                                    minions.ElementAt(0).CharacterHistory.ElementAt(0)[0] == Character.Fisherman &&
                                                                                                                    minions.ElementAt(1).CharacterHistory.ElementAt(0)[0] == Character.Virgin &&
                                                                                                                    minions.ElementAt(2).CharacterHistory.ElementAt(0)[0] == Character.Tinker),
                                                                     Arg.Is<IReadOnlyCollection<Character>>(bluffs => bluffs.Count() == 3));
            });
        }

        [Test]
        public async Task Kazali_MinionActsOnFirstNight()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Kazali,Fisherman,Ravenkeeper,Saint,Sweetheart,Soldier,Mayor");
            setup.Agent(Character.Kazali).MockKazaliMinionChoice(new[] { (Character.Fisherman, Character.Poisoner) });
            setup.Agent(Character.Fisherman).MockPoisoner(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).Received().RequestChoiceFromPoisoner(Arg.Any<IReadOnlyCollection<IOption>>());
        }

        [Test]
        public async Task Kazali_MinionMustRegisterAsEvil()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Kazali,Fisherman,Ravenkeeper,Saint,Sweetheart,Soldier,Steward");
            setup.Agent(Character.Kazali).MockKazaliMinionChoice(new[] { (Character.Fisherman, Character.Baron) });
            var stewardPingOptions = setup.Storyteller.MockGetStewardPing(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(stewardPingOptions, Does.Not.Contain(Character.Baron));
        }

        [Test]
        public async Task Kazali_LosesWhenExecuted()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Kazali,Fisherman,Ravenkeeper,Saint,Sweetheart,Soldier,Mayor");
            setup.Agent(Character.Kazali).MockKazaliMinionChoice(new[] { (Character.Fisherman, Character.Baron) });
            setup.Agent(Character.Kazali).MockNomination(Character.Kazali);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(game.Finished, Is.True);
            Assert.That(game.Winner, Is.EqualTo(Alignment.Good));
        }

        [Test]
        public async Task Kazali_KillsAtNight()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Kazali,Fisherman,Ravenkeeper,Saint,Sweetheart,Soldier,Mayor");
            setup.Agent(Character.Kazali).MockKazaliMinionChoice(new[] { (Character.Fisherman, Character.Baron) });
            setup.Agent(Character.Kazali).MockDemonKill(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Saint).Received().YouAreDead();
        }

        [Test]
        public async Task Kazali_Poisoned()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Kazali,Fisherman,Ravenkeeper,Saint,Sweetheart,Soldier,Mayor");
            setup.Agent(Character.Kazali).MockKazaliMinionChoice(new[] { (Character.Fisherman, Character.Poisoner) });
            setup.Agent(Character.Fisherman).MockPoisoner(Character.Kazali);
            setup.Agent(Character.Kazali).MockDemonKill(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Saint).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Kazali_SweetheartDrunk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Kazali,Fisherman,Ravenkeeper,Saint,Sweetheart,Soldier,Mayor");
            setup.Agent(Character.Kazali).MockKazaliMinionChoice(new[] { (Character.Fisherman, Character.Baron) });
            setup.Agent(Character.Kazali).MockNomination(Character.Sweetheart);
            setup.Storyteller.MockGetSweetheartDrunk(Character.Kazali);
            setup.Agent(Character.Kazali).MockDemonKill(Character.Saint);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Saint).DidNotReceive().YouAreDead();
        }

        [Test]
        public async Task Kazali_MarionetteJinx()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Kazali,Fisherman,Ravenkeeper,Saint,Sweetheart,Soldier,Mayor");
            KazaliMinionsSelection? kazaliMinionsSelection = null;
            setup.Agent(Character.Kazali).When(agent => agent.RequestSelectionOfKazaliMinions(Arg.Any<KazaliMinionsSelection>()))
                .Do(args =>
                {
                    kazaliMinionsSelection = args.ArgAt<KazaliMinionsSelection>(0);
                    var minion = (kazaliMinionsSelection.PossiblePlayers.First(player => player.RealCharacter == Character.Fisherman), Character.Marionette);
                    kazaliMinionsSelection.SelectMinions(new[] { minion });
                });

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            Assert.That(kazaliMinionsSelection, Is.Not.Null);
            Assert.That(kazaliMinionsSelection.CharacterLimitations.TryGetValue(Character.Marionette, out var marionetteCandidates), Is.True);
            Assert.That(marionetteCandidates!.Count(), Is.EqualTo(2));
            Assert.That(marionetteCandidates!.ElementAt(0).Character, Is.EqualTo(Character.Fisherman));
            Assert.That(marionetteCandidates!.ElementAt(1).Character, Is.EqualTo(Character.Mayor));
            await setup.Agent(Character.Fisherman).DidNotReceive().AssignCharacter(Character.Marionette, Alignment.Evil);
            await setup.Agent(Character.Fisherman).DidNotReceive().MinionInformation(Arg.Any<Player>(), Arg.Any<IReadOnlyCollection<Player>>(), Arg.Any<bool>(), Arg.Any<IReadOnlyCollection<Character>>());
        }

        [Test]
        public async Task Kazali_SoldierJinx()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Kazali,Fisherman,Ravenkeeper,Saint,Sweetheart,Soldier,Mayor");
            setup.Agent(Character.Kazali).MockKazaliMinionChoice(new[] { (Character.Soldier, Character.Poisoner) });
            setup.Agent(Character.Soldier).RequestChoiceOfMinionForSoldierSelectedByKazali(Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetOptionForCharacterFromArg(Character.Scarlet_Woman));

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Soldier).Received().AssignCharacter(Character.Scarlet_Woman, Alignment.Evil);
            await setup.Agent(Character.Soldier).DidNotReceive().AssignCharacter(Character.Poisoner, Alignment.Evil);
            await setup.Agent(Character.Soldier).MinionInformation(Arg.Is<Player>(player => player.RealCharacter == Character.Kazali),
                                                                   Arg.Is<IReadOnlyCollection<Player>>(fellowMinions => fellowMinions.Count() == 0),
                                                                   false,
                                                                   Arg.Is<IReadOnlyCollection<Character>>(bluffs => bluffs.Count() == 0));
            await setup.Agent(Character.Kazali).DemonInformation(Arg.Is<IReadOnlyCollection<Player>>(minions => minions.Count() == 1 &&
                                                                                                                minions.ElementAt(0).CharacterHistory.ElementAt(0)[0] == Character.Soldier),
                                                                 Arg.Is<IReadOnlyCollection<Character>>(bluffs => bluffs.Count() == 3));
        }

        [Test]
        public async Task Kazali_SoldierPicksPickedMinionCharacter()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Kazali,Fisherman,Ravenkeeper,Saint,Sweetheart,Soldier,Mayor,Monk,Virgin,Recluse");
            setup.Agent(Character.Kazali).MockKazaliMinionChoice(new[] 
            { 
                (Character.Soldier, Character.Poisoner),
                (Character.Monk, Character.Scarlet_Woman)
            });
            setup.Agent(Character.Soldier).RequestChoiceOfMinionForSoldierSelectedByKazali(Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetOptionForCharacterFromArg(Character.Scarlet_Woman));
            setup.Agent(Character.Kazali).RequestNewKazaliMinion(Arg.Is<Player>(player => player.RealCharacter == Character.Monk), Character.Scarlet_Woman, Arg.Any<IReadOnlyCollection<IOption>>())
                .Returns(args => args.GetOptionForCharacterFromArg(Character.Baron, 2));

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Soldier).Received().AssignCharacter(Character.Scarlet_Woman, Alignment.Evil);
            await setup.Agent(Character.Soldier).DidNotReceive().AssignCharacter(Character.Poisoner, Alignment.Evil);
            await setup.Agent(Character.Soldier).MinionInformation(Arg.Is<Player>(player => player.RealCharacter == Character.Kazali),
                                                                   Arg.Is<IReadOnlyCollection<Player>>(fellowMinions => fellowMinions.Count() == 1 &&
                                                                                                                        fellowMinions.ElementAt(0).CharacterHistory.ElementAt(0)[0] == Character.Monk),
                                                                   false,
                                                                   Arg.Is<IReadOnlyCollection<Character>>(bluffs => bluffs.Count() == 0));;

            await setup.Agent(Character.Monk).Received().AssignCharacter(Character.Baron, Alignment.Evil);
            await setup.Agent(Character.Monk).DidNotReceive().AssignCharacter(Character.Scarlet_Woman, Alignment.Evil);
            await setup.Agent(Character.Soldier).MinionInformation(Arg.Is<Player>(player => player.RealCharacter == Character.Kazali),
                                                                   Arg.Is<IReadOnlyCollection<Player>>(fellowMinions => fellowMinions.Count() == 1 &&
                                                                                                                        fellowMinions.ElementAt(0).CharacterHistory.ElementAt(0)[0] == Character.Soldier),
                                                                   false,
                                                                   Arg.Is<IReadOnlyCollection<Character>>(bluffs => bluffs.Count() == 0)); ;

            await setup.Agent(Character.Kazali).DemonInformation(Arg.Is<IReadOnlyCollection<Player>>(minions => minions.Count() == 1 &&
                                                                                                                minions.ElementAt(0).CharacterHistory.ElementAt(0)[0] == Character.Soldier),
                                                                 Arg.Is<IReadOnlyCollection<Character>>(bluffs => bluffs.Count() == 3));
        }

        [Test]
        public async Task Kazali_BountyHunterJinx()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Kazali,Fisherman,Ravenkeeper,Saint,Sweetheart,Bounty_Hunter,Mayor");
            setup.Agent(Character.Kazali).MockKazaliMinionChoice(new[] { (Character.Bounty_Hunter, Character.Scarlet_Woman) });
            setup.Storyteller.MockGetEvilTownsfolk(Character.Mayor);    // Should not trigger due to jinx.

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Bounty_Hunter).Received().AssignCharacter(Character.Scarlet_Woman, Alignment.Evil);
            await setup.Agent(Character.Mayor).DidNotReceive().ChangeAlignment(Alignment.Evil);
        }

        [Test]
        public async Task Kazali_PicksPhilosopherBountyHunter()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Kazali,Fisherman,Ravenkeeper,Saint,Sweetheart,Philosopher,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Bounty_Hunter);
            setup.Agent(Character.Kazali).MockKazaliMinionChoice(new[] { (Character.Philosopher, Character.Scarlet_Woman) });
            setup.Storyteller.MockGetEvilTownsfolk(Character.Mayor);    // Should not trigger due to jinx.

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Philosopher).Received().AssignCharacter(Character.Scarlet_Woman, Alignment.Evil);
            await setup.Agent(Character.Mayor).DidNotReceive().ChangeAlignment(Alignment.Evil);
        }

        [Test]
        public async Task Kazali_DoesNotPickPhilosopherBountyHunter_CreatesEvilTownsfolk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Kazali,Fisherman,Ravenkeeper,Saint,Sweetheart,Philosopher,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Bounty_Hunter);
            setup.Agent(Character.Kazali).MockKazaliMinionChoice(new[] { (Character.Fisherman, Character.Scarlet_Woman) });
            setup.Storyteller.MockGetEvilTownsfolk(Character.Mayor);
            setup.Storyteller.MockGetBountyHunterPing(Character.Mayor);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).Received().AssignCharacter(Character.Scarlet_Woman, Alignment.Evil);
            await setup.Agent(Character.Mayor).Received().ChangeAlignment(Alignment.Evil);
        }

        [Test]
        public async Task Kazali_DoesNotPickPhilosopherBountyHunter_DoesNotCreateEvilTownsfolk()
        {
            // Arrange
            var (setup, game) = ClocktowerGameBuilder.BuildDefault("Kazali,Fisherman,Ravenkeeper,Saint,Sweetheart,Philosopher,Mayor");
            setup.Agent(Character.Philosopher).MockPhilosopher(Character.Bounty_Hunter);
            setup.Agent(Character.Kazali).MockKazaliMinionChoice(new[] { (Character.Fisherman, Character.Scarlet_Woman) });
            setup.Storyteller.MockGetBountyHunterPing(Character.Kazali);

            // Act
            await game.StartGame();
            await game.RunNightAndDay();

            // Assert
            await setup.Agent(Character.Fisherman).Received().AssignCharacter(Character.Scarlet_Woman, Alignment.Evil);
        }
    }
}