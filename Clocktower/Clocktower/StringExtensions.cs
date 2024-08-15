using System.Text;

namespace Clocktower
{
    internal static class StringExtensions
    {
        public static string TextBetween(this string text, string startText, string endText)
        {
            int start = text.IndexOf(startText);
            if (start == -1)
            {
                return string.Empty;
            }
            start += startText.Length;
            int end = text.IndexOf(endText, start);
            if (end == -1)
            {
                return string.Empty;
            }
            return text[start..end];
        }

        public static string TextBetween(this string text, char startCharacter, char endCharacter)
        {
            int start = text.IndexOf(startCharacter);
            if (start == -1)
            {
                return string.Empty;
            }
            start++;
            int end = text.IndexOf(endCharacter, start);
            if (end == -1)
            {
                return string.Empty;
            }
            return text[start..end];
        }

        public static string TextBefore(this string text, string succeedingText)
        {
            int start = text.IndexOf(succeedingText);
            if (start == -1)
            {
                return text;
            }
            return text[..start];
        }

        public static string TextBefore(this string text, string succeedingText, StringComparison stringComparison)
        {
            int start = text.IndexOf(succeedingText, stringComparison);
            if (start == -1)
            {
                return text;
            }
            return text[..start];
        }

        public static string TextAfter(this string text, string precedingText)
        {
            int position = text.IndexOf(precedingText);
            if (position == -1) 
            {
                return text;
            }
            return text[(position + precedingText.Length)..];
        }

        public static string TextAfter(this string text, string precedingText, StringComparison stringComparison)
        {
            int position = text.IndexOf(precedingText, stringComparison);
            if (position == -1)
            {
                return text;
            }
            return text[(position + precedingText.Length)..];
        }

        public static (int index, string? foundText) FirstIndexOfAnyText(this string text, IEnumerable<string> textsToFind, int startIndex = 0)
        {
            var positions = textsToFind.Select(textToFind => (text.IndexOf(textToFind, startIndex), textToFind))
                                       .Where(position => position.Item1 >= 0)
                                       .ToList();
            if (positions.Count == 0)
            {
                return (-1, null);
            }
            return positions.MinBy(position => position.Item1);
        }

        public static bool ContainsWhitespace(this string text)
        {
            return text.IndexOfAny(new[] { ' ', '\t', '\r', '\n' }) != -1;
        }
    }
}
