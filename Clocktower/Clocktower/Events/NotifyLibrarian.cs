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
            foreach (var librarian in grimoire.GetLivingPlayers(Character.Librarian))
            {
                var librarianOther = grimoire.Players.First(player => player != librarian && player.CharacterType != CharacterType.Outsider);
                var librarianOutsider = librarian.DrunkOrPoisoned ? grimoire.Players.Last(player => player != librarian && player.CharacterType != CharacterType.Outsider)
                                                                 : grimoire.Players.First(player => player.CharacterType == CharacterType.Outsider);
                var character = librarian.DrunkOrPoisoned || librarianOutsider.Tokens.Contains(Token.IsTheDrunk) ? Character.Drunk
                                                                                                                 : librarianOutsider.Character;
                librarian.Agent.NotifyLibrarian(librarianOther, librarianOutsider, character);
                storyteller.NotifyLibrarian(librarian, librarianOther, librarianOutsider, character);
            }

            return Task.CompletedTask;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
