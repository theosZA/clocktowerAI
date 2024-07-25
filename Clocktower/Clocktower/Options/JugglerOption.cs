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
            text = text.Trim();

            if (text.Length == 0)
            {   // An empty juggle is unusual but allowed, e.g. as a Vortox check.
                return true;
            }

            var individualJuggles = text.Split(',').Select(juggle => ReadJuggleFromText(juggle)).ToList();
            if (individualJuggles.Count > 5)
            {
                return false;
            }
            if (individualJuggles.Any(juggle => !juggle.HasValue))
            {
                return false;
            }

#pragma warning disable CS8629 // Nullable value type may be null.
            juggles.AddRange(individualJuggles.Select(juggle => juggle.Value));
#pragma warning restore CS8629 // Nullable value type may be null.
            return true;
        }

        private (Player, Character)? ReadJuggleFromText(string text)
        {
            // If the juggler is being helpful they'll have written the juggle in the form "PLAYER as CHARACTER".
            if (text.Contains(" as ", StringComparison.InvariantCultureIgnoreCase))
            {
                return ReadJuggleFromText(text.TextBefore(" as ", StringComparison.InvariantCultureIgnoreCase), text.TextAfter(" as ", StringComparison.InvariantCultureIgnoreCase));
            }

            // Otherwise we're going to just look for player name and character name matches in the text - this is obviously fragile.
            return ReadJuggleFromText(text, text);
        }

        private (Player, Character)? ReadJuggleFromText(string playerText, string characterText)
        {
            var player = TextParser.ReadPlayerFromText(playerText, PossiblePlayers);
            if (player == null)
            {
                return null;
            }

            var character = TextParser.ReadCharacterFromText(characterText, ScriptCharacters);
            if (!character.HasValue)
            {
                return null;
            }

            return (player, character.Value);
        }

        private readonly List<(Player player, Character character)> juggles = new();
    }
}
