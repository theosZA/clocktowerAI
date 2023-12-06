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
        public bool Finished => false;

        public ClocktowerGame()
        {
            var storytellerForm = new StorytellerForm();
            storyteller = new HumanStoryteller(storytellerForm);

            var playerNames = new[] { "Alison", "Barry", "Casandra", "Donald", "Emma", "Franklin", "Georgina", "Harry" };
            var playerForms = playerNames.ToDictionary(name => name, name => new HumanAgentForm(name));
            var players = playerNames.Select(name => new Player(name, new HumanAgent(playerForms[name])));

            observers = new ObserverCollection(playerForms.Select(form => form.Value.Observer).Append(storytellerForm.Observer));
            grimoire = new Grimoire(players);

            storytellerForm.Show();
            foreach (var form in playerForms)
            {
                form.Value.Show();
            }

            grimoire.AssignCharacters(storyteller);
        }

        public async Task RunNightAndDay()
        {
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
                // Fortune Teller...
                new NotifySteward(storyteller, grimoire)
                // Shugenja...
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
                // Assassin...
                // Godfather...
                // Sweetheart...
                // Tinker...
                new ChoiceFromRavenkeeper(storyteller, grimoire),
                new NotifyEmpath(storyteller, grimoire)
                // Fortune Teller...
                // Undertaker...
            });
        }

        private static async Task RunNightEvents(IEnumerable<IGameEvent> nightEvents)
        {
            foreach (var nightEvent in nightEvents)
            {
                await nightEvent.RunEvent();
            }
        }

        private async Task RunDay()
        {
            // Announce kills that happened in the night.
            var newlyDeadPlayers = grimoire.Players.Where(player => player.Tokens.Contains(Token.DiedAtNight));
            foreach (var newlyDeadPlayer in newlyDeadPlayers)
            {
                newlyDeadPlayer.Tokens.Remove(Token.DiedAtNight);
                newlyDeadPlayer.Kill();
                observers.PlayerDiedAtNight(newlyDeadPlayer);
            }

            // TBD Conversations during the day.

            // Nominations.
            await new Nominations(storyteller, grimoire, observers, random).RunNominations();
        }

        private readonly Grimoire grimoire;
        private readonly IStoryteller storyteller;
        private readonly ObserverCollection observers;
        private readonly Random random = new();

        private int dayNumber = 0;
    }
}
