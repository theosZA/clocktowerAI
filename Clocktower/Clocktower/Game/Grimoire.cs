using Clocktower.Agent;

namespace Clocktower.Game
{
    /// <summary>
    /// The Grimoire holds all of our players.
    /// </summary>
    internal class Grimoire
    {
        public IReadOnlyCollection<Player> Players => players;

        public Grimoire(IEnumerable<Player> players)
        {
            this.players = players.ToList();
        }

        public void AssignCharacters(IStoryteller storyteller)
        {
            foreach (var player in players)
            {
                storyteller.AssignCharacter(player);
                player.Agent.AssignCharacter(player.Character, player.Alignment);
            }
        }

        public IEnumerable<Player> GetAllPlayersEndingWithPlayer(Player lastPlayer)
        {
            int lastPlayerIndex = players.IndexOf(lastPlayer);
            for (int offset = 1; offset < players.Count; ++offset) 
            {
                yield return players[(lastPlayerIndex + offset) % players.Count];
            }
        }

        /// <summary>
        /// Returns all living players with the given character ability (or that believe they are that character, e.g. Drunk or Lunatic).
        /// </summary>
        /// <param name="character">The character ability to filter by.</param>
        /// <returns>A collection of living players with the given character ability.</returns>
        public IEnumerable<Player> GetLivingPlayers(Character character)
        {
            return Players.Where(player => player.Alive && player.Character == character);
        }

        public (Player, Player) GetLivingNeighbours(Player player)
        {
            int myIndex = players.IndexOf(player);
            return (GetNextLivingPlayer(myIndex, players.Count - 1), GetNextLivingPlayer(myIndex, 1));
        }

        private Player GetNextLivingPlayer(int startIndex, int increment)
        {
            int index = (startIndex + increment) % players.Count;
            while (!players[index].Alive)
            {
                index = (index + increment) % players.Count;
            }
            return players[index];
        }

        private readonly List<Player> players;
    }
}
