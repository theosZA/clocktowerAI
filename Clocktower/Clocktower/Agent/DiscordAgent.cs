using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Options;
using DiscordChatBot;

namespace Clocktower.Agent
{
    internal class DiscordAgent : IAgent
    {
        public string PlayerName { get; private init; }

        public IGameObserver Observer => observer;

        public DiscordAgent(ChatClient chatClient, string playerName, IReadOnlyCollection<string> players, string scriptName, IReadOnlyCollection<Character> script)
        {
            this.chatClient = chatClient;
            PlayerName = playerName;
            this.players = players;
            this.scriptName = scriptName;
            this.script = script;
        }

        public async Task StartGame()
        {
            chat = await chatClient.CreateChat(PlayerName);

            observer.Start(chat);

            await chat.SendMessage($"Welcome {PlayerName} to a game of Blood on the Clocktower.");
            await chat.SendMessage(TextBuilder.ScriptToText(scriptName, script));
            await chat.SendMessage(TextBuilder.SetupToText(players.Count));
            await chat.SendMessage(TextBuilder.PlayersToText(players));
        }

        public void AssignCharacter(Character character, Alignment alignment)
        {
            throw new NotImplementedException();
        }

        public void YouAreDead()
        {
            throw new NotImplementedException();
        }

        public void MinionInformation(Player demon, IReadOnlyCollection<Player> fellowMinions)
        {
            throw new NotImplementedException();
        }

        public void DemonInformation(IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            throw new NotImplementedException();
        }

        public void NotifyGodfather(IReadOnlyCollection<Character> outsiders)
        {
            throw new NotImplementedException();
        }

        public void NotifyLibrarian(Player playerA, Player playerB, Character character)
        {
            throw new NotImplementedException();
        }

        public void NotifyLibrarianNoOutsiders()
        {
            throw new NotImplementedException();
        }

        public void NotifyInvestigator(Player playerA, Player playerB, Character character)
        {
            throw new NotImplementedException();
        }

        public void NotifySteward(Player goodPlayer)
        {
            throw new NotImplementedException();
        }

        public void NotifyShugenja(Direction direction)
        {
            throw new NotImplementedException();
        }

        public void NotifyEmpath(Player neighbourA, Player neighbourB, int evilCount)
        {
            throw new NotImplementedException();
        }

        public void NotifyFortuneTeller(Player targetA, Player targetB, bool reading)
        {
            throw new NotImplementedException();
        }

        public void NotifyRavenkeeper(Player target, Character character)
        {
            throw new NotImplementedException();
        }

        public void NotifyUndertaker(Player executedPlayer, Character character)
        {
            throw new NotImplementedException();
        }

        public void ResponseForFisherman(string advice)
        {
            throw new NotImplementedException();
        }

        public void GainCharacterAbility(Character character)
        {
            throw new NotImplementedException();
        }

        public Task<IOption> RequestChoiceFromImp(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public Task<IOption> RequestChoiceFromPoisoner(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public Task<IOption> RequestChoiceFromAssassin(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public Task<IOption> RequestChoiceFromGodfather(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public Task<IOption> RequestChoiceFromPhilosopher(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public Task<IOption> RequestChoiceFromFortuneTeller(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public Task<IOption> RequestChoiceFromMonk(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public Task<IOption> RequestChoiceFromRavenkeeper(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public Task<IOption> PromptSlayerShot(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public Task<IOption> PromptFishermanAdvice(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public Task<IOption> GetNomination(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public Task<IOption> GetVote(IReadOnlyCollection<IOption> options, bool ghostVote)
        {
            throw new NotImplementedException();
        }

        public Task<IOption> OfferPrivateChat(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRollCallStatement()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetMorningPublicStatement()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetEveningPublicStatement()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetProsecution(Player nominee)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetDefence(Player nominator)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetReasonForSelfNomination()
        {
            throw new NotImplementedException();
        }

        public void StartPrivateChat(Player otherPlayer)
        {
            throw new NotImplementedException();
        }

        public Task<(string message, bool endChat)> GetPrivateChat(Player listener)
        {
            throw new NotImplementedException();
        }

        public void PrivateChatMessage(Player speaker, string message)
        {
            throw new NotImplementedException();
        }

        public void EndPrivateChat(Player otherPlayer)
        {
            throw new NotImplementedException();
        }

        private readonly ChatClient chatClient;
        private Chat? chat;
        private readonly DiscordChatObserver observer = new();

        private IReadOnlyCollection<string> players;
        private string scriptName;
        private IReadOnlyCollection<Character> script;
    }
}
