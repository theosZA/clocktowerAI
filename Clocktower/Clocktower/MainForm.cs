using Clocktower.Game;

namespace Clocktower
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private async void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var clocktowerGame = new ClocktowerGame();
                while (!clocktowerGame.Finished)
                {
                    await clocktowerGame.RunNightAndDay();
                }
                clocktowerGame.AnnounceWinner();
            }
            catch (Exception exception)
            {
                statusLabel.Text = exception.Message;
            }
        }
    }
}