using Clocktower.Game;
using System.Text;

namespace Clocktower
{
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Writes to the given StringBuilder with formatting using format specifier '%'.
        /// </summary>
        /// <param name="sb">The StringBulder to add the text to.</param>
        /// <param name="text">
        /// Writes the given text to the StringBuilder, only substituting variables as 
        /// %n: normal, %b: bold,
        /// %p: formatted as a player, %P: formatted as a player list,
        /// %c: formatted as a character, %C: formatted as a character list.
        /// %a: formatted as an alignment
        /// </param>
        /// <param name="objects">
        /// Objects to substitute into the output text. For %p the object must be a Player, for %P the object must be an IEnumerable<Player>, for %c the object must be a Character, and for %C the object must be an IEnumerable<Character>.
        /// You can enable storyteller view for your objects (which may display more info than is publicly known) by including a true boolean as an additional parameter.
        /// </param>
        public static void AppendFormattedText(this StringBuilder sb, string text, params object[] objects)
        {
            bool storytellerView = IsStorytellerView(text, objects);
            int? currentIndex = 0;
            int objectIndex = 0;
            while (currentIndex.HasValue)
            {
                var currentObject = objectIndex < objects.Length ? objects[objectIndex] : null;
                currentIndex = sb.AppendFormattedObject(text, currentIndex.Value, currentObject, enableBoldMarkup: false, storytellerView);
                ++objectIndex;
            }
        }

        /// <summary>
        /// Writes to the given StringBuilder with markup formatting using format specifier '%'.
        /// </summary>
        /// <param name="sb">The StringBulder to add the text to.</param>
        /// <param name="text">
        /// Writes the given text to the StringBuilder, only substituting variables as 
        /// %n: normal, %b: bold,
        /// %p: formatted as a player, %P: formatted as a player list,
        /// %c: formatted as a character, %C: formatted as a character list.
        /// %a: formatted as an alignment
        /// </param>
        /// <param name="objects">
        /// Objects to substitute into the output text. For %p the object must be a Player, for %P the object must be an IEnumerable<Player>, for %c the object must be a Character, and for %C the object must be an IEnumerable<Character>.
        /// You can enable storyteller view for your objects (which may display more info than is publicly known) by including a true boolean as an additional parameter.
        /// </param>
        public static void AppendFormattedMarkupText(this StringBuilder sb, string text, params object[] objects)
        {
            bool storytellerView = IsStorytellerView(text, objects);
            int? currentIndex = 0;
            int objectIndex = 0;
            while (currentIndex.HasValue)
            {
                var currentObject = objectIndex < objects.Length ? objects[objectIndex] : null;
                currentIndex = sb.AppendFormattedObject(text, currentIndex.Value, currentObject, enableBoldMarkup: true, storytellerView);
                ++objectIndex;
            }
        }

        private static bool IsStorytellerView(string text, params object[] objects)
        {
            var expectedObjectCount = text.Count(c => c == '%') ;
            if (expectedObjectCount >= objects.Length)
            {   // No storyteller parameter specified.
                return false;
            }
            // There is at least one extra parameter provided. We specifically look at the final one to see
            // if it's true to indicate storyteller view.
            return objects[^1] is bool isStorytellerView && isStorytellerView;
        }

        private static int? AppendFormattedObject(this StringBuilder sb, string text, int currentIndex, object? currentObject, bool enableBoldMarkup, bool storytellerView)
        {
            int substitutionIndex = text.IndexOf('%', currentIndex);
            if (substitutionIndex < 0)
            {
                // Add the rest of the text.
                sb.Append(text[currentIndex..]);
                return null;    // And done
            }

            // Add everything before the %
            sb.Append(text[currentIndex..substitutionIndex]);
            // Add the next formatted item.
            sb.AppendFormattedObject(text[substitutionIndex + 1], currentObject, enableBoldMarkup, storytellerView);

            return substitutionIndex + 2;
        }

        private static void AppendFormattedObject(this StringBuilder sb, char specifier, object? currentObject, bool enableBoldMarkup, bool storytellerView)
        {
            switch (specifier)
            {
                case 'n':
                    sb.Append(currentObject?.ToString());
                    break;

                case 'b':
                    sb.AppendBoldText(currentObject?.ToString(), enableBoldMarkup);
                    break;

                case 'p':
                    if (currentObject is Player player)
                    {
                        sb.AppendPlayer(player, enableBoldMarkup, storytellerView);
                    }
                    break;

                case 'P':
                    if (currentObject is IList<Player> players)
                    {
                        sb.AppendPlayers(players, enableBoldMarkup, storytellerView);
                    }
                    else if (currentObject is IEnumerable<Player> playersEnumerable)
                    {
                        sb.AppendPlayers(playersEnumerable.ToList(), enableBoldMarkup, storytellerView);
                    }
                    break;

                case 'c':
                    if (currentObject is Character character)
                    {
                        sb.AppendCharacter(character, enableBoldMarkup);
                    }
                    break;

                case 'C':
                    if (currentObject is IList<Character> characters)
                    {
                        sb.AppendCharacters(characters, enableBoldMarkup);
                    }
                    else if (currentObject is IEnumerable<Character> charactersEnumerable)
                    {
                        sb.AppendCharacters(charactersEnumerable.ToList(), enableBoldMarkup);
                    }
                    break;

                case 'a':
                    if (currentObject is Alignment alignment)
                    {
                        sb.AppendAlignment(alignment, enableBoldMarkup);
                    }
                    break;

                default:
                    throw new ArgumentException($"Unknown format specifier %{specifier}", nameof(specifier));
            }
        }

        private static void AppendCharacter(this StringBuilder sb, Character character, bool enableBoldMarkup)
        {
            sb.AppendBoldText(TextUtilities.CharacterToText(character), enableBoldMarkup);
        }

        private static void AppendCharacters(this StringBuilder sb, IList<Character> characters, bool enableBoldMarkup)
        {
            if (characters.Count == 0)
            {
                return;
            }
            sb.AppendCharacter(characters[0], enableBoldMarkup);
            if (characters.Count == 1)
            {
                return;
            }
            for (int i = 1; i < characters.Count - 1; ++i)
            {
                sb.Append(", ");
                sb.AppendCharacter(characters[i], enableBoldMarkup);
            }
            sb.Append(" and ");
            sb.AppendCharacter(characters[characters.Count - 1], enableBoldMarkup);
        }

        private static void AppendPlayer(this StringBuilder sb, Player player, bool enableBoldMarkup, bool storytellerView)
        {
            sb.AppendBoldText(player.Name, enableBoldMarkup);
            if (storytellerView)
            {
                sb.Append(" (");

                if (player.Tokens.HasToken(Token.IsTheDrunk))
                {
                    sb.AppendCharacter(Character.Drunk, enableBoldMarkup);
                    sb.Append('-');
                }
                if (player.Tokens.HasToken(Token.IsThePhilosopher) || player.Tokens.HasToken(Token.IsTheBadPhilosopher))
                {
                    sb.AppendCharacter(Character.Philosopher, enableBoldMarkup);
                    sb.Append('-');
                }
                sb.AppendCharacter(player.RealCharacter, enableBoldMarkup);
                if (player.Tokens.HasToken(Token.IsTheBadPhilosopher))
                {
                    sb.Append('*');    // We use the asterisk here to denote that they never really gained that ability.
                }
                sb.Append(')');
            }
        }

        private static void AppendPlayers(this StringBuilder sb, IList<Player> players, bool enableBoldMarkup, bool storytellerView)
        {
            if (players.Count == 0)
            {
                return;
            }
            sb.AppendPlayer(players[0], enableBoldMarkup, storytellerView);
            if (players.Count == 1)
            {
                return;
            }
            for (int i = 1; i < players.Count - 1; ++i)
            {
                sb.Append(", ");
                sb.AppendPlayer(players[i], enableBoldMarkup, storytellerView);
            }
            sb.Append(" and ");
            sb.AppendPlayer(players[players.Count - 1], enableBoldMarkup, storytellerView);
        }

        private static void AppendAlignment(this StringBuilder sb, Alignment alignment, bool enableBoldMarkup)
        {
            sb.AppendBoldText(alignment.ToString(), enableBoldMarkup);
        }

        private static void AppendBoldText(this StringBuilder sb, string? text, bool enableBoldMarkup)
        {
            if (enableBoldMarkup)
            {
                sb.Append("**");
            }
            sb.Append(text);
            if (enableBoldMarkup)
            {
                sb.Append("**");
            }
        }
    }
}
