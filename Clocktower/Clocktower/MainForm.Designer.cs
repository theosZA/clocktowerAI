﻿namespace Clocktower
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            mainMenu = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newToolStripMenuItem = new ToolStripMenuItem();
            newWhaleBucketToolStripMenuItem = new ToolStripMenuItem();
            aIStorytellerGameToolStripMenuItem = new ToolStripMenuItem();
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            mainMenu.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // mainMenu
            // 
            mainMenu.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            mainMenu.Location = new Point(0, 0);
            mainMenu.Name = "mainMenu";
            mainMenu.Size = new Size(800, 24);
            mainMenu.TabIndex = 0;
            mainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, newWhaleBucketToolStripMenuItem, aIStorytellerGameToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.Size = new Size(184, 22);
            newToolStripMenuItem.Text = "&New...";
            newToolStripMenuItem.Click += newToolStripMenuItem_Click;
            // 
            // newWhaleBucketToolStripMenuItem
            // 
            newWhaleBucketToolStripMenuItem.Name = "newWhaleBucketToolStripMenuItem";
            newWhaleBucketToolStripMenuItem.Size = new Size(184, 22);
            newWhaleBucketToolStripMenuItem.Text = "New &Whale Bucket...";
            newWhaleBucketToolStripMenuItem.Click += newWhaleBucketToolStripMenuItem_Click;
            // 
            // aIStorytellerGameToolStripMenuItem
            // 
            aIStorytellerGameToolStripMenuItem.Name = "aIStorytellerGameToolStripMenuItem";
            aIStorytellerGameToolStripMenuItem.Size = new Size(184, 22);
            aIStorytellerGameToolStripMenuItem.Text = "&AI Storyteller Game...";
            aIStorytellerGameToolStripMenuItem.Click += aIStorytellerGameToolStripMenuItem_Click;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
            statusStrip.Location = new Point(0, 428);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(800, 22);
            statusStrip.TabIndex = 1;
            // 
            // statusLabel
            // 
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(0, 17);
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(statusStrip);
            Controls.Add(mainMenu);
            MainMenuStrip = mainMenu;
            Name = "MainForm";
            Text = "Blood on the Clocktower";
            mainMenu.ResumeLayout(false);
            mainMenu.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip mainMenu;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private ToolStripMenuItem newWhaleBucketToolStripMenuItem;
        private ToolStripMenuItem aIStorytellerGameToolStripMenuItem;
    }
}