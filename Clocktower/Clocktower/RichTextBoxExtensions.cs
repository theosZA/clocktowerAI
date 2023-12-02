namespace Clocktower
{
    public static class RichTextBoxExtensions
    {
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
