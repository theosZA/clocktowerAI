using Clocktower.Agent.Notifier;
using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Options;
using System.Reactive;
using System.Text;

namespace Clocktower.Agent
{
    public partial class HumanAgentForm : Form, IAgent
    {
        public string PlayerName { get; private set; }

        public IGameObserver Observer { get; private init; }

        public bool AutoAct
        {
            get => autoCheckbox.Checked;
            set => autoCheckbox.Checked = value;
        }

        public HumanAgentForm(string playerName, IReadOnlyCollection<string> players, string scriptName, IReadOnlyCollection<Character> script, Random random)
        {
            InitializeComponent();

            this.script = script.ToList();
            this.random = random;

            PlayerName = playerName;
            Text = playerName;

            var notifier = new RichTextBoxNotifier(outputText);
            Observer = new TextObserver(notifier);
            textAgent = new TextAgent(playerName, players, scriptName, script, Observer, notifier);
        }

        public async Task StartGame()
        {
            Show();
            await textAgent.StartGame();
        }

        public async Task AssignCharacter(Character character, Alignment alignment)
        {
            this.character = character;

            SetTitleText();

            await textAgent.AssignCharacter(character, alignment);

            autoClaim = character;
            if (autoClaim.Value.Alignment() == Alignment.Evil)
            {
                autoClaim = script.OfAlignment(Alignment.Good).ToList().RandomPick(random);
            }
        }

        public async Task YouAreDead()
        {
            alive = false;

            SetTitleText();

            await textAgent.YouAreDead();
        }

        public async Task MinionInformation(Player demon, IReadOnlyCollection<Player> fellowMinions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            await textAgent.MinionInformation(demon, fellowMinions, notInPlayCharacters);
        }

        public async Task DemonInformation(IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            autoClaim = notInPlayCharacters.ToList().RandomPick(random);

            await textAgent.DemonInformation(minions, notInPlayCharacters);
        }

        public async Task NotifyGodfather(IReadOnlyCollection<Character> outsiders)
        {
            await textAgent.NotifyGodfather(outsiders);
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

        public async Task NotifyFortuneTeller(Player targetA, Player targetB, bool reading)
        {
            await textAgent.NotifyFortuneTeller(targetA, targetB, reading);
        }

        public async Task NotifyEmpath(Player neighbourA, Player neighbourB, int evilCount)
        {
            await textAgent.NotifyEmpath(neighbourA, neighbourB, evilCount);
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
            originalCharacter = this.character;
            this.character = character;

            SetTitleText();

            await textAgent.GainCharacterAbility(character);
        }

        public async Task<IOption> RequestChoiceFromDemon(Character demonCharacter, IReadOnlyCollection<IOption> options)
        {
            outputText.AppendFormattedText("As the %c please choose a player to kill...\n", demonCharacter);
            return await PopulateOptions(options);
        }

        public async Task<IOption> RequestChoiceFromOjo(IReadOnlyCollection<IOption> options)
        {
            outputText.AppendFormattedText("As the %c please choose a character to kill...\n", Character.Ojo);
            return await PopulateOptions(options);
        }

        public async Task<IOption> RequestChoiceFromPoisoner(IReadOnlyCollection<IOption> options)
        {
            outputText.AppendFormattedText("As the %c please choose a player to poison...\n", Character.Poisoner);
            return await PopulateOptions(options);
        }

        public async Task<IOption> RequestChoiceFromWitch(IReadOnlyCollection<IOption> options)
        {
            outputText.AppendFormattedText("As the %c please choose a player to curse...\n", Character.Witch);
            return await PopulateOptions(options);
        }

        public async Task<IOption> RequestChoiceFromAssassin(IReadOnlyCollection<IOption> options)
        {
            outputText.AppendFormattedText("As the %c, if you wish to use your ability tonight, please choose a player to kill...\n", Character.Assassin);
            return await PopulateOptions(options);
        }

        public async Task<IOption> RequestChoiceFromGodfather(IReadOnlyCollection<IOption> options)
        {
            outputText.AppendFormattedText("As the %c please choose a player to kill...\n", Character.Godfather);
            return await PopulateOptions(options);
        }

        public async Task<IOption> RequestChoiceFromDevilsAdvocate(IReadOnlyCollection<IOption> options)
        {
            outputText.AppendFormattedText("As the %c please choose a player to protect from execution.\n", Character.Devils_Advocate);
            return await PopulateOptions(options);
        }

        public async Task<IOption> RequestChoiceFromPhilosopher(IReadOnlyCollection<IOption> options)
        {
            if (character == Character.Cannibal)
            {
                outputText.AppendText("Do you wish to use your ability tonight?\n");
                bool useAbility = await PopulateOptions(OptionsBuilder.YesOrNo) is YesOption;
                if (!useAbility)
                {
                    return options.First(option => option is PassOption);
                }
                outputText.AppendText("Please choose a Townsfolk or Outsider character...\n");
                return await PopulateOptions(options.Where(option => option is not PassOption).ToList());
            }
            else
            {
                outputText.AppendFormattedText("As the %c, if you wish to use your ability tonight, please choose a Townsfolk or Outsider character whose ability you wish to acquire...\n", Character.Philosopher);
                return await PopulateOptions(options);
            }
        }

        public async Task<IOption> RequestChoiceFromFortuneTeller(IReadOnlyCollection<IOption> options)
        {
            if (character == Character.Cannibal)
            {
                outputText.AppendText("Please choose two players...\n");
            }
            else
            {
                outputText.AppendFormattedText("As the %c please choose two players...\n", Character.Fortune_Teller);
            }
            return await PopulateOptions(options);
        }

        public async Task<IOption> RequestChoiceFromMonk(IReadOnlyCollection<IOption> options)
        {
            if (character == Character.Cannibal)
            {
                outputText.AppendText("Please choose a player...\n");
            }
            else
            {
                outputText.AppendFormattedText("As the %c, please choose a player to protect...\n", Character.Monk);
            }
            return await PopulateOptions(options);
        }

        public async Task<IOption> RequestChoiceFromRavenkeeper(IReadOnlyCollection<IOption> options)
        {
            if (character == Character.Cannibal)
            {
                outputText.AppendText("Please choose a player...\n");
            }
            else
            {
                outputText.AppendFormattedText("As the %c, please choose a player whose character you wish to learn...\n", Character.Ravenkeeper);
            }
            return await PopulateOptions(options);
        }

        public async Task<IOption> RequestChoiceFromButler(IReadOnlyCollection<IOption> options)
        {
            outputText.AppendFormattedText("As the %c, please choose a player. Tomorrow, you will only be able vote on a nomination if they have already voted for that nomination.\n", Character.Butler);
            return await PopulateOptions(options);
        }

        public async Task<IOption> PromptFishermanAdvice(IReadOnlyCollection<IOption> options)
        {
            outputText.AppendFormattedText("Do you wish to go to the Storyteller for your %c advice?\n", Character.Fisherman);
            return await PopulateOptions(options);
        }

        public async Task<IOption> PromptShenanigans(IReadOnlyCollection<IOption> options)
        {
            outputText.AppendText("You have the option now to use or bluff any abilities that are to be publicly used during the day.\n");
            var choice = await PopulateOptions(options);

            if (choice is SlayerShotOption slayerOption)
            {
                // We need to choose who to slay.
                if (AutoAct)
                {
                    slayerOption.SetTarget(slayerOption.PossiblePlayers.Where(player => player.Alive && player.Name != Name).ToList().RandomPick(random));
                }
                else
                {
                    outputText.AppendFormattedText("Choose who you wish to target with your claimed %c ability.\n", Character.Slayer);
                    var target = (await PopulateOptions(slayerOption.PossiblePlayers.ToOptions())).GetPlayer();
                    slayerOption.SetTarget(target);
                }
            }
            else if (choice is JugglerOption jugglerOption)
            {
                // We need to populate the juggle.
                if (AutoAct)
                {
                    jugglerOption.AddJuggles(Enumerable.Range(0, 5).Select(_ => (jugglerOption.PossiblePlayers.ToList().RandomPick(random), jugglerOption.ScriptCharacters.ToList().RandomPick(random))));
                }
                else
                {
                    var juggleDialog = new JuggleDialog(jugglerOption.PossiblePlayers, jugglerOption.ScriptCharacters);
                    var result = juggleDialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        jugglerOption.AddJuggles(juggleDialog.GetJuggles());
                    }
                }
            }

            return choice;
        }

        public async Task<IOption> GetNomination(IReadOnlyCollection<IOption> options)
        {
            outputText.AppendText("Please nominate a player or pass...\n");
            return await PopulateOptions(options);
        }

        public async Task<IOption> GetVote(IReadOnlyCollection<IOption> options, bool ghostVote)
        {
            var voteOption = (VoteOption)(options.First(option => option is VoteOption));
            outputText.AppendFormattedText("If you wish, you may vote for executing %p or pass.", voteOption.Nominee);
            if (ghostVote)
            {
                outputText.AppendText(" (Note that because you are dead, you only have one vote remaining for the rest of the game.)");
            }
            outputText.AppendText("\n");
            return await PopulateOptions(options);
        }

        public async Task<IOption> OfferPrivateChat(IReadOnlyCollection<IOption> options)
        {
            var canPass = options.Any(option => option is PassOption);
            if (canPass)
            {
                outputText.AppendText("Is there someone you wish to speak to privately as a priority? You can pass to wait to see if anyone wants to speak to you first.\n");
            }
            else
            {
                outputText.AppendText("Who will you go speak to privately.\n");
            }
            return await PopulateOptions(options);
        }

        public async Task<string> GetRollCallStatement()
        {
            outputText.AppendText("For this roll call, provide your public statement about your character (or bluff) and possibly elaborate on what you learned or how you used your character. (This is optional - leave empty to say nothing.)\n");
            return await GetSpeech(autoActText: autoClaim.HasValue ? $"I am the { TextUtilities.CharacterToText(autoClaim.Value)}.": string.Empty);
        }

        public async Task<string> GetMorningPublicStatement()
        {
            outputText.AppendText("Before the group breaks off for private conversations, do you wish to say anything publicly? (Leave empty to say nothing.)\n");
            return await GetSpeech(autoActText: string.Empty);
        }

        public async Task<string> GetEveningPublicStatement()
        {
            outputText.AppendText("Before nominations are opened, do you wish to say anything publicly? (Leave empty to say nothing.)\n");
            return await GetSpeech(autoActText: string.Empty);
        }

        public async Task<string> GetProsecution(Player nominee)
        {
            outputText.AppendFormattedText("You have nominated %p. Present the case to have them executed. (Leave empty to say nothing.)\n", nominee);
            return await GetSpeech(autoActText: $"I believe {nominee.Name} is evil and should be executed.");
        }

        public async Task<string> GetDefence(Player nominator)
        {
            outputText.AppendFormattedText("You have been nominated by %p. Present the case for your defence. (Leave empty to say nothing.)\n", nominator);
            return await GetSpeech(autoActText: "I'm not evil. Please believe me.");
        }

        public async Task<string> GetReasonForSelfNomination()
        {
            outputText.AppendText("You have nominated yourself. You may present your reason now. (Leave empty to say nothing.)\n");
            return await GetSpeech(autoActText: "There are reasons for me to be executed now. Trust me.");
        }

        public async Task StartPrivateChat(Player otherPlayer)
        {
            firstMessageInChat = true;
            await textAgent.StartPrivateChat(otherPlayer);
        }

        public async Task<(string message, bool endChat)> GetPrivateChat(Player listener)
        {
            outputText.AppendFormattedText("What will you say to %p? You may say nothing to end the conversation, unless they haven't spoken yet.\n", listener);
            var speech = await GetSpeech(autoActText: firstMessageInChat && autoClaim.HasValue ? $"I am the {TextUtilities.CharacterToText(autoClaim.Value)}." : string.Empty);
            if (!string.IsNullOrEmpty(speech))
            {
                firstMessageInChat = false;
                outputText.AppendFormattedText($"%b: %n\n", PlayerName, speech);
            }
            return (speech, false);
        }

        public async Task PrivateChatMessage(Player speaker, string message)
        {
            await textAgent.PrivateChatMessage(speaker, message);
        }

        public async Task EndPrivateChat(Player otherPlayer)
        {
            await textAgent.EndPrivateChat(otherPlayer);
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
            // If Juggle is an option and we are the Juggler, always pick it since we only get one chance.
            if ((character == Character.Juggler || autoClaim == Character.Juggler) && !usedOncePerGameDayAbility)
            {
                var jugglerOption = options.FirstOrDefault(option => option is JugglerOption);
                if (jugglerOption != null)
                {
                    usedOncePerGameDayAbility = true;
                    return jugglerOption;
                }
            }
            // If Slayer is an option and we are the Slayer, pick it 50% of the time.
            if ((character == Character.Slayer || autoClaim == Character.Slayer) && !usedOncePerGameDayAbility && random.Next(2) == 1)
            {
                var slayerOption = options.FirstOrDefault(option => option is SlayerShotOption);
                if (slayerOption != null)
                {
                    usedOncePerGameDayAbility = true;
                    return slayerOption;
                }
            }

            // If Pass is an option, pick it 40% of the time.
            var passOption = options.FirstOrDefault(option => option is PassOption);
            if (passOption != null && random.Next(5) < 2)
            {
                return passOption;
            }

            // For now, just pick an option at random.
            // Exclude dead players and ourself from our choices.
            // Also exclude public claims as they are handled above.
            var autoOptions = options.Where(option => option is not PassOption)
                                     .Where(option => option is not PlayerOption playerOption || (playerOption.Player.Alive && playerOption.Player.Name != PlayerName))
                                     .Where(option => option is not VoteOption voteOption || (voteOption.Nominee.Alive && voteOption.Nominee.Name != PlayerName))
                                     .Where(option => option is not SlayerShotOption)
                                     .Where(option => option is not JugglerOption)
                                     .ToList();
            if (autoOptions.Count > 0)
            {
                return autoOptions.RandomPick(random);
            }

            // No okay options. Then pick Pass if we can.
            if (passOption != null)
            {
                return passOption;
            }
            return options.ToList().RandomPick(random);
        }

        private async Task<string> GetSpeech(string autoActText)
        {
            if (AutoAct)
            {
                WriteSpeechToOutput(autoActText);
                return autoActText;
            }

            return await GetSpeech();
        }

        private Task<string> GetSpeech()
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

        private void WriteSpeechToOutput(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                outputText.AppendBoldText($">> ...\n", Color.Green);
            }
            else
            {
                outputText.AppendBoldText($">> \"{text}\"\n", Color.Green);
            }
        }

        private void SetTitleText()
        {
            Text = PlayerName;
            if (character != null)
            {
                Text += " (";
                if (originalCharacter != null)
                {
                    Text += $"{TextUtilities.CharacterToText(originalCharacter.Value)}-";
                }
                Text += $"{TextUtilities.CharacterToText(character.Value)})";
            }
            if (!alive)
            {
                Text += " GHOST";
            }
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
            WriteSpeechToOutput(text);

            submitButton.Enabled = false;
            responseTextBox.Enabled = false;
            responseTextBox.Text = null;

            OnText?.Invoke(text);
        }

        private readonly IAgent textAgent;

        private readonly List<Character> script;
        private readonly Random random;

        private Character? originalCharacter;
        private Character? character;
        private Character? autoClaim;
        private bool alive = true;
        private bool usedOncePerGameDayAbility = false;
        private bool firstMessageInChat = true;

        public delegate void ChoiceEventHandler(IOption choice);
        private event ChoiceEventHandler? OnChoice;
        private IReadOnlyCollection<IOption>? options;

        public delegate void TextEventHandler(string text);
        private event TextEventHandler? OnText;
    }
}
