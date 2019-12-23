using System.Windows.Forms;
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
            this.SuspendLayout();
            // 
            // logEntryTextBox
            // 
            this.logEntryTextBox.Location = new System.Drawing.Point(12, 12);
            this.logEntryTextBox.Name = "logEntryTextBox";
            this.logEntryTextBox.Size = new System.Drawing.Size(220, 20);
            this.logEntryTextBox.TabIndex = 0;
            // 
            // LogEntryInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 44);
            this.Controls.Add(this.logEntryTextBox);
//            this.Location = new System.Drawing.Point(100, 100);
            this.Name = "LogEntryInputForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Log time";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox logEntryTextBox;
    }
}

