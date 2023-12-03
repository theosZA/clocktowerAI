using Clocktower.Agent;
using Clocktower.Night;
using System.ComponentModel;

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
            storytellerForm.Show();

            var playerNames = new[] { "Alison", "Barry", "Casandra", "Donald", "Emma", "Franklin", "Georgina", "Harry" };
            var forms = playerNames.Select(name => new HumanAgentForm(name)).ToList();
            var agents = forms.Select(form => (IAgent)new HumanAgent(form)).ToList();

            grimoire = new Grimoire(playerNames.Zip(agents).ToDictionary(x => x.First, x => x.Second));

            foreach (var form in forms)
            {
                form.Show();
            }

            grimoire.AssignCharacters(storyteller);

            dayNumber = 1;
            phase = Phase.Night;
        }

        public void RunPhase()
        {
            switch (phase)
            {
                case Phase.Night:
                    storyteller.Night(dayNumber);
                    grimoire.Night(dayNumber);
                    if (dayNumber == 1)
                    {
                        RunFirstNight();
                    }
                    else
                    {
                        RunNight();
                    }
                    break;

                case Phase.Morning:
                    storyteller.Day(dayNumber);
                    grimoire.Day(dayNumber);
                    // TBD
                    break;

                case Phase.Day:
                case Phase.Evening:
                    // TBD
                    break;

                default:
                    throw new InvalidEnumArgumentException(nameof(phase));
            }

            AdvancePhase();
        }

        private void RunFirstNight()
        {
            RunNightEvents(new INightEvent[]
            {
                // Philosopher...
                new MinionInformation(storyteller, grimoire),
                new DemonInformation(storyteller, grimoire),
                // Poisoner...
                new NotifyGodfather(storyteller, grimoire),
                new NotifyLibrarian(storyteller, grimoire),
                // Investigator...
                new NotifyEmpath(storyteller, grimoire),
                // Fortune Teller...
                new NotifySteward(storyteller, grimoire)
                // Shugenja...
            });
        }

        private void RunNight()
        {
            RunNightEvents(new INightEvent[]
            {
                // Philosopher...
                // Poisoner...
                // Monk...
                // Scarlet Woman...
                // Imp...
                // Assassin...
                // Godfather...
                // Sweetheart...
                // Tinker...
                // Ravenkeeper...
                new NotifyEmpath(storyteller, grimoire)
                // Fortune Teller...
                // Undertaker...
            });
        }

        private void RunNightEvents(INightEvent[] nightEvents, int currentIndex = 0)
        {
            if (currentIndex >= nightEvents.Length)
            {   // finished
                return;
            }

            nightEvents[currentIndex].RunEvent(() => { RunNightEvents(nightEvents, currentIndex + 1); });
        }

        private void AdvancePhase()
        {
            switch (phase)
            {
                case Phase.Night:
                    phase = Phase.Morning;
                    break;

                case Phase.Morning:
                    phase = Phase.Day;
                    break;

                case Phase.Day:
                    phase = Phase.Evening;
                    break;

                case Phase.Evening:
                    phase = Phase.Night;
                    ++dayNumber;
                    break;
            }
        }

        private Grimoire grimoire;
        private IStoryteller storyteller;

        private int dayNumber;
        private Phase phase;
    }
}
