using Clocktower.Agent.Notifier;
using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Options;
using DiscordChatBot;
using System.Text;

namespace Clocktower.Agent
{
    internal class DiscordAgent : IAgent
    {
        public string PlayerName { get; private init; }

        public IGameObserver Observer { get; private init; }

        public DiscordAgent(ChatClient chatClient, string playerName, IReadOnlyCollection<string> players, string scriptName, IReadOnlyCollection<Character> script)
        {
            var discordNotifier = new DiscordNotifier(chatClient, OnStartGame);
            Observer = new TextObserver(discordNotifier);
            textAgent = new TextAgent(playerName, players, scriptName, script, Observer, discordNotifier);
            prompter = new(playerName);
            PlayerName = playerName;
            this.players = players;
            this.scriptName = scriptName;
            this.script = script;
        }

        public async Task StartGame()
        {
            await textAgent.StartGame();
        }

        public async Task AssignCharacter(Character character, Alignment alignment)
        {
            await textAgent.AssignCharacter(character, alignment);
        }

        public async Task YouAreDead()
        {
            await textAgent.YouAreDead();
        }

        public async Task MinionInformation(Player demon, IReadOnlyCollection<Player> fellowMinions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            await textAgent.MinionInformation(demon, fellowMinions, notInPlayCharacters);
        }

        public async Task DemonInformation(IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            await textAgent.DemonInformation(minions, notInPlayCharacters);
        }

        public async Task NotifyGodfather(IReadOnlyCollection<Character> outsiders)
        {
            await textAgent.NotifyGodfather(outsiders);
        }

        public async Task NotifyWasherwoman(Player playerA, Player playerB, Character character)
        {
            await textAgent.NotifyWasherwoman(playerA, playerB, character);
        }

        public async Task NotifyLibrarian(Player playerA, Player playerB, Character character)
        {
            await textAgent.NotifyLibrarian(playerA, playerB, character);
        }

        public async Task NotifyLibrarianNoOutsiders()
        {
            await textAgent.NotifyLibrarianNoOutsiders();
        }

        public async Task NotifyInvestigator(Player playerA, Player playerB, Character character)
        {
            await textAgent.NotifyInvestigator(playerA, playerB, character);
        }

        public async Task NotifyChef(int evilPairCount)
        {
            await textAgent.NotifyChef(evilPairCount);
        }

        public async Task NotifySteward(Player goodPlayer)
        {
            await textAgent.NotifySteward(goodPlayer);
        }

        public async Task NotifyNoble(IReadOnlyCollection<Player> nobleInformation)
        {
            await textAgent.NotifyNoble(nobleInformation);
        }

        public async Task NotifyShugenja(Direction direction)
        {
            await textAgent.NotifyShugenja(direction);
        }

        public async Task NotifyEmpath(Player neighbourA, Player neighbourB, int evilCount)
        {
            await textAgent.NotifyEmpath(neighbourA, neighbourB, evilCount);
        }

        public async Task NotifyFortuneTeller(Player targetA, Player targetB, bool reading)
        {
            await textAgent.NotifyFortuneTeller(targetA, targetB, reading);
        }

        public async Task NotifyRavenkeeper(Player target, Character character)
        {
            await textAgent.NotifyRavenkeeper(target, character);
        }

        public async Task NotifyUndertaker(Player executedPlayer, Character character)
        {
            await textAgent.NotifyUndertaker(executedPlayer, character);
        }

        public async Task NotifyBalloonist(Player newPlayer)
        {
            await textAgent.NotifyBalloonist(newPlayer);
        }

        public async Task NotifyJuggler(int jugglerCount)
        {
            await textAgent.NotifyJuggler(jugglerCount);
        }

        public async Task ShowGrimoireToSpy(Grimoire grimoire)
        {
            await textAgent.ShowGrimoireToSpy(grimoire);
        }

        public async Task ResponseForFisherman(string advice)
        {
            await textAgent.ResponseForFisherman(advice);
        }

        public async Task GainCharacterAbility(Character character)
        {
            this.characterAbility = character;
            await textAgent.GainCharacterAbility(character);
        }

        public async Task<IOption> RequestChoiceFromDemon(Character demonCharacter, IReadOnlyCollection<IOption> options)
        {
            return await prompter.RequestChoice(options, "As the %c please choose a player to kill.", demonCharacter);
        }

        public async Task<IOption> RequestChoiceFromOjo(IReadOnlyCollection<IOption> options)
        {
            return await prompter.RequestChoice(options, "As the %c please choose a _character_ to kill.", Character.Ojo);
        }

        public async Task<IOption> RequestChoiceFromPoisoner(IReadOnlyCollection<IOption> options)
        {
            return await prompter.RequestChoice(options, "As the %c please choose a player to poison.", Character.Poisoner);
        }

        public async Task<IOption> RequestChoiceFromWitch(IReadOnlyCollection<IOption> options)
        {
            return await prompter.RequestChoice(options, "As the %c please choose a player to curse.", Character.Witch);
        }

        public async Task<IOption> RequestChoiceFromAssassin(IReadOnlyCollection<IOption> options)
        {
            return await prompter.RequestChoice(options, "As the %c, you may use your once-per-game ability tonight to kill a player. Respond with the name of a player to use the ability, or `PASS` if you want to save your ability for later.", Character.Assassin);
        }

        public async Task<IOption> RequestChoiceFromGodfather(IReadOnlyCollection<IOption> options)
        {
            return await prompter.RequestChoice(options, "As the %c please choose a player to kill.", Character.Godfather);
        }

        public async Task<IOption> RequestChoiceFromDevilsAdvocate(IReadOnlyCollection<IOption> options)
        {
            return await prompter.RequestChoice(options, "As the %c please choose a player to protect from execution.", Character.Devils_Advocate);
        }

        public async Task<IOption> RequestChoiceFromPhilosopher(IReadOnlyCollection<IOption> options)
        {
            if (characterAbility == Character.Cannibal)
            {
                bool useAbility = await prompter.RequestChoice(OptionsBuilder.YesOrNo, "Do you wish to use your ability tonight?") is YesOption;
                if (!useAbility)
                {
                    return options.First(option => option is PassOption);
                }
                return await prompter.RequestChoice(options.Where(option => option is not PassOption).ToList(), "Please choose a Townsfolk or Outsider character...");
            }

            return await prompter.RequestChoice(options, "As the %c, do you wish to use your ability tonight? Respond with the Townsfolk or Outsider character whose ability you wish to acquire, or `PASS` if you want to save your ability for later.", Character.Philosopher);
        }

        public async Task<IOption> RequestChoiceFromFortuneTeller(IReadOnlyCollection<IOption> options)
        {
            if (characterAbility == Character.Cannibal)
            {
                return await prompter.RequestChoice(options, "Please choose two players. Respond in the form: *PLAYER1* and *PLAYER2*");
            }

            return await prompter.RequestChoice(options, "As the %c please choose two players. Respond in the form: *PLAYER1* and *PLAYER2*", Character.Fortune_Teller);
        }

        public async Task<IOption> RequestChoiceFromMonk(IReadOnlyCollection<IOption> options)
        {
            if (characterAbility == Character.Cannibal)
            {
                return await prompter.RequestChoice(options, "Please choose a player.");
            }

            return await prompter.RequestChoice(options, "As the %c please choose a player to protect from the demon tonight.", Character.Monk);
        }

        public async Task<IOption> RequestChoiceFromRavenkeeper(IReadOnlyCollection<IOption> options)
        {
            if (characterAbility == Character.Cannibal)
            {
                return await prompter.RequestChoice(options, "Please choose a player.");
            }

            return await prompter.RequestChoice(options, "As the %c please choose a player whose character you wish to learn.", Character.Ravenkeeper);
        }

        public async Task<IOption> RequestChoiceFromButler(IReadOnlyCollection<IOption> options)
        {
            return await prompter.RequestChoice(options, "As the %c please choose a player. Tomorrow, you will only be able vote on a nomination if they have already voted for that nomination.", Character.Butler);
        }

        public async Task<IOption> PromptFishermanAdvice(IReadOnlyCollection<IOption> options)
        {
            return await prompter.RequestChoice(options, "Do you wish to go now to the Storyteller for your %c advice rather than saving it for later?", Character.Fisherman);
        }

        public async Task<IOption> PromptShenanigans(IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder();
            var objects = new List<object>();

            sb.AppendLine("You have the option now to use or bluff any abilities that are to be publicly used during the day.");
            if (options.Any(option => option is SlayerShotOption))
            {
                sb.AppendLine("- `Slayer: player_name` if you wish to claim %c and target the specified player.");
                objects.Add(Character.Slayer);
            }
            if (options.Any(option => option is JugglerOption))
            {
                sb.AppendLine("- `Juggler: player_name as character, player_name as character, ...` with up to 5 player-character pairs if you wish to claim %c and guess players as specific characters. (Players and characters may be repeated");
                objects.Add(Character.Juggler);
            }
            if (options.Any(option => option is PassOption))
            {
                sb.AppendLine("- `PASS` if you don't wish to use or bluff any of these abilities.");
            }
            if (options.Any(option => option is AlwaysPassOption))
            {
                sb.AppendLine("- `ALWAYS PASS` if you never wish to bluff any of these abilities (though you'll still be prompted if you do have an ability you can use).");
            }

            return await prompter.RequestShenanigans(options, sb.ToString(), objects);
        }

        public async Task<IOption> GetNomination(IReadOnlyCollection<IOption> options)
        {
            return await prompter.RequestChoice(options, "You may nominate a player. Either provide the name of the player you wish to nominate or respond with `PASS`. The players you can nominate are: %P.",
                                                options.Where(option => option is PlayerOption)
                                                       .Select(option => ((PlayerOption)option).Player));
        }

        public async Task<IOption> GetVote(IReadOnlyCollection<IOption> options, bool ghostVote)
        {
            var voteOption = (VoteOption)options.First(option => option is VoteOption);
            return await prompter.RequestChoice(options, "If you wish, you may vote for executing %p. %nRespond with `EXECUTE` to execute them or `PASS` if you don't wish to execute them.", voteOption.Nominee,
                                                ghostVote ? "(Note that because you are dead, you may only vote to execute once more for the rest of the game.) " : string.Empty);
        }

        public async Task<IOption> OfferPrivateChat(IReadOnlyCollection<IOption> options)
        {
            var canPass = options.Any(option => option is PassOption);
            if (canPass)
            {
                return await prompter.RequestChoice(options, "Is there someone you wish to speak to privately as a priority? If you want to wait and first see if anyone wants to speak with you, respond with `PASS`. The players you can talk to are: %P.",
                                                    options.Where(option => option is PlayerOption)
                                                           .Select(option => ((PlayerOption)option).Player));
            }
            else
            {
                return await prompter.RequestChoice(options, "Who will you speak with privately? The players you can talk to are: %P.",
                                                    options.Where(option => option is PlayerOption)
                                                            .Select(option => ((PlayerOption)option).Player));
            }
        }

        public async Task<string> GetRollCallStatement()
        {
            return await prompter.RequestDialogue("For this roll call, provide your public statement about your character (or bluff) and possibly elaborate on what you learned or how you used your character. (This is optional - respond with just the single word `PASS` to say nothing.)");
        }

        public async Task<string> GetMorningPublicStatement()
        {
            return await prompter.RequestDialogue("Before the group breaks off for private conversations, say anything that you want to publicly. (Respond with `PASS` if you don't wish to say anything.)");
        }

        public async Task<string> GetEveningPublicStatement()
        {
            return await prompter.RequestDialogue("Before nominations are opened, say anything that you want to publicly. (Respond with `PASS` if you don't wish to say anything.)");
        }

        public async Task<string> GetProsecution(Player nominee)
        {
            return await prompter.RequestDialogue("You have nominated %p. Present the case to have them executed. (Respond with `PASS` if you don't wish to say anything.)", nominee);
        }

        public async Task<string> GetDefence(Player nominator)
        {
            return await prompter.RequestDialogue("You have been nominated by %p. Present the case for your defence. (Respond with `PASS` if you don't wish to say anything.)", nominator);
        }

        public async Task<string> GetReasonForSelfNomination()
        {
            return await prompter.RequestDialogue("You have nominated yourself. You may present your reason now. (Respond with `PASS` if you don't wish to say anything.)");
        }

        public async Task StartPrivateChat(Player otherPlayer)
        {
            await textAgent.StartPrivateChat(otherPlayer);
        }

        public async Task<(string message, bool endChat)> GetPrivateChat(Player listener)
        {
            return await prompter.RequestChatDialogue("What will you say to %p? To end the conversation respond with `PASS` or conclude what you wish to say with \"Goodbye\".", listener);
        }

        public async Task PrivateChatMessage(Player speaker, string message)
        {
            await textAgent.PrivateChatMessage(speaker, message);
        }

        public async Task EndPrivateChat(Player otherPlayer)
        {
            await textAgent.EndPrivateChat(otherPlayer);
        }

        private Task OnStartGame(Chat chat)
        {
            prompter.SendMessageAndGetResponse = (async (message) => await chat.SendMessageAndGetResponse(message));

            return Task.CompletedTask;

        }

        private readonly IAgent textAgent;
        private readonly TextPlayerPrompter prompter;

        private readonly IReadOnlyCollection<string> players;
        private readonly string scriptName;
        private readonly IReadOnlyCollection<Character> script;
        private Character? characterAbility;
    }
}
