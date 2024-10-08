namespace Clocktower.Agent
{
    partial class AiStorytellerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            usageStatusStrip = new StatusStrip();
            usageStatusLabel = new ToolStripStatusLabel();
            chatTextBox = new RichTextBox();
            usageStatusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // usageStatusStrip
            // 
            usageStatusStrip.Items.AddRange(new ToolStripItem[] { usageStatusLabel });
            usageStatusStrip.Location = new Point(0, 614);
            usageStatusStrip.Name = "usageStatusStrip";
            usageStatusStrip.Size = new Size(472, 22);
            usageStatusStrip.TabIndex = 1;
            usageStatusStrip.Text = "statusStrip1";
            // 
            // usageStatusLabel
            // 
            usageStatusLabel.Name = "usageStatusLabel";
            usageStatusLabel.Size = new Size(180, 17);
            usageStatusLabel.Text = "Usage: 0 = 0 + 0, Latest: 0 = 0 + 0";
            // 
            // chatTextBox
            // 
            chatTextBox.Dock = DockStyle.Fill;
            chatTextBox.Location = new Point(0, 0);
            chatTextBox.Name = "chatTextBox";
            chatTextBox.ReadOnly = true;
            chatTextBox.Size = new Size(472, 614);
            chatTextBox.TabIndex = 2;
            chatTextBox.Text = "";
            // 
            // AiStorytellerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(472, 636);
            Controls.Add(chatTextBox);
            Controls.Add(usageStatusStrip);
            Name = "AiStorytellerForm";
            Text = "AI Storyteller";
            usageStatusStrip.ResumeLayout(false);
            usageStatusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private StatusStrip usageStatusStrip;
        private ToolStripStatusLabel usageStatusLabel;
        private RichTextBox chatTextBox;
    }
}