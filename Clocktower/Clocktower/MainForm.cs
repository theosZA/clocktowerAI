using Clocktower.Game;
using System.Diagnostics;

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
                var setupDialog = new SetupDialog(random);
                var result = setupDialog.ShowDialog();
                if (result != DialogResult.OK)
                {
                    return;
                }

                var clocktowerGame = new ClocktowerGame(setupDialog, random);
                while (!clocktowerGame.Finished)
                {
                    await clocktowerGame.RunNightAndDay();
                }
                clocktowerGame.AnnounceWinner();
            }
            catch (Exception exception)
            {
                statusLabel.Text = exception.Message;
                Debug.WriteLine(exception.ToString());
            }
        }

        private Random random = new();
    }
}