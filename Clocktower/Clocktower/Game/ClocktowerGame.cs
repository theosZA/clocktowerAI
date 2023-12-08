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
            var storytellerForm = new StorytellerForm(random);
            storyteller = new HumanStoryteller(storytellerForm);

            var playerNames = new[] { "Alison", "Barry", "Casandra", "Donald", "Emma", "Franklin", "Georgina", "Harry" };

            var playerForms = playerNames.ToDictionary(name => name, name => new HumanAgentForm(name, random));
            observers = new ObserverCollection(playerForms.Select(form => form.Value.Observer).Append(storytellerForm.Observer));

            // For now we assign hardcoded characters.
            var charactersAlignments = new[]
            {
                (Character.Imp, Alignment.Evil),
                (Character.Slayer, Alignment.Good),
                (Character.Shugenja, Alignment.Good),
                (Character.Librarian, Alignment.Good),
                (Character.Recluse, Alignment.Good),
                (Character.Godfather, Alignment.Evil),
                (Character.Fortune_Teller, Alignment.Good),
                (Character.Ravenkeeper, Alignment.Good)
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
            await RunNightEvents(new IGameEvent[]
            {
                new AssignDrunk(storyteller, grimoire),
                new AssignFortuneTellerRedHerring(storyteller, grimoire)
            });
        }

        private async Task RunFirstNight()
        {
            await RunNightEvents(new IGameEvent[]
            {
                // Philosopher...
                new MinionInformation(storyteller, grimoire),
                new DemonInformation(storyteller, grimoire),
                // Poisoner...
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
            await RunNightEvents(new IGameEvent[]
            {
                // Philosopher...
                // Poisoner...
                // Monk...
                // Scarlet Woman...
                new ChoiceFromImp(storyteller, grimoire),
                new ChoiceFromAssassin(storyteller, grimoire),
                new ChoiceFromGodfather(storyteller, grimoire),
                // Sweetheart...
                // Tinker...
                new ChoiceFromRavenkeeper(storyteller, grimoire),
                new NotifyEmpath(storyteller, grimoire),
                new ChoiceFromFortuneTeller(storyteller, grimoire)
                // Undertaker...
            });
        }

        private async Task RunNightEvents(IEnumerable<IGameEvent> nightEvents)
        {
            foreach (var nightEvent in nightEvents)
            {
                await nightEvent.RunEvent();
                if (Finished)
                {
                    return;
                }
            }
        }

        private async Task RunDay()
        {
            AnnounceNightKills();
            if (!Finished)
            {
                // TBD Conversations during the day.

                await new Nominations(storyteller, grimoire, observers, random).RunNominations();
            }
        }

        private void AnnounceNightKills()
        {
            var newlyDeadPlayers = grimoire.Players.Where(player => player.Tokens.Contains(Token.DiedAtNight) || player.Tokens.Contains(Token.KilledByDemon));
            foreach (var newlyDeadPlayer in newlyDeadPlayers)
            {
                observers.PlayerDiedAtNight(newlyDeadPlayer);
                newlyDeadPlayer.Tokens.Remove(Token.DiedAtNight);
                newlyDeadPlayer.Tokens.Remove(Token.KilledByDemon);
                newlyDeadPlayer.Kill();
                if (Finished)
                {
                    return;
                }
            }
        }

        private readonly Grimoire grimoire;
        private readonly IStoryteller storyteller;
        private readonly ObserverCollection observers;
        private readonly Random random = new();

        private int dayNumber = 0;
    }
}
