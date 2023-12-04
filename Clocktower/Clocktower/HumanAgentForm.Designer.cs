namespace Clocktower
{
    partial class HumanAgentForm
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
            choicesComboBox = new ComboBox();
            chooseButton = new Button();
            SuspendLayout();
            // 
            // outputText
            // 
            outputText.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            outputText.Location = new Point(14, 12);
            outputText.Name = "outputText";
            outputText.Size = new Size(435, 351);
            outputText.TabIndex = 0;
            outputText.Text = "";
            // 
            // choicesComboBox
            // 
            choicesComboBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            choicesComboBox.Enabled = false;
            choicesComboBox.FormattingEnabled = true;
            choicesComboBox.Location = new Point(18, 373);
            choicesComboBox.Name = "choicesComboBox";
            choicesComboBox.Size = new Size(288, 23);
            choicesComboBox.TabIndex = 1;
            // 
            // chooseButton
            // 
            chooseButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            chooseButton.Enabled = false;
            chooseButton.Location = new Point(312, 373);
            chooseButton.Name = "chooseButton";
            chooseButton.Size = new Size(75, 23);
            chooseButton.TabIndex = 2;
            chooseButton.Text = "Choose";
            chooseButton.UseVisualStyleBackColor = true;
            chooseButton.Click += chooseButton_Click;
            // 
            // HumanAgentForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(462, 413);
            Controls.Add(chooseButton);
            Controls.Add(choicesComboBox);
            Controls.Add(outputText);
            Name = "HumanAgentForm";
            Text = "HumanAgentForm";
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox outputText;
        private ComboBox choicesComboBox;
        private Button chooseButton;
    }
}