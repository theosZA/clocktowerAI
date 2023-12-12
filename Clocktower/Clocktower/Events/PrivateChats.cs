using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Storyteller;
using System.Windows.Forms;

namespace Clocktower.Events
{
    internal class PrivateChats : IGameEvent
    {
        public PrivateChats(IStoryteller storyteller, Grimoire grimoire, IGameObserver observers, Random random)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.observers = observers;
            this.random = random;
        }

        public async Task RunEvent()
        {
            List<Player> playersNotChattingYet = grimoire.Players.ToList();
            List<(Player, Player)> playersChatting = new();

            // Priority chats: Give everyone, in random order, a chance to choose someone to talk to.
            List<Player> playersToGetPriorityOption = new(playersNotChattingYet);
            playersToGetPriorityOption.Shuffle(random);
            while (playersToGetPriorityOption.Count > 0 && playersNotChattingYet.Count > 1)
            {
                var currentPlayer = playersToGetPriorityOption[0];
                playersToGetPriorityOption.RemoveAt(0);
                var target = await currentPlayer.Agent.OfferPrivateChatOptional(playersNotChattingYet.Except(new[] { currentPlayer }));
                if (target != null)
                {
                    playersNotChattingYet.Remove(currentPlayer);
                    playersNotChattingYet.Remove(target);
                    playersChatting.Add((currentPlayer, target));
                    observers.PrivateChatStarts(currentPlayer, target);
                }
            }

            // Regular chats: Make everyone who isn't chatting yet, in random order, choose someone to talk to.
            playersNotChattingYet.Shuffle(random);
            while (playersNotChattingYet.Count > 1)
            {
                var currentPlayer = playersNotChattingYet[0];
                playersNotChattingYet.RemoveAt(0);
                var target = await currentPlayer.Agent.OfferPrivateChatRequired(playersNotChattingYet);
                playersNotChattingYet.Remove(target);
                playersChatting.Add((currentPlayer, target));
                observers.PrivateChatStarts(currentPlayer, target);
            }

            // Start the chats.
            Dictionary<(Player, Player), List<(Player, string)>> chatLogs = new();
            foreach (var (playerA, playerB) in playersChatting)
            {
                playerA.Agent.StartPrivateChat(playerB);
                playerB.Agent.StartPrivateChat(playerA);
                chatLogs.Add((playerA, playerB), new List<(Player, string)>());
            }

            // Now run the chats in parallel. Each chat ends when both players have spoken 4 times, or when one player declines to say anything.
            for (int i = 0; i < 8; i++)
            {
                var todaysChats = new List<(Player, Player)>(playersChatting);
                foreach (var (playerA, playerB) in todaysChats)
                {
                    var speaker = (i % 2 == 0) ? playerA : playerB;
                    var listener = (i % 2 == 0) ? playerB : playerA;
                    var message = await speaker.Agent.GetPrivateChat(listener);
                    if (string.IsNullOrEmpty(message))
                    {
                        if (i > 0)
                        {
                            playersChatting.Remove((playerA, playerB));
                            playerA.Agent.EndPrivateChat(playerB);
                            playerB.Agent.EndPrivateChat(playerA);
                        }
                    }
                    else
                    {
                        listener.Agent.PrivateChatMessage(speaker, message);
                        chatLogs[(playerA, playerB)].Add((speaker, message));
                    }
                }
            }

            // Stop any remaining chats.
            foreach (var (playerA, playerB) in playersChatting)
            {
                playerA.Agent.EndPrivateChat(playerB);
                playerB.Agent.EndPrivateChat(playerA);
            }

            // Provide the full chat logs for the Storyteller.
            foreach (var chat in chatLogs)
            {
                var (playerA, playerB) = chat.Key;
                foreach (var (speaker, message) in chat.Value)
                {
                    var listener = speaker == playerA ? playerB : playerA;
                    storyteller.PrivateChatMessage(speaker, listener, message);
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IGameObserver observers;
        private readonly Random random;
    }
}
