using Clocktower.Agent;
using Clocktower.Agent.Config;
using Clocktower.Game;
using Clocktower.Setup;
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
                await SetupAndRunGame(scriptFileName: null, aiStoryteller: true);
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

        private async Task SetupAndRunGame(string? scriptFileName, bool aiStoryteller = false)
        {
            var (gameSetup, aiModel) = DetermineSetup(scriptFileName, aiStoryteller);
            if (gameSetup == null)
            {
                return;
            }

            var agents = (await AgentFactory.CreateAgentsFromConfig(gameSetup, random)).ToList();
            var playerNames = agents.Select(agent => agent.PlayerName).ToList();

            var storyteller = StorytellerFactory.CreateStoryteller(playerNames, gameSetup.ScriptName, gameSetup.Script, random, aiModel);

            var clocktowerGame = new ClocktowerGame(gameSetup, storyteller, agents, random);
            await clocktowerGame.StartGame();
            while (!clocktowerGame.Finished)
            {
                await clocktowerGame.RunNightAndDay();
            }
            await clocktowerGame.AnnounceWinner();
        }

        private (IGameSetup? gameSetup, string? aiModel) DetermineSetup(string? scriptFileName, bool aiStoryteller = false)
        {
            if (aiStoryteller)
            {
                return DetermineSetupForAiStoryteller();
            }

            return (DetermineSetupForHumanStoryteller(scriptFileName), null);
        }

        private IGameSetup? DetermineSetupForHumanStoryteller(string? scriptFileName)
        {
            var setupDialog = new SetupDialog(scriptFileName, random, ForcedAlignments, ForcedCharacters);
            var result = setupDialog.ShowDialog();
            return result == DialogResult.OK ? setupDialog : null;
        }

        private (IGameSetup? gameSetup, string? aiModel) DetermineSetupForAiStoryteller()
        {
            var setupAiGame = new SetupAiGame();
            var dialogChoice = setupAiGame.ShowDialog();
            if (dialogChoice != DialogResult.OK)
            {
                return (null, null);
            }

            var setupDialog = new SetupDialog(setupAiGame.ScriptFileName, random, ForcedAlignments, ForcedCharacters)
            {
                PlayerCount = setupAiGame.PlayerCount
            };
            setupDialog.RandomizeSetup();

            return (setupDialog, setupAiGame.Model);
        }

        private IReadOnlyCollection<Alignment?> ForcedAlignments => GetPlayerConfiguration().Select(config => config.Alignment).ToList();
        private IReadOnlyCollection<Character?> ForcedCharacters => GetPlayerConfiguration().Select(config => config.Character).ToList();

        private IEnumerable<PlayerConfig> GetPlayerConfiguration()
        {
            var playerConfigurationSection = ConfigurationManager.GetSection("PlayerConfig") as PlayerConfigSection ?? throw new Exception("Invalid or missing PlayerConfig section");
            return playerConfigurationSection.Players.PlayerConfigs;
        }

        private readonly Random random = new();
    }
}