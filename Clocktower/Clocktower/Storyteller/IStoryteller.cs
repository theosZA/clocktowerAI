using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Options;

namespace Clocktower.Storyteller
{
    public interface IStoryteller
    {
        IGameObserver Observer { get; }

        void Start();

        void PrivateChatMessage(Player speaker, Player listener, string message);

        Task<IOption> GetDemonBluffs(Player demon, IReadOnlyCollection<IOption> demonBluffOptions);
        Task<IOption> GetNewImp(IReadOnlyCollection<IOption> impCandidates);
        Task<IOption> GetDrunk(IReadOnlyCollection<IOption> drunkCandidates);
        Task<IOption> GetSweetheartDrunk(IReadOnlyCollection<IOption> drunkCandidates);
        Task<IOption> GetFortuneTellerRedHerring(Player fortuneTeller, IReadOnlyCollection<IOption> redHerringCandidates);
        Task<IOption> GetInvestigatorPings(Player investigator, IReadOnlyCollection<IOption> investigatorPingCandidates);
        Task<IOption> GetLibrarianPings(Player librarian, IReadOnlyCollection<IOption> librarianPingCandidates);
        Task<IOption> GetStewardPing(Player steward, IReadOnlyCollection<IOption> stewardPingCandidates);
        Task<IOption> GetEmpathNumber(Player empath, Player neighbourA, Player neighbourB, IReadOnlyCollection<IOption> empathOptions);
        Task<IOption> GetFortuneTellerReading(Player fortuneTeller, Player targetA, Player targetB, IReadOnlyCollection<IOption> fortuneTellerOptions);
        Task<IOption> GetShugenjaDirection(Player shugenja, Grimoire grimoire, IReadOnlyCollection<IOption> shugenjaOptions);
        Task<IOption> GetCharacterForRavenkeeper(Player ravenkeeper, Player target, IReadOnlyCollection<IOption> ravenkeeperOptions);
        Task<IOption> GetCharacterForUndertaker(Player undertaker, Player executedPlayer, IReadOnlyCollection<IOption> undertakerOptions);
        Task<IOption> ShouldKillTinker(Player tinker, IReadOnlyCollection<IOption> yesOrNo);
        Task<IOption> ShouldKillWithSlayer(Player slayer, Player target, IReadOnlyCollection<IOption> yesOrNo);
        Task<string> GetFishermanAdvice(Player fisherman);

        void AssignCharacter(Player player);

        void MinionInformation(Player minion, Player demon, IReadOnlyCollection<Player> fellowMinions);
        void DemonInformation(Player demon, IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters);

        void NotifyGodfather(Player godfather, IReadOnlyCollection<Character> outsiders);
        void NotifyLibrarian(Player librarian, Player playerA, Player playerB, Character character);
        void NotifyLibrarianNoOutsiders(Player librarian);
        void NotifyInvestigator(Player investigator, Player playerA, Player playerB, Character character);
        void NotifySteward(Player steward, Player goodPlayer);
        void NotifyShugenja(Player shugenja, Direction direction);
        void NotifyEmpath(Player empath, Player neighbourA, Player neighbourB, int evilCount);
        void NotifyFortuneTeller(Player fortuneTeller, Player targetA, Player targetB, bool reading);
        void NotifyUndertaker(Player undertaker, Player executedPlayer, Character executedCharacter);

        void ChoiceFromImp(Player imp, Player target);
        void ChoiceFromPoisoner(Player poisoner, Player target);
        void ChoiceFromAssassin(Player assassin, Player? target);
        void ChoiceFromGodfather(Player godfather, Player target);
        void ChoiceFromMonk(Player monk, Player target);
        void ChoiceFromRavenkeeper(Player ravenkeeper, Player target, Character character);
        void ChoiceFromPhilosopher(Player philosopher, Player? philosopherDrunkedPlayer, Character newCharacterAbility);

        void ScarletWomanTrigger(Player demon, Player scarletWoman);
    }
}
