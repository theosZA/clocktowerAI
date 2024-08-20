using Clocktower.Agent.Notifier;
using Clocktower.Game;
using System.Text;

namespace Clocktower.Agent.Observer
{
    internal class TextObserver : IGameObserver
    {
        public Func<int, Task>? OnNight { get; set; }
        public Func<int, Task>? OnDay { get; set; }
        public Func<int, Task>? OnNominationsStart { get; set; }
        public Func<Player, Player, Task>? OnPrivateChatStart { get; set; }

        public TextObserver(IMarkupNotifier notifier, bool storytellerView = false)
        {
            this.notifier = notifier;
            this.storytellerView = storytellerView;
        }

        public async Task AnnounceWinner(Alignment winner, IReadOnlyCollection<Player> winners, IReadOnlyCollection<Player> losers)
        {
            const bool forceStorytellerView = true;

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendFormattedText("The %a team has won!", winner);
            sb.AppendLine();
            sb.AppendFormattedText("- Winning with the %a team are: %P.", winner, winners, forceStorytellerView);
            sb.AppendLine();
            sb.AppendFormattedText("- Losing with the %a team are: %P.", winner == Alignment.Good ? Alignment.Evil : Alignment.Good, losers, forceStorytellerView);

            await SendMessage(sb);
        }

        public async Task Night(int nightNumber)
        {
            if (OnNight != null)
            {
                await OnNight(nightNumber);
            }

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine($"## Night {nightNumber}");

            await SendMessage(sb);
        }

        public async Task Day(int dayNumber)
        {
            if (OnDay != null)
            {
                await OnDay(dayNumber);
            }

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine($"## Day {dayNumber}");

            await SendMessage(sb);
        }

        public async Task AnnounceLivingPlayers(IReadOnlyCollection<Player> players)
        {
            StringBuilder sb = new();

            sb.AppendLine($"There are {players.Count(player => player.Alive)} players still alive. Our players are...");
            sb.Append(notifier.CreatePlayerRoll(players, storytellerView));

            await SendMessage(sb);
        }

        public async Task NoOneDiedAtNight()
        {
            await SendMessage("No one died in the night.");
        }

        public async Task PlayerDiedAtNight(Player newlyDeadPlayer)
        {
            await SendMessage("%p died in the night.", newlyDeadPlayer, storytellerView);
        }

        public async Task PlayerDies(Player newlyDeadPlayer)
        {
            await SendMessage("%p dies.", newlyDeadPlayer, storytellerView);
        }

        public async Task PlayerIsExecuted(Player executedPlayer, bool playerDies)
        {
            if (playerDies)
            {
                await SendMessage("%p is executed and dies.", executedPlayer, storytellerView);
            }
            else if (executedPlayer.Alive)
            {
                await SendMessage("%p is executed but does not die.", executedPlayer, storytellerView);
            }
            else
            {
                await SendMessage("%p's corpse is executed.", executedPlayer, storytellerView);
            }
        }

        public async Task DayEndsWithNoExecution()
        {
            await SendMessage("There is no execution and the day ends.");
        }

        public async Task StartNominations(int numberOfLivingPlayers, int votesToPutOnBlock)
        {
            await SendMessage("Nominations for who will be executed are now open. There are %b players currently still alive, so we'll require %b votes to put a nominee on the block.",
                              numberOfLivingPlayers, votesToPutOnBlock);

            if (OnNominationsStart != null)
            {
                await OnNominationsStart(numberOfLivingPlayers);
            }
        }

        public async Task AnnounceNomination(Player nominator, Player nominee, int? votesToTie, int? votesToPutOnBlock)
        {
            var sb = new StringBuilder();

            sb.AppendFormattedText("%p nominates %p. ", nominator, nominee, storytellerView);
            if (votesToTie.HasValue && votesToPutOnBlock.HasValue)
            {
                sb.AppendFormattedText("%b votes to tie, %b votes to put them on the block.", votesToTie.Value, votesToPutOnBlock.Value);
            }
            else if (votesToTie.HasValue)
            {
                sb.AppendFormattedText("%b votes to tie.", votesToTie.Value);
            }
            else if (votesToPutOnBlock.HasValue)
            {
                sb.AppendFormattedText("%b votes to put them on the block.", votesToPutOnBlock.Value);
            }

            await SendMessage(sb);
        }

        public async Task AnnounceSecretVote(Player nominee)
        {
            await SendMessage("The vote on %p will be conducted in secret.", nominee, storytellerView);
        }

        public async Task AnnounceVote(Player voter, Player nominee, bool votedToExecute)
        {
            if (votedToExecute)
            {
                if (voter.Alive)
                {
                    await SendMessage("%p votes to execute %p.", voter, nominee, storytellerView);
                }
                else
                {
                    await SendMessage("%p uses their ghost vote to execute %p.", voter, nominee, storytellerView);
                }
            }
            else
            {
                await SendMessage("%p does not vote.", voter, storytellerView);
            }
        }

        public async Task AnnounceVoteResult(Player nominee, int? voteCount, VoteResult voteResult)
        {
            if (!voteCount.HasValue)
            {
                await SendMessage("The voting on %p has concluded.", nominee, storytellerView);
                return;
            }

            var resultText = voteResult switch
            {
                VoteResult.OnTheBlock => ". That is enough to put them on the block",
                VoteResult.Tied => " which is a tie. No one is on the block",
                VoteResult.InsufficientVotes => " which is not enough",
                _ => string.Empty
            };

            await SendMessage($"%p received %b vote{(voteCount == 1 ? string.Empty : "s")}{resultText}.", nominee, voteCount.Value, storytellerView);
        }

        public async Task AnnounceSlayerShot(Player slayer, Player target, bool success)
        {
            var sb = new StringBuilder();

            sb.AppendFormattedText("%p claims %c and takes a shot at %p. ", slayer, Character.Slayer, target, storytellerView);
            if (success)
            {
                sb.AppendFormattedText("%p dies.", target, storytellerView);
            }
            else
            {
                sb.Append("Nothing happens.");
            }

            await SendMessage(sb);
        }

        public async Task AnnounceJuggles(Player juggler, IEnumerable<(Player player, Character character)> juggles)
        {
            var sb = new StringBuilder();

            sb.AppendFormattedText("%p claims %c and guesses the following characters: ", juggler, Character.Juggler, storytellerView);
            bool firstJuggle = true;
            foreach (var juggle in juggles)
            {
                if (!firstJuggle)
                {
                    sb.Append(", ");
                }
                sb.AppendFormattedText("%p as the %c", juggle.player, juggle.character, storytellerView);
                firstJuggle = false;
            }
            sb.Append('.');

            await SendMessage(sb);
        }

        public async Task PublicStatement(Player player, string statement)
        {
            await SendMessage("%p:\n>>> %n", player, statement, storytellerView);
        }

        public async Task PrivateChatStarts(Player playerA, Player playerB)
        {
            await SendMessage("%p goes for a private chat with %p.", playerA, playerB, storytellerView);

            if (OnPrivateChatStart != null)
            {
                await OnPrivateChatStart(playerA, playerB);
            }
        }

        public async Task StartRollCall(int playersAlive)
        {
            await SendMessage("Since there are only %b players still alive, we will hold an optional roll call. " +
                              "Everyone will have a chance to claim their character and elaborate on what they learned or how they used their character's ability.",
                              playersAlive);
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
        private readonly bool storytellerView;
    }
}
