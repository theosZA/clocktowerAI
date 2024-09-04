using Clocktower.Agent;
using Clocktower.Game;

namespace Clocktower.Options
{
    public class KazaliMinionsOption : IOption
    {
        public string Name => "Kazali Minions...";

        public IReadOnlyCollection<(Player player, Character character)> MinionAssignment => minionAssignment;

        public bool AssignmentOk
        {
            get
            {
                if (minionAssignment.Count != MinionCount)
                {
                    return false;
                }
                // Ensure no duplicate players or characters.
                var players = new HashSet<Player>();
                var characters = new HashSet<Character>();
                foreach (var (player, minionCharacter) in minionAssignment)
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
        }

        public int MinionCount { get; private init; }
        public IReadOnlyCollection<Player> PossiblePlayers { get; private init; }
        public IReadOnlyCollection<Character> MinionCharacters { get; private init; }

        public KazaliMinionsOption(int minionCount, IReadOnlyCollection<Player> players, IReadOnlyCollection<Character> minionCharacters)
        {
            MinionCount = minionCount;
            PossiblePlayers = players;
            MinionCharacters = minionCharacters;
        }

        public void ChooseMinions(IEnumerable<(Player player, Character minionCharacter)> minions)
        {
            minionAssignment.Clear();
            minionAssignment.AddRange(minions);
        }

        public bool AddMinionChoicesFromText(string text)
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

            ChooseMinions(individualMinions.Select(playerAsMinion => playerAsMinion!.Value));
            return AssignmentOk;
        }

        private readonly List<(Player player, Character minionCharacter)> minionAssignment = new();
    }
}
