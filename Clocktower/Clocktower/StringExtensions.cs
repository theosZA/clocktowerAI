namespace Clocktower
{
    internal static class StringExtensions
    {
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
