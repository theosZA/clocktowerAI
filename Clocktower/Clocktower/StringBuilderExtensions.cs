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
            int substitutionsLeft = 0;
            int length = text.Length;
            for (int i = length - 1; i >= 0; --i)
            {
                if (text[i] == '%')
                {
                    ++substitutionsLeft;
                }
            }
            bool storytellerView = false;
            if (objects.Length > substitutionsLeft && objects[substitutionsLeft] is bool b)
            {
                storytellerView = b;
            }

            int substitutionIndex = text.IndexOf('%');
            if (substitutionIndex == 0)
            {
                switch (text[1])
                {
                    case 'n':
                        sb.Append(objects[0].ToString());
                        break;

                    case 'b':
                        sb.AppendBoldText(objects[0].ToString() ?? string.Empty);
                        break;

                    case 'p':
                        if (objects[0] is Player player)
                        {
                            sb.AppendPlayer(player, storytellerView);
                        }
                        break;

                    case 'P':
                        if (objects[0] is IList<Player> players)
                        {
                            sb.AppendPlayers(players, storytellerView);
                        }
                        else if (objects[0] is IEnumerable<Player> playersEnumerable)
                        {
                            sb.AppendPlayers(playersEnumerable.ToList(), storytellerView);
                        }
                        break;

                    case 'c':
                        if (objects[0] is Character character)
                        {
                            sb.AppendCharacter(character);
                        }
                        break;

                    case 'C':
                        if (objects[0] is IList<Character> characters)
                        {
                            sb.AppendCharacters(characters);
                        }
                        else if (objects[0] is IEnumerable<Character> charactersEnumerable)
                        {
                            sb.AppendCharacters(charactersEnumerable.ToList());
                        }
                        break;

                    case 'a':
                        if (objects[0] is Alignment alignment)
                        {
                            sb.AppendText($"{alignment}", TextUtilities.AlignmentToColor(alignment));
                        }
                        break;

                    default:
                        throw new ArgumentException($"Unknown format specifier %{text[1]}", nameof(text));
                }
                sb.AppendFormattedText(text.Substring(2), objects.Skip(1).ToArray());
            }
            else if (substitutionIndex < 0)
            {
                sb.Append(text);
            }
            else
            {
                sb.Append(text[..substitutionIndex]);
                sb.AppendFormattedText(text[substitutionIndex..], objects);
            }
        }

        public static void AppendCharacter(this StringBuilder sb, Character character)
        {
            sb.AppendText(TextUtilities.CharacterToText(character), TextUtilities.CharacterToColor(character));
        }

        public static void AppendCharacters(this StringBuilder sb, IList<Character> characters)
        {
            if (characters.Count == 0)
            {
                return;
            }
            sb.AppendCharacter(characters[0]);
            if (characters.Count == 1)
            {
                return;
            }
            for (int i = 1; i < characters.Count - 1; ++i)
            {
                sb.Append(", ");
                sb.AppendCharacter(characters[i]);
            }
            sb.Append(" and ");
            sb.AppendCharacter(characters[characters.Count - 1]);
        }

        public static void AppendPlayer(this StringBuilder sb, Player player, bool storytellerView = false)
        {
            sb.AppendBoldText(player.Name);
            if (storytellerView)
            {
                sb.Append(" (");

                if (player.Tokens.Contains(Token.IsTheDrunk))
                {
                    sb.AppendText(TextUtilities.CharacterToText(Character.Drunk), TextUtilities.AlignmentToColor(player.Alignment));
                    sb.Append("-");
                }
                if (player.Tokens.Contains(Token.IsThePhilosopher) || player.Tokens.Contains(Token.IsTheBadPhilosopher))
                {
                    sb.AppendText(TextUtilities.CharacterToText(Character.Philosopher), TextUtilities.AlignmentToColor(player.Alignment));
                    sb.Append("-");
                }
                sb.AppendText($"{TextUtilities.CharacterToText(player.Character)}", TextUtilities.AlignmentToColor(player.Alignment));
                if (player.Tokens.Contains(Token.IsTheBadPhilosopher))
                {
                    sb.Append("*");    // We use the asterisk here to denote that they never really gained that ability.
                }
                sb.Append(")");
            }
        }

        public static void AppendPlayers(this StringBuilder sb, IList<Player> players, bool storytellerView = false)
        {
            if (players.Count == 0)
            {
                return;
            }
            sb.AppendPlayer(players[0], storytellerView);
            if (players.Count == 1)
            {
                return;
            }
            for (int i = 1; i < players.Count - 1; ++i)
            {
                sb.Append(", ");
                sb.AppendPlayer(players[i], storytellerView);
            }
            sb.Append(" and ");
            sb.AppendPlayer(players[players.Count - 1], storytellerView);
        }

        public static void AppendText(this StringBuilder sb, string text, Color _)
        {
            // Colored text is not supported.
            sb.Append(text);
        }

        public static void AppendBoldText(this StringBuilder sb, string text)
        {
            // Bold text is not supported.
            sb.Append(text);
        }

        public static void AppendBoldText(this StringBuilder sb, string text, Color color)
        {
            // Bold text is not supported.
            sb.AppendText(text, color);
        }
    }
}
