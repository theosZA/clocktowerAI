﻿using Clocktower.Game;
using DiscordChatBot;
using System.Text;

namespace Clocktower.Observer
{
    internal class DiscordChatObserver : IGameObserver
    {
        public void Start(Chat chat)
        {
            this.chat = chat;
        }

        public void QueueMessage(string text, params object[] objects)
        {
            chat?.QueueMessage(TextUtilities.FormatMarkupText(text, objects));
        }

        public async Task SendMessage(string text, params object[] objects)
        {
            if (chat != null)
            {
                await chat.SendMessage(TextUtilities.FormatMarkupText(text, objects));
            }
        }

        public async Task SendMessageWithImage(string imageFileName, string text, params object[] objects)
        {
            if (chat != null)
            {
                await chat.SendMessage(TextUtilities.FormatMarkupText(text, objects), imageFileName);
            }
        }

        public async Task AnnounceNomination(Player nominator, Player nominee, int? votesToTie, int? votesToPutOnBlock)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedMarkupText("%p nominates %p. ", nominator, nominee);
            if (votesToTie.HasValue && votesToPutOnBlock.HasValue)
            {
                sb.AppendFormattedMarkupText("%b votes to tie, %b votes to put them on the block.", votesToTie.Value, votesToPutOnBlock.Value);
            }
            else if (votesToTie.HasValue)
            {
                sb.AppendFormattedMarkupText("%b votes to tie.", votesToTie.Value);
            }
            else if (votesToPutOnBlock.HasValue)
            {
                sb.AppendFormattedMarkupText("%b votes to put them on the block.", votesToPutOnBlock.Value);
            }
            await SendMessage(sb.ToString());
        }

        public async Task AnnounceSecretVote(Player nominee)
        {
            await SendMessage("The vote on %p will be conducted in secret.", nominee);
        }

        public Task AnnounceVote(Player voter, Player nominee, bool votedToExecute)
        {
            if (votedToExecute)
            {
                if (voter.Alive)
                {
                    QueueMessage("%p votes to execute %p.", voter, nominee);
                }
                else
                {
                    QueueMessage("%p uses their ghost vote to execute %p.", voter, nominee);
                }
            }
            else
            {
                QueueMessage("%p does not vote.", voter);
            }

            return Task.CompletedTask;
        }

        public async Task AnnounceVoteResult(Player nominee, int? voteCount, VoteResult voteResult)
        {
            if (!voteCount.HasValue)
            {
                await SendMessage("The voting on %p has concluded.", nominee);
                return;
            }

            var resultText = voteResult switch
            {
                VoteResult.OnTheBlock => ". That is enough to put them on the block",
                VoteResult.Tied => " which is a tie. No one is on the block",
                VoteResult.InsufficientVotes => " which is not enough",
                _ => string.Empty
            };

            await SendMessage($"%p received %b vote{(voteCount == 1 ? string.Empty : "s")}{resultText}.", nominee, voteCount.Value);
        }

        public async Task AnnounceSlayerShot(Player slayer, Player target, bool success)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedMarkupText("%p claims %c and takes a shot at %p. ", slayer, Character.Slayer, target);
            if (success)
            {
                sb.AppendFormattedMarkupText("%p dies.\n", target);
            }
            else
            {
                sb.Append("Nothing happens.\n");
            }
            await SendMessage(sb.ToString());
        }

        public async Task AnnounceJuggles(Player juggler, IEnumerable<(Player player, Character character)> juggles)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedMarkupText("%p claims %c and guesses the following characters: ", juggler, Character.Juggler);
            bool firstJuggle = true;
            foreach (var juggle in juggles)
            {
                if (!firstJuggle)
                {
                    sb.Append(", ");
                }
                sb.AppendFormattedMarkupText("%p as the %c", juggle.player, juggle.character);
                firstJuggle = false;
            }
            sb.Append('.');
            await SendMessage(sb.ToString());
        }

        public async Task AnnounceWinner(Alignment winner, IReadOnlyCollection<Player> winners, IReadOnlyCollection<Player> losers)
        {
            const bool forceStorytellerView = true;
            if (winner == Alignment.Good)
            {
                await SendMessage("\nThe GOOD team has won!\nWinning with the good team are: %P.\nLosing with the evil team are: %P.", winners, losers, forceStorytellerView);
            }
            else
            {
                await SendMessage("\nThe EVIL team has won!\nWinning with the evil team are: %P.\nLosing with the good team are: %P.", winners, losers, forceStorytellerView);
            }
        }

        public async Task Day(int dayNumber)
        {
            await SendMessage($"## Day {dayNumber}");
        }

        public async Task DayEndsWithNoExecution()
        {
            await SendMessage("There is no execution and the day ends.");
        }

        public async Task AnnounceLivingPlayers(IReadOnlyCollection<Player> players)
        {
            StringBuilder sb = new();

            sb.AppendLine($"There are {players.Count(player => player.Alive)} players still alive. Our players are...");
            foreach (var player in players)
            {
                if (player.Alive)
                {
                    sb.AppendFormattedMarkupText("%p", player);
                    sb.AppendLine();
                }
                else
                {
                    sb.AppendFormattedText("👻 %p 👻", player);   // dead players will not be bolded
                    if (player.HasGhostVote)
                    {
                        sb.AppendLine(" (ghost vote available)");
                    }
                    else
                    {
                        sb.AppendLine();
                    }
                }
            }

            await SendMessage(sb.ToString());
        }

        public async Task Night(int nightNumber)
        {
            await SendMessage($"## Night {nightNumber}");
        }

        public async Task NoOneDiedAtNight()
        {
            await SendMessage("No one died in the night.");
        }

        public async Task PlayerDiedAtNight(Player newlyDeadPlayer)
        {
            await SendMessage("%p died in the night.", newlyDeadPlayer);
        }

        public async Task PlayerDies(Player newlyDeadPlayer)
        {
            await SendMessage("%p dies.", newlyDeadPlayer);
        }

        public async Task PlayerIsExecuted(Player executedPlayer, bool playerDies)
        {
            if (playerDies)
            {
                await SendMessage("%p is executed and dies.", executedPlayer);
            }
            else if (executedPlayer.Alive)
            {
                await SendMessage("%p is executed but does not die.", executedPlayer);
            }
            else
            {
                await SendMessage("%p's corpse is executed.", executedPlayer);
            }
        }

        public async Task PrivateChatStarts(Player playerA, Player playerB)
        {
            await SendMessage("%p goes for a private chat with %p.", playerA, playerB);
        }

        public async Task PublicStatement(Player player, string statement)
        {
            await SendMessage("%p:\n>>> %n", player, statement);
        }

        public async Task StartNominations(int numberOfLivingPlayers, int votesToPutOnBlock)
        {
            await SendMessage($"Nominations for who will be executed are now open. There are {numberOfLivingPlayers} players currently still alive, so we'll require {votesToPutOnBlock} votes to put a nominee on the block.");
        }

        public async Task StartRollCall(int playersAlive)
        {
            await SendMessage("Since there are only %b players still alive, we will hold an optional roll call. Everyone will have a chance to claim their character and elaborate on what they learned or how they used their character's ability.", playersAlive);
        }

        private Chat? chat;
    }
}
