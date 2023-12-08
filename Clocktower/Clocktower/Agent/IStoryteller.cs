using Clocktower.Game;
using Clocktower.Options;

namespace Clocktower.Agent
{
    public interface IStoryteller
    {
        Task<IOption> GetNewImp(IReadOnlyCollection<IOption> impCandidates);
        Task<IOption> GetDrunk(IReadOnlyCollection<IOption> drunkCandidates);
        Task<IOption> GetSweetheartDrunk(IReadOnlyCollection<IOption> drunkCandidates);
        Task<IOption> GetFortuneTellerRedHerring(IReadOnlyCollection<IOption> redHerringCandidates);
        Task<IOption> GetStewardPing(Player steward, IReadOnlyCollection<IOption> stewardPingCandidates);
        Task<IOption> GetEmpathNumber(Player empath, Player neighbourA, Player neighbourB, IReadOnlyCollection<IOption> empathOptions);
        Task<IOption> GetFortuneTellerReading(Player empath, Player targetA, Player targetB, IReadOnlyCollection<IOption> fortuneTellerOptions);
        Task<IOption> GetShugenjaDirection(Player shugenja, Grimoire grimoire, IReadOnlyCollection<IOption> shugenjaOptions);
        Task<IOption> GetCharacterForRavenkeeper(Player ravenkeeper, Player target, IReadOnlyCollection<IOption> ravenkeeperOptions);
        Task<IOption> GetCharacterForUndertaker(Player undertaker, Player executedPlayer, IReadOnlyCollection<IOption> undertakerOptions);
        Task<IOption> ShouldKillTinker(Player tinker, IReadOnlyCollection<IOption> yesOrNo);

        void AssignCharacter(Player player);

        void MinionInformation(Player minion, Player demon, IReadOnlyCollection<Player> fellowMinions);
        void DemonInformation(Player demon, IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters);

        void NotifyGodfather(Player godfather, IReadOnlyCollection<Character> outsiders);
        void NotifyLibrarian(Player librarian, Player playerA, Player playerB, Character character);
        void NotifyInvestigator(Player investigator, Player playerA, Player playerB, Character character);
        void NotifySteward(Player steward, Player goodPlayer);
        void NotifyShugenja(Player shugenja, bool clockwise);
        void NotifyEmpath(Player empath, Player neighbourA, Player neighbourB, int evilCount);
        void NotifyUndertaker(Player undertaker, Player executedPlayer, Character executedCharacter);

        void ChoiceFromImp(Player imp, Player target);
        void ChoiceFromPoisoner(Player poisoner, Player target);
        void ChoiceFromAssassin(Player assassin, Player? target);
        void ChoiceFromGodfather(Player godfather, Player target);
        void ChoiceFromMonk(Player monk, Player target);
        void ChoiceFromRavenkeeper(Player ravenkeeper, Player target, Character character);

        void ScarletWomanTrigger(Player demon, Player scarletWoman);
    }
}
