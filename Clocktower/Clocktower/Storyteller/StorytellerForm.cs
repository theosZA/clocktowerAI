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

        public async Task<IOption> GetFortuneTellerRedHerring(IReadOnlyCollection<IOption> redHerringCandidates)
        {
            outputText.AppendFormattedText("Choose one player to be the red herring (to register as the Demon) for the %c...\n", Character.Fortune_Teller);

            return await PopulateOptions(redHerringCandidates);
        }

        public async Task<IOption> GetInvestigatorPings(Player investigator, IReadOnlyCollection<IOption> investigatorPingCandidates)
        {
            outputText.AppendFormattedText("Choose two players who %p will see as a minion, and the character they will see them as.", investigator, StorytellerView);
            if (investigator.DrunkOrPoisoned)
            {
                outputText.AppendBoldText(" They are drunk or poisoned so this should generally be bad information.", Color.Purple);
            }
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
            if (librarian.DrunkOrPoisoned)
            {
                outputText.AppendBoldText(" They are drunk or poisoned so this should generally be bad information.", Color.Purple);
            }
            outputText.AppendText("\n");

            return await PopulateOptions(librarianPingCandidates);
        }

        public async Task<IOption> GetStewardPing(Player steward, IReadOnlyCollection<IOption> stewardPingCandidates)
        {
            outputText.AppendFormattedText("Choose one player who %p will see as a good player.", steward, StorytellerView);
            if (steward.DrunkOrPoisoned)
            {
                outputText.AppendBoldText(" They are drunk or poisoned so this should generally be bad information.", Color.Purple);
            }
            outputText.AppendText("\n");

            return await PopulateOptions(stewardPingCandidates);
        }

        public async Task<IOption> GetEmpathNumber(Player empath, Player neighbourA, Player neighbourB, IReadOnlyCollection<IOption> empathOptions)
        {
            outputText.AppendFormattedText("Choose what number to show to %p. Their living neighbours are %p and %p.", empath, neighbourA, neighbourB, StorytellerView);
            if (empath.DrunkOrPoisoned)
            {
                outputText.AppendBoldText(" They are drunk or poisoned so this should generally be bad information.", Color.Purple);
            }
            else if (neighbourA.Alignment != Alignment.Evil && neighbourA.CanRegisterAsEvil)
            {
                outputText.AppendFormattedText(" Remember that %p could register as %a.", neighbourA, Alignment.Evil, StorytellerView);
            }
            else if (neighbourB.Alignment != Alignment.Evil && neighbourB.CanRegisterAsEvil)
            {
                outputText.AppendFormattedText(" Remember that %p could register as %a.", neighbourB, Alignment.Evil, StorytellerView);
            }
            outputText.AppendText("\n");

            return await PopulateOptions(empathOptions);
        }

        public async Task<IOption> GetFortuneTellerReading(Player fortuneTeller, Player targetA, Player targetB, IReadOnlyCollection<IOption> readingOptions)
        {
            outputText.AppendFormattedText("Choose whether to say 'Yes' or 'No' to %p to indicate whether they've seen a Demon between %p and %p.", fortuneTeller, targetA, targetB, StorytellerView);
            if (fortuneTeller.DrunkOrPoisoned)
            {
                outputText.AppendBoldText(" They are drunk or poisoned so this should generally be bad information.", Color.Purple);
            }
            else if (targetA.CharacterType != CharacterType.Demon && targetA.CanRegisterAsDemon)
            {
                outputText.AppendFormattedText(" Remember that %p could register as a demon.", targetA, Alignment.Evil, StorytellerView);
            }
            else if (targetB.CharacterType != CharacterType.Demon && targetB.CanRegisterAsDemon)
            {
                outputText.AppendFormattedText(" Remember that %p could register as a demon.", targetB, Alignment.Evil, StorytellerView);
            }
            outputText.AppendText("\n");

            return await PopulateOptions(readingOptions);
        }

        public async Task<IOption> GetShugenjaDirection(Player shugenja, Grimoire grimoire, IReadOnlyCollection<IOption> shugenjaOptions)
        {
            outputText.AppendFormattedText("Choose whether to indicate 'Clockwise' or 'Counter-clockwise' to %p to indicate the direction to the nearest evil player.", shugenja, StorytellerView);
            if (shugenja.DrunkOrPoisoned)
            {
                outputText.AppendBoldText(" They are drunk or poisoned so this should generally be bad information.", Color.Purple);
            }

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
            if (ravenkeeper.DrunkOrPoisoned)
            {
                outputText.AppendBoldText(" They are drunk or poisoned so this should generally be bad information.", Color.Purple);
            }
            outputText.AppendText("\n");

            return await PopulateOptions(ravenkeeperOptions);
        }

        public async Task<IOption> GetCharacterForUndertaker(Player undertaker, Player executedPlayer, IReadOnlyCollection<IOption> undertakerOptions)
        {
            outputText.AppendFormattedText("%p was executed yesterday. Choose a character for %p to learn.", executedPlayer, undertaker, StorytellerView);
            if (undertaker.DrunkOrPoisoned)
            {
                outputText.AppendBoldText(" They are drunk or poisoned so this should generally be bad information.", Color.Purple);
            }
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

        public void AssignCharacter(Player player)
        {
            if (player.Tokens.Contains(Token.IsTheDrunk))
            {
                outputText.AppendFormattedText("%p believes they are the %c but they are actually the %c.\n", player, player.Character, Character.Drunk);
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

        public void NotifyLibrarian(Player librarian, Player playerA, Player playerB, Character character)
        {
            outputText.AppendFormattedText("%p learns that %p or %p is the %c.\n", librarian, playerA, playerB, character, StorytellerView);
        }

        public void NotifyInvestigator(Player investigator, Player playerA, Player playerB, Character character)
        {
            outputText.AppendFormattedText("%p learns that %p or %p is the %c.\n", investigator, playerA, playerB, character, StorytellerView);
        }

        public void NotifySteward(Player steward, Player goodPlayer)
        {
            outputText.AppendFormattedText("%p learns that %p is a good player.\n", steward, goodPlayer, StorytellerView);
        }

        public void NotifyShugenja(Player shugenja, bool clockwise)
        {
            outputText.AppendFormattedText("%p learns that the nearest %a to them is in the %b direction.\n", shugenja, Alignment.Evil, clockwise ? "clockwise" : "counter-clockwise", StorytellerView);
        }

        public void NotifyEmpath(Player empath, Player neighbourA, Player neighbourB, int evilCount)
        {
            outputText.AppendFormattedText($"%p learns that %b of their living neighbours (%p and %p) {(evilCount == 1 ? "is" : "are")} evil.\n", empath, evilCount, neighbourA, neighbourB, StorytellerView);
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

        public void ScarletWomanTrigger(Player demon, Player scarletWoman)
        {
            outputText.AppendFormattedText("%p has died and so %p becomes the new %c.\n", demon, scarletWoman, Character.Imp, StorytellerView);
        }

        private Task<IOption> PopulateOptions(IReadOnlyCollection<IOption> options)
        {
            if (AutoAct)
            {
                // If Pass is an option, pick it 75% of the time.
                var passOption = options.FirstOrDefault(option => option is PassOption);
                if (passOption != null && random.Next(4) < 3)
                {
                    return Task.FromResult(passOption);
                }

                // For now, just pick an option at random.
                // Exclude dead players from our choices.
                var autoOptions = options.Where(option => option is not PassOption)
                                         .Where(option => option is not PlayerOption playerOption || playerOption.Player.Alive)
                                         .ToList();
                return Task.FromResult(autoOptions.RandomPick(random));
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

        private readonly Random random;

        public delegate void ChoiceEventHandler(IOption choice);
        private event ChoiceEventHandler? OnChoice;
        private IReadOnlyCollection<IOption>? options;

        private const bool StorytellerView = true;
    }
}
