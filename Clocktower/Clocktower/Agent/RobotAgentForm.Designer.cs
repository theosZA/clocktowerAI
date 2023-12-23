namespace Clocktower.Agent
{
    partial class RobotAgentForm
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
            chatTextBox = new RichTextBox();
            usageStatusStrip = new StatusStrip();
            usageStatusLabel = new ToolStripStatusLabel();
            summaryTextBox = new RichTextBox();
            usageStatusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // chatTextBox
            // 
            chatTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            chatTextBox.Location = new Point(12, 13);
            chatTextBox.Name = "chatTextBox";
            chatTextBox.ReadOnly = true;
            chatTextBox.Size = new Size(448, 455);
            chatTextBox.TabIndex = 0;
            chatTextBox.Text = "";
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
            // summaryTextBox
            // 
            summaryTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            summaryTextBox.Location = new Point(12, 474);
            summaryTextBox.Name = "summaryTextBox";
            summaryTextBox.ReadOnly = true;
            summaryTextBox.Size = new Size(448, 126);
            summaryTextBox.TabIndex = 2;
            summaryTextBox.Text = "";
            // 
            // RobotAgentForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(472, 636);
            Controls.Add(summaryTextBox);
            Controls.Add(usageStatusStrip);
            Controls.Add(chatTextBox);
            Name = "RobotAgentForm";
            Text = "RobotAgentForm";
            usageStatusStrip.ResumeLayout(false);
            usageStatusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox chatTextBox;
        private StatusStrip usageStatusStrip;
        private ToolStripStatusLabel usageStatusLabel;
        private RichTextBox summaryTextBox;
    }
}