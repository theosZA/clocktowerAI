﻿using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Options;

namespace Clocktower.Agent
{
    public interface IAgent
    {
        string PlayerName { get; }

        IGameObserver Observer { get; }

        void StartGame();

        void AssignCharacter(Character character, Alignment alignment);
        void YouAreDead();

        void MinionInformation(Player demon, IReadOnlyCollection<Player> fellowMinions);
        void DemonInformation(IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters);

        void NotifyGodfather(IReadOnlyCollection<Character> outsiders);
        void NotifyLibrarian(Player playerA, Player playerB, Character character);
        void NotifyInvestigator(Player playerA, Player playerB, Character character);
        void NotifySteward(Player goodPlayer);
        void NotifyShugenja(Direction direction);

        void NotifyEmpath(Player neighbourA, Player neighbourB, int evilCount);
        void NotifyFortuneTeller(Player targetA, Player targetB, bool reading);
        void NotifyRavenkeeper(Player target, Character character);
        void NotifyUndertaker(Player executedPlayer, Character character);

        void ResponseForFisherman(string advice);
        void GainCharacterAbility(Character character);

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

        void StartPrivateChat(Player otherPlayer);
        Task<string> GetPrivateChat(Player listener);
        void PrivateChatMessage(Player speaker, string message);

        void EndPrivateChat(Player otherPlayer);
    }
}
