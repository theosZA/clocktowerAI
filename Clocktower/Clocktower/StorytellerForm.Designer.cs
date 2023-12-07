namespace Clocktower
{
    partial class StorytellerForm
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
            outputText = new RichTextBox();
            chooseButton = new Button();
            choicesComboBox = new ComboBox();
            SuspendLayout();
            // 
            // outputText
            // 
            outputText.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            outputText.Location = new Point(12, 12);
            outputText.Name = "outputText";
            outputText.Size = new Size(776, 391);
            outputText.TabIndex = 0;
            outputText.Text = "";
            // 
            // chooseButton
            // 
            chooseButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            chooseButton.Enabled = false;
            chooseButton.Location = new Point(306, 415);
            chooseButton.Name = "chooseButton";
            chooseButton.Size = new Size(75, 23);
            chooseButton.TabIndex = 4;
            chooseButton.Text = "Choose";
            chooseButton.UseVisualStyleBackColor = true;
            chooseButton.Click += chooseButton_Click;
            // 
            // choicesComboBox
            // 
            choicesComboBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            choicesComboBox.Enabled = false;
            choicesComboBox.FormattingEnabled = true;
            choicesComboBox.Location = new Point(12, 415);
            choicesComboBox.Name = "choicesComboBox";
            choicesComboBox.Size = new Size(288, 23);
            choicesComboBox.TabIndex = 3;
            // 
            // StorytellerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(chooseButton);
            Controls.Add(choicesComboBox);
            Controls.Add(outputText);
            Name = "StorytellerForm";
            Text = "Storyteller";
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox outputText;
        private Button chooseButton;
        private ComboBox choicesComboBox;
    }
}