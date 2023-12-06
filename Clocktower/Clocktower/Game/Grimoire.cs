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
            // For now we assign hardcoded characters.

            //players[0].AssignCharacter(Character.Steward, Alignment.Good);
            players[0].AssignCharacter(Character.Investigator, Alignment.Good);
            players[1].AssignCharacter(Character.Imp, Alignment.Evil);
            players[2].AssignCharacter(Character.Godfather, Alignment.Evil);
            players[3].AssignCharacter(Character.Recluse, Alignment.Good);
            players[4].AssignCharacter(Character.Slayer, Alignment.Good);
            players[5].AssignCharacter(Character.Empath, Alignment.Good);
            players[6].AssignCharacter(Character.Drunk, Alignment.Good,
                                       Character.Librarian, Alignment.Good);
            players[7].AssignCharacter(Character.Ravenkeeper, Alignment.Good);

            // Notify storyteller of characters.
            foreach (var player in players)
            {
                storyteller.AssignCharacter(player);
            }
        }

        public IEnumerable<Player> GetAllPlayersEndingWithPlayer(Player lastPlayer)
        {
            int lastPlayerIndex = players.IndexOf(lastPlayer);
            for (int offset = 1; offset <= players.Count; ++offset)
            {
                yield return players[(lastPlayerIndex + offset) % players.Count];
            }
        }

        public Player? GetAlivePlayer(Character believedCharacter)
        {
            return players.FirstOrDefault(player => player.Alive && player.Character == believedCharacter);
        }

        public Player? GetPlayer(Character believedCharacter)
        {
            return players.FirstOrDefault(player => player.Character == believedCharacter);
        }

        public Player GetRequiredPlayer(Character believedCharacter)
        {
            return players.First(player => player.Character == believedCharacter);
        }

        public Player GetRequiredRealPlayer(Character realCharacter)
        {
            return players.First(player => player.RealCharacter == realCharacter);
        }

        public Player GetDemon()
        {
            return players.First(player => player.RealCharacter.HasValue && CharacterTypeFromCharacter(player.RealCharacter.Value) == CharacterType.Demon);
        }

        public IEnumerable<Player> GetMinions()
        {
            return players.Where(player => player.RealCharacter.HasValue && CharacterTypeFromCharacter(player.RealCharacter.Value) == CharacterType.Minion);
        }

        public IEnumerable<Character> GetOutsiders()
        {
            return players.Where(player => player.RealCharacter.HasValue && CharacterTypeFromCharacter(player.RealCharacter.Value) == CharacterType.Outsider)
                          .Select(player => player.RealCharacter ?? (Character)(-1));
        }

        public (Player, Player) GetLivingNeighbours(Player player)
        {
            int myIndex = players.IndexOf(player);
            return (GetNextLivingPlayer(myIndex, players.Count - 1), GetNextLivingPlayer(myIndex, 1));
        }

        private static CharacterType CharacterTypeFromCharacter(Character character)
        {
            if ((int)character < 1000)
            {
                return CharacterType.Townsfolk;
            }
            if ((int)character < 2000)
            {
                return CharacterType.Outsider;
            }
            if ((int)character < 3000)
            {
                return CharacterType.Demon;
            }
            return CharacterType.Minion;
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
