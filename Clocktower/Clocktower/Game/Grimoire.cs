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

        public List<Character> DemonBluffs { get; set; } = new();

        public Grimoire(IEnumerable<IAgent> agents, Character[] characters)
        {
            players = agents.Select((agent, i) => new Player(this, agent, characters[i], alignment: characters[i].Alignment()))
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
        /// Returns all players that we should treat as having the given ability, whether they actually have that ability or not.
        /// </summary>
        /// <param name="character">The character ability to filter by.</param>
        /// <returns>A collection of players who we should treat as having the given character ability.</returns>
        public IEnumerable<Player> GetPlayersWithAbility(Character character)
        {
            return Players.Where(player => player.ShouldRunAbility(character));
        }

        /// <summary>
        /// Returns all players that we should treat as having the given ability, whether they actually have that ability or not, that have not
        /// yet used their ability.
        /// </summary>
        /// <param name="character">The character ability to filter by.</param>
        /// <returns>A collection of players who we should treat as having the given character ability.</returns>
        public IEnumerable<Player> GetPlayersWithUnusedAbility(Character character)
        {
            return Players.Where(player => player.ShouldRunAbility(character) && !player.Tokens.HasToken(Token.UsedOncePerGameAbility));
        }

        /// <summary>
        /// Returns all players that actually have the given ability. This will exclude drunk or poisoned players, as well as
        /// characters who think they have this ability but do not, e.g. Lunatic and Marionette.
        /// </summary>
        /// <param name="character">The character ability to filter by.</param>
        /// <returns>A collection of players who actually have the ability.</returns>
        public IEnumerable<Player> GetHealthyPlayersWithRealAbility(Character character)
        {
            return Players.Where(player => player.ShouldRunAbility(character) && !player.DrunkOrPoisoned);
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
