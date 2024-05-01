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

        public static string TextAfter(this string text, string precedingText)
        {
            int end = text.IndexOf(precedingText);
            if (end == -1) 
            {
                return text;
            }
            return text[(end + 1)..];
        }

        public static bool ContainsWhitespace(this string text)
        {
            return text.IndexOfAny(new[] { ' ', '\t', '\r', '\n' }) != -1;
        }
    }
}
