using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Options;
using OpenAi;
using System.Text;

namespace Clocktower.Agent.RobotAgent
{
    internal class RobotAgent : IAgent
    {
        /// <summary>
        /// Event is triggered whenever a new message is added to the chat. Note that this will include all
        /// messages (though not summaries) even if the actual prompts to the AI don't include all the messages.
        /// </summary>
        public event ChatMessageHandler? OnChatMessage;

        /// <summary>
        /// Event is triggered whenever the AI summarizes the previous day.
        /// </summary>
        public event DaySummaryHandler? OnDaySummary;

        /// <summary>
        /// Event is triggered whenever tokens are used, i.e. whenever a request is made to the AI.
        /// </summary>
        public event TokenCountHandler? OnTokenCount;

        public string PlayerName { get; private set; }

        public Character? Character { get; private set; }
        public Character? OriginalCharacter { get; private set; }

        public bool Alive { get; private set; } = true;

        public IGameObserver Observer => chatAiObserver;

        public RobotAgent(string model, string playerName, string personality, IReadOnlyCollection<string> playersNames, string scriptName, IReadOnlyCollection<Character> script, Action onStart, Action onStatusChange)
        {
            PlayerName = playerName;

            clocktowerChat = new(model, playerName, personality, playersNames, scriptName, script);
            clocktowerChat.OnChatMessage += InternalOnChatMessage;
            clocktowerChat.OnDaySummary += InternalOnDaySummary;
            clocktowerChat.OnTokenCount += InternalOnTokenCount;

            chatAiObserver = new(clocktowerChat, this);

            this.onStart = onStart;
            this.onStatusChange = onStatusChange;
        }

        public Task AssignCharacter(Character character, Alignment alignment)
        {
            Character = character;

            clocktowerChat.AddFormattedMessage("You are the %c. You are %a.", character, alignment);

            onStatusChange();

            return Task.CompletedTask;
        }

        public Task DemonInformation(IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            this.minions = minions;

            var sb = new StringBuilder();

            sb.Append("As the demon, you learn that ");

            var nonMarionetteMinions = minions.Where(minion => minion.RealCharacter != Game.Character.Marionette).ToList();
            if (nonMarionetteMinions.Count > 0)
            {
                sb.AppendFormattedMarkupText($"%P {(nonMarionetteMinions.Count > 1 ? "are your minions" : "is your minion")}, ", nonMarionetteMinions);
            }

            var marionette = minions.FirstOrDefault(minion => minion.RealCharacter == Game.Character.Marionette);
            if (marionette != null)
            {
                sb.AppendFormattedMarkupText("%p is your %c, ", marionette, Game.Character.Marionette);
            }

            sb.AppendFormattedMarkupText("and that the following characters are not in play: %C.", notInPlayCharacters);
            clocktowerChat.AddMessage(sb.ToString());

            return Task.CompletedTask;
        }

        public Task EndPrivateChat(Player otherPlayer)
        {
            clocktowerChat.AddFormattedMessage("The private chat with %p is over.", otherPlayer);
            return Task.CompletedTask;
        }

        public Task GainCharacterAbility(Character character)
        {
            OriginalCharacter = Character;
            Character = character;

            onStatusChange();

            return Task.CompletedTask;
        }

        public async Task<string> GetDefence(Player nominator)
        {
            return await clocktowerChat.RequestDialogue("You have been nominated by %p. Present the case for your defence.", nominator);
        }

        public async Task<string> GetEveningPublicStatement()
        {
            return await clocktowerChat.RequestDialogue("Before nominations are opened, say anything that you want to publicly. This is a good chance to share speculation about what you think is happening in the game, " +
                                                        "who you think may be evil, and who would be a good player to execute today. (Respond with just the single word PASS if you don't wish to say anything.)");
        }

        public async Task<string> GetMorningPublicStatement()
        {
            return await clocktowerChat.RequestDialogue("Before the group breaks off for private conversations, say anything that you want to publicly. There's no need to waste time encouraging collaboration and transparency - " +
                                                        "instead this is a chance to share information (or misinformation) that you want to publicly share. (Respond with just the single word PASS if you don't wish to say anything.)");
        }

        public async Task<IOption> GetNomination(IReadOnlyCollection<IOption> options)
        {
            var potentialNominees = options.Where(option => option is PlayerOption)
                                           .Select(option => ((PlayerOption)option).Player)
                                           .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("You may nominate a player. Please provide your reasoning as an internal monologue, considering at least a few possible candidates for execution " +
                          "as well as the possibility of not nominating anyone. Normally you want to be nominating players you suspect are evil, especially the Demon. " +
                          "(If you're evil, then you want to be nominating players that you can frame as evil.) Though sometimes strategic considerations might make you " +
                          "want to nominate a good player instead. Also keep in mind how many votes will be required and whether you think that it's possible you'll get " +
                          "the needed votes for execution from your fellow players.");
            sb.AppendFormattedText("Conclude with the name of the player to nominate or PASS. The players you can nominate are: %P.", potentialNominees);
            if (potentialNominees.Any(player => !player.Alive))
            {
                var aliveCandidates = potentialNominees.Where(player => player.Alive).ToList();
                switch (aliveCandidates.Count)
                {
                    case 0:
                        sb.Append(" Of these players, none are still alive.");
                        break;

                    case 1:
                        sb.AppendFormattedText(" Of these players, %P is still alive.", aliveCandidates);
                        break;

                    default:
                        sb.AppendFormattedText(" Of these players, %P are still alive.", aliveCandidates);
                        break;
                }
            }

            return await clocktowerChat.RequestNomination(options, sb.ToString());
        }

        public async Task<(string message, bool endChat)> GetPrivateChat(Player listener)
        {
            var chatMessage = (messagesInCurrentChat == 0)
                ? await clocktowerChat.RequestChatDialogue("What will you say to %p?", listener)
                : await clocktowerChat.RequestChatDialogue("What will you say to %p? (Once you're ready to finish this conversation, you can conclude what you say with \"Goodbye\").", listener);
            messagesInCurrentChat++;
            return chatMessage;
        }

        public async Task<string> GetProsecution(Player nominee)
        {
            return await clocktowerChat.RequestDialogue("You have nominated %p. Present the case to have them executed.", nominee);
        }

        public async Task<string> GetReasonForSelfNomination()
        {
            return await clocktowerChat.RequestDialogue("You have nominated yourself. You may present your reason now.");
        }

        public async Task<string> GetRollCallStatement()
        {
            return await clocktowerChat.RequestDialogue("For this roll call, provide your public statement about your character (or bluff) and possibly elaborate on what you learned or how you used your character. (This is optional - respond with just the single word PASS to say nothing.)");
        }

        public async Task<IOption> GetVote(IReadOnlyCollection<IOption> options, bool ghostVote)
        {
            var nominee = ((VoteOption)options.First(option => option is VoteOption)).Nominee;

            var sb = new StringBuilder();
            if (nominee.Agent == this)
            {
                sb.AppendFormattedText("If you wish, you may vote for executing yourself.");
            }
            else
            {
                sb.AppendFormattedText("If you wish, you may vote for executing %p.", nominee);
            }
            if (ghostVote)
            {
                sb.Append(" (Note that because you are dead, you may only vote to execute once more for the rest of the game.)");
            }
            sb.AppendLine();
            sb.AppendFormattedText(" Please provide your reasoning as an internal monologue, considering the evidence surrounding their information and character, and pros and cons for executing %p." +
                                   " Some factors to keep in mind: If you're good then executing other good players early on isn't a terrible idea as it can help some characters learn some information," +
                                   " but as you get closer to the end of the game you really need to be executing evil players, and especially in the final 3 or 4, you *must* be executing the demon." +
                                   " (If you're evil then you want to be executing valuable good players, and *not* the Demon, but at the same time you still need to look like your votes are helping the good players.)" +
                                   " It is not generally expected that you vote on yourself, and if there are already enough votes for the execution to pass, you may wish to hold off on over-voting " +
                                   " to give town the possibility for considering another nomination.",
                                 nominee);
            if (ghostVote)
            {
                sb.AppendLine();
                sb.Append(" And since you only have one more vote available for the rest of the game, you need to decide whether or not this is the best time to spend that vote." +
                          " (Most commonly, ghost votes are kept until the last day when there are just 3 or 4 players left alive.)");
            }
            sb.AppendLine();
            sb.AppendFormattedText("Conclude with either EXECUTE to execute %p or PASS if you don't wish to execute them.", nominee);
            var prompt = sb.ToString();

            return await clocktowerChat.RequestVote(options, prompt);
        }

        public Task MinionInformation(Player demon, IReadOnlyCollection<Player> fellowMinions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            this.demon = demon;
            if (fellowMinions.Any())
            {
                clocktowerChat.AddFormattedMessage($"As a minion, you learn that %p is your demon and your fellow {(fellowMinions.Count > 1 ? "minions are" : "minion is")} %P.", demon, fellowMinions);
            }
            else
            {
                clocktowerChat.AddFormattedMessage("As a minion, you learn that %p is your demon.", demon);
            }
            if (notInPlayCharacters.Any())
            {
                clocktowerChat.AddFormattedMessage("You also learn that the following characters are not in play: %C.", notInPlayCharacters);
            }
            return Task.CompletedTask;
        }

        public Task NotifyChef(int evilPairCount)
        {
            if (Character == Game.Character.Cannibal)
            {
                clocktowerChat.AddFormattedMessage($"You learn: %b", evilPairCount);
            }
            else
            {
                clocktowerChat.AddFormattedMessage($"You learn that there {(evilPairCount == 1 ? "is %b pair" : "are %b pairs")} of evil players.", evilPairCount);
            }
            return Task.CompletedTask;
        }

        public Task NotifyEmpath(Player neighbourA, Player neighbourB, int evilCount)
        {
            if (Character == Game.Character.Cannibal)
            {
                clocktowerChat.AddFormattedMessage($"You learn: %b", evilCount);
            }
            else
            {
                clocktowerChat.AddFormattedMessage($"You learn that %b of your living neighbours (%p and %p) {(evilCount == 1 ? "is" : "are")} evil.", evilCount, neighbourA, neighbourB);
            }
            return Task.CompletedTask;
        }

        public Task NotifyFortuneTeller(Player targetA, Player targetB, bool reading)
        {
            if (Character == Game.Character.Cannibal)
            {
                clocktowerChat.AddFormattedMessage($"You learn: %b", reading ? "Yes" : "No");
            }
            else if (reading)
            {
                clocktowerChat.AddFormattedMessage("Yes, one of %p or %p is the demon.", targetA, targetB);
            }
            else
            {
                clocktowerChat.AddFormattedMessage("No, neither of %p or %p is the demon.", targetA, targetB);
            }
            return Task.CompletedTask;
        }

        public Task NotifyGodfather(IReadOnlyCollection<Character> outsiders)
        {
            if (outsiders.Count == 0)
            {
                clocktowerChat.AddFormattedMessage("You learn that there are no outsiders in play.");
            }
            else
            {
                clocktowerChat.AddFormattedMessage("You learn that the following outsiders are in play: %C.", outsiders);
            }
            return Task.CompletedTask;
        }

        public Task NotifyInvestigator(Player playerA, Player playerB, Character character)
        {
            clocktowerChat.AddFormattedMessage("You learn that either %p or %p is the %c.", playerA, playerB, character);
            return Task.CompletedTask;
        }

        public Task NotifyJuggler(int jugglerCount)
        {
            if (Character == Game.Character.Cannibal)
            {
                clocktowerChat.AddFormattedMessage($"You learn: %b", jugglerCount);
            }
            else
            {
                clocktowerChat.AddFormattedMessage("You learn that %b of your juggles were correct.", jugglerCount);
            }
            return Task.CompletedTask;
        }

        public Task NotifyLibrarian(Player playerA, Player playerB, Character character)
        {
            clocktowerChat.AddFormattedMessage("You learn that either %p or %p is the %c.", playerA, playerB, character);
            return Task.CompletedTask;
        }

        public Task NotifyLibrarianNoOutsiders()
        {
            if (Character == Game.Character.Cannibal)
            {
                clocktowerChat.AddFormattedMessage($"You learn: %b", 0);
            }
            else
            {
                clocktowerChat.AddFormattedMessage("You learn that there are no outsiders in play.");
            }
            return Task.CompletedTask;
        }

        public Task NotifyRavenkeeper(Player target, Character character)
        {
            if (Character == Game.Character.Cannibal)
            {
                clocktowerChat.AddFormattedMessage($"You learn: %c", character);
            }
            else
            {
                clocktowerChat.AddFormattedMessage("You learn that %p is the %c.", target, character);
            }
            return Task.CompletedTask;
        }

        public Task NotifyShugenja(Direction direction)
        {
            var directionText = direction == Direction.Clockwise ? "clockwise" : "counter-clockwise";
            if (Character == Game.Character.Cannibal)
            {
                clocktowerChat.AddFormattedMessage($"You learn: %b", directionText);
            }
            else
            {
                clocktowerChat.AddFormattedMessage("You learn that the nearest %a to you is in the %b direction.", Alignment.Evil, directionText);
            }
            return Task.CompletedTask;
        }

        public Task NotifySteward(Player goodPlayer)
        {
            if (Character == Game.Character.Cannibal)
            {
                clocktowerChat.AddFormattedMessage($"You learn: %p", goodPlayer);
            }
            else
            {
                clocktowerChat.AddFormattedMessage("You learn that %p is a good player.", goodPlayer);
            }
            return Task.CompletedTask;
        }

        public Task NotifyNoble(IReadOnlyCollection<Player> nobleInformation)
        {
            if (Character == Game.Character.Cannibal)
            {
                clocktowerChat.AddFormattedMessage($"You learn: %P", nobleInformation);
            }
            else
            {
                clocktowerChat.AddFormattedMessage("You learn that there is exactly 1 evil player among %P", nobleInformation);
            }
            return Task.CompletedTask;
        }

        public Task NotifyUndertaker(Player executedPlayer, Character character)
        {
            if (Character == Game.Character.Cannibal)
            {
                clocktowerChat.AddFormattedMessage($"You learn: %c", character);
            }
            else
            {
                clocktowerChat.AddFormattedMessage("You learn that %p is the %c.", executedPlayer, character);
            }
            return Task.CompletedTask;
        }

        public Task NotifyWasherwoman(Player playerA, Player playerB, Character character)
        {
            clocktowerChat.AddFormattedMessage("You learn that either %p or %p is the %c.", playerA, playerB, character);
            return Task.CompletedTask;
        }

        public async Task<IOption> OfferPrivateChat(IReadOnlyCollection<IOption> options)
        {
            var canPass = options.Any(option => option is PassOption);
            if (canPass)
            {
                return await clocktowerChat.RequestChoice(options, "Is there someone you wish to speak to privately as a priority? Either provide the name of the player (with no extra text) or respond with PASS if you want to wait and see if anyone wants to speak with you first. The players you can talk to are: %P.",
                                                            options.Where(option => option is PlayerOption)
                                                                   .Select(option => ((PlayerOption)option).Player));
            }
            else
            {
                return await clocktowerChat.RequestChoice(options, "Who will you speak with privately? Provide the name of the player (with no extra text). The players you can talk to are: %P.",
                                                            options.Where(option => option is PlayerOption)
                                                                   .Select(option => ((PlayerOption)option).Player));
            }
        }

        public Task PrivateChatMessage(Player speaker, string message)
        {
            clocktowerChat.AddFormattedMessage($"%p: %n", speaker, message);
            return Task.CompletedTask;
        }

        public async Task<IOption> PromptFishermanAdvice(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChat.RequestChoice(options, "Do you wish to go now to the Storyteller for your %c advice rather than saving it for later? Respond with YES or NO only.", Game.Character.Fisherman);
        }

        public async Task<IOption> PromptShenanigans(IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder();
            var objects = new List<object>();

            sb.AppendLine("You have the option now to use or bluff any abilities that are to be publicly used during the day. You can explain your reasoning but your response should conclude with exactly one of the following options.");
            if (options.Any(option => option is SlayerShotOption))
            {
                sb.AppendLine("- SLAYER: PLAYER_NAME if you wish to claim %c and target the specified player.");
                objects.Add(Game.Character.Slayer);
            }
            if (options.Any(option => option is JugglerOption))
            {
                sb.AppendLine("- JUGGLER: PLAYER_NAME AS CHARACTER, PLAYER_NAME AS CHARACTER, ... with up to 5 player-character pairs if you wish to claim %c and guess players as specific characters. (Players and characters may be repeated");
                objects.Add(Game.Character.Juggler);
            }
            if (options.Any(option => option is PassOption))
            {
                sb.AppendLine("- PASS if you don't wish to use or bluff any of these abilities.");
            }
            if (options.Any(option => option is AlwaysPassOption))
            {
                sb.AppendLine("- ALWAYS PASS if you aren't bluffing any of these characters and so will skip this prompt in the future (unless you do have an ability you can use).");
            }

            return await clocktowerChat.RequestShenanigans(options, sb.ToString(), objects);
        }

        public async Task<IOption> RequestChoiceFromAssassin(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChat.RequestChoice(options, "As the %c, you may use your once-per-game ability tonight to kill a player. Respond with the name of a player to use the ability, or PASS if you want to save your ability for later.", Game.Character.Assassin);
        }

        public async Task<IOption> RequestChoiceFromFortuneTeller(IReadOnlyCollection<IOption> options)
        {
            if (Character == Game.Character.Cannibal)
            {
                return await clocktowerChat.RequestChoice(options, "For the ability you have gained as the %c, please choose two players. Please format your response as: PLAYER1 and PLAYER2", Game.Character.Cannibal);
            }
            return await clocktowerChat.RequestChoice(options, "As the %c please choose two players. Please format your response as: PLAYER1 and PLAYER2", Game.Character.Fortune_Teller);
        }

        public async Task<IOption> RequestChoiceFromGodfather(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChat.RequestChoice(options, "As the %c please choose a player to kill. Respond with the name of a player you wish to kill tonight.", Game.Character.Godfather);
        }

        public async Task<IOption> RequestChoiceFromDevilsAdvocate(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChat.RequestChoice(options, "As the %c please choose a player to protect from execution.", Game.Character.Devils_Advocate);
        }

        public async Task<IOption> RequestChoiceFromDemon(Character demonCharacter, IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChat.RequestChoice(options, "As the %c please choose a player to kill. Respond with the name of a player you wish to kill tonight.", demonCharacter);
        }

        public async Task<IOption> RequestChoiceFromOjo(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChat.RequestChoice(options, "As the %c please choose a character to kill. Respond with just the specific character from the script.", Game.Character.Ojo);
        }

        public async Task<IOption> RequestChoiceFromMonk(IReadOnlyCollection<IOption> options)
        {
            if (Character == Game.Character.Cannibal)
            {
                return await clocktowerChat.RequestChoice(options, "For the ability you have gained as the %c, please choose a player. Respond with the name of a player.", Game.Character.Cannibal);
            }
            return await clocktowerChat.RequestChoice(options, "As the %c please choose a player to protect from the demon tonight. Respond with the name of the player you wish to protect.", Game.Character.Monk);
        }

        public async Task<IOption> RequestChoiceFromPhilosopher(IReadOnlyCollection<IOption> options)
        {
            if (Character == Game.Character.Cannibal)
            {
                bool useAbility = await clocktowerChat.RequestChoice(OptionsBuilder.YesOrNo, "Do you wish to use the ability you have gained as the %c tonight?", Game.Character.Cannibal) is YesOption;
                if (!useAbility)
                {
                    return options.First(option => option is PassOption);
                }
                return await clocktowerChat.RequestChoice(options.Where(option => option is not PassOption).ToList(), "For the ability you have gained as the %c, please choose a Townsfolk or Outsider character.", Game.Character.Cannibal);
            }

            return await clocktowerChat.RequestChoice(options, "As the %c, do you wish to use your ability tonight? Respond with the Townsfolk or Outsider character whose ability you wish to acquire, or PASS if you want to save your ability for later.", Game.Character.Philosopher);
        }

        public async Task<IOption> RequestChoiceFromPoisoner(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChat.RequestChoice(options, "As the %c please choose a player to poison. Respond with the name of the player you wish to poison.", Game.Character.Poisoner);
        }

        public async Task<IOption> RequestChoiceFromWitch(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChat.RequestChoice(options, "As the %c please choose a player to curse. Respond with the name of the player on whom you wish to use your character ability.", Game.Character.Witch);
        }

        public async Task<IOption> RequestChoiceFromRavenkeeper(IReadOnlyCollection<IOption> options)
        {
            if (Character == Game.Character.Cannibal)
            {
                return await clocktowerChat.RequestChoice(options, "For the ability you have gained as the %c, please choose a player. Respond with the name of a player.", Game.Character.Cannibal);
            }
            return await clocktowerChat.RequestChoice(options, "As the %c please choose a player whose character you wish to learn. Respond with the name of a player.", Game.Character.Ravenkeeper);
        }

        public async Task<IOption> RequestChoiceFromButler(IReadOnlyCollection<IOption> options)
        {
            if (Character == Game.Character.Cannibal)
            {
                return await clocktowerChat.RequestChoice(options, "As the %c you have gained the ability of the %c. Please choose a player. Tomorrow, you will only be able vote on a nomination if they have already voted for that nomination.",
                                                          Game.Character.Cannibal, Game.Character.Butler);
            }
            return await clocktowerChat.RequestChoice(options, "As the %c please choose a player. Tomorrow, you will only be able vote on a nomination if they have already voted for that nomination.", Game.Character.Butler);
        }

        public Task ResponseForFisherman(string advice)
        {
            clocktowerChat.AddFormattedMessage("Storyteller: %n", advice.Trim());
            return Task.CompletedTask;
        }

        public Task ShowGrimoireToSpy(Grimoire grimoire)
        {
            clocktowerChat.AddFormattedMessage($"As the %c, you can now look over the Grimoire...\n{TextBuilder.GrimoireToText(grimoire)}", Game.Character.Spy);
            return Task.CompletedTask;
        }

        public Task StartGame()
        {
            onStart();
            return Task.CompletedTask;
        }

        public Task StartPrivateChat(Player otherPlayer)
        {
            clocktowerChat.AddFormattedMessage("You have begun a private chat with %p.", otherPlayer);
            messagesInCurrentChat = 0;
            return Task.CompletedTask;
        }

        public Task YouAreDead()
        {
            Alive = false;

            clocktowerChat.AddFormattedMessage("You are dead and are now a ghost. While dead, you still participate in the game, you may still talk, and you still win or lose with your team. In fact, the game is usually decided by the votes " +
                                               "and opinions of the dead players. You no longer have your character ability, you may no longer nominate, and you have only one vote for the rest of the game, so use it wisely.");

            onStatusChange();

            return Task.CompletedTask;
        }

        public async Task PromptForBluff()
        {
            if (Character == null)
            {
                return;
            }
            else if (Character.Value.CharacterType() == CharacterType.Demon)
            {
                await clocktowerChat.Request("Which good character will you bluff as to the good players? You have been provided a selection of characters that are not in play that you can safely use for bluffs, or you could bluff as another " +
                                             "character on the script at the risk of double-claiming that character. Alternatively you could hedge your bets by claiming 2 or 3 different characters to the good players.");
                if (minions != null)
                {
                    if (minions.Count > 1)
                    {
                        clocktowerChat.AddFormattedMessage("Remember that your minions (%P) may also need safe characters to bluff as. Consider having private conversations with them during the day.", minions);
                    }
                    else
                    {
                        clocktowerChat.AddFormattedMessage("Remember that your minion (%P) may also need a safe character to bluff as. Consider having a private conversation with them during the day.", minions);
                    }
                }
            }
            else if (Character.Value.CharacterType() == CharacterType.Minion)
            {
                if (Character == Game.Character.Godfather)
                {
                    await clocktowerChat.Request("Which good character will you bluff as to the good players? As the %c you know which Outsiders are in play and therefore which ones are safe to use as a bluff. Or you could bluff as a Townsfolk " +
                                                 "character on the script at the risk of double-claiming that character. Alternatively you could hedge your bets by claiming 2 or 3 different characters to the good players.", Game.Character.Godfather);
                }
                else
                {
                    await clocktowerChat.Request("Which good character will you bluff as to the good players? You may also hedge your bets by claiming 2 or 3 different characters to the good players.");
                }
                if (demon != null)
                {
                    clocktowerChat.AddFormattedMessage("Remember thar your demon (%p) has received a selection of safe characters to bluff. Consider having a private conversation with them during the day and see if you want to revise your bluff.", demon);
                }
            }
            else  // good player
            {
                await clocktowerChat.Request("As a good player you may choose to be open about your character. However there are reasons that you may want to bluff about which character you are (a character that benefits from being targeted by " +
                                             "the demon at night may want to bluff as a strong recurring information-gaining character to bait an attack by the demon, or vice-versa), or only inform a limited selection of players " +
                                             "about your true character. Alternatively you could hedge your bets by claiming 2 or 3 different characters to each player you talk to. What will your approach be?");
                clocktowerChat.AddMessage("Regardless of whether you bluff or not, remember that as a good player, you will want to be honest about your character and your information towards the end of the game (when there are only " +
                                          "3 or 4 players left alive).");
            }
        }

        public async Task PromptForOverview()
        {
            if (Character == null)
            {
                return;
            }
            if (Character.Value.Alignment() == Alignment.Evil)
            {
                await clocktowerChat.Request("Please provide a list of all players in the game, and for each player list whether they're alive or dead and good or evil, and the character or characters that you think they're most likely to be (and in brackets how confident " +
                                             "you are of this opinion). For example: * Zeke - Alive - Good - Slayer (almost certain) or Soldier (unlikely)");
            }
            else // Alignment is Good
            {
                await clocktowerChat.Request("Please provide a list of all players in the game, and for each player list whether they're alive or dead, whether you believe they're good or evil (and in brackets how confident you are of this opinion), " +
                                             "and the character or characters that you think they're most likely to be (and in brackets how confident you are of this opinion). " +
                                             "For example: * Zeke - Alive - Good (very likely) - Slayer (almost certain) or Imp (unlikely)");
            }
        }

        public async Task PromptForDemonGuess()
        {
            if (Character == null)
            {
                return;
            }
            if (Character.Value.Alignment() == Alignment.Evil)
            {
                await clocktowerChat.Request("This may be the last day for nominations. Which of the living players, other than the demon, do you think you can convince the good players is the actual demon?");
            }
            else // Alignment is Good
            {
                await clocktowerChat.Request("This may be the last day for nominations. Which of the living player do you thing is the demon, and why? How do you think you can convince the rest of your fellow good players to vote to execute them?");
            }
        }

        private void InternalOnChatMessage(Role role, string message)
        {
            OnChatMessage?.Invoke(role, message);
        }

        private void InternalOnDaySummary(int dayNumber, string summary)
        {
            OnDaySummary?.Invoke(dayNumber, summary);
        }

        private void InternalOnTokenCount(int promptTokens, int completionTokens, int totalTokens)
        {
            OnTokenCount?.Invoke(promptTokens, completionTokens, totalTokens);
        }

        private readonly ClocktowerChatAi clocktowerChat;
        private readonly ChatAiObserver chatAiObserver;
        private readonly Action onStart;
        private readonly Action onStatusChange;

        private IReadOnlyCollection<Player>? minions;
        private Player? demon;

        private int messagesInCurrentChat = 0;
    }
}
