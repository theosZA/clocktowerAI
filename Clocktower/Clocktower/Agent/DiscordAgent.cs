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

        public async Task MinionInformation(Player demon, IReadOnlyCollection<Player> fellowMinions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            if (fellowMinions.Any())
            {
                await observer.SendMessage($"As a minion, you learn that %p is your demon and your fellow {(fellowMinions.Count > 1 ? "minions are" : "minion is")} %P.", demon, fellowMinions);
            }
            else
            {
                await observer.SendMessage("As a minion, you learn that %p is your demon.", demon);
            }
            if (notInPlayCharacters.Any())
            {
                await observer.SendMessage("You also learn that the following characters are not in play: %C.", notInPlayCharacters);
            }
        }

        public async Task DemonInformation(IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            var sb = new StringBuilder();

            sb.Append("As the demon, you learn that ");

            var nonMarionetteMinions = minions.Where(minion => minion.RealCharacter != Character.Marionette).ToList();
            if (nonMarionetteMinions.Count > 0)
            {
                sb.AppendFormattedMarkupText($"%P {(nonMarionetteMinions.Count > 1 ? "are your minions" : "is your minion")}, ", nonMarionetteMinions);
            }

            var marionette = minions.FirstOrDefault(minion => minion.RealCharacter == Character.Marionette);
            if (marionette != null)
            {
                sb.AppendFormattedMarkupText("%p is your %c, ", marionette, Character.Marionette);
            }

            sb.AppendFormattedMarkupText("and that the following characters are not in play: %C.", notInPlayCharacters);

            await observer.SendMessage(sb.ToString());
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
            if (characterAbility == Character.Cannibal)
            {
                await observer.SendMessage("You learn: %b.", 0);
                return;
            }
            await observer.SendMessage("You learn that there are no outsiders in play.");
        }

        public async Task NotifyInvestigator(Player playerA, Player playerB, Character character)
        {
            await observer.SendMessage("You learn that either %p or %p is the %c.", playerA, playerB, character);
        }

        public async Task NotifyChef(int evilPairCount)
        {
            if (characterAbility == Character.Cannibal)
            {
                await observer.SendMessage("You learn: %b.", evilPairCount);
                return;
            }
            await observer.SendMessage($"You learn that there {(evilPairCount == 1 ? "is %b pair" : "are %b pairs")} of evil players.", evilPairCount);
        }

        public async Task NotifySteward(Player goodPlayer)
        {
            if (characterAbility == Character.Cannibal)
            {
                await observer.SendMessage("You learn: %p.", goodPlayer);
                return;
            }
            await observer.SendMessage("You learn that %p is a good player.", goodPlayer);
        }

        public async Task NotifyNoble(IReadOnlyCollection<Player> nobleInformation)
        {
            if (characterAbility == Character.Cannibal)
            {
                await observer.SendMessage("You learn: %P.", nobleInformation);
                return;
            }
            await observer.SendMessage("You learn that there is exactly 1 evil player among %P", nobleInformation);
        }

        public async Task NotifyShugenja(Direction direction)
        {
            var directionText = direction == Direction.Clockwise ? "clockwise" : "counter-clockwise";
            if (characterAbility == Character.Cannibal)
            {
                await observer.SendMessage("You learn: %b.", directionText);
                return;
            }
            await observer.SendMessage("You learn that the nearest %a to you is in the %b direction.", Alignment.Evil, directionText);
        }

        public async Task NotifyEmpath(Player neighbourA, Player neighbourB, int evilCount)
        {
            if (characterAbility == Character.Cannibal)
            {
                await observer.SendMessage("You learn: %b.", evilCount);
                return;
            }
            await observer.SendMessage($"You learn that %b of your living neighbours (%p and %p) {(evilCount == 1 ? "is" : "are")} evil.", evilCount, neighbourA, neighbourB);
        }

        public async Task NotifyFortuneTeller(Player targetA, Player targetB, bool reading)
        {
            if (characterAbility == Character.Cannibal)
            {
                await observer.SendMessage("You learn: %b.", reading ? "Yes" : "No");
                return;
            }

            if (reading)
            {
                await observer.SendMessage("**Yes**, one of %p or %p is the demon.", targetA, targetB);
            }
            else
            {
                await observer.SendMessage("**No**, neither of %p or %p is the demon.", targetA, targetB);
            }
        }

        public async Task NotifyRavenkeeper(Player target, Character character)
        {
            if (characterAbility == Character.Cannibal)
            {
                await observer.SendMessage("You learn: %c.", character);
                return;
            }
            await observer.SendMessage("You learn that %p is the %c.", target, character);
        }

        public async Task NotifyUndertaker(Player executedPlayer, Character character)
        {
            if (characterAbility == Character.Cannibal)
            {
                await observer.SendMessage("You learn: %c.", character);
                return;
            }
            await observer.SendMessage("You learn that %p is the %c.", executedPlayer, character);
        }

        public async Task NotifyBalloonist(Player newPlayer)
        {
            if (characterAbility == Character.Cannibal)
            {
                await observer.SendMessage("You learn: %p.", newPlayer);
                return;
            }
            await observer.SendMessage("As the %c, the next player you learn is %p.", Character.Balloonist, newPlayer);
        }

        public async Task NotifyJuggler(int jugglerCount)
        {
            if (characterAbility == Character.Cannibal)
            {
                await observer.SendMessage("You learn: %b.", jugglerCount);
                return;
            }
            await observer.SendMessage("You learn that %b of your juggles were correct.", jugglerCount);
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
