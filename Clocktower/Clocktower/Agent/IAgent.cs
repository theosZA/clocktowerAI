﻿using Clocktower.Agent.Observer;
using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Selection;

namespace Clocktower.Agent
{
    public interface IAgent
    {
        string PlayerName { get; }

        IGameObserver Observer { get; }

        Task StartGame();
        Task EndGame();

        Task AssignCharacter(Character character, Alignment alignment);
        Task ChangeAlignment(Alignment alignment);
        Task YouAreDead();

        Task MinionInformation(Player demon, IReadOnlyCollection<Player> fellowMinions, bool damselInPlay, IReadOnlyCollection<Character> notInPlayCharacters);
        Task DemonInformation(IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters);

        Task NotifyGodfather(IReadOnlyCollection<Character> outsiders);
        Task NotifyWasherwoman(Player playerA, Player playerB, Character character);
        Task NotifyLibrarian(Player playerA, Player playerB, Character character);
        Task NotifyLibrarianNoOutsiders();
        Task NotifyInvestigator(Player playerA, Player playerB, Character character);
        Task NotifyChef(int evilPairCount);
        Task NotifySteward(Player goodPlayer);
        Task NotifyBountyHunter(Player evilPlayer);
        Task NotifyNoble(IReadOnlyCollection<Player> nobleInformation);
        Task NotifyShugenja(Direction direction);

        Task NotifyEmpath(Player neighbourA, Player neighbourB, int evilCount);
        Task NotifyOracle(int evilCount);
        Task NotifyFortuneTeller(Player targetA, Player targetB, bool reading);
        Task NotifyRavenkeeper(Player target, Character character);
        Task NotifyUndertaker(Player executedPlayer, Character character);
        Task NotifyBalloonist(Player newPlayer);
        Task NotifyHighPriestess(Player player);
        Task NotifyJuggler(int jugglerCount);

        Task ShowGrimoire(Character character, Grimoire grimoire);
        Task ShowNightwatchman(Player nightwatchman);
        Task LearnOfWidow();
        Task ResponseForFisherman(string advice);
        Task OnGainCharacterAbility(Character character);

        Task RequestSelectionOfKazaliMinions(KazaliMinionsSelection kazaliMinionsSelection);
        Task<IOption> RequestNewKazaliMinion(Player minionTarget, Character unavailableMinionCharacter, IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceOfMinionForSoldierSelectedByKazali(IReadOnlyCollection<IOption> options);

        Task<IOption> RequestChoiceFromDemon(Character demonCharacter, IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromPukka(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromOjo(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromPoisoner(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromWidow(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromWitch(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromAssassin(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromGodfather(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromDevilsAdvocate(IReadOnlyCollection<IOption> options);

        Task<IOption> RequestChoiceFromPhilosopher(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromNightwatchman(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromHuntsman(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromFortuneTeller(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromSnakeCharmer(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromMonk(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromLycanthrope(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromRavenkeeper(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromButler(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromOgre(IReadOnlyCollection<IOption> options);
        Task<IOption> PromptFishermanAdvice(IReadOnlyCollection<IOption> options);

        Task<IOption> PromptShenanigans(IReadOnlyCollection<IOption> options, bool afterNominations, Player? playerOnTheBlock);

        Task<IOption> GetNomination(Player? playerOnTheBlock, int? votesToTie, int? votesToPutOnBlock, IReadOnlyCollection<IOption> options);
        Task<IOption> GetVote(IReadOnlyCollection<IOption> options, bool ghostVote);
        Task<IOption> OfferPrivateChat(IReadOnlyCollection<IOption> options);

        Task<string> GetRollCallStatement();
        Task<string> GetMorningPublicStatement();
        Task<string> GetEveningPublicStatement();
        Task<string> GetProsecution(Player nominee);
        Task<string> GetDefence(Player nominator);
        Task<string> GetReasonForSelfNomination();

        Task StartPrivateChat(Player otherPlayer);
        Task<(string message, bool endChat)> GetPrivateChat(Player listener);
        Task PrivateChatMessage(Player speaker, string message);

        Task EndPrivateChat(Player otherPlayer);
    }
}
