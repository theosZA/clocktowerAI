using Clocktower.Agent;
using Clocktower.Agent.Notifier;
using Clocktower.Agent.Observer;
using Clocktower.Game;
using Clocktower.Options;
using System.Text;

namespace Clocktower.Storyteller
{
    internal class TextStoryteller : IStoryteller
    {
        public IGameObserver Observer { get; private init; }

        public Func<Task>? OnStartGame { get; set; }
        public Action<string>? SendMarkupText { get; set; }
        public Func<string, IReadOnlyCollection<IOption>, Task<IOption>>? PromptMarkupText { get; set; }
        public Func<string, Task<string>>? PromptForTextResponse { get; set; }

        public TextStoryteller(IMarkupNotifier notifier)
        {
            Observer = new TextObserver(notifier, storytellerView: true);
        }

        public async Task StartGame()
        {
            if (OnStartGame != null)
            {
                await OnStartGame();
            }
        }

        public Task<IOption> GetMarionette(IReadOnlyCollection<IOption> marionetteCandidates)
        {
            return Prompt(marionetteCandidates, "Choose one good player who will instead be the %c...", Character.Marionette);
        }

        public Task<IOption> GetEvilTownsfolk(Player bountyHunter, IReadOnlyCollection<IOption> evilTownsfolkCandidates)
        {
            if (evilTownsfolkCandidates.Any(option => option is PassOption))
            {
                return Prompt(evilTownsfolkCandidates, "%p is now a %c, so you may optionally choose one Townsfolk to become evil...", bountyHunter, Character.Bounty_Hunter, StorytellerView);
            }
            else
            {
                return Prompt(evilTownsfolkCandidates, "Because a %c is in this game, choose one Townsfolk who will be evil...", Character.Bounty_Hunter);
            }
        }

        public Task<IOption> GetWidowPing(Player widow, IReadOnlyCollection<IOption> widowPingCandidates)
        {
            return Prompt(widowPingCandidates, "Choose one good player who will learn that there is a %c in play...", Character.Widow);
        }

        public Task<IOption> GetDemonBluffs(Player demon, IReadOnlyCollection<IOption> demonBluffOptions)
        {
            return Prompt(demonBluffOptions, "Choose 3 out-of-play characters to show to the demon, %p...", demon, StorytellerView);
        }

        public Task<IOption> GetAdditionalDemonBluffs(Player demon, Player snitch, IReadOnlyCollection<IOption> demonBluffOptions)
        {
            return Prompt(demonBluffOptions,
                          "Because %p has a %c while %p is the %c, they are shown 3 additional out-of-play characters. Choose 3 more out-of-play characters to show to the demon, %p...",
                          demon, Character.Marionette, snitch, Character.Snitch, demon, StorytellerView);
        }

        public Task<IOption> GetMinionBluffs(Player minion, IReadOnlyCollection<IOption> minionBluffOptions)
        {
            return Prompt(minionBluffOptions, "Choose 3 out-of-play characters to show to the minion, %p...", minion, StorytellerView);
        }

        public Task<IOption> GetNewImp(IReadOnlyCollection<IOption> impCandidates)
        {
            return Prompt(impCandidates, "The %c has star-passed. Choose a minion to become the new %c...", Character.Imp, Character.Imp);
        }

        public Task<IOption> GetOjoVictims(Player ojo, Character targetCharacter, IReadOnlyCollection<IOption> victimOptions)
        {
            if (victimOptions.FirstOrDefault() is PlayerOption)
            {
                // Case where options are all a single player.
                return Prompt(victimOptions, "%p has chosen to kill the %c. Choose a matching player to be the %'s victim....", ojo, targetCharacter, Character.Ojo, StorytellerView);
            }
            else
            {
                // Case where options are all subsets of living players.
                return Prompt(victimOptions, "%p has chosen to kill the %c. Since there is no such player, choose any number of players to die (usually 1 player)...", ojo, targetCharacter, StorytellerView);
            }
        }

        public Task<IOption> GetTownsfolkPoisonedByVigormortis(Player minion, IReadOnlyCollection<IOption> poisonOptions)
        {
            return Prompt(poisonOptions, "The %c has killed %p, a minion. Choose which of their townsfolk neighbours should be poisoned...", Character.Vigormortis, minion, StorytellerView);
        }

        public Task<IOption> GetDrunk(IReadOnlyCollection<IOption> drunkCandidates)
        {
            return Prompt(drunkCandidates, "Choose one townsfolk who will be the %c...", Character.Drunk);
        }

        public Task<IOption> GetSweetheartDrunk(IReadOnlyCollection<IOption> drunkCandidates)
        {
            return Prompt(drunkCandidates, "The %c has died. Choose one player to be drunk from now on...", Character.Sweetheart);
        }

        public Task<IOption> GetFortuneTellerRedHerring(Player fortuneTeller, IReadOnlyCollection<IOption> redHerringCandidates)
        {
            return Prompt(redHerringCandidates, "Choose one player to be the red herring (to register as the demon) for %p...", fortuneTeller, StorytellerView);
        }

        public Task<IOption> GetWasherwomanPings(Player washerwoman, IReadOnlyCollection<IOption> washerwomanPingCandidates)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("Choose two players who %p will see as a townsfolk, and the character they will see them as.", washerwoman, StorytellerView);
            AppendDrunkDisclaimer(sb, washerwoman);
            var possibleTownsfolk = new HashSet<Player>(washerwomanPingCandidates.Select(option => ((CharacterForTwoPlayersOption)option).PlayerA));
            foreach (var misregister in possibleTownsfolk.Where(player => player.CanRegisterAsTownsfolk && player.CharacterType != CharacterType.Townsfolk))
            {
                sb.AppendFormattedText(" Remember that %p could register as a townsfolk.", misregister, StorytellerView);
            }

            return Prompt(sb, washerwomanPingCandidates);
        }

        public Task<IOption> GetInvestigatorPings(Player investigator, IReadOnlyCollection<IOption> investigatorPingCandidates)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("Choose two players who %p will see as a minion, and the character they will see them as.", investigator, StorytellerView);
            AppendDrunkDisclaimer(sb, investigator);
            var possibleMinions = new HashSet<Player>(investigatorPingCandidates.Select(option => ((CharacterForTwoPlayersOption)option).PlayerA));
            foreach (var misregister in possibleMinions.Where(player => player.CanRegisterAsMinion && player.CharacterType != CharacterType.Minion))
            {
                sb.AppendFormattedText(" Remember that %p could register as a minion.", misregister, StorytellerView);
            }

            return Prompt(sb, investigatorPingCandidates);
        }

        public Task<IOption> GetLibrarianPings(Player librarian, IReadOnlyCollection<IOption> librarianPingCandidates)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("Choose two players who %p will see as an outsider, and the character they will see them as.", librarian, StorytellerView);
            AppendDrunkDisclaimer(sb, librarian);

            return Prompt(sb, librarianPingCandidates);
        }

        public Task<IOption> GetStewardPing(Player steward, IReadOnlyCollection<IOption> stewardPingCandidates)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("Choose one player who %p will see as a good player.", steward, StorytellerView);
            AppendDrunkDisclaimer(sb, steward);

            return Prompt(sb, stewardPingCandidates);
        }

        public Task<IOption> GetBountyHunterPing(Player bountyHunter, IReadOnlyCollection<IOption> bountyHunterPingCandidates)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("Choose one player who %p will see as an evil player. (This should almost always be a living player to ensure they can continue to gain from their ability.)", bountyHunter, StorytellerView);
            AppendDrunkDisclaimer(sb, bountyHunter);

            return Prompt(sb, bountyHunterPingCandidates);
        }

        public Task<IOption> GetNobleInformation(Player noble, IReadOnlyCollection<IOption> nobleInformationOptions)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("Choose one evil player and two good players for %p to see.", noble, StorytellerView);
            AppendDrunkDisclaimer(sb, noble);

            return Prompt(sb, nobleInformationOptions);
        }

        public Task<IOption> GetChefNumber(Player chef, IEnumerable<Player> playersThatCanMisregister, IReadOnlyCollection<IOption> chefOptions)
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

            return Prompt(sb, chefOptions);
        }

        public Task<IOption> GetEmpathNumber(Player empath, Player neighbourA, Player neighbourB, IReadOnlyCollection<IOption> empathOptions)
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

            return Prompt(sb, empathOptions);
        }

        public Task<IOption> GetOracleNumber(Player oracle, IReadOnlyCollection<Player> deadPlayers, IReadOnlyCollection<IOption> oracleOptions)
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

            return Prompt(sb, oracleOptions);
        }

        public Task<IOption> GetJugglerNumber(Player juggler, int realJugglerNumber, IReadOnlyCollection<IOption> jugglerOptions)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("Choose what number to show to %p. They made %b correct guesses during the day.", juggler, realJugglerNumber, StorytellerView);
            AppendDrunkDisclaimer(sb, juggler);

            return Prompt(sb, jugglerOptions);
        }

        public Task<IOption> GetFortuneTellerReading(Player fortuneTeller, Player targetA, Player targetB, IReadOnlyCollection<IOption> readingOptions)
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

            return Prompt(sb, readingOptions);
        }

        public Task<IOption> GetShugenjaDirection(Player shugenja, Grimoire grimoire, IReadOnlyCollection<IOption> shugenjaOptions)
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

            return Prompt(sb, shugenjaOptions);
        }

        public Task<IOption> GetCharacterForRavenkeeper(Player ravenkeeper, Player target, IReadOnlyCollection<IOption> ravenkeeperOptions)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("%p has died and has chosen to learn the character of %p. Choose a character for them to learn.", ravenkeeper, target, StorytellerView);
            AppendDrunkDisclaimer(sb, ravenkeeper);

            return Prompt(sb, ravenkeeperOptions);
        }

        public Task<IOption> GetCharacterForUndertaker(Player undertaker, Player executedPlayer, IReadOnlyCollection<IOption> undertakerOptions)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("%p was executed yesterday. Choose a character for %p to learn.", executedPlayer, undertaker, StorytellerView);
            AppendDrunkDisclaimer(sb, undertaker);

            return Prompt(sb, undertakerOptions);
        }

        public Task<IOption> GetPlayerForBalloonist(Player balloonist, Player? previousPlayerSeenByBalloonist, IReadOnlyCollection<IOption> balloonistOptions)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("Who should %p see with their %c ability?", balloonist, Character.Balloonist, StorytellerView);
            if (previousPlayerSeenByBalloonist != null)
            {
                sb.AppendFormattedText(" Last night they saw %p.", previousPlayerSeenByBalloonist, StorytellerView);
            }
            AppendDrunkDisclaimer(sb, balloonist);

            return Prompt(sb, balloonistOptions);
        }

        public Task<IOption> GetPlayerForHighPriestess(Player highPriestess, IReadOnlyCollection<IOption> highPriestessOptions)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("With their %c ability, who do you believe %p should talk to the most?", Character.High_Priestess, highPriestess, StorytellerView);
            AppendDrunkDisclaimer(sb, highPriestess);

            return Prompt(sb, highPriestessOptions);
        }

        public Task<IOption> GetMayorBounce(Player mayor, Player? killer, IReadOnlyCollection<IOption> mayorOptions)
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

            return Prompt(sb, mayorOptions);
        }

        public Task<IOption> ShouldKillTinker(Player tinker, IReadOnlyCollection<IOption> yesOrNo)
        {
            return Prompt(yesOrNo, "%p can die at any time. Kill them now?", tinker, StorytellerView);
        }

        public Task<IOption> ShouldKillWithSlayer(Player slayer, Player target, IReadOnlyCollection<IOption> yesOrNo)
        {
            return Prompt(yesOrNo, "%p is claiming %c and targetting %p. Should %p die?", slayer, Character.Slayer, target, target, StorytellerView);
        }

        public Task<IOption> ShouldSaveWithPacifist(Player pacifist, Player executedPlayer, IReadOnlyCollection<IOption> yesOrNo)
        {
            return Prompt(yesOrNo, "%p is to be executed. Should they be saved from death by %p with the %c ability?", executedPlayer, pacifist, Character.Pacifist, StorytellerView);
        }

        public Task<IOption> ShouldExecuteWithVirgin(Player virgin, Player nominator, IReadOnlyCollection<IOption> yesOrNo)
        {
            return Prompt(yesOrNo, "%p has nominated %p. Should %p register as a Townsfolk and be executed?\n", nominator, virgin, nominator, StorytellerView);
        }

        public Task<IOption> ShouldRegisterForJuggle(Player juggler, Player juggledPlayer, Character juggledCharacter, IReadOnlyCollection<IOption> yesOrNo)
        {
            return Prompt(yesOrNo, "%p has juggled %p as the %c. Should %p register as this character and count as a correct juggle?", juggler, juggledPlayer, juggledCharacter, juggledPlayer, StorytellerView);
        }

        public Task<IOption> ShouldRegisterForSnakeCharmer(Player snakeCharmer, Player target, IReadOnlyCollection<IOption> yesOrNo)
        {
            return Prompt(yesOrNo, "%p has targeted %p with their ability as the %c. Should %p register as the Demon and trigger a character swap?", snakeCharmer, target, Character.Snake_Charmer, target, StorytellerView);
        }

        public Task<IOption> ShouldRegisterAsGoodForLycanthrope(Player lycanthrope, Player target, IReadOnlyCollection<IOption> yesOrNo)
        {
            return Prompt(yesOrNo, "%p has targeted %p with their ability as the %c. Should %p register as good so that they will die and prevent any other deaths tonight?", lycanthrope, target, Character.Lycanthrope, target, StorytellerView);
        }

        public Task<IOption> ShouldRegisterAsEvilForOgre(Player ogre, Player target, IReadOnlyCollection<IOption> yesOrNo)
        {
            return Prompt(yesOrNo, "%p has targeted %p with their ability as the %c. Should %p register as evil?", ogre, target, Character.Ogre, target, StorytellerView);
        }

        public Task<IOption> ShouldRegisterAsMinionForVigormortis(Player vigormortis, Player target, IReadOnlyCollection<IOption> yesOrNo)
        {
            return Prompt(yesOrNo, "%p has killed %p. Should %p register as a minion such that one of their townsfolk neighbours are poisoned?", vigormortis, target, target, StorytellerView);
        }

        public Task<string> GetFishermanAdvice(Player fisherman)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("%p would like their %c advice.", fisherman, Character.Fisherman, StorytellerView);
            AppendDrunkDisclaimer(sb, fisherman);

            return PromptForText(sb);
        }

        public Task<IOption> ChooseFakeCannibalAbility(Player cannibal, Player executedPlayer, IReadOnlyCollection<IOption> characterAbilityOptions)
        {
            return Prompt(characterAbilityOptions, "%p has been executed and died. What ability should %p believe they gained as the %c?", executedPlayer, cannibal, Character.Cannibal, StorytellerView);
        }

        public Task<IOption> ChooseDamselCharacter(Player damsel, Player huntsman, IReadOnlyCollection<IOption> characterOptions)
        {
            return Prompt(characterOptions, "%p has successfully guessed that %p is the %c. What Townsfolk character should %p become?", huntsman, damsel, Character.Damsel, damsel, StorytellerView);
        }

        public Task<IOption> ChooseNewDamsel(Player damsel, Player huntsman, IReadOnlyCollection<IOption> playerOptions)
        {
            return Prompt(playerOptions, "%p is no longer the %c and there is still a %c in play. Which good player should become the %c?", damsel, Character.Damsel, Character.Huntsman, Character.Damsel, StorytellerView);
        }

        public void AssignCharacter(Player player)
        {
            if (player.Character != player.RealCharacter)
            {
                SendMessage("%p believes they are the %c but they are actually the %c.", player, player.Character, player.RealCharacter, StorytellerView);
            }
            else
            {
                SendMessage("%p is the %c.", player, player.Character);
            }
        }

        public void PrivateChatMessage(Player speaker, Player listener, string message)
        {
            SendMessage("%p to %p: %n", speaker, listener, message, StorytellerView);
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

            SendMessage(sb);
        }

        public void NewKazaliMinion(Player kazali, Player minionTarget, Character oldMinionCharacter, Character newMinionCharacter)
        {
            SendMessage("Since %c is no longer available, %p has selected for %p to become the %c.", oldMinionCharacter, kazali, minionTarget, newMinionCharacter, StorytellerView);
        }

        public void KazaliSoldierMinion(Player soldier, Character minionCharacterPickedBySoldier)
        {
            SendMessage("Because the %c picked the %c to become their minion, %p may choose which minion to become. They have chosen to become the %c.",
                        Character.Kazali, Character.Soldier, soldier, minionCharacterPickedBySoldier, StorytellerView);
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
            SendMessage(sb);
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

            SendMessage(sb);
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
            SendMessage(sb);
        }

        public void NotifyWasherwoman(Player washerwoman, Player playerA, Player playerB, Character character)
        {
            SendMessage("%p learns that %p or %p is the %c.", washerwoman, playerA, playerB, character, StorytellerView);
        }

        public void NotifyLibrarian(Player librarian, Player playerA, Player playerB, Character character)
        {
            SendMessage("%p learns that %p or %p is the %c.", librarian, playerA, playerB, character, StorytellerView);
        }

        public void NotifyLibrarianNoOutsiders(Player librarian)
        {
            SendMessage("%p learns that there are no outsiders in play.", librarian, StorytellerView);
        }

        public void NotifyInvestigator(Player investigator, Player playerA, Player playerB, Character character)
        {
            SendMessage("%p learns that %p or %p is the %c.", investigator, playerA, playerB, character, StorytellerView);
        }

        public void NotifyChef(Player chef, int evilPairCount)
        {
            SendMessage($"%p learns that there {(evilPairCount == 1 ? "is %b pair" : "are %b pairs")} of evil players.", chef, evilPairCount, StorytellerView);
        }

        public void NotifyNoble(Player noble, IReadOnlyCollection<Player> nobleInformation)
        {
            SendMessage("%p learns that there is exactly 1 evil player among %P", noble, nobleInformation, StorytellerView);
        }

        public void NotifySteward(Player steward, Player goodPlayer)
        {
            SendMessage("%p learns that %p is a good player.", steward, goodPlayer, StorytellerView);
        }

        public void NotifyBountyHunter(Player bountyHunter, Player evilPlayer)
        {
            SendMessage("%p learns that %p is an evil player.", bountyHunter, evilPlayer, StorytellerView);
        }

        public void NotifyShugenja(Player shugenja, Direction direction)
        {
            SendMessage("%p learns that the nearest %a to them is in the %b direction.", shugenja, Alignment.Evil, direction == Direction.Clockwise ? "clockwise" : "counter-clockwise", StorytellerView);
        }

        public void NotifyEmpath(Player empath, Player neighbourA, Player neighbourB, int evilCount)
        {
            SendMessage($"%p learns that %b of their living neighbours (%p and %p) {(evilCount == 1 ? "is" : "are")} %a.", empath, evilCount, neighbourA, neighbourB, Alignment.Evil, StorytellerView);
        }

        public void NotifyOracle(Player oracle, int evilCount)
        {
            SendMessage($"%p learns that %b of the dead players {(evilCount == 1 ? "is" : "are")} %a.", oracle, evilCount, Alignment.Evil, StorytellerView);
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
            SendMessage(sb);
        }

        public void NotifyUndertaker(Player undertaker, Player executedPlayer, Character executedCharacter)
        {
            SendMessage($"%p learns that the recently executed %p is the %c.", undertaker, executedPlayer, executedCharacter, StorytellerView);
        }

        public void NotifyBalloonist(Player balloonist, Player newPlayer)
        {
            SendMessage("The next player that %p learns is %p.", balloonist, newPlayer, StorytellerView);
        }

        public void NotifyJuggler(Player juggler, int jugglerCount)
        {
            SendMessage("%p learns that %b of their juggles were correct.", juggler, jugglerCount, StorytellerView);
        }

        public void ShowGrimoireToSpy(Player spy, Grimoire grimoire)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("%p now has a chance to look over the Grimoire...", spy, StorytellerView);
            sb.AppendLine();
            sb.Append(TextBuilder.GrimoireToText(grimoire));
            SendMessage(sb);
        }

        public void ShowNightwatchman(Player nightwatchman, Player target, bool shown)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("%p has chosen for %p to learn that they are the %c.", nightwatchman, target, Character.Nightwatchman, StorytellerView);
            if (!shown)
            {
                sb.Append(" There is no effect.");
            }
            SendMessage(sb);
        }

        public void ChoiceFromDemon(Player demon, Player target)
        {
            SendMessage("%p has chosen to kill %p.", demon, target, StorytellerView);
        }

        public void ChoiceFromPukka(Player pukka, Player target)
        {
            SendMessage("%p has chosen to poison %p.", pukka, target, StorytellerView);
        }

        public void ChoiceFromOjo(Player ojo, Character targetCharacter, IReadOnlyCollection<Player> victims)
        {
            switch (victims.Count)
            {
                case 0:
                    SendMessage("%p has chosen to kill the %c. No one is killed.", ojo, targetCharacter, StorytellerView);
                    break;

                case 1:
                    SendMessage("%p has chosen to kill the %c. %p is the %c's victim.", ojo, targetCharacter, victims.ElementAt(0), Character.Ojo, StorytellerView);
                    break;

                default:
                    SendMessage("%p has chosen to kill the %c. %P are the %c's victims.", ojo, targetCharacter, victims, Character.Ojo, StorytellerView);
                    break;
            }
        }

        public void ChoiceFromPoisoner(Player poisoner, Player target)
        {
            SendMessage("%p has chosen to poison %p.", poisoner, target, StorytellerView);
        }

        public void ChoiceFromWidow(Player widow, Player target)
        {
            SendMessage("%p has chosen to poison %p.", widow, target, StorytellerView);
        }

        public void ChoiceFromWitch(Player witch, Player target)
        {
            SendMessage("%p has chosen to curse %p.", witch, target, StorytellerView);
        }

        public void ChoiceFromAssassin(Player assassin, Player? target)
        {
            if (target == null)
            {
                SendMessage("%p is not using their ability tonight.", assassin, StorytellerView);
            }
            else
            {
                SendMessage("%p has chosen to kill %p.", assassin, target, StorytellerView);
            }
        }

        public void ChoiceFromGodfather(Player godfather, Player target)
        {
            SendMessage("%p has chosen to kill %p.", godfather, target, StorytellerView);
        }

        public void ChoiceFromDevilsAdvocate(Player devilsAdvocate, Player target)
        {
            SendMessage("%p has chosen to protect %p.", devilsAdvocate, target, StorytellerView);
        }

        public void ChoiceFromSnakeCharmer(Player snakeCharmer, Player target, bool success)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("%p has chosen %p with their %c ability. ", snakeCharmer, target, Character.Snake_Charmer, StorytellerView);

            if (success)
            {
                // Omit the characters of each player because it's confusing to read when we use their old characters.
                sb.AppendFormattedText("%p and %p swap characters and alignments, and %p is now poisoned.", snakeCharmer, target, target);
            }
            else
            {
                sb.Append("There is no effect.");
            }

            SendMessage(sb);
        }

        public void ChoiceFromMonk(Player monk, Player target)
        {
            SendMessage("%p has chosen to protect %p.", monk, target, StorytellerView);
        }

        public void ChoiceFromLycanthrope(Player lycanthrope, Player target, bool success)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText("%p has targeted %p with their %c ability", lycanthrope, target, Character.Lycanthrope, StorytellerView);
            if (success)
            {
                sb.AppendFormattedText(" and killed them. No other players can die tonight.");
            }
            else
            {
                sb.AppendFormattedText(" but failed to kill them.");
            }

            SendMessage(sb);
        }

        public void ChoiceFromRavenkeeper(Player ravenkeeper, Player target, Character character)
        {
            SendMessage("%p chooses %p and learns that they are the %c.", ravenkeeper, target, character, StorytellerView);
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
            SendMessage(sb);
        }

        public void ChoiceFromButler(Player butler, Player target)
        {
            SendMessage("%p has chosen %p to be their master.", butler, target, StorytellerView);
        }

        public void ChoiceFromOgre(Player ogre, Player target)
        {
            SendMessage("%p has chosen to become the alignment of %p.", ogre, target, StorytellerView);
        }

        public void FailedHuntsmanGuess(Player huntsman, Player damsel)
        {
            SendMessage("%p has guessed that %p is the %c. There is no effect.", huntsman, damsel, Character.Damsel, StorytellerView);
        }

        public void ScarletWomanTrigger(Player demon, Player scarletWoman)
        {
            SendMessage("%p has died and so %p becomes the new %c.", demon, scarletWoman, demon.RealCharacter, StorytellerView);
        }

        public void AcrobatTrigger(Player acrobat, Player triggeringGoodNeighbour)
        {
            SendMessage("%p dies since %p is drunk or poisoned.", acrobat, triggeringGoodNeighbour, StorytellerView);
        }

        private void SendMessage(string text, params object[] objects)
        {
            SendMarkupText?.Invoke(TextUtilities.FormatMarkupText(text, objects));
        }

        private void SendMessage(StringBuilder sb)
        {
            SendMarkupText?.Invoke(sb.ToString());
        }

        private Task<IOption> Prompt(IReadOnlyCollection<IOption> options, string text, params object[] objects)
        {
            if (PromptMarkupText == null)
            {
                throw new InvalidOperationException($"{nameof(PromptMarkupText)} must be set in order to prompt text-based storyteller");
            }

            var markupText = TextUtilities.FormatMarkupText(text, objects);
            return PromptMarkupText(markupText, options);
        }

        private Task<IOption> Prompt(StringBuilder sb, IReadOnlyCollection<IOption> options)
        {
            if (PromptMarkupText == null)
            {
                throw new InvalidOperationException($"{nameof(PromptMarkupText)} must be set in order to prompt text-based storyteller");
            }

            return PromptMarkupText(sb.ToString(), options);
        }

        private Task<string> PromptForText(StringBuilder sb)
        {
            if (PromptForTextResponse == null)
            {
                throw new InvalidOperationException($"{nameof(PromptForTextResponse)} must be set in order to prompt text-based storyteller for a text response");
            }

            return PromptForTextResponse(sb.ToString());
        }

        private static bool AppendDrunkDisclaimer(StringBuilder sb, Player player)
        {
            if (player.Tokens.HasToken(Token.IsTheBadPhilosopher))
            {
                sb.Append($" **[color:purple]They were drunk or poisoned when they used their {TextUtilities.CharacterToText(Character.Philosopher)} ability, so they are not really the {TextUtilities.CharacterToText(player.Character)}. Therefore this should generally be bad information.[/color]**");
                return true;
            }
            else if (player.DrunkOrPoisoned)
            {
                sb.Append(" **[color:purple]They are drunk or poisoned so this should generally be bad information.[/color]**");
                return true;
            }
            return false;
        }

        private const bool StorytellerView = true;
    }
}
