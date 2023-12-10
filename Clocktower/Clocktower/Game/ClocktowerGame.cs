using Clocktower.Agent;
using Clocktower.Events;
using Clocktower.Observer;

namespace Clocktower.Game
{
    /// <summary>
    /// An instance of a Blood on the Clocktower game.
    /// </summary>
    internal class ClocktowerGame
    {
        public bool Finished => GetWinner().HasValue;

        public ClocktowerGame()
        {
            // "A Simple Matter"
            scriptCharacters = new List<Character>
            {   // Townsfolk
                Character.Steward,
                Character.Investigator,
                Character.Librarian,
                Character.Shugenja,
                Character.Empath,
                Character.Fortune_Teller,
                Character.Undertaker,
                Character.Monk,
                Character.Fisherman,
                Character.Slayer,
                Character.Philosopher,
                Character.Soldier,
                Character.Ravenkeeper,
                // Outsiders
                Character.Tinker,
                Character.Sweetheart,
                Character.Recluse,
                Character.Drunk,
                // Minions
                Character.Godfather,
                Character.Poisoner,
                Character.Assassin,
                Character.Scarlet_Woman,
                // Demons
                Character.Imp
            };

            var storytellerForm = new StorytellerForm(random);
            storyteller = new HumanStoryteller(storytellerForm);

            var playerNames = new[] { "Alison", "Bart", "Casandra", "Donald", "Emma", "Franklin", "Georgina", "Harry" };

            var playerForms = playerNames.ToDictionary(name => name, name => new HumanAgentForm(name, random));
            observers = new ObserverCollection(playerForms.Select(form => form.Value.Observer).Append(storytellerForm.Observer));

            // For now we assign hardcoded characters.
            var charactersAlignments = new[]
            {
                (Character.Imp, Alignment.Evil),
                (Character.Monk, Alignment.Good),
                (Character.Undertaker, Alignment.Good),
                (Character.Librarian, Alignment.Good),
                (Character.Slayer, Alignment.Good),
                (Character.Investigator, Alignment.Good),
                (Character.Soldier, Alignment.Good),
                (Character.Scarlet_Woman, Alignment.Evil)
            };

            var players = playerNames.Select((name, i) => new Player(name, new HumanAgent(playerForms[name]), charactersAlignments[i].Item1, charactersAlignments[i].Item2)).ToList();

            grimoire = new Grimoire(players);

            storytellerForm.Show();
            foreach (var form in playerForms)
            {
                form.Value.Show();
            }

            grimoire.AssignCharacters(storyteller);
        }

        public Alignment? GetWinner()
        {
            // The game is over if there are no living demons...
            if (!grimoire.Players.Any(player => player.Alive && player.CharacterType == CharacterType.Demon))
            {
                return Alignment.Good;
            }

            // ...or there are fewer than 3 players alive.
            if (grimoire.Players.Count(player => player.Alive) < 3)
            {
                return Alignment.Evil;
            }

            return null;
        }

        public void AnnounceWinner()
        {
            var winner = GetWinner();
            if (winner.HasValue)
            {
                observers.AnnounceWinner(winner.Value,
                                         grimoire.Players.Where(player => player.Alignment == winner.Value).ToList(),
                                         grimoire.Players.Where(player => player.Alignment != winner.Value).ToList());
            }
        }

        public async Task RunNightAndDay()
        {
            if (dayNumber == 0)
            {
                await Setup();
            }

            ++dayNumber;

            observers.Night(dayNumber);
            if (dayNumber == 1)
            {
                await RunFirstNight();
            }
            else
            {
                await RunNight();
            }

            observers.Day(dayNumber);
            await RunDay();
        }

        private async Task Setup()
        {
            if (addDrunkToGame)
            {
                await new AssignDrunk(storyteller, grimoire).RunEvent();
            }
            await new AssignFortuneTellerRedHerring(storyteller, grimoire).RunEvent();
        }

        private async Task RunFirstNight()
        {
            await RunEventSequence(new IGameEvent[]
            {
                // Philosopher...
                new MinionInformation(storyteller, grimoire),
                new DemonInformation(storyteller, grimoire),
                new ChoiceFromPoisoner(storyteller, grimoire),
                new NotifyGodfather(storyteller, grimoire),
                new NotifyLibrarian(storyteller, grimoire),
                new NotifyInvestigator(storyteller, grimoire),
                new NotifyEmpath(storyteller, grimoire),
                new ChoiceFromFortuneTeller(storyteller, grimoire),
                new NotifySteward(storyteller, grimoire),
                new NotifyShugenja(storyteller, grimoire)
            });
        }

        private async Task RunNight()
        {
            // Clear expired tokens.
            foreach (var player in grimoire.Players)
            {
                player.Tokens.Remove(Token.PoisonedByPoisoner);
                player.Tokens.Remove(Token.ProtectedByMonk);
                player.Tokens.Remove(Token.AlreadyClaimedSlayer);   // We allow players to claim Slayer once each day to allow for Philosopher into Slayer.
            }

            await RunEventSequence(new IGameEvent[]
            {
                // Philosopher...
                new ChoiceFromPoisoner(storyteller, grimoire),
                new ChoiceFromMonk(storyteller, grimoire),
                // Scarlet Woman - this is their theoretical place in the night order, but they actually become the demon immediately
                new ChoiceFromImp(storyteller, grimoire),
                new ChoiceFromAssassin(storyteller, grimoire),
                new ChoiceFromGodfather(storyteller, grimoire),
                new SweetheartDrunk(storyteller, grimoire),
                new TinkerOption(storyteller, grimoire, observers, duringDay: false),
                new ChoiceFromRavenkeeper(storyteller, grimoire, scriptCharacters),
                new NotifyEmpath(storyteller, grimoire),
                new ChoiceFromFortuneTeller(storyteller, grimoire),
                new NotifyUndertaker(storyteller, grimoire, scriptCharacters)
            });

            // Clear expired tokens.
            foreach (var player in grimoire.Players)
            {
                player.Tokens.Remove(Token.Executed);
            }
        }

        private async Task RunDay()
        {
            if (dayNumber > 1)
            {
                AnnounceNightKills();
            }

            var nominations = new Nominations(storyteller, grimoire, observers, random);

            await RunEventSequence(new IGameEvent[]
            {
                new TinkerOption(storyteller, grimoire, observers, duringDay: true),
                new SlayerShot(storyteller, grimoire, observers, random),
                // TBD Conversations during the day. Add the following options in once we support conversations (otherwise they're duplicated without need).
                // --private conversations--
                // Slayer
                // --public conversations--
                // Tinker
                // Slayer
                nominations,
                new SlayerShot(storyteller, grimoire, observers, random) { Nominations = nominations },
                new TinkerOption(storyteller, grimoire, observers, duringDay: true)
            });

            if (!Finished)
            {
                if (nominations.PlayerToBeExecuted == null)
                {
                    observers.DayEndsWithNoExecution();
                }
                else
                {
                    bool playerDies = nominations.PlayerToBeExecuted.Alive;
                    observers.PlayerIsExecuted(nominations.PlayerToBeExecuted, playerDies);
                    if (playerDies)
                    {
                        new Kills(storyteller, grimoire).Execute(nominations.PlayerToBeExecuted);
                    }
                }
            }
        }

        private async Task RunEventSequence(IEnumerable<IGameEvent> events)
        {
            foreach (var gameEvent in events)
            {
                await gameEvent.RunEvent();
                if (Finished)
                {
                    return;
                }
            }
        }

        private void AnnounceNightKills()
        {
            var newlyDeadPlayers = grimoire.Players.Where(player => player.Tokens.Contains(Token.DiedAtNight) || player.Tokens.Contains(Token.KilledByDemon)).ToList();
            if (newlyDeadPlayers.Count == 0)
            {
                observers.NoOneDiedAtNight();
                return;
            }
            foreach (var newlyDeadPlayer in newlyDeadPlayers)
            {
                observers.PlayerDiedAtNight(newlyDeadPlayer);
                newlyDeadPlayer.Tokens.Remove(Token.DiedAtNight);
                newlyDeadPlayer.Tokens.Remove(Token.KilledByDemon);
                newlyDeadPlayer.Kill();
            }
        }

        private readonly List<Character> scriptCharacters;
        private readonly Grimoire grimoire;
        private readonly IStoryteller storyteller;
        private readonly ObserverCollection observers;
        private readonly Random random = new();

        private int dayNumber = 0;

        private bool addDrunkToGame = true;
    }
}
