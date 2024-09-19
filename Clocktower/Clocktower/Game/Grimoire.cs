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
        
        public Player? MostRecentlyExecutedPlayerToDie { get; set; }

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

        public async Task AssignCharacters(IStoryteller storyteller)
        {
            foreach (var player in players)
            {
                storyteller.AssignCharacter(player);
                await player.Agent.AssignCharacter(player.Character, player.Alignment);

                if (player.Character == Character.Spy || player.Character == Character.Widow)
                {
                    OnSpyOrWindowInPlay();
                }

                if (player.Character == Character.Damsel)
                {
                    OnDamselInPlay(player);
                }
            }
        }

        public async Task ChangeCharacter(Player player, Character newCharacter, Alignment? newAlignment = null)
        {
            if (newCharacter == Character.Spy || newCharacter == Character.Widow)
            {
                OnSpyOrWindowInPlay();
            }

            if (newCharacter == Character.Marionette)
            {
                foreach (var affectedPlayer in players)
                {
                    affectedPlayer.Tokens.ClearTokensOnPlayerLosingAbility(player);
                }
                player.Tokens.Add(Token.IsTheMarionette, player);
                await player.ChangeAlignment(Alignment.Evil, notifyAgent: false);
                return;
            }

            foreach (var affectedPlayer in players)
            {
                affectedPlayer.Tokens.ClearTokensForPlayer(player);
            }

            if (newCharacter == Character.Damsel)
            {
                OnDamselInPlay(player);
            }

            await player.ChangeCharacter(newCharacter, newAlignment);
        }

        public async Task GainCharacterAbility(Player player, Character characterAbility)
        {
            player.Tokens.Remove(Token.DamselJinxPoisoned);

            await player.GainCharacterAbility(characterAbility);

            if (player.Character == Character.Spy || player.Character == Character.Widow)
            {
                OnSpyOrWindowInPlay();
            }

            if (player.Character == Character.Damsel)
            {
                OnDamselInPlay(player);
            }
        }

        public void ClearTokensOnPlayerDeath(Player player)
        {
            foreach (var affectedPlayer in players)
            {
                affectedPlayer.Tokens.ClearTokensOnPlayerDeath(player);
            }

            player.Tokens.Remove(Token.BountyHunterPing);
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
        /// <param name="characterAbility">The character ability to filter by.</param>
        /// <returns>A collection of players who we should treat as having the given character ability.</returns>
        public IEnumerable<Player> PlayersForWhomWeShouldRunAbility(Character characterAbility)
        {
            return players.Where(player => player.ShouldRunAbility(characterAbility));
        }

        /// <summary>
        /// Returns all players that actually have the given ability. This will exclude drunk or poisoned players, as well as
        /// characters who think they have this ability but do not, e.g. Lunatic and Marionette.
        /// </summary>
        /// <param name="characterAbility">The character ability to filter by.</param>
        /// <returns>A collection of players who actually have the ability.</returns>
        public IEnumerable<Player> PlayersWithHealthyAbility(Character characterAbility)
        {
            return players.Where(player => player.HasHealthyAbility(characterAbility));
        }

        /// <summary>
        /// Returns the first player that actually has the given ability. This will exclude drunk or poisoned players, as well as
        /// characters who think they have this ability but do not, e.g. Lunatic and Marionette.
        /// </summary>
        /// <param name="characterAbility">The character ability to filter by.</param>
        /// <returns>One player with the given ability, or null if no players have the ability.</returns>
        public Player? GetPlayerWithHealthyAbility(Character characterAbility)
        {
            return PlayersWithHealthyAbility(characterAbility).FirstOrDefault();
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

        private void OnSpyOrWindowInPlay()
        {
            spyOrWidowHasBeenInPlay = true;

            foreach (var damsel in players.WithCharacter(Character.Damsel))
            {
                damsel.Tokens.Add(Token.DamselJinxPoisoned, damsel);
            }
        }

        private void OnDamselInPlay(Player damsel)
        {
            if (spyOrWidowHasBeenInPlay)
            {
                damsel.Tokens.Add(Token.DamselJinxPoisoned, damsel);
            }
        }

        private readonly List<Player> players;
        private Alignment? winner;

        // Flag to indicate that a minion has had the chance to see the Grimoire.
        // There are jinxes that ensure that this isn't overpowered.
        private bool spyOrWidowHasBeenInPlay = false;
    }
}
