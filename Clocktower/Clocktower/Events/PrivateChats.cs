using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class PrivateChats : IGameEvent
    {
        public PrivateChats(IStoryteller storyteller, Grimoire grimoire, IGameObserver observers, Random random, int dayNumber, int playerCount)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.observers = observers;
            this.random = random;
            this.dayNumber = dayNumber;
            this.playerCount = playerCount;
        }

        public async Task RunEvent()
        {
            for (int i = 0; i < HowManyPrivateChats(dayNumber, playerCount, grimoire.Players.Count(player => player.Alive)); i++)
            {
                var playersChatting = await GetPairsOfChattingPlayers();
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
        }

        private async Task<IReadOnlyCollection<(Player, Player)>> GetPairsOfChattingPlayers()
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
                    await observers.PrivateChatStarts(currentPlayer, target);
                }
            }

            // Regular chats: Make everyone who isn't chatting yet, in random order, choose someone to talk to.
            while (playersNotChattingYet.Count > 1)
            {
                var currentPlayer = playersNotChattingYet.RandomPick(random);
                playersNotChattingYet.Remove(currentPlayer);
                var target = playersNotChattingYet.Count == 1 ? playersNotChattingYet[0]
                                                              : await currentPlayer.Agent.OfferPrivateChatRequired(playersNotChattingYet);
                playersNotChattingYet.Remove(target);
                playersChatting.Add((currentPlayer, target));
                await observers.PrivateChatStarts(currentPlayer, target);
            }

            return playersChatting;
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

            await playerA.Agent.StartPrivateChat(playerB);
            await playerB.Agent.StartPrivateChat(playerA);
            
            // A maximum of 4 messages each, 8 in total.
            for (int i = 0; i < 8; i++)
            {
                var speaker = (i % 2 == 0) ? playerA : playerB;
                var listener = (i % 2 == 0) ? playerB : playerA;
                var (message, endChat) = await speaker.Agent.GetPrivateChat(listener);
                if (string.IsNullOrEmpty(message))
                {
                    if (i > 0)
                    {   // End chat early if one player says nothing. (Exception for the very first message, to allow the second player a chance to say something.)
                        break;
                    }
                }
                else
                {
                    await listener.Agent.PrivateChatMessage(speaker, message);
                    chatLog.Add((speaker, message));
                }
                if (endChat)
                {   // End chat if the speaking player has indicated that they wish to end the chat.
                    break;
                }
            }

            await playerA.Agent.EndPrivateChat(playerB);
            await playerB.Agent.EndPrivateChat(playerA);

            return chatLog;
        }

        private static int HowManyPrivateChats(int dayNumber, int playerCount, int alivePlayersLeft)
        {
            if (alivePlayersLeft <= 4)
            {
                return 0;
            }

            int firstDayPrivateChats = playerCount switch
            {
                <= 7 => 2,
                <= 9 => 3,
                <= 11 => 4,
                <= 13 => 5,
                _ => 6
            };
            // Reduce the number of chats by 1 per day to a minimum of 1.
            int currentDayPrivateChats = firstDayPrivateChats - (dayNumber - 1);
            return currentDayPrivateChats < 1 ? 1 : currentDayPrivateChats;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IGameObserver observers;
        private readonly Random random;
        private readonly int dayNumber;
        private readonly int playerCount;
    }
}
