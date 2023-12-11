namespace Clocktower.Agent
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
            autoCheckbox = new CheckBox();
            submitButton = new Button();
            responseTextBox = new TextBox();
            SuspendLayout();
            // 
            // outputText
            // 
            outputText.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            outputText.Location = new Point(14, 12);
            outputText.Name = "outputText";
            outputText.ReadOnly = true;
            outputText.Size = new Size(446, 319);
            outputText.TabIndex = 0;
            outputText.Text = "";
            // 
            // choicesComboBox
            // 
            choicesComboBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            choicesComboBox.Enabled = false;
            choicesComboBox.FormattingEnabled = true;
            choicesComboBox.Location = new Point(12, 338);
            choicesComboBox.Name = "choicesComboBox";
            choicesComboBox.Size = new Size(288, 23);
            choicesComboBox.TabIndex = 1;
            // 
            // chooseButton
            // 
            chooseButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            chooseButton.Enabled = false;
            chooseButton.Location = new Point(306, 337);
            chooseButton.Name = "chooseButton";
            chooseButton.Size = new Size(75, 23);
            chooseButton.TabIndex = 2;
            chooseButton.Text = "Choose";
            chooseButton.UseVisualStyleBackColor = true;
            chooseButton.Click += chooseButton_Click;
            // 
            // autoCheckbox
            // 
            autoCheckbox.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            autoCheckbox.AutoSize = true;
            autoCheckbox.Location = new Point(408, 341);
            autoCheckbox.Name = "autoCheckbox";
            autoCheckbox.Size = new Size(52, 19);
            autoCheckbox.TabIndex = 3;
            autoCheckbox.Text = "Auto";
            autoCheckbox.UseVisualStyleBackColor = true;
            // 
            // submitButton
            // 
            submitButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            submitButton.Enabled = false;
            submitButton.Location = new Point(385, 455);
            submitButton.Name = "submitButton";
            submitButton.Size = new Size(75, 23);
            submitButton.TabIndex = 9;
            submitButton.Text = "Submit";
            submitButton.UseVisualStyleBackColor = true;
            submitButton.Click += submitButton_Click;
            // 
            // responseTextBox
            // 
            responseTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            responseTextBox.Enabled = false;
            responseTextBox.Location = new Point(12, 367);
            responseTextBox.Multiline = true;
            responseTextBox.Name = "responseTextBox";
            responseTextBox.Size = new Size(367, 111);
            responseTextBox.TabIndex = 8;
            // 
            // HumanAgentForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(472, 490);
            Controls.Add(submitButton);
            Controls.Add(responseTextBox);
            Controls.Add(autoCheckbox);
            Controls.Add(chooseButton);
            Controls.Add(choicesComboBox);
            Controls.Add(outputText);
            Name = "HumanAgentForm";
            Text = "HumanAgentForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox outputText;
        private ComboBox choicesComboBox;
        private Button chooseButton;
        private CheckBox autoCheckbox;
        private Button submitButton;
        private TextBox responseTextBox;
    }
}