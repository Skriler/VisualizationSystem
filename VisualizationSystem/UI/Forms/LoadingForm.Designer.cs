﻿namespace VisualizationSystem.UI.Forms
{
    partial class LoadingForm
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
            progressBarLoading = new ProgressBar();
            lblMessage = new Label();
            SuspendLayout();
            // 
            // progressBarLoading
            // 
            progressBarLoading.Location = new Point(21, 46);
            progressBarLoading.Name = "progressBarLoading";
            progressBarLoading.Size = new Size(144, 20);
            progressBarLoading.Style = ProgressBarStyle.Marquee;
            progressBarLoading.TabIndex = 0;
            // 
            // lblMessage
            // 
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(82, 19);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(16, 15);
            lblMessage.TabIndex = 1;
            lblMessage.Text = "...";
            // 
            // LoadingForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(189, 78);
            Controls.Add(lblMessage);
            Controls.Add(progressBarLoading);
            FormBorderStyle = FormBorderStyle.None;
            Name = "LoadingForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Loading...";
            Paint += LoadingForm_Paint;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ProgressBar progressBarLoading;
        private Label lblMessage;
    }
}