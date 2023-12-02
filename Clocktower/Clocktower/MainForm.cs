using Clocktower.Game;

namespace Clocktower
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // No configuration of the game yet. We just create a new instance of the game with 8 human players with fixed characters.
            clocktowerGame = new();
        }

        private ClocktowerGame? clocktowerGame;
    }
}