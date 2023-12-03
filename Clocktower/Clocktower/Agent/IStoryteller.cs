using Clocktower.Game;

namespace Clocktower.Agent
{
    internal interface IStoryteller
    {
        public void AssignCharacter(string name, Character character, Alignment alignment);
        public void AssignCharacter(string name, Character realCharacter, Alignment realAlignment,
                                                 Character believedCharacter, Alignment believedAlignment);

        public void Night(int nightNumber);
        public void Day(int dayNumber);
        public void PlayerDiedAtNight(Player newlyDeadPlayer);

        public void MinionInformation(Player minion, Player demon, IReadOnlyCollection<Player> fellowMinions);
        public void DemonInformation(Player demon, IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters);

        public void NotifyGodfather(Player godfather, IReadOnlyCollection<Character> outsiders);
        public void NotifySteward(Player steward, Player goodPlayer);
        public void NotifyLibrarian(Player librarian, Player playerA, Player playerB, Character character);
        public void NotifyEmpath(Player empath, Player neighbourA, Player neighbourB, int evilCount);

        public void ChoiceFromImp(Player imp, Player target);
    }
}
