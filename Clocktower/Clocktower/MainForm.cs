using Clocktower.Agent;
using Clocktower.Agent.Config;
using Clocktower.Game;
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
                var playerConfigsSection = ConfigurationManager.GetSection("PlayerConfig") as PlayerConfigSection ?? throw new Exception("Invalid or missing PlayerConfig section");
                var playerConfigs = playerConfigsSection.Players.PlayerConfigs.ToList();
                var forcedAlignments = playerConfigs.Select(config => config.Alignment).ToList();
                var forcedCharacters = playerConfigs.Select(config => config.Character).ToList();

                var setupDialog = new SetupDialog("Transferring to Trouble", random, forcedAlignments, forcedCharacters);
                var result = setupDialog.ShowDialog();
                if (result != DialogResult.OK)
                {
                    return;
                }

                var agents = await AgentFactory.CreateAgentsFromConfig(setupDialog, random);
                var clocktowerGame = new ClocktowerGame(setupDialog, agents.ToList(), random);
                await clocktowerGame.StartGame();
                while (!clocktowerGame.Finished)
                {
                    await clocktowerGame.RunNightAndDay();
                }
                await clocktowerGame.AnnounceWinner();
            }
            catch (Exception exception)
            {
                statusLabel.Text = exception.Message;
                Debug.WriteLine(exception.ToString());
            }
        }

        private readonly Random random = new();
    }
}