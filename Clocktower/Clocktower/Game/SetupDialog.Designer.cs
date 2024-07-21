namespace Clocktower.Game
{
    partial class SetupDialog
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
            startButton = new Button();
            randomizeButton = new Button();
            playerCountUpDown = new NumericUpDown();
            playerCountLabel = new Label();
            charactersPanel = new TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)playerCountUpDown).BeginInit();
            SuspendLayout();
            // 
            // startButton
            // 
            startButton.Enabled = false;
            startButton.Location = new Point(7, 9);
            startButton.Name = "startButton";
            startButton.Size = new Size(75, 23);
            startButton.TabIndex = 0;
            startButton.Text = "Start";
            startButton.UseVisualStyleBackColor = true;
            startButton.Click += StartGame;
            // 
            // randomizeButton
            // 
            randomizeButton.Location = new Point(271, 9);
            randomizeButton.Name = "randomizeButton";
            randomizeButton.Size = new Size(75, 23);
            randomizeButton.TabIndex = 1;
            randomizeButton.Text = "Randomize";
            randomizeButton.UseVisualStyleBackColor = true;
            randomizeButton.Click += RandomizeBag;
            // 
            // playerCountUpDown
            // 
            playerCountUpDown.Location = new Point(170, 9);
            playerCountUpDown.Maximum = new decimal(new int[] { 15, 0, 0, 0 });
            playerCountUpDown.Minimum = new decimal(new int[] { 7, 0, 0, 0 });
            playerCountUpDown.Name = "playerCountUpDown";
            playerCountUpDown.Size = new Size(54, 23);
            playerCountUpDown.TabIndex = 2;
            playerCountUpDown.Value = new decimal(new int[] { 12, 0, 0, 0 });
            playerCountUpDown.ValueChanged += UpdateCounters;
            // 
            // playerCountLabel
            // 
            playerCountLabel.AutoSize = true;
            playerCountLabel.Location = new Point(117, 11);
            playerCountLabel.Name = "playerCountLabel";
            playerCountLabel.Size = new Size(47, 15);
            playerCountLabel.TabIndex = 3;
            playerCountLabel.Text = "Players:";
            // 
            // charactersPanel
            // 
            charactersPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            charactersPanel.AutoScroll = true;
            charactersPanel.ColumnCount = 4;
            charactersPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            charactersPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            charactersPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            charactersPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            charactersPanel.Location = new Point(9, 43);
            charactersPanel.Name = "charactersPanel";
            charactersPanel.RowCount = 15;
            charactersPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            charactersPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            charactersPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            charactersPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            charactersPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            charactersPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            charactersPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            charactersPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            charactersPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            charactersPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            charactersPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            charactersPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            charactersPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            charactersPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            charactersPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            charactersPanel.Size = new Size(883, 455);
            charactersPanel.TabIndex = 4;
            // 
            // SetupDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(904, 518);
            Controls.Add(charactersPanel);
            Controls.Add(playerCountLabel);
            Controls.Add(playerCountUpDown);
            Controls.Add(randomizeButton);
            Controls.Add(startButton);
            Name = "SetupDialog";
            Text = "Set up new game";
            ((System.ComponentModel.ISupportInitialize)playerCountUpDown).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button startButton;
        private Button randomizeButton;
        private NumericUpDown playerCountUpDown;
        private Label playerCountLabel;
        private TableLayoutPanel charactersPanel;
    }
}