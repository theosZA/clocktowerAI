using Clocktower.Agent;
using Clocktower.Agent.Config;
using Clocktower.Game;
using Clocktower.Storyteller;
using System.Configuration;
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
                var scriptDialog = new OpenFileDialog
                {
                    Title = "Choose script",
                    Filter = "Clocktower scripts|*.json",
                    InitialDirectory = Path.Combine(Application.StartupPath, "Scripts")
                };
                var dialogChoice = scriptDialog.ShowDialog();
                if (dialogChoice == DialogResult.OK)
                {
                    await SetupAndRunGame(scriptDialog.FileName);
                }
            }
            catch (Exception exception)
            {
                statusLabel.Text = exception.Message;
                Debug.WriteLine(exception.ToString());
            }
        }

        private async void newWhaleBucketToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                await SetupAndRunGame(scriptFileName: null);
            }
            catch (Exception exception)
            {
                statusLabel.Text = exception.Message;
                Debug.WriteLine(exception.ToString());
            }
        }

        private async Task SetupAndRunGame(string? scriptFileName)
        {
            var playerConfigsSection = ConfigurationManager.GetSection("PlayerConfig") as PlayerConfigSection ?? throw new Exception("Invalid or missing PlayerConfig section");
            var playerConfigs = playerConfigsSection.Players.PlayerConfigs.ToList();
            var forcedAlignments = playerConfigs.Select(config => config.Alignment).ToList();
            var forcedCharacters = playerConfigs.Select(config => config.Character).ToList();

            var setupDialog = new SetupDialog(scriptFileName, random, forcedAlignments, forcedCharacters);
            var result = setupDialog.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            var storyteller = new StorytellerForm(random);
            var agents = await AgentFactory.CreateAgentsFromConfig(setupDialog, random);
            var clocktowerGame = new ClocktowerGame(setupDialog, storyteller, agents.ToList(), random);
            await clocktowerGame.StartGame();
            while (!clocktowerGame.Finished)
            {
                await clocktowerGame.RunNightAndDay();
            }
            await clocktowerGame.AnnounceWinner();
        }

        private readonly Random random = new();
    }
}