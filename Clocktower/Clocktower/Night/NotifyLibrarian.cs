using Clocktower.Agent;
using Clocktower.Game;

namespace Clocktower.Night
{
    internal class NotifyLibrarian : INightEvent
    {
        public NotifyLibrarian(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public void RunEvent(Action onEventFinished)
        {
            var librarian = grimoire.GetPlayer(Character.Librarian);
            if (librarian != null)
            {
                // For now we give them a hardcoded pair of players and the outsider 'Drunk'.
                var librarianTargetA = librarian.DrunkOrPoisoned ? grimoire.GetRequiredPlayer(Character.Imp) : grimoire.GetRequiredRealPlayer(Character.Drunk);
                var librarianTargetB = grimoire.GetRequiredPlayer(Character.Empath);
                librarian.Agent.NotifyLibrarian(librarianTargetA, librarianTargetB, Character.Drunk);
                storyteller.NotifyLibrarian(librarian, librarianTargetA, librarianTargetB, Character.Drunk);
            }

            onEventFinished();
        }

        private IStoryteller storyteller;
        private Grimoire grimoire;
    }
}
