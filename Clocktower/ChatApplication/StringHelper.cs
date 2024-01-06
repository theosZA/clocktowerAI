namespace ChatApplication
{
    internal static class StringHelper
    {
        public static string TextBetween(this string text, char startCharacter, char endCharacter)
        {
            int start = text.IndexOf(startCharacter) + 1;
            int end = text.IndexOf(endCharacter);
            return text[start..end];
        }

        public static string TextAfter(this string text, string startText)
        {
            int start = text.IndexOf(startText) + startText.Length;
            return text[start..];
        }
    }
}
