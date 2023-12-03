using Clocktower.Game;

namespace Clocktower.Agent
{
    public interface IAgent
    {
        public void AssignCharacter(Character character, Alignment alignment);

        public void Night(int nightNumber);
        public void Day(int dayNumber);

        public void MinionInformation(Player demon, IReadOnlyCollection<Player> fellowMinions);
        public void DemonInformation(IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters);

        public void NotifyGodfather(IReadOnlyCollection<Character> outsiders);
        public void NotifySteward(Player goodPlayer);
        public void NotifyLibrarian(Player playerA, Player playerB, Character character);
        public void NotifyEmpath(Player neighbourA, Player neighbourB, int evilCount);

        public Task<Player> GetImpChoice(IReadOnlyCollection<Player> players);
    }
}
