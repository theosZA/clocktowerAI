﻿using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Options;

namespace Clocktower.Storyteller
{
    public interface IStoryteller
    {
        IGameObserver Observer { get; }

        void Start();

        void PrivateChatMessage(Player speaker, Player listener, string message);

        Task<IOption> GetMarionette(IReadOnlyCollection<IOption> marionetteCandidates);
        Task<IOption> GetDemonBluffs(Player demon, IReadOnlyCollection<IOption> demonBluffOptions);
        Task<IOption> GetAdditionalDemonBluffs(Player demon, Player snitch, IReadOnlyCollection<IOption> demonBluffOptions);
        Task<IOption> GetMinionBluffs(Player minion, IReadOnlyCollection<IOption> minionBluffOptions);
        Task<IOption> GetNewImp(IReadOnlyCollection<IOption> impCandidates);
        Task<IOption> GetOjoVictims(Player ojo, Character targetCharacter, IReadOnlyCollection<IOption> victimOptions);

        Task<IOption> GetDrunk(IReadOnlyCollection<IOption> drunkCandidates);
        Task<IOption> GetSweetheartDrunk(IReadOnlyCollection<IOption> drunkCandidates);
        Task<IOption> GetFortuneTellerRedHerring(Player fortuneTeller, IReadOnlyCollection<IOption> redHerringCandidates);
        Task<IOption> GetWasherwomanPings(Player washerwoman, IReadOnlyCollection<IOption> washerwomanPingCandidates);
        Task<IOption> GetInvestigatorPings(Player investigator, IReadOnlyCollection<IOption> investigatorPingCandidates);
        Task<IOption> GetLibrarianPings(Player librarian, IReadOnlyCollection<IOption> librarianPingCandidates);
        Task<IOption> GetStewardPing(Player steward, IReadOnlyCollection<IOption> stewardPingCandidates);
        Task<IOption> GetNobleInformation(Player noble, IReadOnlyCollection<IOption> nobleInformationOptions);
        Task<IOption> GetChefNumber(Player chef, IEnumerable<Player> playersThatCanMisregister, IReadOnlyCollection<IOption> chefOptions);
        Task<IOption> GetEmpathNumber(Player empath, Player neighbourA, Player neighbourB, IReadOnlyCollection<IOption> empathOptions);
        Task<IOption> GetJugglerNumber(Player juggler, int realJugglerNumber, IReadOnlyCollection<IOption> jugglerOptions);
        Task<IOption> GetFortuneTellerReading(Player fortuneTeller, Player targetA, Player targetB, IReadOnlyCollection<IOption> fortuneTellerOptions);
        Task<IOption> GetShugenjaDirection(Player shugenja, Grimoire grimoire, IReadOnlyCollection<IOption> shugenjaOptions);
        Task<IOption> GetCharacterForRavenkeeper(Player ravenkeeper, Player target, IReadOnlyCollection<IOption> ravenkeeperOptions);
        Task<IOption> GetCharacterForUndertaker(Player undertaker, Player executedPlayer, IReadOnlyCollection<IOption> undertakerOptions);
        Task<IOption> GetMayorBounce(Player mayor, Player? killer, IReadOnlyCollection<IOption> mayorOptions);
        Task<IOption> ShouldKillTinker(Player tinker, IReadOnlyCollection<IOption> yesOrNo);
        Task<IOption> ShouldKillWithSlayer(Player slayer, Player target, IReadOnlyCollection<IOption> yesOrNo);
        Task<IOption> ShouldSaveWithPacifist(Player pacifist, Player executedPlayer, IReadOnlyCollection<IOption> yesOrNo);
        Task<IOption> ShouldExecuteWithVirgin(Player virgin, Player nominator, IReadOnlyCollection<IOption> yesOrNo);
        Task<IOption> ShouldRegisterForJuggle(Player juggler, Player juggledPlayer, Character juggledCharacter, IReadOnlyCollection<IOption> yesOrNo);
        Task<string> GetFishermanAdvice(Player fisherman);
        Task<IOption> ChooseFakeCannibalAbility(Player cannibal, Player executedPlayer, IReadOnlyCollection<IOption> characterAbilityOptions);

        void AssignCharacter(Player player);

        void MinionInformation(Player minion, Player demon, IReadOnlyCollection<Player> fellowMinions, IReadOnlyCollection<Character> notInPlayCharacters);
        void DemonInformation(Player demon, IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters);

        void NotifyGodfather(Player godfather, IReadOnlyCollection<Character> outsiders);
        void NotifyWasherwoman(Player washerwoman, Player playerA, Player playerB, Character character);
        void NotifyLibrarian(Player librarian, Player playerA, Player playerB, Character character);
        void NotifyLibrarianNoOutsiders(Player librarian);
        void NotifyInvestigator(Player investigator, Player playerA, Player playerB, Character character);
        void NotifyChef(Player chef, int evilPairCount);
        void NotifySteward(Player steward, Player goodPlayer);
        void NotifyNoble(Player noble, IReadOnlyCollection<Player> nobleInformation);
        void NotifyShugenja(Player shugenja, Direction direction);
        void NotifyEmpath(Player empath, Player neighbourA, Player neighbourB, int evilCount);
        void NotifyFortuneTeller(Player fortuneTeller, Player targetA, Player targetB, bool reading);
        void NotifyUndertaker(Player undertaker, Player executedPlayer, Character executedCharacter);
        void NotifyJuggler(Player juggler, int jugglerCount);
        void ShowGrimoireToSpy(Player spy, Grimoire grimoire);

        void ChoiceFromDemon(Player demon, Player target);
        void ChoiceFromOjo(Player ojo, Character targetCharacter, IReadOnlyCollection<Player> victims);
        void ChoiceFromPoisoner(Player poisoner, Player target);
        void ChoiceFromWitch(Player witch, Player target);
        void ChoiceFromAssassin(Player assassin, Player? target);
        void ChoiceFromGodfather(Player godfather, Player target);
        void ChoiceFromDevilsAdvocate(Player devilsAdvocate, Player target);
        void ChoiceFromMonk(Player monk, Player target);
        void ChoiceFromRavenkeeper(Player ravenkeeper, Player target, Character character);
        void ChoiceFromPhilosopher(Player philosopher, Player? philosopherDrunkedPlayer, Character newCharacterAbility);
        void ChoiceFromButler(Player butler, Player target);

        void ScarletWomanTrigger(Player demon, Player scarletWoman);
    }
}
