namespace Clocktower
{
    public partial class StorytellerForm : Form
    {
        public StorytellerForm()
        {
            InitializeComponent();
        }

        public void AssignCharacter(string name, Character character, Alignment alignment)
        {
            outputText.AppendBoldText(name);
            outputText.AppendText(" is the ");
            outputText.AppendText(TextUtilities.CharacterToText(character), TextUtilities.AlignmentToColor(alignment));
            outputText.AppendText(".\n");
        }

        public void AssignCharacter(string name, Character realCharacter, Alignment realAlignment,
                                                 Character believedCharacter, Alignment believedAlignment)
        {
            outputText.AppendBoldText(name);
            outputText.AppendText(" believes they are the ");
            outputText.AppendText(TextUtilities.CharacterToText(believedCharacter), TextUtilities.AlignmentToColor(believedAlignment));
            outputText.AppendText(" but they are actually the ");
            outputText.AppendText(TextUtilities.CharacterToText(realCharacter), TextUtilities.AlignmentToColor(realAlignment));
            outputText.AppendText(".\n");
        }
    }
}
