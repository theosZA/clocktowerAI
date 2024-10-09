using Clocktower.Agent.Notifier;
using Clocktower.Agent.Observer;
using Clocktower.Agent.Requester;
using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Selection;
using System.Text;

namespace Clocktower.Agent
{
    internal class TextAgent : IAgent
    {
        public TextAgent(string playerName, IReadOnlyCollection<string> players, string scriptName, IReadOnlyCollection<Character> script, IGameObserver observer, IMarkupNotifier notifier, IMarkupRequester requester)
        {
            PlayerName = playerName;
            Observer = observer;

            this.notifier = notifier;
            this.requester = requester;

            this.players = players;
            this.scriptName = scriptName;
            this.script = script;
        }

        public string PlayerName { get; }

        public IGameObserver Observer { get; }

        public Func<Task>? OnStartGame { get; set; }
        public Func<Task>? OnEndGame { get; set; }
        public Func<Character, Alignment, Task>? OnAssignCharacter { get; set; }
        public Func<Alignment, Task>? OnChangeAlignment { get; set; }
        public Func<Character, Task>? OnGainingCharacterAbility { get; set; }
        public Func<Task>? OnDead { get; set; }
        public Func<Player, Task>? YourDemonIs { get; set; }
        public Func<IReadOnlyCollection<Player>, Task>? YourMinionsAre { get; set; }

        public async Task StartGame()
        {
            if (OnStartGame != null)
            {
                await OnStartGame();
            }

            await notifier.Start(PlayerName, players, scriptName, script);
        }

        public async Task EndGame()
        {
            if (OnEndGame != null)
            {
                await OnEndGame();
            }
        }

        public async Task AssignCharacter(Character character, Alignment alignment)
        {
            if (this.character.HasValue)
            {
                await SendMessage("You are now the %c. You are %a.", character, alignment);
            }
            else
            {
                await SendMessage("You are the %c. You are %a.", character, alignment);
            }
            this.character = character;
            this.alignment = alignment;

            if (OnAssignCharacter != null)
            {
                await OnAssignCharacter(character, alignment);
            }
        }

        public async Task ChangeAlignment(Alignment alignment)
        {
            await SendMessage("You are now %a.", alignment);
            this.alignment = alignment;

            if (OnChangeAlignment != null)
            {
                await OnChangeAlignment(alignment);
            }
        }

        public async Task YouAreDead()
        {
            await SendMessage("You are dead and are now a ghost. You may only vote one more time.");

            if (OnDead != null)
            {
                await OnDead();
            }
        }

        public async Task MinionInformation(Player demon, IReadOnlyCollection<Player> fellowMinions, bool damselInPlay, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            var sb = new StringBuilder();

            if (fellowMinions.Any())
            {
                sb.AppendFormattedText($"As a minion, you learn that %p is your demon and your fellow {(fellowMinions.Count > 1 ? "minions are" : "minion is")} %P.", demon, fellowMinions);
            }
            else
            {
                sb.AppendFormattedText("As a minion, you learn that %p is your demon.", demon);
            }
            if (damselInPlay)
            {
                sb.AppendFormattedText(" You also learn that there is a %c in play.", Character.Damsel);
            }
            if (notInPlayCharacters.Any())
            {
                sb.AppendFormattedText(" You also learn that the following characters are not in play: %C.", notInPlayCharacters);
            }

            await SendMessage(sb);

            if (YourDemonIs != null)
            {
                await YourDemonIs(demon);
            }
        }

        public async Task DemonInformation(IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            var sb = new StringBuilder();

            sb.Append("As the demon, you learn that ");

            var nonMarionetteMinions = minions.Where(minion => minion.RealCharacter != Character.Marionette).ToList();
            if (nonMarionetteMinions.Count > 0)
            {
                sb.AppendFormattedText($"%P {(nonMarionetteMinions.Count > 1 ? "are your minions" : "is your minion")}, ", nonMarionetteMinions);
            }

            var marionette = minions.FirstOrDefault(minion => minion.RealCharacter == Character.Marionette);
            if (marionette != null)
            {
                sb.AppendFormattedText("%p is your %c, ", marionette, Character.Marionette);
            }

            sb.AppendFormattedText("and that the following characters are not in play: %C.", notInPlayCharacters);

            await SendMessage(sb);

            if (YourMinionsAre != null)
            {
                await YourMinionsAre(minions);
            }
        }

        public async Task NotifyGodfather(IReadOnlyCollection<Character> outsiders)
        {
            if (outsiders.Count == 0)
            {
                await SendMessage("You learn that there are no outsiders in play.");
            }
            else
            {
                await SendMessage("You learn that the following outsiders are in play: %C.", outsiders);
            }
        }

        public async Task NotifyWasherwoman(Player playerA, Player playerB, Character character)
        {
            await SendMessage("You learn that either %p or %p is the %c.", playerA, playerB, character);
        }

        public async Task NotifyLibrarian(Player playerA, Player playerB, Character character)
        {
            await SendMessage("You learn that either %p or %p is the %c.", playerA, playerB, character);
        }

        public async Task NotifyLibrarianNoOutsiders()
        {
            if (character == Character.Cannibal)
            {
                await Learn(0);
                return;
            }
            await SendMessage("You learn that there are no outsiders in play.");
        }

        public async Task NotifyInvestigator(Player playerA, Player playerB, Character character)
        {
            await SendMessage("You learn that either %p or %p is the %c.", playerA, playerB, character);
        }

        public async Task NotifyChef(int evilPairCount)
        {
            if (character == Character.Cannibal)
            {
                await Learn(evilPairCount);
                return;
            }
            await SendMessage($"You learn that there {(evilPairCount == 1 ? "is %b pair" : "are %b pairs")} of evil players.", evilPairCount);
        }

        public async Task NotifySteward(Player goodPlayer)
        {
            if (character == Character.Cannibal)
            {
                await Learn(goodPlayer);
                return;
            }
            await SendMessage("You learn that %p is a good player.", goodPlayer);
        }

        public async Task NotifyBountyHunter(Player evilPlayer)
        {
            if (character == Character.Cannibal)
            {
                await Learn(evilPlayer);
                return;
            }
            await SendMessage("You learn that %p is an evil player.", evilPlayer);
        }

        public async Task NotifyNoble(IReadOnlyCollection<Player> nobleInformation)
        {
            if (character == Character.Cannibal)
            {
                await Learn(nobleInformation);
                return;
            }
            await SendMessage("You learn that there is exactly 1 evil player among %P", nobleInformation);
        }

        public async Task NotifyShugenja(Direction direction)
        {
            var directionText = direction == Direction.Clockwise ? "clockwise" : "counter-clockwise";
            if (character == Character.Cannibal)
            {
                await Learn(directionText);
                return;
            }
            await SendMessage("You learn that the nearest %a to you is in the %b direction.", Alignment.Evil, directionText);
        }

        public async Task NotifyEmpath(Player neighbourA, Player neighbourB, int evilCount)
        {
            if (character == Character.Cannibal)
            {
                await Learn(evilCount);
                return;
            }
            await SendMessage($"You learn that %b of your living neighbours (%p and %p) {(evilCount == 1 ? "is" : "are")} %a.", evilCount, neighbourA, neighbourB, Alignment.Evil);
        }

        public async Task NotifyOracle(int evilCount)
        {
            if (character == Character.Cannibal)
            {
                await Learn(evilCount);
                return;
            }
            await SendMessage($"You learn that %b of the dead players {(evilCount == 1 ? "is" : "are")} %a.", evilCount, Alignment.Evil);
        }

        public async Task NotifyFortuneTeller(Player targetA, Player targetB, bool reading)
        {
            var readingText = reading ? "Yes" : "No";
            if (character == Character.Cannibal)
            {
                await Learn(readingText);
                return;
            }

            if (reading)
            {
                await SendMessage("%b, one of %p or %p is the demon.", readingText, targetA, targetB);
            }
            else
            {
                await SendMessage("%b, neither of %p or %p is the demon.", readingText, targetA, targetB);
            }
        }

        public async Task NotifyRavenkeeper(Player target, Character character)
        {
            if (character == Character.Cannibal)
            {
                await Learn(character);
                return;
            }
            await SendMessage("You learn that %p is the %c.", target, character);
        }

        public async Task NotifyUndertaker(Player executedPlayer, Character character)
        {
            if (character == Character.Cannibal)
            {
                await Learn(character);
                return;
            }
            await SendMessage("You learn that %p is the %c.", executedPlayer, character);
        }

        public async Task NotifyBalloonist(Player newPlayer)
        {
            if (character == Character.Cannibal)
            {
                await Learn(newPlayer);
                return;
            }
            await SendMessage("As the %c, the next player you learn is %p.", Character.Balloonist, newPlayer);
        }

        public async Task NotifyHighPriestess(Player player)
        {
            if (character == Character.Cannibal)
            {
                await Learn(player);
                return;
            }
            await SendMessage("As the %c, you learn that %p is the player the Storyteller believes you should talk to most.", Character.High_Priestess, player);
        }

        public async Task NotifyJuggler(int jugglerCount)
        {
            if (character == Character.Cannibal)
            {
                await Learn(jugglerCount);
                return;
            }
            await SendMessage("You learn that %b of your juggles were correct.", jugglerCount);
        }

        public async Task ShowGrimoire(Character character, Grimoire grimoire)
        {
            var sb = new StringBuilder();

            sb.AppendFormattedText("As the %c, you can now look over the Grimoire...", character);
            sb.AppendLine();
            sb.Append(TextBuilder.GrimoireToText(grimoire));

            await SendMessage(sb);
        }

        public async Task ShowNightwatchman(Player nightwatchman)
        {
            await SendMessage("You learn that %p is the %c.", nightwatchman, Character.Nightwatchman);
        }

        public async Task LearnOfWidow()
        {
            await SendMessage("You learn that there is a %c in the game.", Character.Widow);
        }

        public async Task ResponseForFisherman(string advice)
        {
            await SendMessage("%b: %n", "Storyteller", advice.Trim());
        }

        public async Task OnGainCharacterAbility(Character character)
        {
            this.character = character;
            
            if (OnGainingCharacterAbility != null)
            {
                await OnGainingCharacterAbility(character);
            }
        }

        public async Task RequestSelectionOfKazaliMinions(KazaliMinionsSelection kazaliMinionsSelection)
        {
            var sb = new StringBuilder();
            int minionCount = kazaliMinionsSelection.MinionCount;
            sb.AppendFormattedText($"As the %c, please choose %n player{(minionCount == 1 ? string.Empty : "s")} to be your minion{(minionCount == 1 ? string.Empty : "s")}.", Character.Kazali, minionCount);
            if (kazaliMinionsSelection.CharacterLimitations.TryGetValue(Character.Marionette, out var marionettePlayers))
            {
                sb.AppendFormattedText(" (Reminder that if you choose to create a %c, it must be one of your neighbors, %p or %p.)", Character.Marionette, marionettePlayers.ElementAt(0), marionettePlayers.ElementAt(1));
            }
            await requester.RequestKazaliMinions(sb.ToString(), kazaliMinionsSelection);
        }

        public async Task<IOption> RequestNewKazaliMinion(Player minionTarget, Character unavailableMinionCharacter, IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("Your previous choice as the %c for %p to become the %c is no longer valid. Please choose a new Minion for %p to become.",
                                   Character.Kazali, minionTarget, unavailableMinionCharacter, minionTarget);
            return await requester.RequestCharacter(sb.ToString(), options);
        }

        public async Task<IOption> RequestChoiceOfMinionForSoldierSelectedByKazali(IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("You have been chosen by the %c to become a Minion. Since you are currently the %c, you may choose which Minion to become.", Character.Kazali, Character.Soldier);
            return await requester.RequestCharacter(sb.ToString(), options);

        }

        public async Task<IOption> RequestChoiceFromDemon(Character demonCharacter, IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("As the %c, please choose a player to kill.", demonCharacter);
            return await requester.RequestPlayerForDemonKill(sb.ToString(), options);
        }

        public async Task<IOption> RequestChoiceFromPukka(IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("As the %c, please choose a player to poison who will die tomorrow night.", Character.Pukka);
            return await requester.RequestPlayerForDemonKill(sb.ToString(), options);
        }

        public async Task<IOption> RequestChoiceFromOjo(IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("As the %c please choose a *character* to kill.", Character.Ojo);
            return await requester.RequestCharacterForDemonKill(sb.ToString(), options);
        }

        public async Task<IOption> RequestChoiceFromPoisoner(IReadOnlyCollection<IOption> options)
        {
            return await RequestPlayer(options, "As the %c, please choose a player to poison.", Character.Poisoner);
        }

        public async Task<IOption> RequestChoiceFromWidow(IReadOnlyCollection<IOption> options)
        {
            return await RequestPlayer(options, "As the %c, please choose a player to poison.", Character.Widow);
        }

        public async Task<IOption> RequestChoiceFromWitch(IReadOnlyCollection<IOption> options)
        {
            return await RequestPlayer(options, "As the %c, please choose a player to curse.", Character.Witch);
        }

        public async Task<IOption> RequestChoiceFromAssassin(IReadOnlyCollection<IOption> options)
        {
            return await RequestPlayer(options, "As the %c, you may use your once-per-game ability tonight to kill a player.", Character.Assassin);
        }

        public async Task<IOption> RequestChoiceFromGodfather(IReadOnlyCollection<IOption> options)
        {
            return await RequestPlayer(options, "As the %c, please choose a player to kill.", Character.Godfather);
        }

        public async Task<IOption> RequestChoiceFromDevilsAdvocate(IReadOnlyCollection<IOption> options)
        {
            return await RequestPlayer(options, "As the %c, please choose a player to protect from execution.", Character.Devils_Advocate);
        }

        public async Task<IOption> RequestChoiceFromPhilosopher(IReadOnlyCollection<IOption> options)
        {
            if (character == Character.Cannibal)
            {
                bool useAbility = await CannibalRequestUseAbility(OptionsBuilder.YesOrNo) is YesOption;
                if (!useAbility)
                {
                    return options.First(option => option is PassOption);
                }
                return await CannibalRequestCharacter(options.Where(option => option is not PassOption).ToList());
            }

            return await RequestCharacter(options, "As the %c, you may use your once-per-game ability tonight to gain the ability of a Townsfolk or Outsider character.", Character.Philosopher);
        }

        public async Task<IOption> RequestChoiceFromNightwatchman(IReadOnlyCollection<IOption> options)
        {
            if (character == Character.Cannibal)
            {
                bool useAbility = await CannibalRequestUseAbility(OptionsBuilder.YesOrNo) is YesOption;
                if (!useAbility)
                {
                    return options.First(option => option is PassOption);
                }
                return await CannibalRequestPlayer(options.Where(option => option is not PassOption).ToList());
            }

            return await RequestPlayer(options, "As the %c, you may use your once-per-game ability tonight so that one player will learn who you are.", Character.Nightwatchman);
        }

        public async Task<IOption> RequestChoiceFromHuntsman(IReadOnlyCollection<IOption> options)
        {
            if (character == Character.Cannibal)
            {
                bool useAbility = await CannibalRequestUseAbility(OptionsBuilder.YesOrNo) is YesOption;
                if (!useAbility)
                {
                    return options.First(option => option is PassOption);
                }
                return await CannibalRequestPlayer(options.Where(option => option is not PassOption).ToList());
            }

            return await RequestPlayer(options, "As the %c, you may use your once-per-game ability tonight to guess who the %c might be.", Character.Huntsman, Character.Damsel);
        }

        public async Task<IOption> RequestChoiceFromFortuneTeller(IReadOnlyCollection<IOption> options)
        {
            if (character == Character.Cannibal)
            {
                return await CannibalRequestTwoPlayers(options);
            }
            return await RequestTwoPlayers(options, "As the %c, please choose two players to learn about tonight.", Character.Fortune_Teller);
        }

        public async Task<IOption> RequestChoiceFromSnakeCharmer(IReadOnlyCollection<IOption> options)
        {
            if (character == Character.Cannibal)
            {
                return await CannibalRequestPlayer(options);
            }

            var sb = new StringBuilder();
            sb.AppendFormattedText("As the %c, please choose an alive player to see if they are the Demon.", Character.Snake_Charmer);
            if (options.Any(option => option is PlayerOption playerOption && playerOption.Player.Name == PlayerName))
            {
                sb.Append(" (Remember that you are always allowed to choose yourself if you don't wish to risk swapping with the Demon.)");
            }

            return await RequestPlayer(options, sb.ToString());
        }

        public async Task<IOption> RequestChoiceFromMonk(IReadOnlyCollection<IOption> options)
        {
            if (character == Character.Cannibal)
            {
                return await CannibalRequestPlayer(options);
            }
            return await RequestPlayer(options, "As the %c, please choose a player to protect from the demon tonight.", Character.Monk);
        }

        public async Task<IOption> RequestChoiceFromLycanthrope(IReadOnlyCollection<IOption> options)
        {
            if (character == Character.Cannibal)
            {
                return await CannibalRequestPlayer(options);
            }
            return await RequestPlayer(options, "As the %c, please choose a player to try kill tonight.", Character.Lycanthrope);
        }

        public async Task<IOption> RequestChoiceFromRavenkeeper(IReadOnlyCollection<IOption> options)
        {
            if (character == Character.Cannibal)
            {
                return await CannibalRequestPlayer(options);
            }
            return await RequestPlayer(options, "As the %c, please choose a player whose character you wish to learn.", Character.Ravenkeeper);
        }

        public async Task<IOption> RequestChoiceFromButler(IReadOnlyCollection<IOption> options)
        {
            if (character == Character.Cannibal)
            {
                return await RequestPlayer(options,
                                           "As the %c, you have gained the ability of the %c. Please choose a player. Tomorrow, you will only be able vote on a nomination if they have already voted for that nomination.",
                                           Character.Cannibal, Character.Butler);
            }
            return await RequestPlayer(options, "As the %c, please choose a player. Tomorrow, you will only be able vote on a nomination if they have already voted for that nomination.", Character.Butler);
        }

        public async Task<IOption> RequestChoiceFromOgre(IReadOnlyCollection<IOption> options)
        {
            if (character == Character.Cannibal)
            {
                return await CannibalRequestPlayer(options);
            }
            return await RequestPlayer(options, "As the %c, please choose a player. Your alignment will become the same as theirs.", Character.Ogre);
        }

        public async Task<IOption> PromptFishermanAdvice(IReadOnlyCollection<IOption> options)
        {
            return await RequestUseAbility(options, "Do you wish to go now to the Storyteller for your %c advice rather than saving it for later?", Character.Fisherman);
        }

        public async Task<IOption> PromptShenanigans(IReadOnlyCollection<IOption> options, bool afterNominations, Player? playerOnTheBlock)
        {
            var sb = new StringBuilder();
            if (afterNominations)
            {
                if (playerOnTheBlock != null)
                {
                    sb.AppendFormattedText("Nominations are now closed and %p is to be executed. ", playerOnTheBlock);
                }
                else
                {
                    sb.Append("Nominations are now closed and no one is to be executed. ");
                }
                sb.Append("You have a last chance today to use or bluff any abilities that are to be publicly used during the day.");
            }
            else
            {
                sb.Append("Before nominations open, you have the option now to use or bluff any abilities that are to be publicly used during the day.");
            }

            return await requester.RequestShenanigans(sb.ToString(), options);
        }

        public async Task<IOption> GetNomination(Player? playerOnTheBlock, int? votesToTie, int? votesToPutOnBlock, IReadOnlyCollection<IOption> options)
        {
            var sb = new StringBuilder();
            sb.Append("You may nominate a player.");
            if (playerOnTheBlock != null && votesToTie.HasValue)
            {
                sb.AppendFormattedText(" (%p is currently on the block with %b votes", playerOnTheBlock, votesToTie.Value);
                if (votesToPutOnBlock.HasValue)
                {
                    sb.AppendFormattedText(" and it will require %b votes to replace them on the block.)", votesToPutOnBlock.Value);
                }
                else
                {
                    sb.Append(" and that can't be beaten, only tied.)");
                }
            }
            else if (votesToPutOnBlock.HasValue)
            {
                sb.AppendFormattedText(" (No one is currently on the block and it will require %b votes to put someone on the block.)", votesToPutOnBlock.Value);
            }

            return await requester.RequestNomination(sb.ToString(), options);
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

            return await requester.RequestVote(sb.ToString(), ghostVote, options);
        }

        public async Task<IOption> OfferPrivateChat(IReadOnlyCollection<IOption> options)
        {
            var canPass = options.Any(option => option is PassOption);
            if (canPass)
            {
                return await requester.RequestPlayerForChat("Is there someone you wish to speak to privately as a priority?", options);
            }
            else
            {
                return await requester.RequestPlayerForChat("Who will you speak with privately?", options);
            }
        }

        public async Task<string> GetRollCallStatement()
        {
            return await requester.RequestStatement("For this roll call, you may provide your public statement about your character (or bluff) and possibly elaborate on what you learned or how you used your character ability.",
                                                    IMarkupRequester.Statement.RollCall);
        }

        public async Task<string> GetMorningPublicStatement()
        {
            return await requester.RequestStatement("Before the group breaks off for private conversations, you have the option to say anything that you want to publicly.", IMarkupRequester.Statement.Morning);
        }

        public async Task<string> GetEveningPublicStatement()
        {
            return await requester.RequestStatement("Before nominations are opened, say anything that you want to publicly.", IMarkupRequester.Statement.Evening);
        }

        public async Task<string> GetProsecution(Player nominee)
        {
            return await requester.RequestStatement(TextUtilities.FormatMarkupText("You have nominated %p. Present the case to have them executed.", nominee), IMarkupRequester.Statement.Prosection);
        }

        public async Task<string> GetDefence(Player nominator)
        {
            return await requester.RequestStatement(TextUtilities.FormatMarkupText("You have been nominated by %p. Present the case for your defence.", nominator), IMarkupRequester.Statement.Defence);
        }

        public async Task<string> GetReasonForSelfNomination()
        {
            return await requester.RequestStatement("You have nominated yourself. You may present your reason now", IMarkupRequester.Statement.SelfNomination);
        }

        public async Task StartPrivateChat(Player otherPlayer)
        {
            var imageFileName = Path.Combine("Images", otherPlayer.Name + ".jpg");
            await SendMessageWithImage(imageFileName, "You have begun a private chat with %p.", otherPlayer);
        }

        public async Task<(string message, bool endChat)> GetPrivateChat(Player listener)
        {
            return await requester.RequestMessageForChat(TextUtilities.FormatMarkupText("What will you say to %p?", listener));
        }

        public async Task PrivateChatMessage(Player speaker, string message)
        {
            await SendMessage($"%p:\n>>> %n", speaker, message);
        }

        public async Task EndPrivateChat(Player otherPlayer)
        {
            await SendMessage("The private chat with %p is over.", otherPlayer);
        }

        private async Task Learn(IEnumerable<Player> players)
        {
            await SendMessage("You learn: %P.", players);
        }

        private async Task Learn(Player player)
        {
            await SendMessage("You learn: %p.", player);
        }

        private async Task Learn(Character character)
        {
            await SendMessage("You learn: %c.", character);
        }

        private async Task Learn(int number)
        {
            await SendMessage("You learn: %b.", number);
        }

        private async Task Learn(string text)
        {
            await SendMessage("You learn: %b.", text);
        }

        private async Task SendMessage(StringBuilder stringBuilder)
        {
            await notifier.Notify(stringBuilder.ToString());
        }

        private async Task SendMessage(string text)
        {
            await notifier.Notify(text);
        }

        private async Task SendMessage(string text, params object[] objects)
        {
            await notifier.Notify(TextUtilities.FormatMarkupText(text, objects));
        }

        private async Task SendMessageWithImage(string imageFileName, string text, params object[] objects)
        {
            await notifier.NotifyWithImage(TextUtilities.FormatMarkupText(text, objects), imageFileName);
        }

        private async Task<IOption> RequestUseAbility(IReadOnlyCollection<IOption> options, string text, params object[] objects)
        {
            return await requester.RequestUseAbility(TextUtilities.FormatMarkupText(text, objects), options);
        }

        private async Task<IOption> RequestCharacter(IReadOnlyCollection<IOption> options, string text, params object[] objects)
        {
            return await requester.RequestCharacter(TextUtilities.FormatMarkupText(text, objects), options);
        }

        private async Task<IOption> RequestPlayer(IReadOnlyCollection<IOption> options, string text, params object[] objects)
        {
            return await requester.RequestPlayerTarget(TextUtilities.FormatMarkupText(text, objects), options);
        }

        private async Task<IOption> RequestTwoPlayers(IReadOnlyCollection<IOption> options, string text, params object[] objects)
        {
            return await requester.RequestTwoPlayersTarget(TextUtilities.FormatMarkupText(text, objects), options);
        }

        private async Task<IOption> CannibalRequestUseAbility(IReadOnlyCollection<IOption> options)
        {
            return await RequestUseAbility(options, "For the ability you have gained as the %c, please choose whether you wish to use the ability now.", Character.Cannibal);
        }

        private async Task<IOption> CannibalRequestCharacter(IReadOnlyCollection<IOption> options)
        {
            return await RequestCharacter(options, "For the ability you have gained as the %c, please choose a character.", Character.Cannibal);
        }

        private async Task<IOption> CannibalRequestPlayer(IReadOnlyCollection<IOption> options)
        {
            return await RequestPlayer(options, "For the ability you have gained as the %c, please choose a player.", Character.Cannibal);
        }

        private async Task<IOption> CannibalRequestTwoPlayers(IReadOnlyCollection<IOption> options)
        {
            return await RequestTwoPlayers(options, "For the ability you have gained as the %c, please choose two players.", Character.Cannibal);
        }

        private readonly IMarkupNotifier notifier;
        private readonly IMarkupRequester requester;

        private readonly IReadOnlyCollection<string> players;
        private readonly string scriptName;
        private readonly IReadOnlyCollection<Character> script;
        private Character? character;
        private Alignment alignment;
    }
}
