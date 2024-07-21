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
            prompter = new(playerName);
            PlayerName = playerName;
            this.players = players;
            this.scriptName = scriptName;
            this.script = script;
        }

        public async Task StartGame()
        {
            var chat = await chatClient.CreateChat(PlayerName);

            observer.Start(chat);
            prompter.SendMessageAndGetResponse = (async (message) => await chat.SendMessageAndGetResponse(message));

            await chat.SendMessage($"Welcome {PlayerName} to a game of Blood on the Clocktower.");
            await chat.SendMessage(TextBuilder.ScriptToText(scriptName, script, markup: true));
            var scriptPdf = $"Scripts/{scriptName}.pdf";
            if (File.Exists(scriptPdf))
            {
                await chat.SendFile(scriptPdf);
            }
            await chat.SendMessage(TextBuilder.SetupToText(players.Count, script));
            await chat.SendMessage(TextBuilder.PlayersToText(players, markup: true));
        }

        public async Task AssignCharacter(Character character, Alignment alignment)
        {
            this.characterAbility = character;
            await observer.SendMessage("You are the %c. You are %a.", character, alignment);
        }

        public async Task YouAreDead()
        {
            await observer.SendMessage("You are dead and are now a ghost. You may only vote one more time.");
        }

        public async Task MinionInformation(Player demon, IReadOnlyCollection<Player> fellowMinions)
        {
            if (fellowMinions.Any())
            {
                await observer.SendMessage($"As a minion, you learn that %p is your demon and your fellow {(fellowMinions.Count > 1 ? "minions are" : "minion is")} %P.", demon, fellowMinions);
            }
            else
            {
                await observer.SendMessage($"As a minion, you learn that %p is your demon.", demon);
            }
        }

        public async Task DemonInformation(IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            await observer.SendMessage($"As a demon, you learn that %P {(minions.Count > 1 ? "are your minions" : "is your minion")}, and that the following characters are not in play: %C.", minions, notInPlayCharacters);
        }

        public async Task NotifyGodfather(IReadOnlyCollection<Character> outsiders)
        {
            if (outsiders.Count == 0)
            {
                await observer.SendMessage("You learn that there are no outsiders in play.");
            }
            else
            {
                await observer.SendMessage("You learn that the following outsiders are in play: %C.", outsiders);
            }
        }

        public async Task NotifyWasherwoman(Player playerA, Player playerB, Character character)
        {
            await observer.SendMessage("You learn that either %p or %p is the %c.", playerA, playerB, character);
        }

        public async Task NotifyLibrarian(Player playerA, Player playerB, Character character)
        {
            await observer.SendMessage("You learn that either %p or %p is the %c.", playerA, playerB, character);
        }

        public async Task NotifyLibrarianNoOutsiders()
        {
            await observer.SendMessage("You learn that there are no outsiders in play.");
        }

        public async Task NotifyInvestigator(Player playerA, Player playerB, Character character)
        {
            await observer.SendMessage("You learn that either %p or %p is the %c.", playerA, playerB, character);
        }

        public async Task NotifyChef(int evilPairCount)
        {
            await observer.SendMessage($"You learn that there {(evilPairCount == 1 ? "is %b pair" : "are %b pairs")} of evil players.", evilPairCount);
        }

        public async Task NotifySteward(Player goodPlayer)
        {
            await observer.SendMessage("You learn that %p is a good player.", goodPlayer);
        }

        public async Task NotifyShugenja(Direction direction)
        {
            await observer.SendMessage("You learn that the nearest %a to you is in the %b direction.", Alignment.Evil, direction == Direction.Clockwise ? "clockwise" : "counter-clockwise");
        }

        public async Task NotifyEmpath(Player neighbourA, Player neighbourB, int evilCount)
        {
            await observer.SendMessage($"You learn that %b of your living neighbours (%p and %p) {(evilCount == 1 ? "is" : "are")} evil.", evilCount, neighbourA, neighbourB);
        }

        public async Task NotifyFortuneTeller(Player targetA, Player targetB, bool reading)
        {
            if (reading)
            {
                await observer.SendMessage("Yes, one of %p or %p is the demon.", targetA, targetB);
            }
            else
            {
                await observer.SendMessage("No, neither of %p or %p is the demon.", targetA, targetB);
            }
        }

        public async Task NotifyRavenkeeper(Player target, Character character)
        {
            await observer.SendMessage("You learn that %p is the %c.", target, character);
        }

        public async Task NotifyUndertaker(Player executedPlayer, Character character)
        {
            await observer.SendMessage("You learn that %p is the %c.", executedPlayer, character);
        }

        public async Task ShowGrimoireToSpy(Grimoire grimoire)
        {
            await observer.SendMessage($"As the %c, you can now look over the Grimoire...\n{TextBuilder.GrimoireToText(grimoire, markup: true)}", Character.Spy);
        }

        public async Task ResponseForFisherman(string advice)
        {
            await observer.SendMessage("**Storyteller**: %n", advice.Trim());
        }

        public Task GainCharacterAbility(Character character)
        {
            this.characterAbility = character;
            return Task.CompletedTask;
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

        public async Task<IOption> RequestChoiceFromAssassin(IReadOnlyCollection<IOption> options)
        {
            return await prompter.RequestChoice(options, "As the %c, you may use your once-per-game ability tonight to kill a player. Respond with the name of a player to use the ability, or `PASS` if you want to save your ability for later.", Character.Assassin);
        }

        public async Task<IOption> RequestChoiceFromGodfather(IReadOnlyCollection<IOption> options)
        {
            return await prompter.RequestChoice(options, "As the %c please choose a player to kill.", Character.Godfather);
        }

        public async Task<IOption> RequestChoiceFromPhilosopher(IReadOnlyCollection<IOption> options)
        {
            return await prompter.RequestChoice(options, "As the %c, do you wish to use your ability tonight? Respond with the Townsfolk or Outsider character whose ability you wish to acquire, or `PASS` if you want to save your ability for later.", Character.Philosopher);
        }

        public async Task<IOption> RequestChoiceFromFortuneTeller(IReadOnlyCollection<IOption> options)
        {
            return await prompter.RequestChoice(options, "As the %c please choose two players. Respond in the form: *PLAYER1* and *PLAYER2*", Character.Fortune_Teller);
        }

        public async Task<IOption> RequestChoiceFromMonk(IReadOnlyCollection<IOption> options)
        {
            return await prompter.RequestChoice(options, "As the %c please choose a player to protect from the demon tonight.", Character.Monk);
        }

        public async Task<IOption> RequestChoiceFromRavenkeeper(IReadOnlyCollection<IOption> options)
        {
            return await prompter.RequestChoice(options, "As the %c please choose a player whose character you wish to learn.", Character.Ravenkeeper);
        }

        public async Task<IOption> RequestChoiceFromButler(IReadOnlyCollection<IOption> options)
        {
            return await prompter.RequestChoice(options, "As the %c please choose a player. Tomorrow, you will only be able vote on a nomination if it is their nomination or if they have voted for that nomination.", Character.Butler);
        }

        public async Task<IOption> PromptSlayerShot(IReadOnlyCollection<IOption> options)
        {
            if (characterAbility == Character.Slayer)
            {
                return await prompter.RequestChoice(options, "Do you wish to claim %c and use your once-per-game ability to shoot a player? Respond with the name of a player to use the ability, or `PASS` if you want to save your ability for later.", Character.Slayer);
            }
            else
            {
                return await prompter.RequestChoice(options, "Do you wish to bluff as %c and pretend to use the once-per-game ability to shoot a player? Respond with the name of a player to use the ability, or `PASS` if you don't want to use this bluff right now, or `ALWAYS PASS` if you never want to use this bluff.", Character.Slayer);
            }
        }

        public async Task<IOption> PromptFishermanAdvice(IReadOnlyCollection<IOption> options)
        {
            return await prompter.RequestChoice(options, "Do you wish to go now to the Storyteller for your %c advice rather than saving it for later?", Character.Fisherman);
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
            var imageFileName = Path.Combine("Images", otherPlayer.Name + ".jpg");
            await observer.SendMessageWithImage(imageFileName, "You have begun a private chat with %p.", otherPlayer);
        }

        public async Task<(string message, bool endChat)> GetPrivateChat(Player listener)
        {
            return await prompter.RequestChatDialogue("What will you say to %p? To end the conversation respond with `PASS` or conclude what you wish to say with \"Goodbye\".", listener);
        }

        public async Task PrivateChatMessage(Player speaker, string message)
        {
            await observer.SendMessage($"%p:\n>>> %n", speaker, message);
        }

        public async Task EndPrivateChat(Player otherPlayer)
        {
            await observer.SendMessage("The private chat with %p is over.", otherPlayer);
        }

        private readonly ChatClient chatClient;
        private readonly DiscordChatObserver observer = new();
        private readonly TextPlayerPrompter prompter;

        private readonly IReadOnlyCollection<string> players;
        private readonly string scriptName;
        private readonly IReadOnlyCollection<Character> script;
        private Character? characterAbility;
    }
}
