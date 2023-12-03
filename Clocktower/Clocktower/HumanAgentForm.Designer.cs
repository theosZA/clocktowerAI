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
            SuspendLayout();
            // 
            // outputText
            // 
            outputText.Location = new Point(14, 12);
            outputText.Name = "outputText";
            outputText.Size = new Size(774, 379);
            outputText.TabIndex = 0;
            outputText.Text = "";
            // 
            // choicesComboBox
            // 
            choicesComboBox.Enabled = false;
            choicesComboBox.FormattingEnabled = true;
            choicesComboBox.Location = new Point(18, 401);
            choicesComboBox.Name = "choicesComboBox";
            choicesComboBox.Size = new Size(288, 23);
            choicesComboBox.TabIndex = 1;
            // 
            // HumanAgentForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(801, 441);
            Controls.Add(choicesComboBox);
            Controls.Add(outputText);
            Name = "HumanAgentForm";
            Text = "HumanAgentForm";
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox outputText;
        private ComboBox choicesComboBox;
    }
}