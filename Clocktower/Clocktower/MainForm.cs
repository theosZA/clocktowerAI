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
            try
            {
                // No configuration of the game yet. We just create a new instance of the game with 8 human players with fixed characters.
                clocktowerGame = new();

                while (!clocktowerGame.Finished)
                {
                    clocktowerGame.RunPhase();
                }
            }
            catch (Exception exception)
            {
                statusStrip.Text = exception.Message;
            }
        }

        private ClocktowerGame? clocktowerGame;
    }
}