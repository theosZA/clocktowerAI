using Clocktower.Agent;
using Clocktower.Storyteller;

namespace Clocktower.Game
{
    /// <summary>
    /// The Grimoire holds all of our players.
    /// </summary>
    public class Grimoire
    {
        public bool Finished => Winner.HasValue;

        public Alignment? Winner
        {
            get
            {
                // First check if a winner has been determined by character ability.
                if (winner.HasValue)
                {
                    return winner;
                }

                // The game is over if there are no living demons...
                if (!players.Any(player => player.Alive && player.CharacterType == CharacterType.Demon))
                {
                    return Alignment.Good;
                }

                // ...or there are fewer than 3 players alive.
                if (players.Count(player => player.Alive) < 3)
                {
                    return Alignment.Evil;
                }

                return null;
            }
        }

        public IReadOnlyCollection<Player> Players => players;

        public Player? PlayerToBeExecuted { get; set; }

        public bool PhaseShouldEndImmediately { get; set; }

        public Grimoire(IEnumerable<IAgent> agents, Character[] characters)
        {
            players = agents.Select((agent, i) => new Player(agent, characters[i], alignment: characters[i].Alignment()))
                            .ToList();
        }

        public void EndGame(Alignment winner)
        {
            this.winner = winner;
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

                case Character.Butler:
                    RemoveToken(Token.ChosenByButler);
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
        private Alignment? winner;
    }
}
