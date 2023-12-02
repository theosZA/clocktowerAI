namespace Clocktower
{
    public partial class HumanAgentForm : Form
    {
        public HumanAgentForm(string playerName)
        {
            InitializeComponent();

            Text = playerName;
        }

        public void AssignCharacter(Character character, Alignment alignment)
        {
            outputText.AppendText("You are the ");
            outputText.AppendText(TextUtilities.CharacterToText(character), TextUtilities.AlignmentToColor(alignment));
            outputText.AppendText(".");
        }
    }
}
