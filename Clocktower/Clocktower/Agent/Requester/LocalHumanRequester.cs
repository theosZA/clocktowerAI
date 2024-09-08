using Clocktower.Agent.Notifier;
using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Selection;

namespace Clocktower.Agent.Requester
{
    internal class LocalHumanRequester : IMarkupRequester
    {
        public LocalHumanRequester(HumanAgentForm form, IMarkupNotifier notifier, Random random)
        {
            this.form = form;
            this.notifier = notifier;
            this.random = random;
        }

        public Task OnPrivateChatStart(Player playerA, Player playerB)
        {
            if (playerA.Name == form.PlayerName || playerB.Name == form.PlayerName)
            {
                firstMessageInChat = true;
            }

            return Task.CompletedTask;
        }

        public async Task<IOption> RequestCharacterForDemonKill(string prompt, IReadOnlyCollection<IOption> options)
        {
            return await RequestOption(prompt, options);
        }

        public async Task<IOption> RequestPlayerForDemonKill(string prompt, IReadOnlyCollection<IOption> options)
        {
            return await RequestOption(prompt, options);
        }

        public async Task<IOption> RequestUseAbility(string prompt, IReadOnlyCollection<IOption> options)
        {
            return await RequestOption(prompt, options);
        }

        public async Task<IOption> RequestCharacter(string prompt, IReadOnlyCollection<IOption> options)
        {
            return await RequestOption(prompt, options);
        }

        public async Task<IOption> RequestPlayerTarget(string prompt, IReadOnlyCollection<IOption> options)
        {
            return await RequestOption(prompt, options);
        }

        public async Task<IOption> RequestTwoPlayersTarget(string prompt, IReadOnlyCollection<IOption> options)
        {
            return await RequestOption(prompt, options);
        }

        public async Task<IOption> RequestShenanigans(string prompt, IReadOnlyCollection<IOption> options)
        {
            var choice = await RequestOption(prompt, options);

            if (choice is SlayerShotOption slayerOption)
            {
                // We need to choose who to slay.
                if (form.AutoAct)
                {
                    slayerOption.SetTarget(slayerOption.PossiblePlayers.Where(player => player.Alive && player.Name != form.PlayerName).ToList().RandomPick(random));
                }
                else
                {
                    var slayerPrompt = TextUtilities.FormatMarkupText("Choose who you wish to target with your claimed %c ability.", Character.Slayer);
                    var slayerOptions = slayerOption.PossiblePlayers.ToOptions();
                    var slayerChoice = await RequestOption(slayerPrompt, slayerOptions);
                    slayerOption.SetTarget(slayerChoice.GetPlayer());
                }
            }
            else if (choice is JugglerOption jugglerOption)
            {
                // We need to populate the juggle.
                if (form.AutoAct)
                {
                    jugglerOption.AddJuggles(Enumerable.Range(0, 5).Select(_ => (jugglerOption.PossiblePlayers.ToList().RandomPick(random), jugglerOption.ScriptCharacters.ToList().RandomPick(random))));
                }
                else
                {
                    var juggleDialog = new PlayersAsCharactersDialog("Choose your juggles", 5, jugglerOption.PossiblePlayers, jugglerOption.ScriptCharacters,
                                                                     juggles =>
                                                                     {
                                                                         return juggles.All(juggle => jugglerOption.PossiblePlayers.Contains(juggle.player) && jugglerOption.ScriptCharacters.Contains(juggle.character));
                                                                     });
                    var result = juggleDialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        jugglerOption.AddJuggles(juggleDialog.GetPlayersAsCharacters());
                    }
                }
            }
            else if (choice is MinionGuessingDamselOption damselOption)
            {
                // We need to choose who to guess as Damsel.
                if (form.AutoAct)
                {
                    damselOption.SetTarget(damselOption.PossiblePlayers.Where(player => player.Alive && player.Name != form.PlayerName).ToList().RandomPick(random));
                }
                else
                {
                    var minionDamselPrompt = TextUtilities.FormatMarkupText("Choose who you wish to guess as the %c.", Character.Damsel);
                    var damselOptions = damselOption.PossiblePlayers.ToOptions();
                    var damselChoice = await RequestOption(minionDamselPrompt, damselOptions);
                    damselOption.SetTarget(damselChoice.GetPlayer());
                }
            }

            return choice;
        }

        public async Task<IOption> RequestNomination(string prompt, IReadOnlyCollection<IOption> options)
        {
            return await RequestOption(prompt, options);
        }

        public async Task<IOption> RequestVote(string prompt, bool ghostVote, IReadOnlyCollection<IOption> options)
        {
            return await RequestOption(prompt, options);
        }

        public async Task<IOption> RequestPlayerForChat(string prompt, IReadOnlyCollection<IOption> options)
        {
            return await RequestOption(prompt, options);
        }

        public async Task<(string dialogue, bool endChat)> RequestMessageForChat(string prompt)
        {
            await notifier.Notify(prompt);
            var speech = await GetSpeech(autoActText: firstMessageInChat && form.AutoClaim.HasValue ? $"I am the {TextUtilities.CharacterToText(form.AutoClaim.Value)}." : string.Empty);
            if (!string.IsNullOrEmpty(speech))
            {
                firstMessageInChat = false;
                await notifier.Notify(TextUtilities.FormatMarkupText("%b: %n", form.PlayerName, speech));
            }
            return (speech, false);
        }

        public async Task<string> RequestStatement(string prompt, IMarkupRequester.Statement statement)
        {
            await notifier.Notify(prompt);

            var autoActText = statement switch
            {
                IMarkupRequester.Statement.RollCall => form.AutoClaim.HasValue ? $"I am the {TextUtilities.CharacterToText(form.AutoClaim.Value)}." : string.Empty,
                IMarkupRequester.Statement.SelfNomination => "There are reasons for me to be executed now. Trust me.",
                IMarkupRequester.Statement.Prosection => "I believe they are evil and should be executed.",
                IMarkupRequester.Statement.Defence => "I'm not evil. Please believe me.",
                _ => string.Empty
            };

            return await GetSpeech(autoActText);
        }

        public Task RequestKazaliMinions(string prompt, KazaliMinionsSelection kazaliMinionsSelection)
        {
            if (form.AutoAct)
            {
                var players = kazaliMinionsSelection.PossiblePlayers.ToList().RandomPickN(kazaliMinionsSelection.MinionCount, random);
                var characters = kazaliMinionsSelection.MinionCharacters.ToList().RandomPickN(kazaliMinionsSelection.MinionCount, random);
                kazaliMinionsSelection.SelectMinions(players.Zip(characters).ToList());
            }
            else
            {
                var kazaliDialog = new PlayersAsCharactersDialog("Choose your minions", kazaliMinionsSelection.MinionCount, kazaliMinionsSelection.PossiblePlayers, kazaliMinionsSelection.MinionCharacters,
                                                                 kazaliMinionsSelection.ValidateSelection);
                var result = kazaliDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    kazaliMinionsSelection.SelectMinions(kazaliDialog.GetPlayersAsCharacters().ToList());
                }
            }
            
            return Task.CompletedTask;
        }

        private async Task<IOption> RequestOption(string prompt, IReadOnlyCollection<IOption> options)
        {
            await notifier.Notify(prompt);
            return await PopulateOptions(options);
        }

        private async Task<IOption> PopulateOptions(IReadOnlyCollection<IOption> options)
        {
            if (form.AutoAct)
            {
                var autoChosenOption = AutoChooseOption(options);
                await WritePlayerInputToOutput(autoChosenOption.Name);
                return autoChosenOption;
            }

            return await form.RequestOptionChoice(options);
        }

        private IOption AutoChooseOption(IReadOnlyCollection<IOption> options)
        {
            // If Juggle is an option and we are the Juggler, always pick it since we only get one chance.
            if ((form.Character == Character.Juggler || form.AutoClaim == Character.Juggler) && !usedOncePerGameDayAbility)
            {
                var jugglerOption = options.FirstOrDefault(option => option is JugglerOption);
                if (jugglerOption != null)
                {
                    usedOncePerGameDayAbility = true;
                    return jugglerOption;
                }
            }
            // If Slayer is an option and we are the Slayer, pick it 50% of the time.
            if ((form.Character == Character.Slayer || form.AutoClaim == Character.Slayer) && !usedOncePerGameDayAbility && random.Next(2) == 1)
            {
                var slayerOption = options.FirstOrDefault(option => option is SlayerShotOption);
                if (slayerOption != null)
                {
                    usedOncePerGameDayAbility = true;
                    return slayerOption;
                }
            }
            // If Damsel guess is an option and we are a minion, pick it 10% of the time.
            if (form.Character?.CharacterType() == CharacterType.Minion && random.Next(10) == 0)
            {
                var damselOption = options.FirstOrDefault(option => option is MinionGuessingDamselOption);
                if (damselOption != null)
                {
                    return damselOption;
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
                                     .Where(option => option is not PlayerOption playerOption || (playerOption.Player.Alive && playerOption.Player.Name != form.PlayerName))
                                     .Where(option => option is not VoteOption voteOption || (voteOption.Nominee.Alive && voteOption.Nominee.Name != form.PlayerName))
                                     .Where(option => option is not SlayerShotOption)
                                     .Where(option => option is not JugglerOption)
                                     .Where(option => option is not MinionGuessingDamselOption)
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
            if (form.AutoAct)
            {
                await WriteSpeechToOutput(autoActText);
                return autoActText;
            }

            return await GetSpeech();
        }

        private Task<string> GetSpeech()
        {
            return form.RequestText();
        }

        private async Task WriteSpeechToOutput(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                await WritePlayerInputToOutput("...");
            }
            else
            {
                await WritePlayerInputToOutput($"\"{text}\"");
            }
        }

        private async Task WritePlayerInputToOutput(string playerInput)
        {
            await notifier.Notify($"**[color:green]>> {playerInput}[/color]**");
        }

        private readonly HumanAgentForm form;
        private readonly IMarkupNotifier notifier;
        private readonly Random random;

        private bool firstMessageInChat = false;
        private bool usedOncePerGameDayAbility = false;
    }
}
