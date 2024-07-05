using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Options;

namespace Clocktower.Storyteller
{
    public partial class StorytellerForm : Form, IStoryteller
    {
        public IGameObserver Observer { get; private set; }

        public bool AutoAct
        {
            get => autoCheckbox.Checked;
            set => autoCheckbox.Checked = value;
        }

        public StorytellerForm(Random random)
        {
            InitializeComponent();

            this.random = random;

            Observer = new RichTextBoxObserver(outputText)
            {
                StorytellerView = true
            };
        }

        public void Start()
        {
            Show();
        }

        public void PrivateChatMessage(Player speaker, Player listener, string message)
        {
            outputText.AppendFormattedText($"%p to %p: %n\n", speaker, listener, message, StorytellerView);
        }

        public async Task<IOption> GetDemonBluffs(Player demon, IReadOnlyCollection<IOption> demonBluffOptions)
        {
            outputText.AppendFormattedText("Choose 3 out-of-player characters to show to the demon, %p...\n", demon, StorytellerView);

            return await PopulateOptions(demonBluffOptions);
        }

        public async Task<IOption> GetNewImp(IReadOnlyCollection<IOption> impCandidates)
        {
            outputText.AppendFormattedText("The %c has star-passed. Choose a minion to become the new %c...\n", Character.Imp, Character.Imp);

            return await PopulateOptions(impCandidates);
        }

        public async Task<IOption> GetDrunk(IReadOnlyCollection<IOption> drunkCandidates)
        {
            outputText.AppendFormattedText("Choose one townsfolk who will be the %c...\n", Character.Drunk);

            return await PopulateOptions(drunkCandidates);
        }

        public async Task<IOption> GetSweetheartDrunk(IReadOnlyCollection<IOption> drunkCandidates)
        {
            outputText.AppendFormattedText("The %c has died. Choose one player to be drunk from now on...\n", Character.Sweetheart);

            return await PopulateOptions(drunkCandidates);
        }

        public async Task<IOption> GetFortuneTellerRedHerring(Player fortuneTeller, IReadOnlyCollection<IOption> redHerringCandidates)
        {
            outputText.AppendFormattedText("Choose one player to be the red herring (to register as the demon) for %p...\n", fortuneTeller, StorytellerView);

            return await PopulateOptions(redHerringCandidates);
        }

        public async Task<IOption> GetWasherwomanPings(Player washerwoman, IReadOnlyCollection<IOption> washerwomanPingCandidates)
        {
            outputText.AppendFormattedText("Choose two players who %p will see as a townsfolk, and the character they will see them as.", washerwoman, StorytellerView);
            OutputDrunkDisclaimer(washerwoman);
            var possibleTownsfolk = new HashSet<Player>(washerwomanPingCandidates.Select(option => ((CharacterForTwoPlayersOption)option).PlayerA));
            foreach (var misregister in possibleTownsfolk.Where(player => player.CanRegisterAsTownsfolk && player.CharacterType != CharacterType.Townsfolk))
            {
                outputText.AppendFormattedText(" Remember that %p could register as a townsfolk.", misregister, StorytellerView);
            }
            outputText.AppendText("\n");

            return await PopulateOptions(washerwomanPingCandidates);
        }

        public async Task<IOption> GetInvestigatorPings(Player investigator, IReadOnlyCollection<IOption> investigatorPingCandidates)
        {
            outputText.AppendFormattedText("Choose two players who %p will see as a minion, and the character they will see them as.", investigator, StorytellerView);
            OutputDrunkDisclaimer(investigator);
            var possibleMinions = new HashSet<Player>(investigatorPingCandidates.Select(option => ((CharacterForTwoPlayersOption)option).PlayerA));
            foreach (var misregister in possibleMinions.Where(player => player.CanRegisterAsMinion && player.CharacterType != CharacterType.Minion))
            {
                outputText.AppendFormattedText(" Remember that %p could register as a minion.", misregister, StorytellerView);
            }
            outputText.AppendText("\n");

            return await PopulateOptions(investigatorPingCandidates);
        }

        public async Task<IOption> GetLibrarianPings(Player librarian, IReadOnlyCollection<IOption> librarianPingCandidates)
        {
            outputText.AppendFormattedText("Choose two players who %p will see as an outsider, and the character they will see them as.", librarian, StorytellerView);
            OutputDrunkDisclaimer(librarian);
            outputText.AppendText("\n");

            return await PopulateOptions(librarianPingCandidates);
        }

        public async Task<IOption> GetStewardPing(Player steward, IReadOnlyCollection<IOption> stewardPingCandidates)
        {
            outputText.AppendFormattedText("Choose one player who %p will see as a good player.", steward, StorytellerView);
            OutputDrunkDisclaimer(steward);
            outputText.AppendText("\n");

            return await PopulateOptions(stewardPingCandidates);
        }

        public async Task<IOption> GetEmpathNumber(Player empath, Player neighbourA, Player neighbourB, IReadOnlyCollection<IOption> empathOptions)
        {
            outputText.AppendFormattedText("Choose what number to show to %p. Their living neighbours are %p and %p.", empath, neighbourA, neighbourB, StorytellerView);
            if (!OutputDrunkDisclaimer(empath))
            {
                if (neighbourA.Alignment != Alignment.Evil && neighbourA.CanRegisterAsEvil)
                {
                    outputText.AppendFormattedText(" Remember that %p could register as %a.", neighbourA, Alignment.Evil, StorytellerView);
                }
                else if (neighbourB.Alignment != Alignment.Evil && neighbourB.CanRegisterAsEvil)
                {
                    outputText.AppendFormattedText(" Remember that %p could register as %a.", neighbourB, Alignment.Evil, StorytellerView);
                }
            }
            outputText.AppendText("\n");

            return await PopulateOptions(empathOptions);
        }

        public async Task<IOption> GetFortuneTellerReading(Player fortuneTeller, Player targetA, Player targetB, IReadOnlyCollection<IOption> readingOptions)
        {
            outputText.AppendFormattedText("Choose whether to say 'Yes' or 'No' to %p to indicate whether they've seen a Demon between %p and %p.", fortuneTeller, targetA, targetB, StorytellerView);
            if (!OutputDrunkDisclaimer(fortuneTeller))
            {
                if (targetA.CharacterType != CharacterType.Demon && targetA.CanRegisterAsDemon)
                {
                    outputText.AppendFormattedText(" Remember that %p could register as a demon.", targetA, Alignment.Evil, StorytellerView);
                }
                else if (targetB.CharacterType != CharacterType.Demon && targetB.CanRegisterAsDemon)
                {
                    outputText.AppendFormattedText(" Remember that %p could register as a demon.", targetB, Alignment.Evil, StorytellerView);
                }
            }
            outputText.AppendText("\n");

            return await PopulateOptions(readingOptions);
        }

        public async Task<IOption> GetShugenjaDirection(Player shugenja, Grimoire grimoire, IReadOnlyCollection<IOption> shugenjaOptions)
        {
            outputText.AppendFormattedText("Choose whether to indicate 'Clockwise' or 'Counter-clockwise' to %p to indicate the direction to the nearest evil player.", shugenja, StorytellerView);
            OutputDrunkDisclaimer(shugenja);

            IReadOnlyCollection<Player> allPlayersClockwise = grimoire.GetAllPlayersEndingWithPlayer(shugenja).SkipLast(1).ToList();
            IReadOnlyCollection<Player> allPlayersCounterclockwise = allPlayersClockwise.Reverse().ToList();

            var (evilClockwise, stepsClockwise) = allPlayersClockwise.Select((player, i) => (player, i + 1))
                                                                     .First(pair => pair.player.Alignment == Alignment.Evil);
            outputText.AppendFormattedText(" The first evil player going clockwise is %p, %n steps away.", evilClockwise, stepsClockwise, StorytellerView);

            var (evilCounterclockwise, stepsCounterclockwise) = allPlayersCounterclockwise.Select((player, i) => (player, i + 1))
                                                                                          .First(pair => pair.player.Alignment == Alignment.Evil);
            outputText.AppendFormattedText(" The first evil player going counter-clockwise is %p, %n steps away.", evilCounterclockwise, stepsCounterclockwise, StorytellerView);

            if (allPlayersClockwise.Any(player => player.Alignment != Alignment.Evil && player.CanRegisterAsEvil))
            {
                var (possibleEvilClockwise, possibleStepsClockwise) = allPlayersClockwise.Select((player, i) => (player, i + 1))
                                                                                         .First(pair => pair.player.Alignment != Alignment.Evil && pair.player.CanRegisterAsEvil);
                if (possibleStepsClockwise < stepsClockwise)
                {
                    outputText.AppendFormattedText(" %p is %n steps clockwise and they may register as evil.", possibleEvilClockwise, possibleStepsClockwise, StorytellerView);
                }

                var (possibleEvilCounterclockwise, possibleStepsCounterclockwise) = allPlayersCounterclockwise.Select((player, i) => (player, i + 1))
                                                                                                              .First(pair => pair.player.Alignment != Alignment.Evil && pair.player.CanRegisterAsEvil);
                if (possibleStepsCounterclockwise < stepsCounterclockwise)
                {
                    outputText.AppendFormattedText(" %p is %n steps counter-clockwise and they may register as evil.", possibleEvilCounterclockwise, possibleStepsCounterclockwise, StorytellerView);
                }
            }

            outputText.AppendText("\n");

            return await PopulateOptions(shugenjaOptions);
        }

        public async Task<IOption> GetCharacterForRavenkeeper(Player ravenkeeper, Player target, IReadOnlyCollection<IOption> ravenkeeperOptions)
        {
            outputText.AppendFormattedText("%p has died and has chosen to learn the character of %p. Choose a character for them to learn.", ravenkeeper, target, StorytellerView);
            OutputDrunkDisclaimer(ravenkeeper);
            outputText.AppendText("\n");

            return await PopulateOptions(ravenkeeperOptions);
        }

        public async Task<IOption> GetCharacterForUndertaker(Player undertaker, Player executedPlayer, IReadOnlyCollection<IOption> undertakerOptions)
        {
            outputText.AppendFormattedText("%p was executed yesterday. Choose a character for %p to learn.", executedPlayer, undertaker, StorytellerView);
            OutputDrunkDisclaimer(undertaker);
            outputText.AppendText("\n");

            return await PopulateOptions(undertakerOptions);
        }

        public async Task<IOption> ShouldKillTinker(Player tinker, IReadOnlyCollection<IOption> yesOrNo)
        {
            outputText.AppendFormattedText("%p can die at any time. Kill them now?\n", tinker, StorytellerView);
            return await PopulateOptions(yesOrNo);
        }

        public async Task<IOption> ShouldKillWithSlayer(Player slayer, Player target, IReadOnlyCollection<IOption> yesOrNo)
        {
            outputText.AppendFormattedText("%p is claiming %c and targetting %p. Should %p die?\n", slayer, Character.Slayer, target, target, StorytellerView);
            return await PopulateOptions(yesOrNo);
        }

        public async Task<string> GetFishermanAdvice(Player fisherman)
        {
            outputText.AppendFormattedText("%p would like their %c advice.", fisherman, Character.Fisherman, StorytellerView);
            OutputDrunkDisclaimer(fisherman);
            outputText.AppendText("\n");
            return await GetTextResponse();
        }

        public void AssignCharacter(Player player)
        {
            if (player.Character != player.RealCharacter)
            {
                outputText.AppendFormattedText("%p believes they are the %c but they are actually the %c.\n", player, player.Character, player.RealCharacter);
            }
            else
            {
                outputText.AppendFormattedText("%p is the %c.\n", player, player.Character);
            }
        }

        public void MinionInformation(Player minion, Player demon, IReadOnlyCollection<Player> fellowMinions)
        {
            if (fellowMinions.Any())
            {
                outputText.AppendFormattedText($"%p learns that %p is their demon and that their fellow {(fellowMinions.Count > 1 ? "minions are" : "minion is")} %P.\n", minion, demon, fellowMinions, StorytellerView);
            }
            else
            {
                outputText.AppendFormattedText($"%p learns that %p is their demon.\n", minion, demon, StorytellerView);
            }
        }

        public void DemonInformation(Player demon, IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            outputText.AppendFormattedText($"%p learns that %P {(minions.Count > 1 ? "are their minions" : "is their minion")}, and that the following characters are not in play: %C.\n", demon, minions, notInPlayCharacters, StorytellerView);
        }

        public void NotifyGodfather(Player godfather, IReadOnlyCollection<Character> outsiders)
        {
            if (outsiders.Count == 0)
            {
                outputText.AppendFormattedText("%p learns that there are no outsiders in play.\n", godfather, StorytellerView);
                return;
            }
            outputText.AppendFormattedText("%p learns that the following outsiders are in play: %C\n", godfather, outsiders, StorytellerView);
        }

        public void NotifyWasherwoman(Player washerwoman, Player playerA, Player playerB, Character character)
        {
            outputText.AppendFormattedText("%p learns that %p or %p is the %c.\n", washerwoman, playerA, playerB, character, StorytellerView);
        }

        public void NotifyLibrarian(Player librarian, Player playerA, Player playerB, Character character)
        {
            outputText.AppendFormattedText("%p learns that %p or %p is the %c.\n", librarian, playerA, playerB, character, StorytellerView);
        }

        public void NotifyLibrarianNoOutsiders(Player librarian)
        {
            outputText.AppendFormattedText("%p learns that there are no outsiders in play.\n", librarian);
        }

        public void NotifyInvestigator(Player investigator, Player playerA, Player playerB, Character character)
        {
            outputText.AppendFormattedText("%p learns that %p or %p is the %c.\n", investigator, playerA, playerB, character, StorytellerView);
        }

        public void NotifySteward(Player steward, Player goodPlayer)
        {
            outputText.AppendFormattedText("%p learns that %p is a good player.\n", steward, goodPlayer, StorytellerView);
        }

        public void NotifyShugenja(Player shugenja, Direction direction)
        {
            outputText.AppendFormattedText("%p learns that the nearest %a to them is in the %b direction.\n", shugenja, Alignment.Evil, direction == Direction.Clockwise ? "clockwise" : "counter-clockwise", StorytellerView);
        }

        public void NotifyEmpath(Player empath, Player neighbourA, Player neighbourB, int evilCount)
        {
            outputText.AppendFormattedText($"%p learns that %b of their living neighbours (%p and %p) {(evilCount == 1 ? "is" : "are")} evil.\n", empath, evilCount, neighbourA, neighbourB, StorytellerView);
        }

        public void NotifyFortuneTeller(Player fortuneTeller, Player targetA, Player targetB, bool reading)
        {
            outputText.AppendFormattedText("%p learns that ", fortuneTeller, StorytellerView);
            if (reading)
            {
                outputText.AppendBoldText("Yes");
                outputText.AppendFormattedText($", one of %p or %p is the demon.\n", targetA, targetB);
            }
            else
            {
                outputText.AppendBoldText("No");
                outputText.AppendFormattedText($", neither of %p or %p is the demon.\n", targetA, targetB);
            }
        }

        public void NotifyUndertaker(Player undertaker, Player executedPlayer, Character executedCharacter)
        {
            outputText.AppendFormattedText($"%p learns that the recently executed %p is the %c.\n", undertaker, executedPlayer, executedCharacter, StorytellerView);
        }

        public void ChoiceFromImp(Player imp, Player target)
        {
            outputText.AppendFormattedText("%p has chosen to kill %p.\n", imp, target, StorytellerView);
        }

        public void ChoiceFromPoisoner(Player poisoner, Player target)
        {
            outputText.AppendFormattedText("%p has chosen to poison %p.\n", poisoner, target, StorytellerView);
        }

        public void ChoiceFromAssassin(Player assassin, Player? target)
        {
            if (target == null)
            {
                outputText.AppendFormattedText("%p is not using their ability tonight.\n", assassin, StorytellerView);
            }
            else
            {
                outputText.AppendFormattedText("%p has chosen to kill %p.\n", assassin, target, StorytellerView);
            }
        }

        public void ChoiceFromGodfather(Player godfather, Player target)
        {
            outputText.AppendFormattedText("%p has chosen to kill %p.\n", godfather, target, StorytellerView);
        }

        public void ChoiceFromMonk(Player monk, Player target)
        {
            outputText.AppendFormattedText("%p has chosen to protect %p.\n", monk, target, StorytellerView);
        }

        public void ChoiceFromRavenkeeper(Player ravenkeeper, Player target, Character character)
        {
            outputText.AppendFormattedText("%p chooses %p and learns that they are the %c.\n", ravenkeeper, target, character, StorytellerView);
        }

        public void ChoiceFromPhilosopher(Player philosopher, Player? philosopherDrunkedPlayer, Character newCharacterAbility)
        {
            if (philosopher.Tokens.Contains(Token.IsTheBadPhilosopher))
            {
                outputText.AppendFormattedText("%p has chosen to gain the ability of the %c. Since they were drunk or poisoned at the time, they didn't really gain the ability but will continue to be treated like a drunk version of that character.\n",
                                               philosopher, newCharacterAbility, StorytellerView);
            }
            else
            {
                outputText.AppendFormattedText("%p has chosen to gain the ability of the %c.", philosopher, newCharacterAbility, StorytellerView);
                if (philosopherDrunkedPlayer != null)
                {
                    outputText.AppendFormattedText(" Since %p is the %c, they are now drunked by the %c.", philosopherDrunkedPlayer, philosopherDrunkedPlayer.RealCharacter, Character.Philosopher, StorytellerView);
                }
                outputText.AppendText("\n");
            }
        }

        public void ScarletWomanTrigger(Player demon, Player scarletWoman)
        {
            outputText.AppendFormattedText("%p has died and so %p becomes the new %c.\n", demon, scarletWoman, Character.Imp, StorytellerView);
        }

        private Task<IOption> PopulateOptions(IReadOnlyCollection<IOption> options)
        {
            if (AutoAct)
            {
                var autoChosenOption = AutoChooseOption(options);
                outputText.AppendBoldText($">> {autoChosenOption.Name}\n", Color.Green);
                return Task.FromResult(autoChosenOption);
            }

            this.options = options;

            choicesComboBox.Items.Clear();
            foreach (var option in options)
            {
                choicesComboBox.Items.Add(option.Name);
            }
            choicesComboBox.Enabled = true;
            chooseButton.Enabled = true;

            var taskCompletionSource = new TaskCompletionSource<IOption>();

            void onChoiceHandler(IOption option)
            {
                taskCompletionSource.SetResult(option);
                OnChoice -= onChoiceHandler;
            }

            OnChoice += onChoiceHandler;

            return taskCompletionSource.Task;
        }

        private IOption AutoChooseOption(IReadOnlyCollection<IOption> options)
        {
            // If Pass is an option, pick it 75% of the time.
            var passOption = options.FirstOrDefault(option => option is PassOption);
            if (passOption != null && random.Next(4) < 3)
            {
                return passOption;
            }

            // For now, just pick an option at random.
            // Exclude dead players from our choices.
            var autoOptions = options.Where(option => option is not PassOption)
                                     .Where(option => option is not PlayerOption playerOption || playerOption.Player.Alive)
                                     .ToList();
            return autoOptions.RandomPick(random);
        }

        private Task<string> GetTextResponse()
        {
            submitButton.Enabled = true;
            responseTextBox.Enabled = true;

            var taskCompletionSource = new TaskCompletionSource<string>();

            void onTextHandler(string text)
            {
                taskCompletionSource.SetResult(text);
                OnText -= onTextHandler;
            }

            OnText += onTextHandler;

            return taskCompletionSource.Task;
        }

        private bool OutputDrunkDisclaimer(Player player)
        {
            if (player.Tokens.Contains(Token.IsTheBadPhilosopher))
            {
                outputText.AppendBoldText($" They were drunk or poisoned when they used their {TextUtilities.CharacterToText(Character.Philosopher)} ability, so they are not really the {TextUtilities.CharacterToText(player.Character)}. Therefore this should generally be bad information.", Color.Purple);
                return true;
            }
            else if (player.DrunkOrPoisoned)
            {
                outputText.AppendBoldText(" They are drunk or poisoned so this should generally be bad information.", Color.Purple);
                return true;
            }
            return false;
        }

        private void chooseButton_Click(object sender, EventArgs e)
        {
            var option = options?.FirstOrDefault(option => option.Name == (string)choicesComboBox.SelectedItem);
            if (option == null)
            {   // No valid option has been chosen.
                return;
            }

            chooseButton.Enabled = false;
            choicesComboBox.Enabled = false;
            choicesComboBox.Items.Clear();
            choicesComboBox.Text = null;

            outputText.AppendBoldText($">> {option.Name}\n", Color.Green);

            OnChoice?.Invoke(option);
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            var text = responseTextBox.Text;
            if (string.IsNullOrEmpty(text)) 
            {   // No response provided.
                return;
            }

            submitButton.Enabled = false;
            responseTextBox.Enabled = false;
            responseTextBox.Text = null;

            outputText.AppendBoldText($">> \"{text}\"\n", Color.Green);

            OnText?.Invoke(text);
        }

        private readonly Random random;

        public delegate void ChoiceEventHandler(IOption choice);
        private event ChoiceEventHandler? OnChoice;
        private IReadOnlyCollection<IOption>? options;

        public delegate void TextEventHandler(string text);
        private event TextEventHandler? OnText;

        private const bool StorytellerView = true;
    }
}
