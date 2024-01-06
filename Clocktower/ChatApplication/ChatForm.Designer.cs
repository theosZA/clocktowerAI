namespace ChatApplication
{
    partial class ChatForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            chatHistoryView = new DataGridView();
            sendButton = new Button();
            modelsComboBox = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)chatHistoryView).BeginInit();
            SuspendLayout();
            // 
            // chatHistoryView
            // 
            chatHistoryView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            chatHistoryView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            chatHistoryView.Location = new Point(9, 9);
            chatHistoryView.Name = "chatHistoryView";
            chatHistoryView.RowTemplate.Height = 25;
            chatHistoryView.Size = new Size(779, 616);
            chatHistoryView.TabIndex = 0;
            chatHistoryView.RowPrePaint += chatHistoryView_RowPrePaint;
            // 
            // sendButton
            // 
            sendButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            sendButton.Location = new Point(9, 631);
            sendButton.Name = "sendButton";
            sendButton.Size = new Size(75, 23);
            sendButton.TabIndex = 1;
            sendButton.Text = "Send";
            sendButton.UseVisualStyleBackColor = true;
            sendButton.Click += sendButton_Click;
            // 
            // modelsComboBox
            // 
            modelsComboBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            modelsComboBox.FormattingEnabled = true;
            modelsComboBox.Location = new Point(603, 634);
            modelsComboBox.Name = "modelsComboBox";
            modelsComboBox.Size = new Size(185, 23);
            modelsComboBox.TabIndex = 2;
            // 
            // ChatForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 666);
            Controls.Add(modelsComboBox);
            Controls.Add(sendButton);
            Controls.Add(chatHistoryView);
            Name = "ChatForm";
            Text = "Chat";
            ((System.ComponentModel.ISupportInitialize)chatHistoryView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView chatHistoryView;
        private Button sendButton;
        private ComboBox modelsComboBox;
    }
}