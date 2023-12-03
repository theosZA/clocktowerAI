using Clocktower.Game;

namespace Clocktower.Agent
{
    public interface IAgent
    {
        void AssignCharacter(Character character, Alignment alignment);

        void Night(int nightNumber);
        void Day(int dayNumber);
        void PlayerDiedAtNight(Player newlyDeadPlayer);

        void MinionInformation(Player demon, IReadOnlyCollection<Player> fellowMinions);
        void DemonInformation(IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters);

        void NotifyGodfather(IReadOnlyCollection<Character> outsiders);
        void NotifySteward(Player goodPlayer);
        void NotifyLibrarian(Player playerA, Player playerB, Character character);
        void NotifyEmpath(Player neighbourA, Player neighbourB, int evilCount);
        void NotifyRavenkeeper(Player target, Character character);

        void RequestChoiceFromImp(IReadOnlyCollection<Player> players, Action<Player> onChoice);
        void RequestChoiceFromRavenkeeper(IReadOnlyCollection<Player> players, Action<Player> onChoice);
    }
}
