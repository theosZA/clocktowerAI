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

                advanceButton.Enabled = true;

                Advance();
            }
            catch (Exception exception)
            {
                statusLabel.Text = exception.Message;
            }
        }

        private void advanceButton_Click(object sender, EventArgs e)
        {
            try
            {
                Advance();
            }
            catch (Exception exception)
            {
                statusLabel.Text = exception.Message;
            }
        }

        private void Advance()
        {
            if (clocktowerGame == null || clocktowerGame.Finished)
            {
                return;
            }
            clocktowerGame.RunPhase();
            if (clocktowerGame.Finished)
            {
                advanceButton.Enabled = false;
            }
        }

        private ClocktowerGame? clocktowerGame;
    }
}