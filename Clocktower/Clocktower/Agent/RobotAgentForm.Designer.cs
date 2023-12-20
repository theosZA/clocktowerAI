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
            components = new System.ComponentModel.Container();
            summaryTextBox = new RichTextBox();
            refreshTimer = new System.Windows.Forms.Timer(components);
            usageStatusStrip = new StatusStrip();
            usageStatusLabel = new ToolStripStatusLabel();
            usageStatusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // summaryTextBox
            // 
            summaryTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            summaryTextBox.Location = new Point(12, 13);
            summaryTextBox.Name = "summaryTextBox";
            summaryTextBox.ReadOnly = true;
            summaryTextBox.Size = new Size(448, 588);
            summaryTextBox.TabIndex = 0;
            summaryTextBox.Text = "";
            // 
            // refreshTimer
            // 
            refreshTimer.Interval = 5000;
            refreshTimer.Tick += refreshTimer_Tick;
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
            // RobotAgentForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(472, 636);
            Controls.Add(usageStatusStrip);
            Controls.Add(summaryTextBox);
            Name = "RobotAgentForm";
            Text = "RobotAgentForm";
            usageStatusStrip.ResumeLayout(false);
            usageStatusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox summaryTextBox;
        private System.Windows.Forms.Timer refreshTimer;
        private StatusStrip usageStatusStrip;
        private ToolStripStatusLabel usageStatusLabel;
    }
}