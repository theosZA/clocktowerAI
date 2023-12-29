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
            usageStatusStrip = new StatusStrip();
            usageStatusLabel = new ToolStripStatusLabel();
            splitContainer1 = new SplitContainer();
            chatTextBox = new RichTextBox();
            summaryTextBox = new RichTextBox();
            usageStatusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
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
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(chatTextBox);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(summaryTextBox);
            splitContainer1.Size = new Size(472, 614);
            splitContainer1.SplitterDistance = 450;
            splitContainer1.TabIndex = 3;
            // 
            // chatTextBox
            // 
            chatTextBox.Dock = DockStyle.Fill;
            chatTextBox.Location = new Point(0, 0);
            chatTextBox.Name = "chatTextBox";
            chatTextBox.ReadOnly = true;
            chatTextBox.Size = new Size(472, 450);
            chatTextBox.TabIndex = 1;
            chatTextBox.Text = "";
            // 
            // summaryTextBox
            // 
            summaryTextBox.Dock = DockStyle.Fill;
            summaryTextBox.Location = new Point(0, 0);
            summaryTextBox.Name = "summaryTextBox";
            summaryTextBox.ReadOnly = true;
            summaryTextBox.Size = new Size(472, 160);
            summaryTextBox.TabIndex = 3;
            summaryTextBox.Text = "";
            // 
            // RobotAgentForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(472, 636);
            Controls.Add(splitContainer1);
            Controls.Add(usageStatusStrip);
            Name = "RobotAgentForm";
            Text = "RobotAgentForm";
            usageStatusStrip.ResumeLayout(false);
            usageStatusStrip.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private StatusStrip usageStatusStrip;
        private ToolStripStatusLabel usageStatusLabel;
        private SplitContainer splitContainer1;
        private RichTextBox chatTextBox;
        private RichTextBox summaryTextBox;
    }
}