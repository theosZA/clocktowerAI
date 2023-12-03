using Clocktower.Game;

namespace Clocktower.Agent
{
    internal interface IStoryteller
    {
        void AssignCharacter(string name, Character character, Alignment alignment);
        void AssignCharacter(string name, Character realCharacter, Alignment realAlignment,
                                                 Character believedCharacter, Alignment believedAlignment);

        void Night(int nightNumber);
        void Day(int dayNumber);
        void PlayerDiedAtNight(Player newlyDeadPlayer);

        void MinionInformation(Player minion, Player demon, IReadOnlyCollection<Player> fellowMinions);
        void DemonInformation(Player demon, IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters);

        void NotifyGodfather(Player godfather, IReadOnlyCollection<Character> outsiders);
        void NotifySteward(Player steward, Player goodPlayer);
        void NotifyLibrarian(Player librarian, Player playerA, Player playerB, Character character);
        void NotifyEmpath(Player empath, Player neighbourA, Player neighbourB, int evilCount);

        void ChoiceFromImp(Player imp, Player target);
        void ChoiceFromRavenkeeper(Player ravenkeeper, Player target, Character character);
    }
}
