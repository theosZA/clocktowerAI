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

        private async void sendButton_Click(object sender, EventArgs e)
        {
            sendButton.Enabled = false;
            try
            {
                IChat chat = new OpenAiChat("gpt-3.5-turbo-1106", chatHistory.Select(chatMessage => (chatMessage.Role, chatMessage.Message)));
                var response = await chat.GetAssistantResponse();
                if (!string.IsNullOrEmpty(response))
                {
                    chatHistory.Add(new() { Role = Role.Assistant, Message = response.ReplaceLineEndings() });
                }
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

        private readonly BindingList<ChatMessage> chatHistory = new();
    }
}