﻿using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ManualTimeLogger.App
{
    partial class LogEntryInputForm
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
            this.logEntryTextBox = new System.Windows.Forms.TextBox();
            this.autoFillListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // logEntryTextBox
            // 
            this.logEntryTextBox.AcceptsReturn = true;
            this.logEntryTextBox.CausesValidation = false;
            this.logEntryTextBox.Location = new System.Drawing.Point(16, 15);
            this.logEntryTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.logEntryTextBox.Name = "logEntryTextBox";
            this.logEntryTextBox.Size = new System.Drawing.Size(319, 22);
            this.logEntryTextBox.TabIndex = 0;
            this.logEntryTextBox.WordWrap = false;
            // 
            // labelsListBox
            // 
            this.autoFillListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.autoFillListBox.FormattingEnabled = true;
            this.autoFillListBox.ItemHeight = 16;
            this.autoFillListBox.Location = new System.Drawing.Point(182, 6);
            this.autoFillListBox.Name = "autoFillListBox";
            this.autoFillListBox.Size = new System.Drawing.Size(104, 36);
            this.autoFillListBox.Sorted = true;
            this.autoFillListBox.TabIndex = 1;
            this.autoFillListBox.TabStop = false;
            this.autoFillListBox.Visible = false;
            // 
            // LogEntryInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 54);
            this.Controls.Add(this.autoFillListBox);
            this.Controls.Add(this.logEntryTextBox);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "LogEntryInputForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Log time";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox logEntryTextBox;
        private ListBox autoFillListBox;
    }
}

