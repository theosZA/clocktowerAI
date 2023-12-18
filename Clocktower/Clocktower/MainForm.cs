using Clocktower.Game;
using Clocktower.OpenAiApi;
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

        private async void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Code to test the API chat completion.
                var chat = new Chat("You are a helpful assistant.");
                var response = await chat.RequestChatResponse("What are the 3 largest cities in Europe?");
                MessageBox.Show(response);
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