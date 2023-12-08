using Clocktower.Game;
using Clocktower.Options;

namespace Clocktower.Agent
{
    public interface IStoryteller
    {
        Task<IOption> GetDrunk(IReadOnlyCollection<IOption> drunkCandidates);
        Task<IOption> GetFortuneTellerRedHerring(IReadOnlyCollection<IOption> redHerringCandidates);
        Task<IOption> GetStewardPing(Player steward, IReadOnlyCollection<IOption> stewardPingCandidates);
        Task<IOption> GetEmpathNumber(Player empath, Player neighbourA, Player neighbourB, IReadOnlyCollection<IOption> empathOptions);
        Task<IOption> GetFortuneTellerReading(Player empath, Player targetA, Player targetB, IReadOnlyCollection<IOption> fortuneTellerOptions);
        Task<IOption> GetShugenjaDirection(Player shugenja, Grimoire grimoire, IReadOnlyCollection<IOption> shugenjaOptions);

        void AssignCharacter(Player player);

        void MinionInformation(Player minion, Player demon, IReadOnlyCollection<Player> fellowMinions);
        void DemonInformation(Player demon, IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters);

        void NotifyGodfather(Player godfather, IReadOnlyCollection<Character> outsiders);
        void NotifyLibrarian(Player librarian, Player playerA, Player playerB, Character character);
        void NotifyInvestigator(Player investigator, Player playerA, Player playerB, Character character);
        void NotifySteward(Player steward, Player goodPlayer);
        void NotifyShugenja(Player shugenja, bool clockwise);
        void NotifyEmpath(Player empath, Player neighbourA, Player neighbourB, int evilCount);

        void ChoiceFromImp(Player imp, Player target);
        void ChoiceFromAssassin(Player assassin, Player? target);
        void ChoiceFromGodfather(Player godfather, Player target);
        void ChoiceFromRavenkeeper(Player ravenkeeper, Player target, Character character);
    }
}
