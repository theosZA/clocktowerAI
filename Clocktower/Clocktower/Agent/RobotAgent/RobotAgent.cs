using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Options;
using OpenAi;

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

        public RobotAgent(string playerName, IReadOnlyCollection<string> playersNames, IReadOnlyCollection<Character> script, Action onStart, Action onStatusChange)
        {
            PlayerName = playerName;

            clocktowerChat = new(playerName, playersNames, script);
            clocktowerChat.OnChatMessage += InternalOnChatMessage;
            clocktowerChat.OnDaySummary += InternalOnDaySummary;
            clocktowerChat.OnTokenCount += InternalOnTokenCount;

            chatAiObserver = new(clocktowerChat, this);

            this.onStart = onStart;
            this.onStatusChange = onStatusChange;
        }

        public void AssignCharacter(Character character, Alignment alignment)
        {
            Character = character;

            clocktowerChat.AddFormattedMessage("You are the %c. You are %a.", character, alignment);

            onStatusChange();
        }

        public void DemonInformation(IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            this.minions = minions;
            clocktowerChat.AddFormattedMessage($"As a demon, you learn that %P {(minions.Count > 1 ? "are your minions" : "is your minion")}, and that the following characters are not in play: %C.", minions, notInPlayCharacters);
        }

        public void EndPrivateChat(Player otherPlayer)
        {
            clocktowerChat.AddFormattedMessage("The private chat with %p is over.", otherPlayer);
        }

        public void GainCharacterAbility(Character character)
        {
            OriginalCharacter = Character;
            Character = character;

            onStatusChange();
        }

        public async Task<string> GetDefence(Player nominator)
        {
            return await clocktowerChat.RequestDialogue("You have been nominated by %p. Present the case for your defence. (Respond with PASS if you don't wish to say anything.)", nominator);
        }

        public async Task<string> GetEveningPublicStatement()
        {
            return await clocktowerChat.RequestDialogue("Before nominations are opened, say anything that you want to publicly. (Respond with PASS if you don't wish to say anything.)");
        }

        public async Task<string> GetMorningPublicStatement()
        {
            return await clocktowerChat.RequestDialogue("Before the group breaks off for private conversations, say anything that you want to publicly. (Respond with PASS if you don't wish to say anything.)");
        }

        public async Task<IOption> GetNomination(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChat.RequestChoice(options, "You may nominate a player. Either provide the name of the player (with no extra text) or respond with PASS. The players you can nominate are: %P.",
                                                        options.Where(option => option is PlayerOption)
                                                               .Select(option => ((PlayerOption)option).Player));
        }

        public async Task<string> GetPrivateChat(Player listener)
        {
            return await clocktowerChat.RequestDialogue("What will you say to %p? You may say PASS if there's nothing more to say and you're happy to end the conversation (unless they haven't spoken yet).", listener);
        }

        public async Task<string> GetProsecution(Player nominee)
        {
            return await clocktowerChat.RequestDialogue("You have nominated %p. Present the case to have them executed. (Respond with PASS if you don't wish to say anything.)", nominee);
        }

        public async Task<string> GetReasonForSelfNomination()
        {
            return await clocktowerChat.RequestDialogue("You have nominated yourself. You may present your reason now. (Respond with PASS if you don't wish to say anything.)");
        }

        public async Task<string> GetRollCallStatement()
        {
            return await clocktowerChat.RequestDialogue("For this roll call, provide your public statement about your character (or bluff) and possibly elaborate on what you learned or how you used your character. (This is optional - say PASS to say nothing.)");
        }

        public async Task<IOption> GetVote(IReadOnlyCollection<IOption> options, bool ghostVote)
        {
            var voteOption = (VoteOption)options.First(option => option is VoteOption);
            return await clocktowerChat.RequestChoice(options, "If you wish, you may vote for executing %p. %nSay EXECUTE to execute them or PASS if you don't wish to execute them.", voteOption.Nominee,
                                                        ghostVote ? "(Note that because you are dead, you may only vote to execute once more for the rest of the game.)" : string.Empty);
        }

        public void MinionInformation(Player demon, IReadOnlyCollection<Player> fellowMinions)
        {
            this.demon = demon;
            if (fellowMinions.Any())
            {
                clocktowerChat.AddFormattedMessage($"As a minion, you learn that %p is your demon and your fellow {(fellowMinions.Count > 1 ? "minions are" : "minion is")} %P.", demon, fellowMinions);
            }
            else
            {
                clocktowerChat.AddFormattedMessage($"As a minion, you learn that %p is your demon.", demon);
            }
        }

        public void NotifyEmpath(Player neighbourA, Player neighbourB, int evilCount)
        {
            clocktowerChat.AddFormattedMessage($"You learn that %b of your living neighbours (%p and %p) {(evilCount == 1 ? "is" : "are")} evil.", evilCount, neighbourA, neighbourB);
        }

        public void NotifyFortuneTeller(Player targetA, Player targetB, bool reading)
        {
            if (reading)
            {
                clocktowerChat.AddFormattedMessage("Yes, one of %p or %p is the demon.", targetA, targetB);
            }
            else
            {
                clocktowerChat.AddFormattedMessage("No, neither of %p or %p is the demon.", targetA, targetB);
            }
        }

        public void NotifyGodfather(IReadOnlyCollection<Character> outsiders)
        {
            if (outsiders.Count == 0)
            {
                clocktowerChat.AddFormattedMessage("You learn that there are no outsiders in play.");
                return;
            }
            clocktowerChat.AddFormattedMessage("You learn that the following outsiders are in play: %C.", outsiders);
        }

        public void NotifyInvestigator(Player playerA, Player playerB, Character character)
        {
            clocktowerChat.AddFormattedMessage("You learn that either %p or %p is the %c.", playerA, playerB, character);
        }

        public void NotifyLibrarian(Player playerA, Player playerB, Character character)
        {
            clocktowerChat.AddFormattedMessage("You learn that either %p or %p is the %c.", playerA, playerB, character);
        }

        public void NotifyRavenkeeper(Player target, Character character)
        {
            clocktowerChat.AddFormattedMessage("You learn that %p is the %c.", target, character);
        }

        public void NotifyShugenja(Direction direction)
        {
            clocktowerChat.AddFormattedMessage("You learn that the nearest %a to you is in the %b direction.", Alignment.Evil, direction == Direction.Clockwise ? "clockwise" : "counter-clockwise");
        }

        public void NotifySteward(Player goodPlayer)
        {
            clocktowerChat.AddFormattedMessage("You learn that %p is a good player.", goodPlayer);
        }

        public void NotifyUndertaker(Player executedPlayer, Character character)
        {
            clocktowerChat.AddFormattedMessage("You learn that %p is the %c.", executedPlayer, character);
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

        public void PrivateChatMessage(Player speaker, string message)
        {
            clocktowerChat.AddFormattedMessage($"%p: %n", speaker, message);
        }

        public async Task<IOption> PromptFishermanAdvice(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChat.RequestChoice(options, "Do you wish to go to the Storyteller for your %c advice? Respond with YES or NO only.", Game.Character.Fisherman);
        }

        public async Task<IOption> PromptSlayerShot(IReadOnlyCollection<IOption> options)
        {
            if (Character == Game.Character.Slayer)
            {
                return await clocktowerChat.RequestChoice(options, "Do you wish to claim %c and use your once-per-game ability to shoot a player? Respond with the name of a player to use the ability, or PASS if you want to save your ability for later.", Game.Character.Slayer);
            }
            else
            {
                return await clocktowerChat.RequestChoice(options, "Do you wish to bluff as %c and pretend to use the once-per-game ability to shoot a player? Respond with the name of a player to use the ability, or PASS if you don't want to use this bluff right now, or ALWAYS PASS if you never want to use this bluff.", Game.Character.Slayer);
            }
        }

        public async Task<IOption> RequestChoiceFromAssassin(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChat.RequestChoice(options, "As the %c, you may use your once-per-game ability tonight to kill a player. Respond with the name of a player to use the ability, or PASS if you want to save your ability for later.", Game.Character.Assassin);
        }

        public async Task<IOption> RequestChoiceFromFortuneTeller(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChat.RequestChoice(options, "As the %c please choose two players. Please format your response as: PLAYER1 and PLAYER2", Game.Character.Fortune_Teller);
        }

        public async Task<IOption> RequestChoiceFromGodfather(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChat.RequestChoice(options, "As the %c please choose a player to kill. Respond with the name of a player you wish to kill tonight.", Game.Character.Godfather);
        }

        public async Task<IOption> RequestChoiceFromImp(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChat.RequestChoice(options, "As the %c please choose a player to kill. Respond with the name of a player you wish to kill tonight.", Game.Character.Imp);
        }

        public async Task<IOption> RequestChoiceFromMonk(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChat.RequestChoice(options, "As the %c please choose a player to protect from the demon tonight. Respond with the name of the player you wish to protect.", Game.Character.Monk);
        }

        public async Task<IOption> RequestChoiceFromPhilosopher(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChat.RequestChoice(options, "As the %c, do you wish to use your ability tonight? Respond with the Townsfolk or Outsider character whose ability you wish to acquire, or PASS if you want to save your ability for later.", Game.Character.Philosopher);
        }

        public async Task<IOption> RequestChoiceFromPoisoner(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChat.RequestChoice(options, "As the %c please choose a player to poison. Respond with the name of the player you wish to poison.", Game.Character.Poisoner);
        }

        public async Task<IOption> RequestChoiceFromRavenkeeper(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChat.RequestChoice(options, "As the %c please choose a player whose character you wish to learn. Respond with the name of a player.", Game.Character.Ravenkeeper);
        }

        public void ResponseForFisherman(string advice)
        {
            clocktowerChat.AddFormattedMessage("Storyteller: %n", advice.Trim());
        }

        public void StartGame()
        {
            onStart();
        }

        public void StartPrivateChat(Player otherPlayer)
        {
            clocktowerChat.AddFormattedMessage("You have begun a private chat with %p.", otherPlayer);
        }

        public void YouAreDead()
        {
            Alive = false;

            clocktowerChat.AddFormattedMessage("You are dead and are now a ghost. You may only vote one more time.");

            onStatusChange();
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
    }
}
