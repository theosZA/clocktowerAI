using Newtonsoft.Json;
using OpenAi;
using System.ComponentModel;

namespace ChatApplication
{
    public partial class ChatForm : Form
    {
        public ChatForm()
        {
            InitializeComponent();

            chatHistoryView.DataSource = chatHistory;

            chatHistoryView.Columns[nameof(ChatMessage.Message)].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            chatHistoryView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            chatHistoryView.Columns[nameof(ChatMessage.Message)].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            chatHistoryView.Columns[nameof(ChatMessage.Role)].ReadOnly = true;

            modelsComboBox.DataSource = models;
        }

        private void chatHistoryView_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= chatHistoryView.Rows.Count)
            {
                return;
            }

            var row = chatHistoryView.Rows[e.RowIndex];
            if (row.DataBoundItem == null)
            {
                return;
            }

            var chatMessage = (ChatMessage)row.DataBoundItem;

            row.DefaultCellStyle.ForeColor = chatMessage.Role switch
            {
                Role.System => Color.Purple,
                Role.Assistant => Color.Green,
                Role.User => Color.Black,
                _ => throw new NotImplementedException()
            };
        }

        private async Task GetAssistantResponse()
        {
            var model = modelsComboBox.SelectedValue as string ?? string.Empty;
            IChat chat = new OpenAiChat(model, chatHistory.Select(chatMessage => (chatMessage.Role, chatMessage.Message)));
            var response = await chat.GetAssistantResponse();
            if (!string.IsNullOrEmpty(response))
            {
                chatHistory.Add(new() { Role = Role.Assistant, Message = response.ReplaceLineEndings() });
            }
        }

        private void SetChatHistory(IEnumerable<ChatMessage> chatMessages)
        {
            chatHistory.Clear();
            foreach (var chatMessage in chatMessages)
            {
                chatHistory.Add(chatMessage);
            }
        }

        private void SaveChatHistoryToFile(string fileName)
        {
            var jsonContent = JsonConvert.SerializeObject(chatHistory, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(fileName, jsonContent);
        }

        private void LoadChatHistoryFromFile(string fileName)
        {
            var jsonContent = File.ReadAllText(fileName);
            var chatMessages = JsonConvert.DeserializeObject<List<ChatMessage>>(jsonContent) ?? new();
            SetChatHistory(chatMessages);
        }

        private void ImportChatHistoryFromLogFile(string fileName)
        {
            var logFile = new LogFile(fileName);
            SetChatHistory(logFile.BuildChatHistoryFromLatest());
        }

        private async void sendButton_Click(object sender, EventArgs e)
        {
            sendButton.Enabled = false;
            try
            {
                await GetAssistantResponse();
            }
            catch (Exception exception)
            {
                MessageBox.Show($"An error occurred: {exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sendButton.Enabled = true;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";
            saveFileDialog.Title = "Save Chat History";
            saveFileDialog.DefaultExt = "json";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    SaveChatHistoryToFile(saveFileDialog.FileName);
                }
                catch (Exception exception)
                {
                    MessageBox.Show($"An error occurred: {exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";
            openFileDialog.Title = "Load Chat History";
            openFileDialog.DefaultExt = "json";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    LoadChatHistoryFromFile(openFileDialog.FileName);
                }
                catch (Exception exception)
                {
                    MessageBox.Show($"An error occurred: {exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void importFromLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Log Files (*.log)|*.log|All Files (*.*)|*.*";
            openFileDialog.Title = "Import Chat Log";
            openFileDialog.DefaultExt = "log";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ImportChatHistoryFromLogFile(openFileDialog.FileName);
                }
                catch (Exception exception)
                {
                    MessageBox.Show($"An error occurred: {exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                chatHistory.Clear();
            }
            catch (Exception exception)
            {
                MessageBox.Show($"An error occurred: {exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chatHistoryView_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            try
            {
                chatHistory.Remove((ChatMessage)e.Row.DataBoundItem);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"An error occurred: {exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private readonly BindingList<ChatMessage> chatHistory = new();

        private readonly List<string> models = new()
        {
            "gpt-3.5-turbo-1106",
            "gpt-4-1106-preview"
        };
    }
}