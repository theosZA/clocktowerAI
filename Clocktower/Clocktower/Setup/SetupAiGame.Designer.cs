namespace Clocktower
{
    partial class SetupAiGame
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
            playerCountLabel = new Label();
            playerCount = new NumericUpDown();
            modelLabel = new Label();
            modelText = new TextBox();
            scriptLabel = new Label();
            scriptText = new TextBox();
            scriptButton = new Button();
            startButton = new Button();
            ((System.ComponentModel.ISupportInitialize)playerCount).BeginInit();
            SuspendLayout();
            // 
            // playerCountLabel
            // 
            playerCountLabel.AutoSize = true;
            playerCountLabel.Location = new Point(15, 14);
            playerCountLabel.Name = "playerCountLabel";
            playerCountLabel.Size = new Size(105, 15);
            playerCountLabel.TabIndex = 0;
            playerCountLabel.Text = "Number of players";
            // 
            // playerCount
            // 
            playerCount.Location = new Point(15, 32);
            playerCount.Maximum = new decimal(new int[] { 15, 0, 0, 0 });
            playerCount.Minimum = new decimal(new int[] { 7, 0, 0, 0 });
            playerCount.Name = "playerCount";
            playerCount.Size = new Size(57, 23);
            playerCount.TabIndex = 1;
            playerCount.Value = new decimal(new int[] { 8, 0, 0, 0 });
            // 
            // modelLabel
            // 
            modelLabel.AutoSize = true;
            modelLabel.Location = new Point(15, 69);
            modelLabel.Name = "modelLabel";
            modelLabel.Size = new Size(111, 15);
            modelLabel.TabIndex = 2;
            modelLabel.Text = "AI Storyteller model";
            // 
            // modelText
            // 
            modelText.Location = new Point(15, 87);
            modelText.Name = "modelText";
            modelText.Size = new Size(144, 23);
            modelText.TabIndex = 3;
            modelText.Text = "gpt-4o";
            // 
            // scriptLabel
            // 
            scriptLabel.AutoSize = true;
            scriptLabel.Location = new Point(15, 132);
            scriptLabel.Name = "scriptLabel";
            scriptLabel.Size = new Size(37, 15);
            scriptLabel.TabIndex = 4;
            scriptLabel.Text = "Script";
            // 
            // scriptText
            // 
            scriptText.Location = new Point(15, 150);
            scriptText.Name = "scriptText";
            scriptText.ReadOnly = true;
            scriptText.Size = new Size(144, 23);
            scriptText.TabIndex = 5;
            scriptText.Text = "Whale Bucket";
            // 
            // scriptButton
            // 
            scriptButton.Location = new Point(84, 124);
            scriptButton.Name = "scriptButton";
            scriptButton.Size = new Size(75, 23);
            scriptButton.TabIndex = 6;
            scriptButton.Text = "File...";
            scriptButton.UseVisualStyleBackColor = true;
            scriptButton.Click += scriptButton_Click;
            // 
            // startButton
            // 
            startButton.Anchor = AnchorStyles.Bottom;
            startButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            startButton.Location = new Point(55, 204);
            startButton.Name = "startButton";
            startButton.Size = new Size(81, 23);
            startButton.TabIndex = 7;
            startButton.Text = "Start Game";
            startButton.UseVisualStyleBackColor = true;
            startButton.Click += startButton_Click;
            // 
            // SetupAiGame
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(200, 239);
            Controls.Add(startButton);
            Controls.Add(scriptButton);
            Controls.Add(scriptText);
            Controls.Add(scriptLabel);
            Controls.Add(modelText);
            Controls.Add(modelLabel);
            Controls.Add(playerCount);
            Controls.Add(playerCountLabel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "SetupAiGame";
            Text = "Set up AI game";
            ((System.ComponentModel.ISupportInitialize)playerCount).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label playerCountLabel;
        private NumericUpDown playerCount;
        private Label modelLabel;
        private TextBox modelText;
        private Label scriptLabel;
        private TextBox scriptText;
        private Button scriptButton;
        private Button startButton;
    }
}