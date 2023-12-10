using Clocktower.Events;
using Clocktower.Options;
using Clocktower.Storyteller;

namespace Clocktower.Game
{
    /// <summary>
    /// The Grimoire holds all of our players.
    /// </summary>
    public class Grimoire
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

        public void ChangeCharacter(Player player, Character newCharacter)
        {
            player.Tokens.Remove(Token.UsedOncePerGameAbility);
            RemoveTokensForCharacter(player.RealCharacter);
            player.ChangeCharacter(newCharacter);
        }

        public void RemoveTokensForCharacter(Character character)
        {
            // Remove any ongoing effects that character has.
            switch (character)
            {
                case Character.Fortune_Teller:
                    RemoveToken(Token.FortuneTellerRedHerring);
                    break;

                case Character.Monk:
                    RemoveToken(Token.ProtectedByMonk);
                    break;

                case Character.Philosopher:
                    RemoveToken(Token.PhilosopherDrunk);
                    break;

                case Character.Sweetheart:
                    RemoveToken(Token.SweetheartDrunk);
                    break;

                case Character.Poisoner:
                    RemoveToken(Token.PoisonedByPoisoner);
                    break;
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

        private void RemoveToken(Token token)
        {
            foreach (var player in players)
            {
                player.Tokens.Remove(token);
            }
        }

        private readonly List<Player> players;
    }
}
