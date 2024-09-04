using Clocktower.Agent;
using Clocktower.Agent.RobotAgent.Model;
using Clocktower.Game;

namespace Clocktower.Selection
{
    public class KazaliMinionsSelection
    {
        public IReadOnlyCollection<(Player player, Character character)> Minions => minions;

        public int MinionCount { get; private init; }
        public IReadOnlyCollection<Player> PossiblePlayers { get; private init; }
        public IReadOnlyCollection<Character> MinionCharacters { get; private init; }

        // For a given character, only the specified players can be chosen as that character.
        public IDictionary<Character, IReadOnlyCollection<Player>> CharacterLimitations { get; private init; }

        public KazaliMinionsSelection(int minionCount, IReadOnlyCollection<Player> players, IReadOnlyCollection<Character> minionCharacters,
                                      IDictionary<Character, IReadOnlyCollection<Player>> characterLimitations)
        {
            MinionCount = minionCount;
            PossiblePlayers = players;
            MinionCharacters = minionCharacters;
            CharacterLimitations = characterLimitations;
        }

        public bool ValidateSelection(IReadOnlyCollection<(Player player, Character minionCharacter)> minions)
        {
            return DetailedValidateSelection(minions).ok;
        }

        public (bool ok, string? error) SelectMinions(IReadOnlyCollection<(Player player, Character minionCharacter)> minions)
        {
            var result = DetailedValidateSelection(minions);
            if (result.ok)
            {
                this.minions.Clear();
                this.minions.AddRange(minions);
            }
            return result;
        }

        public (bool ok, string? error) SelectMinions(string text)
        {
            var minions = TextParser.ReadPlayersAsCharactersFromText(text, PossiblePlayers, MinionCharacters).ToList();
            if (minions.Any(playerAsMinion => !playerAsMinion.HasValue))
            {
                return (false, null);   // We don't provide an error message here, and rely on the caller to provide a more general error.
            }
            return SelectMinions(minions.Select(playerAsMinion => playerAsMinion!.Value).ToList());
        }

        public (bool ok, string? error) SelectMinions(IEnumerable<PlayerAsCharacter> selections)
        {
            List<(Player, Character)> minions = new();
            foreach (var selection in selections)
            {
                var player = TextParser.ReadPlayerFromText(selection.Player, PossiblePlayers);
                if (player == null)
                {
                    return (false, $"{selection.Player} is not a valid choice of player. Choose from: {string.Join(", ", PossiblePlayers.Select(player => player.Name))}.");
                }
                var character = TextParser.ReadCharacterFromText(selection.Character, MinionCharacters);
                if (character == null)
                {
                    return (false, $"{selection.Character} is not a valid choice of Minion character. Choose from: {string.Join(", ", MinionCharacters.Select(TextUtilities.CharacterToText))}.");
                }
                minions.Add((player, character.Value));
            }
            return SelectMinions(minions);
        }

        private (bool ok, string? error) DetailedValidateSelection(IReadOnlyCollection<(Player player, Character minionCharacter)> minions)
        {
            if (minions.Count != MinionCount)
            {
                return (false, $"You must have exactly {MinionCount} minion assignments.");
            }
            // Ensure only listed players and characters.
            foreach (var (player, minionCharacter) in minions)
            {
                if (!PossiblePlayers.Contains(player))
                {
                    return (false, $"{player.Name} is not a valid choice of player. Choose from: {string.Join(", ", PossiblePlayers.Select(player => player.Name))}.");
                }
                if (!MinionCharacters.Contains(minionCharacter))
                {
                    return (false, $"{TextUtilities.CharacterToText(minionCharacter)} is not a valid choice of Minion character. Choose from: {string.Join(", ", MinionCharacters.Select(TextUtilities.CharacterToText))}.");
                }
            }
            // Ensure no duplicate players or characters.
            var players = new HashSet<Player>();
            var characters = new HashSet<Character>();
            foreach (var (player, minionCharacter) in minions)
            {
                if (players.Contains(player))
                {
                    return (false, $"No player may be chosen more than once and {player.Name} has been chosen twice.");
                }
                players.Add(player);
                if (characters.Contains(minionCharacter))
                {
                    return (false, $"No Minion character may be chosen more than once and {TextUtilities.CharacterToText(minionCharacter)} has been chosen twice.");
                }
                characters.Add(minionCharacter);
            }
            // Ensure all limitations are followed.
            foreach (var (player, minionCharacter) in minions)
            {
                if (CharacterLimitations.TryGetValue(minionCharacter, out var allowedPlayers))
                {
                    if (!allowedPlayers.Contains(player))
                    {
                        return (false, $"{player.Name} may not be the {TextUtilities.CharacterToText(minionCharacter)}; the only players who may be the {TextUtilities.CharacterToText(minionCharacter)} are {string.Join(", ", allowedPlayers.Select(player => player.Name))}");
                    }
                }
            }

            return (true, null);
        }

        private readonly List<(Player player, Character minionCharacter)> minions = new();
    }
}
