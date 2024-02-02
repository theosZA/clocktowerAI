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

        public static string TextBefore(this string text, string endText)
        {
            int start = text.IndexOf(endText);
            if (start == -1)
            {
                return text;
            }
            return text[..start];
        }
    }
}
