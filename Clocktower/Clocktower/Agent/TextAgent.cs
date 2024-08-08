using Clocktower.Agent.Notifier;
using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Options;
using System.Text;

namespace Clocktower.Agent
{
    internal class TextAgent : IAgent
    {
        public TextAgent(string playerName, IReadOnlyCollection<string> players, string scriptName, IReadOnlyCollection<Character> script, IGameObserver observer, IMarkupNotifier notifier)
        {
            PlayerName = playerName;
            Observer = observer;

            this.notifier = notifier;
            this.players = players;
            this.scriptName = scriptName;
            this.script = script;
        }

        public string PlayerName { get; }

        public IGameObserver Observer { get; }

        public async Task StartGame()
        {
            await notifier.Start(PlayerName, players, scriptName, script);
        }

        public async Task AssignCharacter(Character character, Alignment alignment)
        {
            this.character = character;
            await SendMessage("You are the %c. You are %a.", character, alignment);
        }

        public async Task YouAreDead()
        {
            await SendMessage("You are dead and are now a ghost. You may only vote one more time.");
        }

        public async Task MinionInformation(Player demon, IReadOnlyCollection<Player> fellowMinions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            var sb = new StringBuilder();

            if (fellowMinions.Any())
            {
                sb.AppendFormattedMarkupText($"As a minion, you learn that %p is your demon and your fellow {(fellowMinions.Count > 1 ? "minions are" : "minion is")} %P.", demon, fellowMinions);
            }
            else
            {
                sb.AppendFormattedMarkupText("As a minion, you learn that %p is your demon.", demon);
            }
            if (notInPlayCharacters.Any())
            {
                sb.AppendFormattedMarkupText(" You also learn that the following characters are not in play: %C.", notInPlayCharacters);
            }

            await SendMessage(sb);
        }

        public async Task DemonInformation(IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            var sb = new StringBuilder();

            sb.Append("As the demon, you learn that ");

            var nonMarionetteMinions = minions.Where(minion => minion.RealCharacter != Character.Marionette).ToList();
            if (nonMarionetteMinions.Count > 0)
            {
                sb.AppendFormattedMarkupText($"%P {(nonMarionetteMinions.Count > 1 ? "are your minions" : "is your minion")}, ", nonMarionetteMinions);
            }

            var marionette = minions.FirstOrDefault(minion => minion.RealCharacter == Character.Marionette);
            if (marionette != null)
            {
                sb.AppendFormattedMarkupText("%p is your %c, ", marionette, Character.Marionette);
            }

            sb.AppendFormattedMarkupText("and that the following characters are not in play: %C.", notInPlayCharacters);

            await SendMessage(sb);
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
            await SendMessage($"You learn that %b of your living neighbours (%p and %p) {(evilCount == 1 ? "is" : "are")} evil.", evilCount, neighbourA, neighbourB);
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

        public async Task NotifyJuggler(int jugglerCount)
        {
            if (character == Character.Cannibal)
            {
                await Learn(jugglerCount);
                return;
            }
            await SendMessage("You learn that %b of your juggles were correct.", jugglerCount);
        }

        public async Task ShowGrimoireToSpy(Grimoire grimoire)
        {
            var sb = new StringBuilder();

            sb.AppendFormattedMarkupText("As the %c, you can now look over the Grimoire...", Character.Spy);
            sb.AppendLine();
            sb.Append(TextBuilder.GrimoireToText(grimoire, markup: true));

            await SendMessage(sb);
        }

        public async Task ResponseForFisherman(string advice)
        {
            await SendMessage("%b: %n", "Storyteller", advice.Trim());
        }

        public Task GainCharacterAbility(Character character)
        {
            this.character = character;
            return Task.CompletedTask;
        }

        public async Task<IOption> RequestChoiceFromDemon(Character demonCharacter, IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public async Task<IOption> RequestChoiceFromOjo(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public async Task<IOption> RequestChoiceFromPoisoner(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public async Task<IOption> RequestChoiceFromWitch(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public async Task<IOption> RequestChoiceFromAssassin(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public async Task<IOption> RequestChoiceFromGodfather(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public async Task<IOption> RequestChoiceFromDevilsAdvocate(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public async Task<IOption> RequestChoiceFromPhilosopher(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public async Task<IOption> RequestChoiceFromFortuneTeller(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public async Task<IOption> RequestChoiceFromMonk(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public async Task<IOption> RequestChoiceFromRavenkeeper(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public async Task<IOption> RequestChoiceFromButler(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public async Task<IOption> PromptFishermanAdvice(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public async Task<IOption> PromptShenanigans(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public async Task<IOption> GetNomination(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public async Task<IOption> GetVote(IReadOnlyCollection<IOption> options, bool ghostVote)
        {
            throw new NotImplementedException();
        }

        public async Task<IOption> OfferPrivateChat(IReadOnlyCollection<IOption> options)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetRollCallStatement()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetMorningPublicStatement()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetEveningPublicStatement()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetProsecution(Player nominee)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetDefence(Player nominator)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetReasonForSelfNomination()
        {
            throw new NotImplementedException();
        }

        public async Task StartPrivateChat(Player otherPlayer)
        {
            var imageFileName = Path.Combine("Images", otherPlayer.Name + ".jpg");
            await SendMessageWithImage(imageFileName, "You have begun a private chat with %p.", otherPlayer);
        }

        public async Task<(string message, bool endChat)> GetPrivateChat(Player listener)
        {
            throw new NotImplementedException();
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

        private readonly IMarkupNotifier notifier;

        private readonly IReadOnlyCollection<string> players;
        private readonly string scriptName;
        private readonly IReadOnlyCollection<Character> script;
        private Character? character;
    }
}
