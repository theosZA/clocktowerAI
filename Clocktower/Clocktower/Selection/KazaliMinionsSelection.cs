using Clocktower.Agent;
using Clocktower.Game;

namespace Clocktower.Selection
{
    public class KazaliMinionsSelection
    {
        public IReadOnlyCollection<(Player player, Character character)> Minions => minions;

        public int MinionCount { get; private init; }
        public IReadOnlyCollection<Player> PossiblePlayers { get; private init; }
        public IReadOnlyCollection<Character> MinionCharacters { get; private init; }

        public KazaliMinionsSelection(int minionCount, IReadOnlyCollection<Player> players, IReadOnlyCollection<Character> minionCharacters)
        {
            MinionCount = minionCount;
            PossiblePlayers = players;
            MinionCharacters = minionCharacters;
        }

        public bool SelectMinions(IReadOnlyCollection<(Player player, Character minionCharacter)> minions)
        {
            if (!ValidateSelection(minions))
            {
                return false;
            }

            this.minions.Clear();
            this.minions.AddRange(minions);
            return true;
        }

        public bool SelectMinions(string text)
        {
            var individualMinions = TextParser.ReadPlayersAsCharactersFromText(text, PossiblePlayers, MinionCharacters).ToList();
            if (individualMinions.Count != MinionCount)
            {
                return false;
            }
            if (individualMinions.Any(playerAsMinion => !playerAsMinion.HasValue))
            {
                return false;
            }
            return SelectMinions(individualMinions.Select(playerAsMinion => playerAsMinion!.Value).ToList());
        }

        private bool ValidateSelection(IReadOnlyCollection<(Player player, Character minionCharacter)> minions)
        {
            if (minions.Count != MinionCount)
            {
                return false;
            }
            // Ensure only listed players and characters.
            foreach (var (player, minionCharacter) in minions)
            {
                if (!PossiblePlayers.Contains(player))
                {
                    return false;
                }
                if (!MinionCharacters.Contains(minionCharacter))
                {
                    return false;
                }
            }
            // Ensure no duplicate players or characters.
            var players = new HashSet<Player>();
            var characters = new HashSet<Character>();
            foreach (var (player, minionCharacter) in minions)
            {
                if (players.Contains(player))
                {
                    return false;
                }
                players.Add(player);
                if (characters.Contains(minionCharacter))
                {
                    return false;
                }
                characters.Add(minionCharacter);
            }

            return true;
        }

        private readonly List<(Player player, Character minionCharacter)> minions = new();
    }
}
