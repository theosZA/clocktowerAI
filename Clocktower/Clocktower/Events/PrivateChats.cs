using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Storyteller;

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
                var candidates = playersNotChattingYet.Except(new[] { currentPlayer });
                var target = playersNotChattingYet.Count == 2 ? candidates.FirstOrDefault()
                                                              : await currentPlayer.Agent.OfferPrivateChatOptional(candidates);
                if (target != null)
                {
                    playersNotChattingYet.Remove(currentPlayer);
                    playersNotChattingYet.Remove(target);
                    playersToGetPriorityOption.Remove(target);
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
                var target = playersNotChattingYet.Count == 1 ? playersNotChattingYet[0]
                                                              : await currentPlayer.Agent.OfferPrivateChatRequired(playersNotChattingYet);
                playersNotChattingYet.Remove(target);
                playersChatting.Add((currentPlayer, target));
                observers.PrivateChatStarts(currentPlayer, target);
            }

            // Now run the chats in parallel.
            var chatLogs = await RunChats(playersChatting);

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

        private static async Task<IDictionary<(Player playerA, Player playerB), IEnumerable<(Player speaker, string message)>>> RunChats(IReadOnlyCollection<(Player playerA, Player playerB)> chats)
        {
            var tasks = chats.Select(chat => RunChat(chat.playerA, chat.playerB));
            var results = await Task.WhenAll(tasks);

            var chatLogs = new Dictionary<(Player playerA, Player playerB), IEnumerable<(Player speaker, string message)>>();
            for (int i = 0; i < results.Length; i++)
            {
                chatLogs.Add(chats.Skip(i).First(), results[i]);
            }
            return chatLogs;
        }

        private static async Task<IEnumerable<(Player speaker, string message)>> RunChat(Player playerA, Player playerB)
        {
            var chatLog = new List<(Player speaker, string message)>();

            playerA.Agent.StartPrivateChat(playerB);
            playerB.Agent.StartPrivateChat(playerA);
            
            // A maximum of 4 messages each, 8 in total.
            for (int i = 0; i < 8; i++)
            {
                var speaker = (i % 2 == 0) ? playerA : playerB;
                var listener = (i % 2 == 0) ? playerB : playerA;
                var message = await speaker.Agent.GetPrivateChat(listener);
                if (string.IsNullOrEmpty(message))
                {
                    if (i > 0)
                    {   // End chat early if one player says nothing. (Exception for the very first message, to allow the second player a chance to say something.)
                        break;
                    }
                }
                else
                {
                    listener.Agent.PrivateChatMessage(speaker, message);
                    chatLog.Add((speaker, message));
                }
            }

            playerA.Agent.EndPrivateChat(playerB);
            playerB.Agent.EndPrivateChat(playerA);

            return chatLog;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IGameObserver observers;
        private readonly Random random;
    }
}
