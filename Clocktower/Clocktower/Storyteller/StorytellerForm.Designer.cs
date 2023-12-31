﻿namespace Clocktower.Storyteller
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
            autoCheckbox = new CheckBox();
            responseTextBox = new TextBox();
            submitButton = new Button();
            SuspendLayout();
            // 
            // outputText
            // 
            outputText.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            outputText.Location = new Point(12, 12);
            outputText.Name = "outputText";
            outputText.ReadOnly = true;
            outputText.Size = new Size(776, 434);
            outputText.TabIndex = 0;
            outputText.Text = "";
            // 
            // chooseButton
            // 
            chooseButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            chooseButton.Enabled = false;
            chooseButton.Location = new Point(307, 452);
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
            choicesComboBox.Location = new Point(13, 452);
            choicesComboBox.Name = "choicesComboBox";
            choicesComboBox.Size = new Size(288, 23);
            choicesComboBox.TabIndex = 3;
            // 
            // autoCheckbox
            // 
            autoCheckbox.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            autoCheckbox.AutoSize = true;
            autoCheckbox.Location = new Point(736, 456);
            autoCheckbox.Name = "autoCheckbox";
            autoCheckbox.Size = new Size(52, 19);
            autoCheckbox.TabIndex = 5;
            autoCheckbox.Text = "Auto";
            autoCheckbox.UseVisualStyleBackColor = true;
            // 
            // responseTextBox
            // 
            responseTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            responseTextBox.Enabled = false;
            responseTextBox.Location = new Point(13, 481);
            responseTextBox.Multiline = true;
            responseTextBox.Name = "responseTextBox";
            responseTextBox.Size = new Size(694, 51);
            responseTextBox.TabIndex = 6;
            // 
            // submitButton
            // 
            submitButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            submitButton.Enabled = false;
            submitButton.Location = new Point(713, 509);
            submitButton.Name = "submitButton";
            submitButton.Size = new Size(75, 23);
            submitButton.TabIndex = 7;
            submitButton.Text = "Submit";
            submitButton.UseVisualStyleBackColor = true;
            submitButton.Click += submitButton_Click;
            // 
            // StorytellerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 544);
            Controls.Add(submitButton);
            Controls.Add(responseTextBox);
            Controls.Add(autoCheckbox);
            Controls.Add(chooseButton);
            Controls.Add(choicesComboBox);
            Controls.Add(outputText);
            Name = "StorytellerForm";
            Text = "Storyteller";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox outputText;
        private Button chooseButton;
        private ComboBox choicesComboBox;
        private CheckBox autoCheckbox;
        private TextBox responseTextBox;
        private Button submitButton;
    }
}