using Clocktower.Agent;
using Clocktower.Game;

namespace Clocktower.Events
{
    internal class NotifyLibrarian : IGameEvent
    {
        public NotifyLibrarian(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public Task RunEvent()
        {
            var librarian = grimoire.GetAlivePlayer(Character.Librarian);
            if (librarian != null)
            {
                // For now we give them a hardcoded pair of players and the outsider 'Drunk'.
                var librarianTargetA = librarian.DrunkOrPoisoned ? grimoire.GetRequiredPlayer(Character.Imp) : grimoire.GetRequiredRealPlayer(Character.Drunk);
                var librarianTargetB = grimoire.GetRequiredPlayer(Character.Empath);
                librarian.Agent.NotifyLibrarian(librarianTargetA, librarianTargetB, Character.Drunk);
                storyteller.NotifyLibrarian(librarian, librarianTargetA, librarianTargetB, Character.Drunk);
            }

            return Task.CompletedTask;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
