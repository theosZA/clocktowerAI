using Clocktower.Agent;
using Clocktower.Agent.Notifier;
using Clocktower.Agent.Observer;
using Clocktower.Game;
using Clocktower.Options;
using System.Text;

namespace Clocktower.Storyteller
{
    public partial class StorytellerForm : Form, IStoryteller
    {
        public IGameObserver Observer { get; private init; }

        public bool AutoAct
        {
            get => autoCheckbox.Checked;
            set => autoCheckbox.Checked = value;
        }

        public StorytellerForm(Random random)
        {
            InitializeComponent();

            this.random = random;

            notifier = new RichTextBoxNotifier(outputText);
            Observer = new TextObserver(notifier, storytellerView: true);
        }

        public void Start()
        {
            Show();
        }

        public void PrivateChatMessage(Player speaker, Player listener, string message)
        {
            AddFormattedText($"%p to %p: %n", speaker, listener, message, StorytellerView);
        }

        public async Task<IOption> GetMarionette(IReadOnlyCollection<IOption> marionetteCandidates)
        {
            AddFormattedText("Choose one good player who will instead be the %c...", Character.Marionette);

            return await PopulateOptions(marionetteCandidates);
        }

        public async Task<IOption> GetWidowPing(Player widow, IReadOnlyCollection<IOption> widowPingCandidates)
        {
            AddFormattedText("Choose one good player who will learn that %p is the %c...", widow, Character.Widow);

            return await PopulateOptions(widowPingCandidates);
        }

        public async Task<IOption> GetDemonBluffs(Player demon, IReadOnlyCollection<IOption> demonBluffOptions)
        {
            AddFormattedText("Choose 3 out-of-play characters to show to the demon, %p...", demon, StorytellerView);

            return await PopulateOptions(demonBluffOptions);
        }

        public async Task<IOption> GetAdditionalDemonBluffs(Player demon, Player snitch, IReadOnlyCollection<IOption> demonBluffOptions)
        {
            AddFormattedText("Because %p has a %c while %p is the %c, they are shown 3 additional out-of-play characters. Choose 3 more out-of-play characters to show to the demon, %p...",
                             demon, Character.Marionette, snitch, Character.Snitch, demon, StorytellerView);

            return await PopulateOptions(demonBluffOptions);
        }

        public async Task<IOption> GetMinionBluffs(Player minion, IReadOnlyCollection<IOption> minionBluffOptions)
        {
            AddFormattedText("Choose 3 out-of-play characters to show to the minion, %p...", minion, StorytellerView);

            return await PopulateOptions(minionBluffOptions);
        }

        public async Task<IOption> GetNewImp(IReadOnlyCollection<IOption> impCandidates)
        {
            AddFormattedText("The %c has star-passed. Choose a minion to become the new %c...", Character.Imp, Character.Imp);

            return await PopulateOptions(impCandidates);
        }

        public async Task<IOption> GetOjoVictims(Player ojo, Character targetCharacter, IReadOnlyCollection<IOption> victimOptions)
        {
            if (victimOptions.FirstOrDefault() is PlayerOption)
            {
                // Case where options are all a single player.
                AddFormattedText("%p has chosen to kill the %c. Choose a matching player to be the %'s victim....", ojo, targetCharacter, Character.Ojo, StorytellerView);
            }
            else
            {
                // Case where options are all subsets of living players.
                AddFormattedText("%p has chosen to kill the %c. Since there is no such player, choose any number of players to die (usually 1 player)...", ojo, targetCharacter, StorytellerView);
            }

            return await PopulateOptions(victimOptions);
        }

        public async Task<IOption> GetDrunk(IReadOnlyCollection<IOption> drunkCandidates)
        {
            AddFormattedText("Choose one townsfolk who will be the %c...", Character.Drunk);

            return await PopulateOptions(drunkCandidates);
        }

        public async Task<IOption> GetSweetheartDrunk(IReadOnlyCollection<IOption> drunkCandidates)
        {
            AddFormattedText("The %c has died. Choose one player to be drunk from now on...", Character.Sweetheart);

            return await PopulateOptions(drunkCandidates);
        }

        public async Task<IOption> GetFortuneTellerRedHerring(Player fortuneTeller, IReadOnlyCollection<IOption> redHerringCandidates)
        {
            AddFormattedText("Choose one player to be the red herring (to register as the demon) for %p...", fortuneTeller, StorytellerView);

            return await PopulateOptions(redHerringCandidates);
        }

        public async Task<IOption> GetWasherwomanPings(Player washerwoman, IReadOnlyCollection<IOption> washerwomanPingCandidates)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("Choose two players who %p will see as a townsfolk, and the character they will see them as.", washerwoman, StorytellerView);
            AppendDrunkDisclaimer(sb, washerwoman);
            var possibleTownsfolk = new HashSet<Player>(washerwomanPingCandidates.Select(option => ((CharacterForTwoPlayersOption)option).PlayerA));
            foreach (var misregister in possibleTownsfolk.Where(player => player.CanRegisterAsTownsfolk && player.CharacterType != CharacterType.Townsfolk))
            {
                sb.AppendFormattedText(" Remember that %p could register as a townsfolk.", misregister, StorytellerView);
            }
            await notifier.Notify(sb.ToString());

            return await PopulateOptions(washerwomanPingCandidates);
        }

        public async Task<IOption> GetInvestigatorPings(Player investigator, IReadOnlyCollection<IOption> investigatorPingCandidates)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("Choose two players who %p will see as a minion, and the character they will see them as.", investigator, StorytellerView);
            AppendDrunkDisclaimer(sb, investigator);
            var possibleMinions = new HashSet<Player>(investigatorPingCandidates.Select(option => ((CharacterForTwoPlayersOption)option).PlayerA));
            foreach (var misregister in possibleMinions.Where(player => player.CanRegisterAsMinion && player.CharacterType != CharacterType.Minion))
            {
                sb.AppendFormattedText(" Remember that %p could register as a minion.", misregister, StorytellerView);
            }
            await notifier.Notify(sb.ToString());

            return await PopulateOptions(investigatorPingCandidates);
        }

        public async Task<IOption> GetLibrarianPings(Player librarian, IReadOnlyCollection<IOption> librarianPingCandidates)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("Choose two players who %p will see as an outsider, and the character they will see them as.", librarian, StorytellerView);
            AppendDrunkDisclaimer(sb, librarian);
            await notifier.Notify(sb.ToString());

            return await PopulateOptions(librarianPingCandidates);
        }

        public async Task<IOption> GetStewardPing(Player steward, IReadOnlyCollection<IOption> stewardPingCandidates)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("Choose one player who %p will see as a good player.", steward, StorytellerView);
            AppendDrunkDisclaimer(sb, steward);
            await notifier.Notify(sb.ToString());

            return await PopulateOptions(stewardPingCandidates);
        }

        public async Task<IOption> GetNobleInformation(Player noble, IReadOnlyCollection<IOption> nobleInformationOptions)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("Choose one evil player and two good players for %p to see.", noble, StorytellerView);
            AppendDrunkDisclaimer(sb, noble);
            await notifier.Notify(sb.ToString());

            return await PopulateOptions(nobleInformationOptions);
        }

        public async Task<IOption> GetChefNumber(Player chef, IEnumerable<Player> playersThatCanMisregister, IReadOnlyCollection<IOption> chefOptions)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("Choose what number to show to %p.", chef, StorytellerView);
            if (!AppendDrunkDisclaimer(sb, chef))
            {
                foreach (var player in playersThatCanMisregister)
                {
                    sb.AppendFormattedText(" Remember that %p could register as %a.", player, player.Alignment == Alignment.Evil ? Alignment.Good : Alignment.Evil, StorytellerView);
                }
            }
            await notifier.Notify(sb.ToString());

            return await PopulateOptions(chefOptions);
        }

        public async Task<IOption> GetEmpathNumber(Player empath, Player neighbourA, Player neighbourB, IReadOnlyCollection<IOption> empathOptions)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("Choose what number to show to %p. Their living neighbours are %p and %p.", empath, neighbourA, neighbourB, StorytellerView);
            if (!AppendDrunkDisclaimer(sb, empath))
            {
                foreach (var player in new[] { neighbourA, neighbourB })
                {
                    if (player.Alignment != Alignment.Evil && player.CanRegisterAsEvil)
                    {
                        sb.AppendFormattedText(" Remember that %p could register as %a.", player, Alignment.Evil, StorytellerView);
                    }
                    if (player.Alignment == Alignment.Evil && player.CanRegisterAsGood)
                    {
                        sb.AppendFormattedText(" Remember that %p could register as %a.", player, Alignment.Good, StorytellerView);
                    }
                }
            }
            await notifier.Notify(sb.ToString());

            return await PopulateOptions(empathOptions);
        }

        public async Task<IOption> GetOracleNumber(Player oracle, IReadOnlyCollection<Player> deadPlayers, IReadOnlyCollection<IOption> oracleOptions)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("Choose what number to show to %p. The dead players are %P.", oracle, deadPlayers, StorytellerView);
            if (!AppendDrunkDisclaimer(sb, oracle))
            {
                foreach (var player in deadPlayers)
                {
                    if (player.Alignment != Alignment.Evil && player.CanRegisterAsEvil)
                    {
                        sb.AppendFormattedText(" Remember that %p could register as %a.", player, Alignment.Evil, StorytellerView);
                    }
                    if (player.Alignment == Alignment.Evil && player.CanRegisterAsGood)
                    {
                        sb.AppendFormattedText(" Remember that %p could register as %a.", player, Alignment.Good, StorytellerView);
                    }
                }
            }
            await notifier.Notify(sb.ToString());

            return await PopulateOptions(oracleOptions);
        }

        public async Task<IOption> GetJugglerNumber(Player juggler, int realJugglerNumber, IReadOnlyCollection<IOption> jugglerOptions)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("Choose what number to show to %p. They made %b correct guesses during the day.", juggler, realJugglerNumber, StorytellerView);
            AppendDrunkDisclaimer(sb, juggler);
            await notifier.Notify(sb.ToString());

            return await PopulateOptions(jugglerOptions);
        }

        public async Task<IOption> GetFortuneTellerReading(Player fortuneTeller, Player targetA, Player targetB, IReadOnlyCollection<IOption> readingOptions)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("Choose whether to say 'Yes' or 'No' to %p to indicate whether they've seen a Demon between %p and %p.", fortuneTeller, targetA, targetB, StorytellerView);
            if (!AppendDrunkDisclaimer(sb, fortuneTeller))
            {
                if (targetA.CharacterType != CharacterType.Demon && targetA.CanRegisterAsDemon)
                {
                    sb.AppendFormattedText(" Remember that %p could register as a demon.", targetA, Alignment.Evil, StorytellerView);
                }
                else if (targetB.CharacterType != CharacterType.Demon && targetB.CanRegisterAsDemon)
                {
                    sb.AppendFormattedText(" Remember that %p could register as a demon.", targetB, Alignment.Evil, StorytellerView);
                }
            }
            await notifier.Notify(sb.ToString());

            return await PopulateOptions(readingOptions);
        }

        public async Task<IOption> GetShugenjaDirection(Player shugenja, Grimoire grimoire, IReadOnlyCollection<IOption> shugenjaOptions)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("Choose whether to indicate 'Clockwise' or 'Counter-clockwise' to %p to indicate the direction to the nearest evil player.", shugenja, StorytellerView);
            AppendDrunkDisclaimer(sb, shugenja);

            var players = grimoire.Players.ToList();
            int shugenjaPosition = players.IndexOf(shugenja);
            bool nonMisregisteringEvil = false;
            for (int step = 1; step < grimoire.Players.Count / 2 && !nonMisregisteringEvil; step++)
            {

                var clockwisePlayer = players[(shugenjaPosition + 1) % grimoire.Players.Count];
                if (clockwisePlayer.CanRegisterAsEvil)
                {
                    if (clockwisePlayer.CanRegisterAsGood)
                    {
                        sb.AppendFormattedText($" %p is %n {(step == 1 ? "step" : "steps")} clockwise and they may register as either good or evil.", clockwisePlayer, step, StorytellerView);
                    }
                    else
                    {
                        sb.AppendFormattedText($" %p is %n {(step == 1 ? "step" : "steps")} clockwise and they are evil.", clockwisePlayer, step, StorytellerView);
                        nonMisregisteringEvil = true;
                    }
                }
                var counterclockwisePlayer = players[(shugenjaPosition - 1 + grimoire.Players.Count) % grimoire.Players.Count];
                if (counterclockwisePlayer.CanRegisterAsEvil)
                {
                    if (counterclockwisePlayer.CanRegisterAsGood)
                    {
                        sb.AppendFormattedText($" %p is %n {(step == 1 ? "step" : "steps")} counter-clockwise and they may register as either good or evil.", counterclockwisePlayer, step, StorytellerView);
                    }
                    else
                    {
                        sb.AppendFormattedText($" %p is %n {(step == 1 ? "step" : "steps")} counter-clockwise and they are evil.", counterclockwisePlayer, step, StorytellerView);
                        nonMisregisteringEvil = true;
                    }
                }
            }

            await notifier.Notify(sb.ToString());

            return await PopulateOptions(shugenjaOptions);
        }

        public async Task<IOption> GetCharacterForRavenkeeper(Player ravenkeeper, Player target, IReadOnlyCollection<IOption> ravenkeeperOptions)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("%p has died and has chosen to learn the character of %p. Choose a character for them to learn.", ravenkeeper, target, StorytellerView);
            AppendDrunkDisclaimer(sb, ravenkeeper);
            await notifier.Notify(sb.ToString());

            return await PopulateOptions(ravenkeeperOptions);
        }

        public async Task<IOption> GetCharacterForUndertaker(Player undertaker, Player executedPlayer, IReadOnlyCollection<IOption> undertakerOptions)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("%p was executed yesterday. Choose a character for %p to learn.", executedPlayer, undertaker, StorytellerView);
            AppendDrunkDisclaimer(sb, undertaker);
            await notifier.Notify(sb.ToString());

            return await PopulateOptions(undertakerOptions);
        }

        public async Task<IOption> GetPlayerForBalloonist(Player balloonist, Player? previousPlayerSeenByBalloonist, IReadOnlyCollection<IOption> balloonistOptions)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("Who should %p see with their %c ability?", balloonist, Character.Balloonist, StorytellerView);
            if (previousPlayerSeenByBalloonist != null)
            {
                sb.AppendFormattedText(" Last night they saw %p.", previousPlayerSeenByBalloonist, StorytellerView);
            }
            AppendDrunkDisclaimer(sb, balloonist);
            await notifier.Notify(sb.ToString());

            return await PopulateOptions(balloonistOptions);
        }

        public async Task<IOption> GetMayorBounce(Player mayor, Player? killer, IReadOnlyCollection<IOption> mayorOptions)
        {
            var sb = new StringBuilder();
            if (killer == null)
            {
                sb.AppendFormattedText("%p is due to die at night. %c deaths at night may be redirected. Choose who will die.", mayor, Character.Mayor, StorytellerView);
            }
            else
            {
                sb.AppendFormattedText("%p has chosen to kill %p. Usually kills on the %c should be redirected. Choose who will die.", killer, mayor, Character.Mayor, StorytellerView);
            }
            await notifier.Notify(sb.ToString());

            return await PopulateOptions(mayorOptions);
        }

        public async Task<IOption> ShouldKillTinker(Player tinker, IReadOnlyCollection<IOption> yesOrNo)
        {
            AddFormattedText("%p can die at any time. Kill them now?", tinker, StorytellerView);
            return await PopulateOptions(yesOrNo);
        }

        public async Task<IOption> ShouldKillWithSlayer(Player slayer, Player target, IReadOnlyCollection<IOption> yesOrNo)
        {
            AddFormattedText("%p is claiming %c and targetting %p. Should %p die?", slayer, Character.Slayer, target, target, StorytellerView);
            return await PopulateOptions(yesOrNo);
        }

        public async Task<IOption> ShouldSaveWithPacifist(Player pacifist, Player executedPlayer, IReadOnlyCollection<IOption> yesOrNo)
        {
            AddFormattedText("%p is to be executed. Should they be saved from death by %p with the %c ability?", executedPlayer, pacifist, Character.Pacifist, StorytellerView);
            return await PopulateOptions(yesOrNo);
        }

        public async Task<IOption> ShouldExecuteWithVirgin(Player virgin, Player nominator, IReadOnlyCollection<IOption> yesOrNo)
        {
            AddFormattedText("%p has nominated %p. Should %p register as a Townsfolk and be executed?\n", nominator, virgin, nominator, StorytellerView);
            return await PopulateOptions(yesOrNo);
        }

        public async Task<IOption> ShouldRegisterForJuggle(Player juggler, Player juggledPlayer, Character juggledCharacter, IReadOnlyCollection<IOption> yesOrNo)
        {
            AddFormattedText("%p has juggled %p as the %c. Should %p register as this character and count as a correct juggle?", juggler, juggledPlayer, juggledCharacter, juggledPlayer, StorytellerView);
            return await PopulateOptions(yesOrNo);
        }

        public async Task<string> GetFishermanAdvice(Player fisherman)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("%p would like their %c advice.", fisherman, Character.Fisherman, StorytellerView);
            AppendDrunkDisclaimer(sb, fisherman);
            await notifier.Notify(sb.ToString());

            if (AutoAct)
            {
                return "I suggest you execute the demon.";
            }
            return await GetTextResponse();
        }

        public async Task<IOption> ChooseFakeCannibalAbility(Player cannibal, Player executedPlayer, IReadOnlyCollection<IOption> characterAbilityOptions)
        {
            AddFormattedText("%p has been executed and died. What ability should %p believe they gained as the %c?", executedPlayer, cannibal, Character.Cannibal, StorytellerView);
            return await PopulateOptions(characterAbilityOptions);
        }

        public async Task<IOption> ChooseDamselCharacter(Player damsel, Player huntsman, IReadOnlyCollection<IOption> characterOptions)
        {
            AddFormattedText("%p has successfully guessed that %p is the %c. What Townsfolk character should %p become?", huntsman, damsel, Character.Damsel, damsel, StorytellerView);
            return await PopulateOptions(characterOptions);
        }

        public async Task<IOption> ChooseNewDamsel(Player damsel, Player huntsman, IReadOnlyCollection<IOption> playerOptions)
        {
            AddFormattedText("%p is no longer the %c and there is still a %c in play. Which good player should become the %c?", damsel, Character.Damsel, Character.Huntsman, Character.Damsel, StorytellerView);
            return await PopulateOptions(playerOptions);
        }

        public void AssignCharacter(Player player)
        {
            if (player.Character != player.RealCharacter)
            {
                AddFormattedText("%p believes they are the %c but they are actually the %c.", player, player.Character, player.RealCharacter, StorytellerView);
            }
            else
            {
                AddFormattedText("%p is the %c.", player, player.Character);
            }
        }

        public void KazaliMinions(Player kazali, IReadOnlyCollection<(Player, Character)> minionChoices)
        {
            var sb = new StringBuilder();

            sb.AppendFormattedText("%p has chosen the following ", kazali, StorytellerView);
            if (minionChoices.Count == 1)
            {
                sb.Append("minion: ");
            }
            else
            {
                sb.Append("minions: ");
            }
            bool first = true;
            foreach (var choice in minionChoices)
            {
                if (!first)
                {
                    sb.Append(", ");
                }
                sb.AppendFormattedText("%p as %c", choice.Item1, choice.Item2, StorytellerView);
                first = false;
            }

            AddMarkupText(sb.ToString());
        }

        public void NewKazaliMinion(Player kazali, Player minionTarget, Character oldMinionCharacter, Character newMinionCharacter)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("Since %c is no longer available, %p has selected for %p to become the %c.", oldMinionCharacter, kazali, minionTarget, newMinionCharacter, StorytellerView);
            AddMarkupText(sb.ToString());
        }

        public void KazaliSoldierMinion(Player soldier, Character minionCharacterPickedBySoldier)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("Because the %c picked the %c to become their minion, %p may choose which minion to become. They have chosen to become the %c.",
                                   Character.Kazali, Character.Soldier, soldier, minionCharacterPickedBySoldier, StorytellerView);
            AddMarkupText(sb.ToString());
        }

        public void MinionInformation(Player minion, Player demon, IReadOnlyCollection<Player> fellowMinions, bool damselInPlay, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            var sb = new StringBuilder();
            if (fellowMinions.Any())
            {
                sb.AppendFormattedText($"%p learns that %p is their demon and that their fellow {(fellowMinions.Count > 1 ? "minions are" : "minion is")} %P", minion, demon, fellowMinions, StorytellerView);
            }
            else
            {
                sb.AppendFormattedText($"%p learns that %p is their demon", minion, demon, StorytellerView);
            }
            if (damselInPlay)
            {
                sb.AppendFormattedText(", and that there is a %c in play", Character.Damsel, StorytellerView);
            }
            if (notInPlayCharacters.Any())
            {
                sb.AppendFormattedText(", and that the following characters are not in play: %C", notInPlayCharacters, StorytellerView);
            }
            sb.Append('.');
            AddMarkupText(sb.ToString());
        }

        public void DemonInformation(Player demon, IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("%p learn that ", demon, StorytellerView);

            var nonMarionetteMinions = minions.Where(minion => minion.RealCharacter != Character.Marionette).ToList();
            sb.AppendFormattedText($"%P {(nonMarionetteMinions.Count > 1 ? "are their minions" : "is their minion")}, ", nonMarionetteMinions, StorytellerView);

            var marionette = minions.FirstOrDefault(minion => minion.RealCharacter == Character.Marionette);
            if (marionette != null)
            {
                sb.AppendFormattedText("%p is their %c, ", marionette, Character.Marionette, StorytellerView);
            }

            sb.AppendFormattedText("and that the following characters are not in play: %C.", notInPlayCharacters, StorytellerView);

            AddMarkupText(sb.ToString());
        }

        public void NotifyGodfather(Player godfather, IReadOnlyCollection<Character> outsiders)
        {
            var sb = new StringBuilder();
            if (outsiders.Count == 0)
            {
                sb.AppendFormattedText("%p learns that there are no outsiders in play.", godfather, StorytellerView);
                return;
            }
            sb.AppendFormattedText("%p learns that the following outsiders are in play: %C", godfather, outsiders, StorytellerView);
            AddMarkupText(sb.ToString());
        }

        public void NotifyWasherwoman(Player washerwoman, Player playerA, Player playerB, Character character)
        {
            AddFormattedText("%p learns that %p or %p is the %c.", washerwoman, playerA, playerB, character, StorytellerView);
        }

        public void NotifyLibrarian(Player librarian, Player playerA, Player playerB, Character character)
        {
            AddFormattedText("%p learns that %p or %p is the %c.", librarian, playerA, playerB, character, StorytellerView);
        }

        public void NotifyLibrarianNoOutsiders(Player librarian)
        {
            AddFormattedText("%p learns that there are no outsiders in play.", librarian, StorytellerView);
        }

        public void NotifyInvestigator(Player investigator, Player playerA, Player playerB, Character character)
        {
            AddFormattedText("%p learns that %p or %p is the %c.", investigator, playerA, playerB, character, StorytellerView);
        }

        public void NotifyChef(Player chef, int evilPairCount)
        {
            AddFormattedText($"%p learns that there {(evilPairCount == 1 ? "is %b pair" : "are %b pairs")} of evil players.", chef, evilPairCount, StorytellerView);
        }

        public void NotifyNoble(Player noble, IReadOnlyCollection<Player> nobleInformation)
        {
            AddFormattedText("%p learns that there is exactly 1 evil player among %P\n", noble, nobleInformation, StorytellerView);
        }

        public void NotifySteward(Player steward, Player goodPlayer)
        {
            AddFormattedText("%p learns that %p is a good player.", steward, goodPlayer, StorytellerView);
        }

        public void NotifyShugenja(Player shugenja, Direction direction)
        {
            AddFormattedText("%p learns that the nearest %a to them is in the %b direction.", shugenja, Alignment.Evil, direction == Direction.Clockwise ? "clockwise" : "counter-clockwise", StorytellerView);
        }

        public void NotifyEmpath(Player empath, Player neighbourA, Player neighbourB, int evilCount)
        {
            AddFormattedText($"%p learns that %b of their living neighbours (%p and %p) {(evilCount == 1 ? "is" : "are")} %a.", empath, evilCount, neighbourA, neighbourB, Alignment.Evil, StorytellerView);
        }

        public void NotifyOracle(Player oracle, int evilCount)
        {
            AddFormattedText($"%p learns that %b of the dead players {(evilCount == 1 ? "is" : "are")} %a.", oracle, evilCount, Alignment.Evil, StorytellerView);
        }

        public void NotifyFortuneTeller(Player fortuneTeller, Player targetA, Player targetB, bool reading)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("%p learns that ", fortuneTeller, StorytellerView);
            if (reading)
            {
                sb.AppendFormattedText("%b, one of %p or %p is the demon.", "Yes", targetA, targetB, StorytellerView);
            }
            else
            {
                sb.AppendFormattedText("%b, neither of %p or %p is the demon.", "No", targetA, targetB, StorytellerView);
            }
            AddMarkupText(sb.ToString());
        }

        public void NotifyUndertaker(Player undertaker, Player executedPlayer, Character executedCharacter)
        {
            AddFormattedText($"%p learns that the recently executed %p is the %c.", undertaker, executedPlayer, executedCharacter, StorytellerView);
        }

        public void NotifyBalloonist(Player balloonist, Player newPlayer)
        {
            AddFormattedText("The next player that %p learns is %p.", balloonist, newPlayer, StorytellerView);
        }

        public void NotifyJuggler(Player juggler, int jugglerCount)
        {
            AddFormattedText("%p learns that %b of their juggles were correct.", juggler, jugglerCount, StorytellerView);
        }

        public void ShowGrimoireToSpy(Player spy, Grimoire grimoire)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("%p now has a chance to look over the Grimoire...", spy, StorytellerView);
            sb.AppendLine();
            sb.Append(TextBuilder.GrimoireToText(grimoire));
            AddMarkupText(sb.ToString());
        }

        public void ShowNightwatchman(Player nightwatchman, Player target, bool shown)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("%p has chosen for %p to learn that they are the %c.", nightwatchman, target, Character.Nightwatchman, StorytellerView);
            if (!shown)
            {
                sb.Append(" There is no effect.");
            }
            AddMarkupText(sb.ToString());
        }

        public void ChoiceFromDemon(Player demon, Player target)
        {
            AddFormattedText("%p has chosen to kill %p.", demon, target, StorytellerView);
        }

        public void ChoiceFromOjo(Player ojo, Character targetCharacter, IReadOnlyCollection<Player> victims)
        {
            switch (victims.Count)
            {
                case 0:
                    AddFormattedText("%p has chosen to kill the %c. No one is killed.", ojo, targetCharacter, StorytellerView);
                    break;

                case 1:
                    AddFormattedText("%p has chosen to kill the %c. %p is the %c's victim.", ojo, targetCharacter, victims.ElementAt(0), Character.Ojo, StorytellerView);
                    break;

                default:
                    AddFormattedText("%p has chosen to kill the %c. %P are the %c's victims.", ojo, targetCharacter, victims, Character.Ojo, StorytellerView);
                    break;
            }
        }

        public void ChoiceFromPoisoner(Player poisoner, Player target)
        {
            AddFormattedText("%p has chosen to poison %p.", poisoner, target, StorytellerView);
        }

        public void ChoiceFromWidow(Player widow, Player target)
        {
            AddFormattedText("%p has chosen to poison %p.", widow, target, StorytellerView);
        }

        public void ChoiceFromWitch(Player witch, Player target)
        {
            AddFormattedText("%p has chosen to curse %p.", witch, target, StorytellerView);
        }

        public void ChoiceFromAssassin(Player assassin, Player? target)
        {
            if (target == null)
            {
                AddFormattedText("%p is not using their ability tonight.", assassin, StorytellerView);
            }
            else
            {
                AddFormattedText("%p has chosen to kill %p.", assassin, target, StorytellerView);
            }
        }

        public void ChoiceFromGodfather(Player godfather, Player target)
        {
            AddFormattedText("%p has chosen to kill %p.", godfather, target, StorytellerView);
        }

        public void ChoiceFromDevilsAdvocate(Player devilsAdvocate, Player target)
        {
            AddFormattedText("%p has chosen to protect %p.", devilsAdvocate, target, StorytellerView);
        }

        public void ChoiceFromMonk(Player monk, Player target)
        {
            AddFormattedText("%p has chosen to protect %p.", monk, target, StorytellerView);
        }

        public void ChoiceFromRavenkeeper(Player ravenkeeper, Player target, Character character)
        {
            AddFormattedText("%p chooses %p and learns that they are the %c.", ravenkeeper, target, character, StorytellerView);
        }

        public void ChoiceFromPhilosopher(Player philosopher, Player? philosopherDrunkedPlayer, Character newCharacterAbility)
        {
            var sb = new StringBuilder();
            if (philosopher.Tokens.HasToken(Token.IsTheBadPhilosopher))
            {
                sb.AppendFormattedText("%p has chosen to gain the ability of the %c. Since they were drunk or poisoned at the time, " +
                                       "they didn't really gain the ability but will continue to be treated like a drunk version of that character.",
                                       philosopher, newCharacterAbility, StorytellerView);
            }
            else
            {
                sb.AppendFormattedText("%p has chosen to gain the ability of the %c.", philosopher, newCharacterAbility, StorytellerView);
                if (philosopherDrunkedPlayer != null)
                {
                    sb.AppendFormattedText(" Since %p is the %c, they are now drunked by the %c.", philosopherDrunkedPlayer, philosopherDrunkedPlayer.RealCharacter, Character.Philosopher, StorytellerView);
                }
            }
            AddMarkupText(sb.ToString());
        }

        public void ChoiceFromButler(Player butler, Player target)
        {
            AddFormattedText("%p has chosen %p to be their master.", butler, target, StorytellerView);
        }

        public void FailedHuntsmanGuess(Player huntsman, Player damsel)
        {
            AddFormattedText("%p has guessed that %p is the %c. There is no effect.", huntsman, damsel, Character.Damsel, StorytellerView);
        }

        public void ScarletWomanTrigger(Player demon, Player scarletWoman)
        {
            AddFormattedText("%p has died and so %p becomes the new %c.", demon, scarletWoman, demon.RealCharacter, StorytellerView);
        }

        public void AcrobatTrigger(Player acrobat, Player triggeringGoodNeighbour)
        {
            AddFormattedText("%p dies since %p is drunk or poisoned.", acrobat, triggeringGoodNeighbour, StorytellerView);
        }

        private void AddFormattedText(string text, params object[] objects)
        {
            var markupText = TextUtilities.FormatMarkupText(text, objects);
            AddMarkupText(markupText);
        }

        private void AddMarkupText(string markupText)
        {
            notifier.AddToTextBox(markupText);
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

            // For PlayerList options, limit it to just options with a single living non-Demon player.
            if (options.Any(option => option is PlayerListOption))
            {
                var autoPlayerListOptions = options.Where(option =>
                {
                    if (option is not PlayerListOption playerListOption)
                    {
                        return false;
                    }
                    var players = playerListOption.GetPlayers().ToList();
                    if (players.Count != 1)
                    {
                        return false;
                    }
                    var player = players[0];
                    return player.Alive && player.CharacterType != CharacterType.Demon;
                }).ToList();
                if (autoPlayerListOptions.Any())
                {
                    return autoPlayerListOptions.RandomPick(random);
                }
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

        private bool AppendDrunkDisclaimer(StringBuilder stringBuilder, Player player)
        {
            if (player.Tokens.HasToken(Token.IsTheBadPhilosopher))
            {
                stringBuilder.Append($" **[color:purple]They were drunk or poisoned when they used their {TextUtilities.CharacterToText(Character.Philosopher)} ability, so they are not really the {TextUtilities.CharacterToText(player.Character)}. Therefore this should generally be bad information.[/color]**");
                return true;
            }
            else if (player.DrunkOrPoisoned)
            {
                stringBuilder.Append(" **[color:purple]They are drunk or poisoned so this should generally be bad information.[/color]**");
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

        private readonly RichTextBoxNotifier notifier;
        private const bool StorytellerView = true;
    }
}
