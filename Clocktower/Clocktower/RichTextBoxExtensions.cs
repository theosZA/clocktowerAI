using Clocktower.Game;

namespace Clocktower
{
    public static class RichTextBoxExtensions
    {
        /// <summary>
        /// Writes to the given RichTextBox with formatting using format specifier '%'.
        /// </summary>
        /// <param name="box">The RichTextBox to write the text to.</param>
        /// <param name="text">
        /// Writes the given text to the output textbox, only substituting variables as 
        /// %n: normal, %b: bold,
        /// %p: formatted as a player, %P: formatted as a player list,
        /// %c: formatted as a character, %C: formatted as a character list.
        /// %a: formatted as an alignment
        /// </param>
        /// <param name="objects">
        /// Objects to substitute into the output text. For %p the object must be a Player, for %P the object must be an IEnumerable<Player>, for %c the object must be a Character, and for %C the object must be an IEnumerable<Character>.
        /// You can enable storyteller view for your objects (which may display more info than is publicly known) by including a true boolean as an additional parameter.
        /// </param>
        public static void AppendFormattedText(this RichTextBox box, string text, params object[] objects)
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
                        box.AppendText(objects[0].ToString());
                        break;

                    case 'b':
                        box.AppendBoldText(objects[0].ToString() ?? string.Empty);
                        break;

                    case 'p':
                        if (objects[0] is Player player)
                        {
                            box.AppendPlayer(player, storytellerView);
                        }
                        break;

                    case 'P':
                        if (objects[0] is IList<Player> players)
                        {
                            box.AppendPlayers(players, storytellerView);
                        }
                        else if (objects[0] is IEnumerable<Player> playersEnumerable)
                        {
                            box.AppendPlayers(playersEnumerable.ToList(), storytellerView);
                        }
                        break;

                    case 'c':
                        if (objects[0] is Character character)
                        {
                            box.AppendCharacter(character);
                        }
                        break;

                    case 'C':
                        if (objects[0] is IList<Character> characters)
                        {
                            box.AppendCharacters(characters);
                        }
                        else if (objects[0] is IEnumerable<Character> charactersEnumerable)
                        {
                            box.AppendCharacters(charactersEnumerable.ToList());
                        }
                        break;

                    case 'a':
                        if (objects[0] is Alignment alignment)
                        {
                            box.AppendText($"{alignment}", TextUtilities.AlignmentToColor(alignment));
                        }
                        break;

                    default:
                        throw new ArgumentException($"Unknown format specifier %{text[1]}", nameof(text));
                }
                box.AppendFormattedText(text.Substring(2), objects.Skip(1).ToArray());
            }
            else if (substitutionIndex < 0)
            {
                box.AppendText(text);
            }
            else
            {
                box.AppendText(text.Substring(0, substitutionIndex));
                box.AppendFormattedText(text.Substring(substitutionIndex), objects);
            }
        }

        public static void AppendCharacter(this RichTextBox box, Character character)
        {
            box.AppendText(TextUtilities.CharacterToText(character), TextUtilities.CharacterToColor(character));
        }

        public static void AppendCharacters(this RichTextBox box, IList<Character> characters)
        {
            if (characters.Count == 0)
            {
                return;
            }
            box.AppendCharacter(characters[0]);
            if (characters.Count == 1)
            {
                return;
            }
            for (int i = 1; i < characters.Count - 1; ++i)
            {
                box.AppendText(", ");
                box.AppendCharacter(characters[i]);
            }
            box.AppendText(" and ");
            box.AppendCharacter(characters[characters.Count - 1]);
        }

        public static void AppendPlayer(this RichTextBox box, Player player, bool storytellerView = false)
        {
            box.AppendBoldText(player.Name);
            if (storytellerView)
            {
                box.AppendText(" (");

                if (player.Tokens.Contains(Token.IsTheDrunk))
                {
                    box.AppendText(TextUtilities.CharacterToText(Character.Drunk), TextUtilities.AlignmentToColor(player.Alignment));
                    box.AppendText("-");
                }
                box.AppendText($"{TextUtilities.CharacterToText(player.Character)}", TextUtilities.AlignmentToColor(player.Alignment));
                box.AppendText(")");
            }
        }

        public static void AppendPlayers(this RichTextBox box, IList<Player> players, bool storytellerView = false)
        {
            if (players.Count == 0)
            {
                return;
            }
            box.AppendPlayer(players[0], storytellerView);
            if (players.Count == 1)
            {
                return;
            }
            for (int i = 1; i < players.Count - 1; ++i)
            {
                box.AppendText(", ");
                box.AppendPlayer(players[i], storytellerView);
            }
            box.AppendText(" and ");
            box.AppendPlayer(players[players.Count - 1], storytellerView);
        }

        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }

        public static void AppendBoldText(this RichTextBox box, string text)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionFont = new Font(box.Font, FontStyle.Bold);
            box.AppendText(text);
            box.SelectionFont = new Font(box.Font, FontStyle.Regular);
        }

        public static void AppendBoldText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionFont = new Font(box.Font, FontStyle.Bold);
            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionFont = new Font(box.Font, FontStyle.Regular);
            box.SelectionColor = box.ForeColor;
        }
    }
}
