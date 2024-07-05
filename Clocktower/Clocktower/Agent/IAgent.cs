﻿using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Options;

namespace Clocktower.Agent
{
    public interface IAgent
    {
        string PlayerName { get; }

        IGameObserver Observer { get; }

        Task StartGame();

        Task AssignCharacter(Character character, Alignment alignment);
        Task YouAreDead();

        Task MinionInformation(Player demon, IReadOnlyCollection<Player> fellowMinions);
        Task DemonInformation(IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters);

        Task NotifyGodfather(IReadOnlyCollection<Character> outsiders);
        Task NotifyWasherwoman(Player playerA, Player playerB, Character character);
        Task NotifyLibrarian(Player playerA, Player playerB, Character character);
        Task NotifyLibrarianNoOutsiders();
        Task NotifyInvestigator(Player playerA, Player playerB, Character character);
        Task NotifySteward(Player goodPlayer);
        Task NotifyShugenja(Direction direction);

        Task NotifyEmpath(Player neighbourA, Player neighbourB, int evilCount);
        Task NotifyFortuneTeller(Player targetA, Player targetB, bool reading);
        Task NotifyRavenkeeper(Player target, Character character);
        Task NotifyUndertaker(Player executedPlayer, Character character);

        Task ResponseForFisherman(string advice);
        Task GainCharacterAbility(Character character);

        Task<IOption> RequestChoiceFromImp(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromPoisoner(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromAssassin(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromGodfather(IReadOnlyCollection<IOption> options);

        Task<IOption> RequestChoiceFromPhilosopher(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromFortuneTeller(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromMonk(IReadOnlyCollection<IOption> options);
        Task<IOption> RequestChoiceFromRavenkeeper(IReadOnlyCollection<IOption> options);
        Task<IOption> PromptSlayerShot(IReadOnlyCollection<IOption> options);
        Task<IOption> PromptFishermanAdvice(IReadOnlyCollection<IOption> options);

        Task<IOption> GetNomination(IReadOnlyCollection<IOption> options);
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
