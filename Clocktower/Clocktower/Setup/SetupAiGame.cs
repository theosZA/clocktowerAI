namespace Clocktower
{
    public partial class SetupAiGame : Form
    {
        public int PlayerCount => (int)playerCount.Value;
        public string Model => modelText.Text;
        public string? ScriptFileName { get; private set; } = null;

        public SetupAiGame()
        {
            InitializeComponent();
        }

        private void scriptButton_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptFileName = ChooseScript();
                if (ScriptFileName != null)
                {
                    scriptText.Text = Path.GetFileNameWithoutExtension(ScriptFileName);
                }
            }
            catch
            { }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
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
    }
}
