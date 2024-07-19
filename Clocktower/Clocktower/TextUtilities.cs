using Clocktower.Game;
using System.ComponentModel;
using System.Text;

namespace Clocktower
{
    public static class TextUtilities
    {
        public static Color AlignmentToColor(Alignment alignment)
        {
            return alignment switch
            {
                Alignment.Good => Color.Blue,
                Alignment.Evil => Color.Red,
                _ => throw new InvalidEnumArgumentException(nameof(alignment)),
            };
        }

        public static Color CharacterToColor(Character character)
        {
            return AlignmentToColor(character.Alignment());
        }

        public static string CharacterToText(Character character)
        {
            return character.ToString().Replace('_', ' ');
        }

        public static string FormatText(string text, params object[] objects)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedText(text, objects);
            return sb.ToString();
        }

        public static string FormatMarkupText(string text, params object[] objects)
        {
            var sb = new StringBuilder();
            sb.AppendFormattedMarkupText(text, objects);
            return sb.ToString();
        }
    }
}
