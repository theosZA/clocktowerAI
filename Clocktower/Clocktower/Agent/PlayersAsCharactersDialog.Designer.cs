namespace Clocktower.Agent
{
    partial class PlayersAsCharactersDialog
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
            playersAsCharactersTable = new TableLayoutPanel();
            submitButton = new Button();
            SuspendLayout();
            // 
            // jugglesTable
            // 
            playersAsCharactersTable.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            playersAsCharactersTable.ColumnCount = 2;
            playersAsCharactersTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            playersAsCharactersTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            playersAsCharactersTable.Location = new Point(13, 15);
            playersAsCharactersTable.Name = "jugglesTable";
            playersAsCharactersTable.RowCount = 5;
            playersAsCharactersTable.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            playersAsCharactersTable.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            playersAsCharactersTable.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            playersAsCharactersTable.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            playersAsCharactersTable.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            playersAsCharactersTable.Size = new Size(281, 186);
            playersAsCharactersTable.TabIndex = 0;
            // 
            // submitButton
            // 
            submitButton.Anchor = AnchorStyles.Bottom;
            submitButton.Location = new Point(116, 221);
            submitButton.Name = "submitButton";
            submitButton.Size = new Size(77, 23);
            submitButton.TabIndex = 1;
            submitButton.Text = "Submit";
            submitButton.UseVisualStyleBackColor = true;
            submitButton.Click += submitButton_Click;
            // 
            // JuggleDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(306, 256);
            Controls.Add(submitButton);
            Controls.Add(playersAsCharactersTable);
            Name = "JuggleDialog";
            Text = "Choose your juggles";
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel playersAsCharactersTable;
        private Button submitButton;
    }
}