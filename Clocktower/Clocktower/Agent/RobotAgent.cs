using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.OpenAiApi;
using Clocktower.Options;

namespace Clocktower.Agent
{
    internal class RobotAgent : IAgent
    {
        public string PlayerName { get; private set; }

        public Character? Character { get; private set; }
        public Character? OriginalCharacter { get; private set; }

        public bool Alive { get; private set; } = true;

        public IGameObserver Observer => chatAiObserver;

        public RobotAgent(string playerName, IReadOnlyCollection<string> playersNames, IReadOnlyCollection<Character> script, Action onStart, Action onStatusChange, IChatLogger chatLogger, ITokenCounter tokenCounter)
        {
            PlayerName = playerName;
            
            clocktowerChatAi = new(playerName, playersNames, script, chatLogger, tokenCounter);
            chatAiObserver = new(clocktowerChatAi);

            this.onStart = onStart;
            this.onStatusChange = onStatusChange;
        }

        public void AssignCharacter(Character character, Alignment alignment)
        {
            Character = character;

            clocktowerChatAi.AddFormattedMessage("You are the %c. You are %a.", character, alignment);

            onStatusChange();
        }

        public void DemonInformation(IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            clocktowerChatAi.AddFormattedMessage($"As a demon, you learn that %P {(minions.Count > 1 ? "are your minions" : "is your minion")}, and that the following characters are not in play: %C.", minions, notInPlayCharacters);
        }

        public void EndPrivateChat(Player otherPlayer)
        {
            clocktowerChatAi.AddFormattedMessage("The private chat with %p is over.", otherPlayer);
        }

        public void GainCharacterAbility(Character character)
        {
            OriginalCharacter = Character;
            Character = character;
            
            onStatusChange();
        }

        public async Task<string> GetDefence(Player nominator)
        {
            return await clocktowerChatAi.RequestDialogue("You have been nominated by %p. Present the case for your defence. (Say PASS to say nothing.)", nominator);
        }

        public async Task<string> GetEveningPublicStatement()
        {
            return await clocktowerChatAi.RequestDialogue("Before nominations are opened, do you wish to say anything publicly? (Say PASS to say nothing.)");
        }

        public async Task<string> GetMorningPublicStatement()
        {
            return await clocktowerChatAi.RequestDialogue("Before the group breaks off for private conversations, do you wish to say anything publicly? (Say PASS to say nothing.)");
        }

        public async Task<IOption> GetNomination(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChatAi.RequestChoice(options, "You may nominate a player. Either provide the name of the player (with no extra text) or respond with PASS. The players you can nominate are: %P.",
                                                        options.Where(option => option is PlayerOption)
                                                               .Select(option => ((PlayerOption)option).Player));
        }

        public async Task<string> GetPrivateChat(Player listener)
        {
            return await clocktowerChatAi.RequestDialogue("What will you say to %p? You may say PASS if there's nothing more to say and you're happy to end the conversation (unless they haven't spoken yet).", listener);
        }

        public async Task<string> GetProsecution(Player nominee)
        {
            return await clocktowerChatAi.RequestDialogue("You have nominated %p. Present the case to have them executed. (Say PASS to say nothing.)", nominee);
        }

        public async Task<string> GetReasonForSelfNomination()
        {
            return await clocktowerChatAi.RequestDialogue("You have nominated yourself. You may present your reason now. (Say PASS to say nothing.)");
        }

        public async Task<string> GetRollCallStatement()
        {
            return await clocktowerChatAi.RequestDialogue("For this roll call, provide your public statement about your character (or bluff) and possibly elaborate on what you learned or how you used your character. (This is optional - say PASS to say nothing.)");
        }

        public async Task<IOption> GetVote(IReadOnlyCollection<IOption> options, bool ghostVote)
        {
            var voteOption = (VoteOption)(options.First(option => option is VoteOption));
            return await clocktowerChatAi.RequestChoice(options, "If you wish, you may vote for executing %p. %nSay EXECUTE to execute them or PASS if you don't wish to execute them.", voteOption.Nominee,
                                                        ghostVote ? "(Note that because you are dead, you may only vote to execute once more for the rest of the game.)" : string.Empty);
        }

        public void MinionInformation(Player demon, IReadOnlyCollection<Player> fellowMinions)
        {
            if (fellowMinions.Any())
            {
                clocktowerChatAi.AddFormattedMessage($"As a minion, you learn that %p is your demon and your fellow {(fellowMinions.Count > 1 ? "minions are" : "minion is")} %P.", demon, fellowMinions);
            }
            else
            {
                clocktowerChatAi.AddFormattedMessage($"As a minion, you learn that %p is your demon.", demon);
            }
        }

        public void NotifyEmpath(Player neighbourA, Player neighbourB, int evilCount)
        {
            clocktowerChatAi.AddFormattedMessage($"You learn that %b of your living neighbours (%p and %p) {(evilCount == 1 ? "is" : "are")} evil.", evilCount, neighbourA, neighbourB);
        }

        public void NotifyFortuneTeller(Player targetA, Player targetB, bool reading)
        {
            if (reading)
            {
                clocktowerChatAi.AddFormattedMessage("Yes");
                clocktowerChatAi.AddFormattedMessage($", one of %p or %p is the demon.", targetA, targetB);
            }
            else
            {
                clocktowerChatAi.AddFormattedMessage("No");
                clocktowerChatAi.AddFormattedMessage($", neither of %p or %p is the demon.", targetA, targetB);
            }
        }

        public void NotifyGodfather(IReadOnlyCollection<Character> outsiders)
        {
            if (outsiders.Count == 0)
            {
                clocktowerChatAi.AddFormattedMessage("You learn that there are no outsiders in play.");
                return;
            }
            clocktowerChatAi.AddFormattedMessage("You learn that the following outsiders are in play: %C.", outsiders);
        }

        public void NotifyInvestigator(Player playerA, Player playerB, Character character)
        {
            clocktowerChatAi.AddFormattedMessage("You learn that either %p or %p is the %c.", playerA, playerB, character);
        }

        public void NotifyLibrarian(Player playerA, Player playerB, Character character)
        {
            clocktowerChatAi.AddFormattedMessage("You learn that either %p or %p is the %c.", playerA, playerB, character);
        }

        public void NotifyRavenkeeper(Player target, Character character)
        {
            clocktowerChatAi.AddFormattedMessage("You learn that %p is the %c.", target, character);
        }

        public void NotifyShugenja(Direction direction)
        {
            clocktowerChatAi.AddFormattedMessage("You learn that the nearest %a to you is in the %b direction.", Alignment.Evil, direction == Direction.Clockwise ? "clockwise" : "counter-clockwise");
        }

        public void NotifySteward(Player goodPlayer)
        {
            clocktowerChatAi.AddFormattedMessage("You learn that %p is a good player.", goodPlayer);
        }

        public void NotifyUndertaker(Player executedPlayer, Character character)
        {
            clocktowerChatAi.AddFormattedMessage("You learn that %p is the %c.", executedPlayer, character);
        }

        public async Task<IOption> OfferPrivateChat(IReadOnlyCollection<IOption> options)
        {
            var canPass = options.Any(option => option is PassOption);
            if (canPass)
            {
                return await clocktowerChatAi.RequestChoice(options, "Is there someone you wish to speak to privately as a priority? Either provide the name of the player (with no extra text) or respond with PASS if you want to wait and see if anyone wants to speak with you first. The players you can talk to are: %P.",
                                                            options.Where(option => option is PlayerOption)
                                                                   .Select(option => ((PlayerOption)option).Player));
            }
            else
            {
                return await clocktowerChatAi.RequestChoice(options, "Who will you speak with privately? Provide the name of the player (with no extra text). The players you can talk to are: %P.",
                                                            options.Where(option => option is PlayerOption)
                                                                   .Select(option => ((PlayerOption)option).Player));
            }
        }

        public void PrivateChatMessage(Player speaker, string message)
        {
            clocktowerChatAi.AddFormattedMessage($"%p: %n", speaker, message);
        }

        public async Task<IOption> PromptFishermanAdvice(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChatAi.RequestChoice(options, "Do you wish to go to the Storyteller for your %c advice? Respond with YES or NO only.", Game.Character.Fisherman);
        }

        public async Task<IOption> PromptSlayerShot(IReadOnlyCollection<IOption> options)
        {
            if (Character == Game.Character.Slayer)
            {
                return await clocktowerChatAi.RequestChoice(options, "Do you wish to claim %c and use your once-per-game ability to shoot a player? Respond with the name of a player to use the ability, or PASS if you want to save your ability for later.", Game.Character.Slayer);
            }
            else
            {
                return await clocktowerChatAi.RequestChoice(options, "Do you wish to bluff as %c and pretend to use the once-per-game ability to shoot a player? Respond with the name of a player to use the ability, or PASS if you don't want to use this bluff right now, or ALWAYS PASS if you never want to use this bluff.", Game.Character.Slayer);
            }
        }

        public async Task<IOption> RequestChoiceFromAssassin(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChatAi.RequestChoice(options, "As the %c, you may use your once-per-game ability tonight to kill a player. Respond with the name of a player to use the ability, or PASS if you want to save your ability for later.", Game.Character.Assassin);
        }

        public async Task<IOption> RequestChoiceFromFortuneTeller(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChatAi.RequestChoice(options, "As the %c please choose two players. Please format your response as: PLAYER1 and PLAYER2", Game.Character.Fortune_Teller);
        }

        public async Task<IOption> RequestChoiceFromGodfather(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChatAi.RequestChoice(options, "As the %c please choose a player to kill. Respond with the name of a player you wish to kill tonight.", Game.Character.Godfather);
        }

        public async Task<IOption> RequestChoiceFromImp(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChatAi.RequestChoice(options, "As the %c please choose a player to kill. Respond with the name of a player you wish to kill tonight.", Game.Character.Imp);
        }

        public async Task<IOption> RequestChoiceFromMonk(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChatAi.RequestChoice(options, "As the %c please choose a player to protect from the demon tonight. Respond with the name of the player you wish to protect.", Game.Character.Monk);
        }

        public async Task<IOption> RequestChoiceFromPhilosopher(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChatAi.RequestChoice(options, "As the %c, do you wish to use your ability tonight? Respond with the Townsfolk or Outsider character whose ability you wish to acquire, or PASS if you want to save your ability for later.", Game.Character.Philosopher);
        }

        public async Task<IOption> RequestChoiceFromPoisoner(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChatAi.RequestChoice(options, "As the %c please choose a player to poison. Respond with the name of the player you wish to poison.", Game.Character.Poisoner);
        }

        public async Task<IOption> RequestChoiceFromRavenkeeper(IReadOnlyCollection<IOption> options)
        {
            return await clocktowerChatAi.RequestChoice(options, "As the %c please choose a player whose character you wish to learn. Respond with the name of a player.", Game.Character.Ravenkeeper);
        }

        public void ResponseForFisherman(string advice)
        {
            clocktowerChatAi.AddFormattedMessage("Storyteller: %n", advice.Trim());
        }

        public void StartGame()
        {
            onStart();
        }

        public void StartPrivateChat(Player otherPlayer)
        {
            clocktowerChatAi.AddFormattedMessage("You have begun a private chat with %p.", otherPlayer);
        }

        public void YouAreDead()
        {
            Alive = false;

            clocktowerChatAi.AddFormattedMessage("You are dead and are now a ghost. You may only vote one more time.");

            onStatusChange();
        }

        private readonly ClocktowerChatAi clocktowerChatAi;
        private readonly ChatAiObserver chatAiObserver;
        private readonly Action onStart;
        private readonly Action onStatusChange;
    }
}
