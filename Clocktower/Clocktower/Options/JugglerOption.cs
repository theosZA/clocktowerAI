using Clocktower.Game;

namespace Clocktower.Options
{
    internal class JugglerOption : IOption
    {
        public string Name => "Juggle...";

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
            // Cut "Juggler:" from beginning of text and trim the whitespace.
            var jugglesText = text.Trim()[8..].Trim();

            if (jugglesText.Length == 0)
            {   // An empty juggle is unusual but allowed, e.g. as a Vortox check.
                return true;
            }

            var individualJuggles = jugglesText.Split(',').Select(juggle => ReadJuggleFromText(juggle.Trim())).ToList();
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
            if (text.Contains(" as "))
            {
                return ReadJuggleFromText(text.TextBefore(" as "), text.TextAfter(" as "));
            }

            // Otherwise we're going to just look for player name and character name matches in the text - this is obviously fragile.
            return ReadJuggleFromText(text, text);
        }

        private (Player, Character)? ReadJuggleFromText(string playerText, string characterText)
        {
            var player = ReadPlayerFromText(playerText);
            if (player == null)
            {
                return null;
            }

            var character = ReadCharacterFromText(characterText);
            if (!character.HasValue)
            {
                return null;
            }

            return (player, character.Value);
        }

        private Player? ReadPlayerFromText(string text)
        {
            // Look for exact player name match first.
            foreach (var player in PossiblePlayers)
            {
                if (text.Equals(player.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return player;
                }
            }

            // Then just see if there's a match somewhere in the text.
            foreach (var player in PossiblePlayers)
            {
                if (text.Contains(player.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return player;
                }
            }

            // No matches.
            return null;
        }

        private Character? ReadCharacterFromText(string text)
        {
            // Look for exact character match first.
            foreach (var character in ScriptCharacters)
            {
                if (text.Equals(character.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    return character;
                }
            }

            // Then just see if there's a match somewhere in the text.
            foreach (var character in ScriptCharacters)
            {
                if (text.Contains(character.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    return character;
                }
            }

            // No matches.
            return null;
        }

        private readonly List<(Player player, Character character)> juggles = new();
    }
}
