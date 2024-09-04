using Clocktower.Agent;
using Clocktower.Game;

namespace Clocktower.Options
{
    internal class JugglerOption : IOption
    {
        public string Name => "Juggler...";

        public IReadOnlyCollection<(Player player, Character character)> Juggles => juggles;

        public IReadOnlyCollection<Player> PossiblePlayers { get; private init; }
        public IReadOnlyCollection<Character> ScriptCharacters { get; private init; }

        public JugglerOption(IReadOnlyCollection<Player> players, IReadOnlyCollection<Character> scriptCharacters)
        {
            PossiblePlayers = players;
            ScriptCharacters = scriptCharacters;
        }

        public void AddJuggles(IEnumerable<(Player player, Character character)> juggles)
        {
            this.juggles.AddRange(juggles);
        }

        public bool AddJugglesFromText(string text)
        {
            var individualJuggles = TextParser.ReadPlayersAsCharactersFromText(text, PossiblePlayers, ScriptCharacters).ToList();
            if (individualJuggles.Count > 5)
            {
                return false;
            }
            if (individualJuggles.Any(juggle => !juggle.HasValue))
            {
                return false;
            }

            juggles.AddRange(individualJuggles.Select(juggle => juggle!.Value));
            return true;
        }

        private readonly List<(Player player, Character character)> juggles = new();
    }
}
