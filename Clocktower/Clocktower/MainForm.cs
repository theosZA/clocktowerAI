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
                var script = ChooseScript();
                if (script != null)
                {
                    await SetupAndRunGame(script);
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

        private async void aIStorytellerGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var script = ChooseScript();
                if (script != null)
                {
                    await SetupAndRunGame(script, randomSetup: true, aiStoryteller: true);
                }
            }
            catch (Exception exception)
            {
                statusLabel.Text = exception.Message;
                Debug.WriteLine(exception.ToString());
            }
        }

        private static string? ChooseScript()
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
                return scriptDialog.FileName;
            }
            else
            {
                return null;
            }
        }

        private async Task SetupAndRunGame(string? scriptFileName, bool randomSetup = false, bool aiStoryteller = false)
        {
            var playerConfigsSection = ConfigurationManager.GetSection("PlayerConfig") as PlayerConfigSection ?? throw new Exception("Invalid or missing PlayerConfig section");
            var playerConfigs = playerConfigsSection.Players.PlayerConfigs.ToList();
            var forcedAlignments = playerConfigs.Select(config => config.Alignment).ToList();
            var forcedCharacters = playerConfigs.Select(config => config.Character).ToList();

            var setupDialog = new SetupDialog(scriptFileName, random, forcedAlignments, forcedCharacters);
            if (randomSetup)
            {
                setupDialog.PlayerCount = 8;    // TODO: Make customizable
                setupDialog.RandomizeSetup();
            }
            else
            {
                var result = setupDialog.ShowDialog();
                if (result != DialogResult.OK)
                {
                    return;
                }
            }

            var agents = (await AgentFactory.CreateAgentsFromConfig(setupDialog, random)).ToList();
            var playerNames = agents.Select(agent => agent.PlayerName).ToList();

            var aiModel = aiStoryteller ? "gpt-4o" : null;  // TODO: Make customizable
            var storyteller = StorytellerFactory.CreateStoryteller(playerNames, setupDialog.ScriptName, setupDialog.Script, random, aiModel);

            var clocktowerGame = new ClocktowerGame(setupDialog, storyteller, agents, random);
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