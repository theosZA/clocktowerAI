using Clocktower.Game;
using Clocktower.Options;

namespace Clocktower.Agent
{
    public interface IAgent
    {
        void AssignCharacter(Character character, Alignment alignment);
        void YouAreDead();

        void MinionInformation(Player demon, IReadOnlyCollection<Player> fellowMinions);
        void DemonInformation(IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters);

        void NotifyGodfather(IReadOnlyCollection<Character> outsiders);
        void NotifyLibrarian(Player playerA, Player playerB, Character character);
        void NotifyInvestigator(Player playerA, Player playerB, Character character);
        void NotifySteward(Player goodPlayer);
        void NotifyShugenja(bool clockwise);

        void NotifyEmpath(Player neighbourA, Player neighbourB, int evilCount);
        void NotifyFortuneTeller(Player targetA, Player targetB, bool reading);
        void NotifyRavenkeeper(Player target, Character character);
        void NotifyUndertaker(Player executedPlayer, Character character);

        Task<IOption> RequestChoiceFromImp(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromPoisoner(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromAssassin(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromGodfather(IReadOnlyCollection<IOption> options);

        Task<IOption> RequestChoiceFromFortuneTeller(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromMonk(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromRavenkeeper(IReadOnlyCollection<IOption> options);
        Task<IOption> PromptSlayerShot(IReadOnlyCollection<IOption> options);

        Task<IOption> GetNomination(IReadOnlyCollection<IOption> options);
        Task<IOption> GetVote(IReadOnlyCollection<IOption> options);
    }
}
